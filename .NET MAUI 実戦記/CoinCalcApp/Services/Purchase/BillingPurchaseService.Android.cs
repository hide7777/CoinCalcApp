#if ANDROID
using Android.BillingClient.Api;
using Android.Content;
// CoinCalcApp.Services.Purchase という C# 名前空間と
// Android.BillingClient.Api.Purchase クラスが同名衝突するため、
// BillingClient 側を BillingPurchase エイリアスにして区別する。
using BillingPurchase = Android.BillingClient.Api.Purchase;

// 注:Purchase.PurchaseState は .NET バインディングでは独自の PurchaseState 型
// (Java enum 相当のラッパー型)として公開されている。
// int との直接比較はできないため (int) キャストで取り出して比較する。
// Google Play Billing v7 の定義:
//   UNSPECIFIED_STATE = 0
//   PURCHASED         = 1
//   PENDING           = 2

namespace CoinCalcApp.Services.Purchase;

/// <summary>
/// Android 用の課金実装(Google Play Billing Library v7)。
///
/// Xamarin.Android.Google.BillingClient NuGet パッケージが Google Play Billing v7 をバインドしている。
/// v7 では「ProductDetails」「PurchasesUpdatedListener」「AcknowledgePurchase」が中心。
///
/// 一回限り課金(consumable でない)の場合、購入後は必ず Acknowledge する必要がある。
/// Acknowledge しないと 3 日以内に自動的にユーザーへ返金される。
/// </summary>
public sealed partial class BillingPurchaseService
{
    private BillingClient? _billingClient;
    private TaskCompletionSource<PurchaseResult>? _purchaseTcs;

    partial void InitializePlatform()
    {
        var context = Android.App.Application.Context;

        // PurchasesUpdatedListener:購入フロー完了/失敗の通知を受ける
        var listener = new BillingPurchasesUpdatedListener(this);

        _billingClient = BillingClient.NewBuilder(context)
            .SetListener(listener)
            .EnablePendingPurchases(
                PendingPurchasesParams.NewBuilder()
                    .EnableOneTimeProducts()
                    .Build())
            .Build();

        // 起動時に接続を開始。接続が確立したら過去の購入を確認して IsAdFree を同期する。
        _billingClient.StartConnection(new BillingClientStateListenerImpl(this));
    }

    partial void DisposePlatform()
    {
        try
        {
            _billingClient?.EndConnection();
        }
        catch { /* 解放時の例外は無視 */ }
        _billingClient = null;
    }

    private partial async Task<PurchaseResult> PurchaseAdFreePlatformAsync(CancellationToken cancellationToken)
    {
        if (_billingClient is null) return PurchaseResult.Failed;

        // 接続未確立の場合は接続完了を待つ
        if (!_billingClient.IsReady)
        {
            await WaitForConnectionAsync(cancellationToken);
            if (!_billingClient.IsReady) return PurchaseResult.Failed;
        }

        // 1) プロダクト情報を取得
        var productDetails = await QueryAdFreeProductDetailsAsync();
        if (productDetails is null) return PurchaseResult.Failed;

        // 2) 購入フロー起動準備
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity is null) return PurchaseResult.Failed;

        var productDetailsParams = BillingFlowParams.ProductDetailsParams.NewBuilder()
            .SetProductDetails(productDetails)
            .Build();

        var billingFlowParams = BillingFlowParams.NewBuilder()
            .SetProductDetailsParamsList(new[] { productDetailsParams })
            .Build();

        _purchaseTcs = new TaskCompletionSource<PurchaseResult>();
        cancellationToken.Register(() => _purchaseTcs?.TrySetResult(PurchaseResult.Canceled));

