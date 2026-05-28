using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoinCalcApp.Services.Purchase;
using Plugin.AdMob.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace CoinCalcApp.Services.Ads;

/// <summary>
/// Plugin.AdMob (marius-bughiu, v3.0.2) を介して AdMob を扱う本実装。
///
/// - バナーは XAML 側の <c>&lt;admob:BannerAd /&gt;</c> が直接 SDK に通る
/// - インタースティシャルは <see cref="IInterstitialAdService"/> 経由で
///   <c>PrepareAd</c> → <c>ShowAd</c> のサイクルを回す
/// - 頻度キャップは <see cref="InterstitialFrequencyGate"/> に委譲
///
/// 設計上のポイント:
/// 1. <see cref="IInterstitialAdService"/> は Plugin.AdMob 側の DI 登録(UseAdMob)で
///    インスタンスが共有される。プリロードと表示が同一インスタンスで連携する前提。
/// 2. AdMob の規約上、インタースティシャルは ShowAd 後に再 PrepareAd する必要がある。
/// </summary>
public sealed class AdMobAdService : IAdService, IDisposable
{
    private readonly IPurchaseService _purchaseService;
    private readonly IInterstitialAdService _interstitialService;
    private readonly InterstitialFrequencyGate _gate;
    private readonly CompositeDisposable _disposables = new();

    public AdMobAdService(
        IPurchaseService purchaseService,
        IInterstitialAdService interstitialService,
        InterstitialFrequencyGate gate)
    {
        _purchaseService = purchaseService;
        _interstitialService = interstitialService;
        _gate = gate;

        // バナー表示可否 = !IsAdFree(課金未済ならバナーを出す)
        ShouldShowBanner = _purchaseService.IsAdFree
            .Select(isAdFree => !isAdFree)
            .ToReadOnlyReactivePropertySlim(initialValue: !_purchaseService.IsAdFree.Value)
            .AddTo(_disposables);

        // 初回プリロード(課金未済の時のみ)
        if (!_purchaseService.IsAdFree.Value)
        {
            TryPrepareInterstitial();
        }
    }

    public IReadOnlyReactiveProperty<bool> ShouldShowBanner { get; }

    public Task<bool> TryShowInterstitialAsync(CancellationToken cancellationToken = default)
    {
        // 1) 課金済みなら絶対に出さない
        if (_purchaseService.IsAdFree.Value)
            return Task.FromResult(false);

        // 2) プラットフォーム未対応(Windows / MacCatalyst 等)なら出さない
        if (string.IsNullOrEmpty(AdConfig.InterstitialAdUnitId))
            return Task.FromResult(false);

        // 3) 頻度キャップ
        if (!_gate.CanShow())
        {
            // 次回のためにプリロードだけは継続
            TryPrepareInterstitial();
            return Task.FromResult(false);
        }

        // 4) 実際に表示
        try
        {
            _interstitialService.ShowAd();
            _gate.MarkShown();

            // 1 つ消費したので次をプリロード
            TryPrepareInterstitial();
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AdMob] ShowInterstitial failed: {ex}");
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// インタースティシャル広告のプリロードを開始する。
    /// </summary>
    private void TryPrepareInterstitial()
    {
        if (string.IsNullOrEmpty(AdConfig.InterstitialAdUnitId)) return;

        try
        {
            _interstitialService.PrepareAd(AdConfig.InterstitialAdUnitId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AdMob] PrepareInterstitial failed: {ex}");
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
