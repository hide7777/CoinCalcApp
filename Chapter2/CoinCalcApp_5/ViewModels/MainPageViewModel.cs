using System.ComponentModel.DataAnnotations;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Prism.Common;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace CoinCalcApp.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    ///<summary> 
    /// Binding変数を定義します。
    ///</summary> 
    public AsyncReactiveCommand ButtonCommand { get; set; }
 	
    ///<summary>
    /// 1円のテキスト領域定義とチェック内容の定義
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> OneText { get; }
    public ReadOnlyReactiveProperty<string?> OneTextError { get; }

    ///<summary>
    /// 5円のテキスト領域定義とチェック内容の定義
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> FiveText { get; }
    public ReadOnlyReactiveProperty<string?> FiveTextError { get; }

    ///<summary>
    /// 10円のテキスト領域定義とチェック内容の定義
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> TenText { get; }
    public ReadOnlyReactiveProperty<string?> TenTextError { get; }

    ///<summary>
    /// 50円のテキスト領域定義とチェック内容の定義
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> FiftyText { get; }
    public ReadOnlyReactiveProperty<string?> FiftyTextError { get; }

    ///<summary>
    /// 100円のテキスト領域定義とチェック内容の定義
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> OneHundredText { get; }
    public ReadOnlyReactiveProperty<string?> OneHundredTextError { get; }

    ///<summary>
    /// 500円のテキスト領域定義とチェック内容の定義
    ///</summary>
    [Required(ErrorMessage = "必須項目")]
    [IntValidation(ErrorMessage = "半角数字を入力")]
    [Range(0, 1000000, ErrorMessage = "0以上の値を入力")]
    public ReactiveProperty<string> FiveHundredText { get; }
    public ReadOnlyReactiveProperty<string?> FiveHundredTextError { get; }

    ///<summary>
    /// コンストラクタ(画面表示に当たって1回設定すれば良い定義を記載。毎回設定が必要な場合は OnNavigatedToで記載する）
    ///</summary>
    public MainPageViewModel(IPageDialogService pageDialogService,INavigationService navigationService)
            : base(pageDialogService,navigationService)
    {
        //画面に表示するタイトル名を設定
        Title = "硬貨預入手数料計算";

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
        OneTextError = OneText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        FiveTextError = this.FiveText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        TenTextError = this.TenText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        FiftyTextError = this.FiftyText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        OneHundredTextError = this.OneHundredText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();
        FiveHundredTextError = this.FiveHundredText.ObserveErrorChanged.Select(x => x?.Cast<string>()?.FirstOrDefault()).ToReadOnlyReactiveProperty();

        // 計算ボタンの設定
        ButtonCommand = new[] {
                    OneText.ObserveHasErrors,          //1円のバリデーションチェック
                    FiveText.ObserveHasErrors,         //5円のバリデーションチェック
                    TenText.ObserveHasErrors,          //10円のバリデーションチェック
                    FiftyText.ObserveHasErrors,        //50円のバリデーションチェック
                    OneHundredText.ObserveHasErrors,   //100円のバリデーションチェック
                    FiveHundredText.ObserveHasErrors,  //500円のバリデーションチェック
            }.CombineLatestValuesAreAllFalse()              // 全てエラーなしの場合に押せるようにする
         .ToAsyncReactiveCommand()
         .WithSubscribe(ButtonProcessing); //ボタンが押された際の処理メソッドを登録
    }

    ///<summary>
    /// 計算ボタン押下時処理
    ///</summary>
    private async Task ButtonProcessing()
    {
        try
        {
            //パラメータ渡しで、画面入力値を計算結果画面へ引き渡します。
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("one", OneText.Value);
            parameters.Add("five", FiveText.Value);
            parameters.Add("ten", TenText.Value);
            parameters.Add("fifty", FiftyText.Value);
            parameters.Add("oneHundred", OneHundredText.Value);
            parameters.Add("fiveHundred", FiveHundredText.Value);
            
            await NavigationService.NavigateAsync("CalcPage", parameters);
        }
        catch (Exception e)
        {
            await PageDialogService.DisplayAlertAsync("ERROR", e.ToString(), "ERROR");
            System.Diagnostics.Debug.WriteLine("ERROR:" + e.ToString());
        }
    }
}