        // 3) 購入フロー起動(Google Play の支払いシートが表示される)
        var result = _billingClient.LaunchBillingFlow(activity, billingFlowParams);
        if (result.ResponseCode != BillingResponseCode.Ok)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[Purchase Android] LaunchBillingFlow failed: {result.ResponseCode} {result.DebugMessage}");
            return PurchaseResult.Failed;
        }

        // 結果は PurchasesUpdatedListener.OnPurchasesUpdated で _purchaseTcs を完了させる
        return await _purchaseTcs.Task;
    }

    private partial async Task<PurchaseResult> RestorePurchasesPlatformAsync(CancellationToken cancellationToken)
    {
        if (_billingClient is null) return PurchaseResult.Failed;

        if (!_billingClient.IsReady)
        {
            await WaitForConnectionAsync(cancellationToken);
            if (!_billingClient.IsReady) return PurchaseResult.Failed;
        }

        // ストアに問い合わせて、このユーザーの全購入履歴を取得
        var queryParams = QueryPurchasesParams.NewBuilder()
            .SetProductType(BillingClient.ProductType.Inapp)
            .Build();

        var queryResult = await _billingClient.QueryPurchasesAsync(queryParams);
        if (queryResult.Result.ResponseCode != BillingResponseCode.Ok)
        {
            return PurchaseResult.Failed;
        }

        var found = false;
        foreach (var purchase in queryResult.Purchases)
        {
            if (purchase.Products.Contains(PurchaseConfig.AdFreeProductId)
                && (int)purchase.PurchaseState == 1 /* PURCHASED */)
            {
                MarkAdFreeOwned();
                // Acknowledge していない購入があれば Acknowledge する
                if (!purchase.IsAcknowledged)
                {
                    await AcknowledgePurchaseAsync(purchase);
                }
                found = true;
            }
        }

        return found ? PurchaseResult.Success : PurchaseResult.NothingToRestore;
    }

    /// <summary>
    /// プロダクト詳細(ProductDetails)を取得。LaunchBillingFlow で必要。
    /// </summary>
    private async Task<ProductDetails?> QueryAdFreeProductDetailsAsync()
    {
        if (_billingClient is null) return null;

        var product = QueryProductDetailsParams.Product.NewBuilder()
            .SetProductId(PurchaseConfig.AdFreeProductId)
            .SetProductType(BillingClient.ProductType.Inapp)
            .Build();

        var queryParams = QueryProductDetailsParams.NewBuilder()
            .SetProductList(new[] { product })
            .Build();

        var queryResult = await _billingClient.QueryProductDetailsAsync(queryParams);
        if (queryResult.Result.ResponseCode != BillingResponseCode.Ok)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[Purchase Android] QueryProductDetails failed: {queryResult.Result.ResponseCode} {queryResult.Result.DebugMessage}");
            return null;
        }

        return queryResult.ProductDetails?.FirstOrDefault();
    }

    /// <summary>
    /// 新規購入の Acknowledge(消費しないアイテムの場合、必須)。
    /// </summary>
    internal async Task AcknowledgePurchaseAsync(BillingPurchase purchase)
    {
        if (_billingClient is null) return;
        if (purchase.IsAcknowledged) return;

        var acknowledgeParams = AcknowledgePurchaseParams.NewBuilder()
            .SetPurchaseToken(purchase.PurchaseToken)
            .Build();

        var result = await _billingClient.AcknowledgePurchaseAsync(acknowledgeParams);
        if (result.ResponseCode != BillingResponseCode.Ok)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[Purchase Android] Acknowledge failed: {result.ResponseCode} {result.DebugMessage}");
        }
    }

    /// <summary>
    /// 接続が確立するまで最大 5 秒待つ(タイムアウト)。
    /// </summary>
    private async Task WaitForConnectionAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < 50; i++)
        {
            if (_billingClient?.IsReady == true) return;
            if (cancellationToken.IsCancellationRequested) return;
            await Task.Delay(100, cancellationToken);
        }
    }

    /// <summary>
    /// 起動直後の購入状態同期。BillingClient 接続完了時に呼ばれる。
    /// </summary>
    internal async void OnBillingClientConnected()
    {
        try
        {
            if (_billingClient is null) return;
            var queryParams = QueryPurchasesParams.NewBuilder()
                .SetProductType(BillingClient.ProductType.Inapp)
                .Build();
            var queryResult = await _billingClient.QueryPurchasesAsync(queryParams);
            if (queryResult.Result.ResponseCode != BillingResponseCode.Ok) return;

            foreach (var purchase in queryResult.Purchases)
            {
                if (purchase.Products.Contains(PurchaseConfig.AdFreeProductId)
                    && (int)purchase.PurchaseState == 1 /* PURCHASED */)
                {
                    MarkAdFreeOwned();
                    if (!purchase.IsAcknowledged)
                    {
                        await AcknowledgePurchaseAsync(purchase);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Purchase Android] OnBillingClientConnected failed: {ex}");
        }
    }

    // ─────────── 内部:リスナー実装 ───────────

    private sealed class BillingClientStateListenerImpl : Java.Lang.Object, IBillingClientStateListener
    {
        private readonly BillingPurchaseService _owner;

        public BillingClientStateListenerImpl(BillingPurchaseService owner) => _owner = owner;

        public void OnBillingSetupFinished(BillingResult billingResult)
        {
            if (billingResult.ResponseCode == BillingResponseCode.Ok)
            {
                _owner.OnBillingClientConnected();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Purchase Android] Billing setup failed: {billingResult.ResponseCode} {billingResult.DebugMessage}");
            }
        }

        public void OnBillingServiceDisconnected()
        {
            System.Diagnostics.Debug.WriteLine("[Purchase Android] Billing service disconnected");
        }
    }

    private sealed class BillingPurchasesUpdatedListener : Java.Lang.Object, IPurchasesUpdatedListener
    {
        private readonly BillingPurchaseService _owner;

        public BillingPurchasesUpdatedListener(BillingPurchaseService owner) => _owner = owner;

        // IPurchasesUpdatedListener が要求するシグネチャは
        //   void OnPurchasesUpdated(BillingResult result, IList<Purchase> purchases)
        // ここで Purchase は Android.BillingClient.Api.Purchase 型(BillingPurchase エイリアス参照)。
        public async void OnPurchasesUpdated(BillingResult billingResult, IList<BillingPurchase>? purchases)
        {
            try
            {
                if (billingResult.ResponseCode == BillingResponseCode.UserCancelled)
                {
                    _owner._purchaseTcs?.TrySetResult(PurchaseResult.Canceled);
                    return;
                }

                if (billingResult.ResponseCode != BillingResponseCode.Ok || purchases is null)
                {
                    _owner._purchaseTcs?.TrySetResult(PurchaseResult.Failed);
                    return;
                }

                foreach (var purchase in purchases)
                {
                    if (purchase.Products.Contains(PurchaseConfig.AdFreeProductId)
                        && (int)purchase.PurchaseState == 1 /* PURCHASED */)
                    {
                        _owner.MarkAdFreeOwned();
                        // 必須:Acknowledge しないと 3 日後に自動返金される
                        await _owner.AcknowledgePurchaseAsync(purchase);
                    }
                }

                _owner._purchaseTcs?.TrySetResult(PurchaseResult.Success);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Purchase Android] OnPurchasesUpdated failed: {ex}");
                _owner._purchaseTcs?.TrySetResult(PurchaseResult.Failed);
            }
        }
    }
}
#endif
