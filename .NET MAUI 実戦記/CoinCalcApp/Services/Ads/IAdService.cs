using Reactive.Bindings;

namespace CoinCalcApp.Services.Ads;

/// <summary>
/// 広告サービス。バナー / インタースティシャル / 頻度キャップを抽象化。
///
/// 実装は Step 3 で AdMob ベース(Plugin.MAUI.AdmobMauiSdk など)に差し替え予定。
/// 現在は NoOpAdService(何も表示しない)。
///
/// 課金状態の変化は内部で IPurchaseService.IsAdFree を購読して反映する。
/// (ViewModel は IAdService の状態だけ見れば良いように設計)
/// </summary>
public interface IAdService
{
    /// <summary>
    /// バナー広告を表示すべきか。
    /// 通常は !IsAdFree。XAML から直接バインド可能。
    /// </summary>
    IReadOnlyReactiveProperty<bool> ShouldShowBanner { get; }

    /// <summary>
    /// インタースティシャル広告を出すタイミングが来た時に呼ぶ。
    /// 内部で:
    ///   - 課金済みなら何もしない
    ///   - 頻度キャップ(初回 24h 抑止 / 1 日 N 回まで)に引っかかれば何もしない
    ///   - 条件を満たせば実際に広告を表示する
    /// 戻り値:広告が表示された場合 true、それ以外 false。
    /// </summary>
    Task<bool> TryShowInterstitialAsync(CancellationToken cancellationToken = default);
}
