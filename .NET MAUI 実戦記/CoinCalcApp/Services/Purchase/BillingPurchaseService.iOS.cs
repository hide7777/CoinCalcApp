#if IOS || MACCATALYST
using Foundation;
using StoreKit;

// StoreKit 1 全体が iOS 18 で deprecated になり、Microsoft .NET アナライザーが CA1422 を出す。
// しかし StoreKit 2 はまだ .NET から直接呼び出せず(Swift interop 整備待ち)、
// Microsoft 公式サンプルも引き続き StoreKit 1 を使用している現状。
// 機能的には iOS 26 まで動作保証されているため、ファイル全体で CA1422 を抑制する。
#pragma warning disable CA1422

namespace CoinCalcApp.Services.Purchase;

/// <summary>
/// iOS / Mac Catalyst 用の課金実装(StoreKit 1)。
///
/// StoreKit 1 は callback ベースの古い API だが、iOS 26 まで動作保証されており実績豊富。
/// StoreKit 2 は Swift-first で .NET から呼ぶには Swift interop が必要(まだ未整備)なので
/// 当面 StoreKit 1 で実装する。
///
/// 構成:
/// - SKPaymentTransactionObserver:全トランザクションを常時監視する
/// - SKProductsRequest:プロダクト情報(価格等)をストアから取得する
/// - 購入完了/復元時は MarkAdFreeOwned() を呼んで IsAdFree を更新
/// </summary>
public sealed partial class BillingPurchaseService
{
    private TransactionObserver? _observer;
    private TaskCompletionSource<PurchaseResult>? _purchaseTcs;
    private TaskCompletionSource<PurchaseResult>? _restoreTcs;
    private bool _restoreFoundAny;

    partial void InitializePlatform()
    {
        // トランザクションオブザーバを登録。起動時から常時監視を続けることで、
        // 別端末で購入した内容が反映される時や、保留状態の決済が確定する時にも
        // 自動的に IsAdFree を true にできる。
        _observer = new TransactionObserver(this);
        SKPaymentQueue.DefaultQueue.AddTransactionObserver(_observer);
    }

    partial void DisposePlatform()
    {
        if (_observer is not null)
        {
            SKPaymentQueue.DefaultQueue.RemoveTransactionObserver(_observer);
            _observer.Dispose();
            _observer = null;
        }
    }

    private partial Task<PurchaseResult> PurchaseAdFreePlatformAsync(CancellationToken cancellationToken)
    {
        if (!SKPaymentQueue.CanMakePayments)
        {
            // ペアレンタルコントロール等で課金不可
            return Task.FromResult(PurchaseResult.Failed);
        }

        _purchaseTcs = new TaskCompletionSource<PurchaseResult>();

        // キャンセル要求があれば TCS を Canceled で完了
        cancellationToken.Register(() => _purchaseTcs?.TrySetResult(PurchaseResult.Canceled));

        // まずプロダクト情報を取得し、それを使って SKPayment を生成して決済キューに投入する
        var request = new SKProductsRequest(new NSSet(PurchaseConfig.AdFreeProductId));
        request.Delegate = new ProductsRequestDelegate(this, isPurchase: true);
        request.Start();

        return _purchaseTcs.Task;
    }

    private partial Task<PurchaseResult> RestorePurchasesPlatformAsync(CancellationToken cancellationToken)
    {
        _restoreTcs = new TaskCompletionSource<PurchaseResult>();
        _restoreFoundAny = false;

        cancellationToken.Register(() => _restoreTcs?.TrySetResult(PurchaseResult.Canceled));

        // StoreKit に過去のトランザクションを再送するよう要求
        SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();

        return _restoreTcs.Task;
    }

    // ─────────── 内部:プロダクト情報取得 Delegate ───────────

    private sealed class ProductsRequestDelegate : SKProductsRequestDelegate
    {
        private readonly BillingPurchaseService _owner;
        private readonly bool _isPurchase;

        public ProductsRequestDelegate(BillingPurchaseService owner, bool isPurchase)
        {
            _owner = owner;
            _isPurchase = isPurchase;
        }

