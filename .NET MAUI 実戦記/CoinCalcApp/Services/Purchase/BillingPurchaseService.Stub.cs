#if !(IOS || MACCATALYST || ANDROID)

namespace CoinCalcApp.Services.Purchase;

/// <summary>
/// iOS / Android 以外のプラットフォーム用スタブ実装。
/// Windows / Tizen など、課金未対応プラットフォームのビルドを通すためのもの。
/// </summary>
public sealed partial class BillingPurchaseService
{
    partial void InitializePlatform()
    {
        // 何もしない
    }

    partial void DisposePlatform()
    {
        // 何もしない
    }

    private partial Task<PurchaseResult> PurchaseAdFreePlatformAsync(CancellationToken cancellationToken)
    {
        // このプラットフォームでは課金不可
        return Task.FromResult(PurchaseResult.Failed);
    }

    private partial Task<PurchaseResult> RestorePurchasesPlatformAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(PurchaseResult.NothingToRestore);
    }
}

#endif
