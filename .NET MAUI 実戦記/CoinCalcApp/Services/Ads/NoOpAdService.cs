using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoinCalcApp.Services.Purchase;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace CoinCalcApp.Services.Ads;

/// <summary>
/// 何も表示しないスタブ実装。Step 2 ではこれを DI に登録し、現在の(広告なし)動作を維持する。
/// ただし、課金フラグの購読配線は本実装と同じ形にしておく。
/// Step 3 で AdMob 実装に差し替えても、課金との接続は引き継げる。
/// </summary>
public sealed class NoOpAdService : IAdService, IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    public NoOpAdService(IPurchaseService purchaseService)
    {
        // バナー表示可否 = 「課金していない」(購読は本実装でも同じ仕組み)
        ShouldShowBanner = purchaseService.IsAdFree
            .Select(isAdFree => !isAdFree)
            .ToReadOnlyReactivePropertySlim(initialValue: !purchaseService.IsAdFree.Value)
            .AddTo(_disposables);
    }

    public IReadOnlyReactiveProperty<bool> ShouldShowBanner { get; }

    public Task<bool> TryShowInterstitialAsync(CancellationToken cancellationToken = default)
    {
        // Step 2 段階では何もしない。
        // Step 3 で AdMob 実装に差し替え時に、頻度キャップ + 表示処理を追加する。
        return Task.FromResult(false);
    }

    public void Dispose() => _disposables.Dispose();
}
