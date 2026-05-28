using System.ComponentModel.DataAnnotations;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoinCalcApp.Services.Ads;
using CoinCalcApp.Services.Dialog;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace CoinCalcApp.ViewModels;

public class MainPageViewModel : ViewModelBase, IDisposable
{
    private readonly IDialogService _dialog;
    private readonly IAdService _adService;
    private readonly CompositeDisposable _disposables = new();

    /// <summary>計算ボタンのコマンド。</summary>
    public AsyncReactiveCommand ButtonCommand { get; set; }

    /// <summary>バナー広告領域を表示するか(課金未済なら true)。</summary>
    public IReadOnlyReactiveProperty<bool> ShouldShowBanner { get; }

    /// <summary>バナー広告の AdMob 広告ユニット ID。XAML バインド用。</summary>
    public string BannerAdUnitId => AdConfig.BannerAdUnitId;

    // ─────────── 入力プロパティ ───────────

    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> OneText { get; }
    public ReadOnlyReactiveProperty<string?> OneTextError { get; }

    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> FiveText { get; }
    public ReadOnlyReactiveProperty<string?> FiveTextError { get; }

    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> TenText { get; }
    public ReadOnlyReactiveProperty<string?> TenTextError { get; }

    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> FiftyText { get; }
    public ReadOnlyReactiveProperty<string?> FiftyTextError { get; }

    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> OneHundredText { get; }
    public ReadOnlyReactiveProperty<string?> OneHundredTextError { get; }

    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> FiveHundredText { get; }
    public ReadOnlyReactiveProperty<string?> FiveHundredTextError { get; }

    /// <summary>
    /// コンストラクタ。DI 経由でサービスを受け取る。
    /// Prism の INavigationService / IPageDialogService は廃止し、
    /// それぞれ Shell.Current.GoToAsync / IDialogService に置き換える。
    /// </summary>
    public MainPageViewModel(IDialogService dialog, IAdService adService)
    {
        _dialog = dialog;
        _adService = adService;

        Title = "硬貨預入手数料計算";

        // バナー表示可否(IAdService → IReadOnlyReactiveProperty<bool> を購読)
        ShouldShowBanner = _adService.ShouldShowBanner
            .ToReadOnlyReactivePropertySlim(initialValue: _adService.ShouldShowBanner.Value)
            .AddTo(_disposables);

        //各硬貨用のバリデーション属性を設定
        OneText = new ReactiveProperty<string>().SetValidateAttribute(() => this.OneText);
        FiveText = new ReactiveProperty<string>().SetValidateAttribute(() => this.FiveText);
        TenText = new ReactiveProperty<string>().SetValidateAttribute(() => this.TenText);
        FiftyText = new ReactiveProperty<string>().SetValidateAttribute(() => this.FiftyText);
        OneHundredText = new ReactiveProperty<string>().SetValidateAttribute(() => this.OneHundredText);
        FiveHundredText = new ReactiveProperty<string>().SetValidateAttribute(() => this.FiveHundredText);

        //初期値をセット
        OneText.Value = "0";
        FiveText.Value = "0";
        TenText.Value = "0";
        FiftyText.Value = "0";
        OneHundredText.Value = "0";
        FiveHundredText.Value = "0";

        //各硬貨用のエラーテキストを定義
        OneTextError         = OneText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        FiveTextError        = FiveText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        TenTextError         = TenText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        FiftyTextError       = FiftyText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        OneHundredTextError  = OneHundredText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        FiveHundredTextError = FiveHundredText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();

        // 計算ボタン:全フィールドがバリデーションエラーなしの場合のみ押下可
        ButtonCommand = new[] {
                OneText.ObserveHasErrors,
                FiveText.ObserveHasErrors,
                TenText.ObserveHasErrors,
                FiftyText.ObserveHasErrors,
                OneHundredText.ObserveHasErrors,
                FiveHundredText.ObserveHasErrors,
        }.CombineLatestValuesAreAllFalse()
         .ToAsyncReactiveCommand()
         .WithSubscribe(ButtonProcessing);
    }

    /// <summary>
    /// 計算ボタン押下時の処理。
    /// インタースティシャル広告 → 結果画面へ Shell ナビゲーション。
    /// </summary>
    private async Task ButtonProcessing()
    {
        try
        {
            // 課金未済 & 頻度キャップ OK & 広告ロード済みなら表示
            await _adService.TryShowInterstitialAsync();

            // Shell.Current.GoToAsync のクエリ辞書形式でパラメータを渡す。
            // CalcPageViewModel 側で IQueryAttributable.ApplyQueryAttributes が呼ばれる。
            var parameters = new Dictionary<string, object>
            {
                { "one",         OneText.Value         ?? "0" },
                { "five",        FiveText.Value        ?? "0" },
                { "ten",         TenText.Value         ?? "0" },
                { "fifty",       FiftyText.Value       ?? "0" },
                { "oneHundred",  OneHundredText.Value  ?? "0" },
                { "fiveHundred", FiveHundredText.Value ?? "0" },
            };

            await Shell.Current.GoToAsync("CalcPage", parameters);
        }
        catch (Exception e)
        {
            await _dialog.DisplayAlertAsync("ERROR", e.ToString(), "OK");
            System.Diagnostics.Debug.WriteLine("ERROR:" + e.ToString());
        }
    }

    public void Dispose() => _disposables.Dispose();
}
