using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoinCalcApp.ViewModels;
using Reactive.Bindings.Extensions;

namespace CoinCalcApp.Views;

public partial class MainPage : ContentPage
{
    // ShouldShowBanner の購読を OnAppearing/OnDisappearing のペアで管理する。
    // MainPage は DI で Transient 登録のため、購読しっぱなしだと Singleton 側
    // (IAdService.ShouldShowBanner)の通知先に破棄済み Page が溜まりリークやクラッシュ要因になる。
    // 表示中だけ購読し、離れたら確実に解除する。
    private CompositeDisposable? _bannerSubscriptions;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // ─────────── バナー広告:ロード成功時のみ表示 ───────────
        // Plugin.AdMob 3.0.2 の既知問題(Issue #64)対応。
        // バナーは XAML で IsVisible="False" にしておき、ここで OnAdLoaded を受けてから
        // 課金未済の場合に限り表示する。OnAdFailedToLoad では何もしない(非表示のまま)。
        //
        // ラムダ式で書くことで Plugin.AdMob 側のデリゲート型(EventHandler<IAdError> の
        // IAdError がどの名前空間にあるか、nullable annotation 等)に依存せず
        // コンパイラに型推論させる。
        MainBanner.OnAdLoaded += (s, e) =>
        {
            if (BindingContext is MainPageViewModel vm && vm.ShouldShowBanner.Value)
            {
                MainBanner.IsVisible = true;
            }
        };

        MainBanner.OnAdFailedToLoad += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine(
                $"[AdMob] MainBanner OnAdFailedToLoad: {e}");
            MainBanner.IsVisible = false;
        };

        // ─────────── イベント取りこぼし対策 ───────────
        // MainPage は起動時のルートページとして ShellContent から生成されるため、
        // InitializeComponent() の中で BannerAd のネイティブビューが生成され、広告ロードも
        // 開始される。ロードが非常に高速に完了するケースでは、ここでハンドラを登録する前に
        // OnAdLoaded が発火済みで、ハンドラが二度と呼ばれない可能性がある。
        // IsLoaded プロパティで現状を確認し、既にロード済みなら即時表示する。
        if (MainBanner.IsLoaded && viewModel.ShouldShowBanner.Value)
        {
            MainBanner.IsVisible = true;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // ─────────── 課金完了/復元でバナーを即座に隠す ───────────
        // OnAdLoaded は広告ロード時に一度だけ発火するため、その後に課金状態が変わっても
        // バナーの IsVisible は更新されない。ShouldShowBanner(= !IsAdFree)を購読し、
        // false になった瞬間にバナーを隠す。
        _bannerSubscriptions = new CompositeDisposable();

        if (BindingContext is MainPageViewModel vm)
        {
            vm.ShouldShowBanner
                .Subscribe(show =>
                {
                    if (!show)
                        MainBanner.IsVisible = false;
                })
                .AddTo(_bannerSubscriptions);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // 画面を離れる際に購読を確実に解除する(Transient な Page の購読リーク防止)。
        _bannerSubscriptions?.Dispose();
        _bannerSubscriptions = null;
    }
}
