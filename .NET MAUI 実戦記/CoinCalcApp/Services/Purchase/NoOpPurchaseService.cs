using Reactive.Bindings;

namespace CoinCalcApp.Services.Purchase;

/// <summary>
/// 何も処理しないスタブ実装。Step 2 では DI に登録され、現状の動作を維持する。
/// Step 4 で StoreKit / Google Play Billing を扱う実装(Plugin.InAppBilling 利用)に差し替える。
///
/// 開発・デバッグ時には、IsAdFreeOverride で広告非表示状態をシミュレートできる。
/// </summary>
public sealed class NoOpPurchaseService : IPurchaseService
{
    private readonly ReactivePropertySlim<bool> _isAdFree;

    public NoOpPurchaseService(bool initialIsAdFree = false)
    {
        _isAdFree = new ReactivePropertySlim<bool>(initialIsAdFree);
        IsAdFree = _isAdFree.ToReadOnlyReactivePropertySlim();
    }

    public IReadOnlyReactiveProperty<bool> IsAdFree { get; }

    public Task<PurchaseResult> PurchaseAdFreeAsync(CancellationToken cancellationToken = default)
    {
        // Step 2 段階では実購入処理はないので "失敗" を返す。
        // Step 4 で実装される時に意味のある動作になる。
        return Task.FromResult(PurchaseResult.Failed);
    }

    public Task<PurchaseResult> RestorePurchasesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(PurchaseResult.NothingToRestore);
    }

    /// <summary>
    /// 開発時用フック。デバッグメニュー等から呼び出して
    /// 課金済み状態をシミュレートできるようにしておく。
    /// 本番ビルドでは UI から到達しないこと。
    /// </summary>
    internal void DebugSetIsAdFree(bool value) => _isAdFree.Value = value;
}
