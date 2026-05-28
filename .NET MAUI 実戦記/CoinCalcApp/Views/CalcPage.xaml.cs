using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoinCalcApp.ViewModels;
using Reactive.Bindings.Extensions;

namespace CoinCalcApp.Views;

public partial class CalcPage : ContentPage
{
    // ShouldShowBanner の購読を OnAppearing/OnDisappearing のペアで管理する。
    // CalcPage は DI で Transient 登録のため画面遷移のたびに生成される。購読しっぱなしだと
    // Singleton 側(IAdService.ShouldShowBanner)の通知先に破棄済み Page が溜まり、メモリリークや
    // 破棄済みビューへの通知(クラッシュ要因)になる。表示中だけ購読し、離れたら確実に解除する。
    private CompositeDisposable? _bannerSubscriptions;

    // バナーの設計上の高さ(XAML の HeightRequest と一致させる)。
    private const double BannerHeight = 60;
    private const double BannerTopMargin = 5;

    // 画面下端のジェスチャーナビ領域の高さ(DIU)。Android で実測してキャッシュする。
    // iOS は ContentPage の ios:Page.UseSafeArea="True" 側で確保するため 0 のまま。
    private double _bottomInsetDiu;

    public CalcPage(CalcPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        // クエリパラメータ受け取りは ViewModel 側で IQueryAttributable を実装することで処理。
        // Shell はナビゲーション時に Page.BindingContext が IQueryAttributable なら
        // ApplyQueryAttributes をフレームワーク側から呼び出してくれる。

        // ─────────── バナー広告:ロード成功時のみ表示 ───────────
        // Plugin.AdMob 3.0.2 の既知問題(Issue #64)対応。
        // バナーは XAML で IsVisible="False" にしておき、ここで OnAdLoaded を受けてから
        // 課金未済の場合に限り表示する。OnAdFailedToLoad では何もしない(非表示のまま)。
        //
        // ラムダ式で書くことで Plugin.AdMob 側のデリゲート型(EventHandler<IAdError> の
        // IAdError がどの名前空間にあるか、nullable annotation 等)に依存せず
        // コンパイラに型推論させる。
        CalcBanner.OnAdLoaded += (s, e) =>
        {
            // 課金未済の時だけ表示する。
            // ShouldShowBanner は AdMobAdService → ViewModel 経由で「!IsAdFree」を表す。
            if (BindingContext is CalcPageViewModel vm && vm.ShouldShowBanner.Value)
            {
                CalcBanner.IsVisible = true;
                // バナーが出たので、その高さ分だけ課金エリアの下端余白を増やす。
                UpdateBottomSpacing();
            }
        };

        CalcBanner.OnAdFailedToLoad += (s, e) =>
        {
            // ロード失敗時は非表示のまま(リトライしない、UI が「広告枠だけ空」になるのを防ぐ)。
            System.Diagnostics.Debug.WriteLine(
                $"[AdMob] CalcBanner OnAdFailedToLoad: {e}");
            CalcBanner.IsVisible = false;
            UpdateBottomSpacing();
        };

        // ─────────── イベント取りこぼし対策 ───────────
        // ハンドラ登録より前に OnAdLoaded が発火済みのケースに備え、IsLoaded で現状確認。
        if (CalcBanner.IsLoaded && viewModel.ShouldShowBanner.Value)
        {
            CalcBanner.IsVisible = true;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // ─────────── 課金完了/復元でバナーを即座に隠す ───────────
        // OnAdLoaded は広告ロード時に一度だけ発火するため、その後に課金状態が変わっても
        // バナーの IsVisible は更新されない。ShouldShowBanner(= !IsAdFree)を購読し、
        // false になった瞬間にバナーを隠す。これで「広告を消す」購入/復元の直後に、
        // 画面遷移を待たずその場でバナーが消える。
        _bannerSubscriptions = new CompositeDisposable();

        if (BindingContext is CalcPageViewModel vm)
        {
            vm.ShouldShowBanner
                .Subscribe(show =>
                {
                    if (!show)
                        CalcBanner.IsVisible = false;
                    UpdateBottomSpacing();
                })
                .AddTo(_bannerSubscriptions);
        }

        // 画面表示のたびに下端余白を確定させる。OnHandlerChanged の時点では
        // ウィンドウインセットがまだ取れていないことがあるため、ここで再取得する。
        ApplyBottomInset();
        UpdateBottomSpacing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // 画面を離れる際に購読を確実に解除する(Transient な Page の購読リーク防止)。
        _bannerSubscriptions?.Dispose();
        _bannerSubscriptions = null;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        // ハンドラ確定後にもインセット取得を試みる(端末によってはこの時点で取れる)。
        ApplyBottomInset();
        UpdateBottomSpacing();
    }

    // ─────────── Android: 画面下端のジェスチャーナビ領域の高さを実測する ───────────
    // Android の画面下端にあるジェスチャーバー(ホームインジケータ)の高さは機種ごとに
    // 異なるため、OS から実際のインセット値(下端)を取得して _bottomInsetDiu に保持する。
    // iOS は ios:Page.UseSafeArea="True" 側で確保するため、ここでは何もしない(0 のまま)。
    private void ApplyBottomInset()
    {
#if ANDROID
        try
        {
            var activity = Platform.CurrentActivity;
            var decorView = activity?.Window?.DecorView;
            var insets = decorView?.RootWindowInsets;
            if (insets is null)
                return;

            int bottomPx;
            if (OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                // API 30+:システムバー(ナビバー)とジェスチャー領域の大きい方を採用
                var sysBars = insets.GetInsets(
                    Android.Views.WindowInsets.Type.SystemBars());
                var gestures = insets.GetInsets(
                    Android.Views.WindowInsets.Type.SystemGestures());
                bottomPx = System.Math.Max(sysBars.Bottom, gestures.Bottom);
            }
            else
            {
#pragma warning disable CA1422 // 旧 API はフォールバック用途でのみ使用
                bottomPx = insets.SystemWindowInsetBottom;
#pragma warning restore CA1422
            }

            // px → device-independent units (DIU) へ換算
            var density = DeviceDisplay.MainDisplayInfo.Density;
            _bottomInsetDiu = density > 0 ? bottomPx / density : bottomPx;
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CalcPage] inset read failed: {ex}");
        }
#endif
    }

    // ─────────── 下端の余白を、バナー表示状態に応じて再計算する ───────────
    // ・固定バナー(Row 1)の下端 Margin = ジェスチャーナビ領域分
    // ・課金エリア(ScrollView 内・最後尾)の下端 Padding:
    //     バナー表示時 = バナー高さ + 上下 Margin + ナビ領域(バナーに隠れない分を確保)
    //     バナー非表示時 = ナビ領域のみ(余白を空けすぎない)
    //   これで、スクロール末尾でも「購入の復元」が固定バナー/ナビ領域に隠れず押せる。
    private void UpdateBottomSpacing()
    {
        // バナーがナビ領域と重ならないよう、バナー下端に余白を入れる。
        CalcBanner.Margin = new Thickness(10, BannerTopMargin, 10, _bottomInsetDiu + 5);

        double clearance;
        if (CalcBanner.IsVisible)
        {
            // バナー高さ + バナー上 Margin + バナー下 Margin(ナビ領域含む)+ 余白
            clearance = BannerHeight + BannerTopMargin + (_bottomInsetDiu + 5) + 8;
        }
        else
        {
            // バナーが出ないので、ナビ領域分の余白だけでよい
            clearance = _bottomInsetDiu + 8;
        }

        PurchaseArea.Padding = new Thickness(0, 0, 0, clearance);
    }
}