        public override void ReceivedResponse(SKProductsRequest request, SKProductsResponse response)
        {
            var product = response.Products?.FirstOrDefault(p => p.ProductIdentifier == PurchaseConfig.AdFreeProductId);

            if (product is null)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Purchase iOS] Product '{PurchaseConfig.AdFreeProductId}' not found in store. " +
                    "App Store Connect での In-App Purchase 登録を確認してください。");
                if (_isPurchase)
                    _owner._purchaseTcs?.TrySetResult(PurchaseResult.Failed);
                return;
            }

            if (_isPurchase)
            {
                // 決済キューに投入(支払いシートはここから OS が表示する)
                // .NET 10 iOS 26.2 バインディングでは SKPayment.CreateFrom が正しい API
                // (Microsoft Learn の最新ドキュメントで Methods 一覧に CreateFrom(SKProduct) を確認済み)
                var payment = SKPayment.CreateFrom(product);
                SKPaymentQueue.DefaultQueue.AddPayment(payment);
                // 結果は TransactionObserver 側で受け取って _purchaseTcs を完了させる
            }
        }

        public override void RequestFailed(SKRequest request, NSError error)
        {
            System.Diagnostics.Debug.WriteLine($"[Purchase iOS] Products request failed: {error.LocalizedDescription}");
            if (_isPurchase)
                _owner._purchaseTcs?.TrySetResult(PurchaseResult.Failed);
        }
    }

    // ─────────── 内部:トランザクションオブザーバ ───────────

    private sealed class TransactionObserver : SKPaymentTransactionObserver
    {
        private readonly BillingPurchaseService _owner;

        public TransactionObserver(BillingPurchaseService owner)
        {
            _owner = owner;
        }

        public override void UpdatedTransactions(SKPaymentQueue queue, SKPaymentTransaction[] transactions)
        {
            foreach (var t in transactions)
            {
                if (t.Payment.ProductIdentifier != PurchaseConfig.AdFreeProductId)
                    continue;

                switch (t.TransactionState)
                {
                    case SKPaymentTransactionState.Purchased:
                        // 新規購入が確定
                        _owner.MarkAdFreeOwned();
                        SKPaymentQueue.DefaultQueue.FinishTransaction(t);
                        _owner._purchaseTcs?.TrySetResult(PurchaseResult.Success);
                        break;

                    case SKPaymentTransactionState.Restored:
                        // 復元処理で既存購入が再送された
                        _owner.MarkAdFreeOwned();
                        _owner._restoreFoundAny = true;
                        SKPaymentQueue.DefaultQueue.FinishTransaction(t);
                        break;

                    case SKPaymentTransactionState.Failed:
                        // ユーザーキャンセル または 決済失敗
                        var canceled = t.Error?.Code == (long)SKError.PaymentCancelled;
                        SKPaymentQueue.DefaultQueue.FinishTransaction(t);
                        _owner._purchaseTcs?.TrySetResult(
                            canceled ? PurchaseResult.Canceled : PurchaseResult.Failed);
                        break;

                    case SKPaymentTransactionState.Deferred:
                        // ペアレンタル承認待ち等。ユーザーは何もしていない、操作可能なまま戻すべき。
                        _owner._purchaseTcs?.TrySetResult(PurchaseResult.Canceled);
                        break;

                    case SKPaymentTransactionState.Purchasing:
                        // 進行中、何もしない
                        break;
                }
            }
        }

        // .NET iOS バインディングのメソッド名は Objective-C の
        //   paymentQueueRestoreCompletedTransactionsFinished:
        // から PaymentQueue プレフィックスが落ちて
        //   RestoreCompletedTransactionsFinished(SKPaymentQueue queue)
        // となる(SKPaymentTransactionObserver の virtual メソッド)。
        public override void RestoreCompletedTransactionsFinished(SKPaymentQueue queue)
        {
            // 復元処理が全件終わった時に呼ばれる
            _owner._restoreTcs?.TrySetResult(
                _owner._restoreFoundAny ? PurchaseResult.Success : PurchaseResult.NothingToRestore);
        }

        public override void RestoreCompletedTransactionsFailedWithError(SKPaymentQueue queue, NSError error)
        {
            System.Diagnostics.Debug.WriteLine($"[Purchase iOS] Restore failed: {error.LocalizedDescription}");
            _owner._restoreTcs?.TrySetResult(PurchaseResult.Failed);
        }
    }
}

#pragma warning restore CA1422
#endif
