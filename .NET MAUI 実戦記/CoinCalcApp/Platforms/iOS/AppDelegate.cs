using Foundation;
using Google.MobileAds;
using UIKit;

namespace CoinCalcApp
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);

            // Plugin.AdMob (iOS) の SDK 本初期化。
            // Plugin.AdMob の公式手順:UseAdMob() で Handler/DI は登録されるが、
            // Google Mobile Ads SDK 自体の起動は AppDelegate で明示的に呼ぶ必要がある。
            //
            // GitHub Issue #56 の知見:
            // SupportedOSPlatformVersion / MinimumOSVersion が 14.x のままだと
            // MobileAds.SharedInstance が null になる(Jc.GMA.iOS バインディングの要件が 15.0)。
            // → csproj と Info.plist の両方で 15.0 以上に揃える必要あり。
            MobileAds.SharedInstance.Start(_ => { });

            return result;
        }
    }
}
