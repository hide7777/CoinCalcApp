using CoinCalcApp.Domain.Services;
using CoinCalcApp.Services.Ads;
using CoinCalcApp.Services.Dialog;
using CoinCalcApp.Services.Purchase;
using CoinCalcApp.ViewModels;
using CoinCalcApp.Views;
using Microsoft.Extensions.Logging;
using Plugin.AdMob;
using Plugin.AdMob.Services;

namespace CoinCalcApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseAdMob()  // Plugin.AdMob:バナー Handler 登録 + IInterstitialAdService の DI 登録。
                         // SDK の本初期化は AppDelegate.FinishedLaunching の
                         // MobileAds.SharedInstance.Start() で行う(Plugin.AdMob 公式手順)。
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var services = builder.Services;

        // ─────────── ドメイン層 ───────────
        services.AddSingleton<IFeeDataSource, BundledFeeDataSource>();
        services.AddSingleton<IBankFeeRepository, JsonBankFeeRepository>();
        services.AddSingleton<IFeeCalculator, FeeCalculator>();

        // ─────────── ダイアログ ───────────
        services.AddSingleton<IDialogService, DialogService>();

        // ─────────── 課金(Step 4: Microsoft 公式パターンでの本実装) ───────────
        // iOS は StoreKit 1、Android は Google Play Billing v7 を直接呼ぶ。
        services.AddSingleton<IPurchaseService, BillingPurchaseService>();

        // ─────────── 広告(Plugin.AdMob による本実装) ───────────
        // IInterstitialAdService は Plugin.AdMob 側の UseAdMob() で登録済み。
        services.AddSingleton<IPreferences>(_ => Preferences.Default);
        services.AddSingleton<InterstitialFrequencyGate>();
        services.AddSingleton<IAdService, AdMobAdService>();

        // ─────────── Pages & ViewModels ───────────
        services.AddTransient<MainPage>();
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<CalcPage>();
        services.AddTransient<CalcPageViewModel>();

        // AppShell は Singleton(アプリ全体で 1 つ)
        services.AddSingleton<AppShell>();

        return builder.Build();
    }
}
