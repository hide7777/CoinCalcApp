using Reactive.Bindings;

namespace CoinCalcApp.Services.Purchase;

/// <summary>
/// プラットフォーム別の課金処理を呼び出す IPurchaseService 実装。
///
/// iOS では partial メソッド経由で StoreKit 1 を、Android では Google Play Billing v7 を呼び出す。
/// partial 実装ファイルは csproj の Compile Include で TargetFramework に応じて切り替える。
///
/// 設計ポイント:
/// 1. IsAdFree は ReactivePropertySlim で公開し、購入完了/復元時に即座に true に切り替わる
/// 2. Preferences で永続化することで、オフライン時も起動直後に判定可能
/// 3. ストアとの実通信に失敗してもアプリが壊れない(IsAdFree のローカル値を信頼)
/// 4. 復元処理はストアの真実を信頼してローカル値を上書き
/// </summary>
public sealed partial class BillingPurchaseService : IPurchaseService, IDisposable
{
    private readonly ReactivePropertySlim<bool> _isAdFree;
    private readonly IPreferences _prefs;

    public BillingPurchaseService(IPreferences prefs)
    {
        _prefs = prefs;

        // 起動時の初期値は Preferences に保存された購入状態(オフライン即時判定)
        var owned = _prefs.Get(PurchaseConfig.AdFreeOwnedKey, false);
        _isAdFree = new ReactivePropertySlim<bool>(owned);

        // プラットフォーム別の初期化(StoreKit / BillingClient の接続開始)
        InitializePlatform();
    }

    public IReadOnlyReactiveProperty<bool> IsAdFree => _isAdFree;

    /// <summary>
    /// 「広告を消す ¥160」ボタン押下時の処理。
    /// 実際のストア決済フローを起動し、結果を PurchaseResult で返す。
    /// 成功時には IsAdFree が true に切り替わり、Preferences にも永続化される。
    /// </summary>
    public async Task<PurchaseResult> PurchaseAdFreeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // 既に購入済みなら何もしない(誤連打防止)
            if (_isAdFree.Value)
                return PurchaseResult.Success;

            return await PurchaseAdFreePlatformAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return PurchaseResult.Canceled;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Purchase] PurchaseAdFreeAsync failed: {ex}");
            return PurchaseResult.Failed;
        }
    }

    /// <summary>
    /// 「購入の復元」ボタン押下時の処理。
    /// ストアに問い合わせて、過去にこのアカウントで購入済みかを確認する。
    /// </summary>
    public async Task<PurchaseResult> RestorePurchasesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await RestorePurchasesPlatformAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return PurchaseResult.Canceled;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Purchase] RestorePurchasesAsync failed: {ex}");
            return PurchaseResult.Failed;
        }
    }

    /// <summary>
    /// プラットフォーム実装から呼び出される。購入成功時または復元成功時に IsAdFree を true にして
    /// Preferences にも永続化する。冪等(複数回呼ばれても問題なし)。
    /// </summary>
    internal void MarkAdFreeOwned()
    {
        if (_isAdFree.Value) return;
        _prefs.Set(PurchaseConfig.AdFreeOwnedKey, true);
        // UI 側からの購読(課金ボタンの表示制御等)が即座に反応するように
        MainThread.BeginInvokeOnMainThread(() => _isAdFree.Value = true);
    }

    public void Dispose()
    {
        DisposePlatform();
        _isAdFree.Dispose();
    }

    // ─────────── partial メソッド(プラットフォーム別実装で定義) ───────────

    /// <summary>プラットフォーム固有の初期化(ストア接続、トランザクション監視開始)。</summary>
    partial void InitializePlatform();

    /// <summary>プラットフォーム固有の購入フロー実行。</summary>
    private partial Task<PurchaseResult> PurchaseAdFreePlatformAsync(CancellationToken cancellationToken);

    /// <summary>プラットフォーム固有の購入復元フロー実行。</summary>
    private partial Task<PurchaseResult> RestorePurchasesPlatformAsync(CancellationToken cancellationToken);

    /// <summary>プラットフォーム固有のリソース解放(リスナー解除など)。</summary>
    partial void DisposePlatform();
}
