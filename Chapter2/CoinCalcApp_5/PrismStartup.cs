using CoinCalcApp.ViewModels;
using CoinCalcApp.Views;

namespace CoinCalcApp;

internal static class PrismStartup
{
    public static void Configure(PrismAppBuilder builder)
    {
        builder.RegisterTypes(RegisterTypes) .CreateWindow("NavigationPage/MainPage");
    }

    private static void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<MainPage,MainPageViewModel>().RegisterInstance(SemanticScreenReader.Default);
        containerRegistry.RegisterForNavigation<CalcPage,CalcPageViewModel>().RegisterInstance(SemanticScreenReader.Default);
    }
}
