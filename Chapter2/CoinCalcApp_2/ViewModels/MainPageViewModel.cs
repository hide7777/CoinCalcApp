using Reactive.Bindings;

namespace CoinCalcApp_2.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    //Binding変数を定義します。
    public AsyncReactiveCommand ButtonCommand { get; }
    public ReactivePropertySlim<bool> IsButtonEnable = new ReactivePropertySlim<bool>(true);

    public String OneText { get; }
    public String FiveText { get; }
    public String TenText { get; }
    public String FiftyText { get; }
    public String OneHundredText { get; }
    public String FiveHundredText { get; }

    //コンストラクタ
    public MainPageViewModel(IPageDialogService pageDialogService,
INavigationService navigationService) : base(pageDialogService, navigationService)
    {
        //画面に表示するタイトル名を設定します。
        Title = "硬貨預入手数料計算";

        //初期値をセットします。
        OneText = "0";
        FiveText = "0";
        TenText = "0";
        FiftyText = "0";
        OneHundredText = "0";
        FiveHundredText = "0";

        //計算ボタンが押された際の処理メソッドを登録します。
        ButtonCommand = new AsyncReactiveCommand().
        WithSubscribe(ButtonProcessing);
    }

    private async Task ButtonProcessing()
    {
        //遷移先の計算画面を指定します。
        await NavigationService.NavigateAsync("CalcPage");
    }
}

