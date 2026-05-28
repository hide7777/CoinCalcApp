using CoinCalcApp.Views;

namespace CoinCalcApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // CalcPage は MainPage からの計算ボタン押下で動的に遷移するため、
        // 起動時の ShellContent には含めず、Route として登録だけしておく。
        Routing.RegisterRoute("CalcPage", typeof(CalcPage));
    }
}
