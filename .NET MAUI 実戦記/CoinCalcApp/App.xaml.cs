using Microsoft.Maui.Controls;

namespace CoinCalcApp;

public partial class App : Application
{
    private readonly AppShell _shell;

    public App(AppShell shell)
    {
        InitializeComponent();
        _shell = shell;
    }

    /// <summary>
    /// MAUI 10 推奨パターン:Application.MainPage の代わりに CreateWindow をオーバーライドする。
    /// (MAUI 10 では Application.MainPage の getter/setter が両方 [Obsolete] 化されている。)
    /// このメソッドは複数ウィンドウ環境(MacCatalyst, iPad 等)でも正しく動く。
    /// </summary>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_shell);
    }
}
