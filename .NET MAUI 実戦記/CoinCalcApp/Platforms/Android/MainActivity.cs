using Android.App;
using Android.Content.PM;
using Android.OS;

namespace CoinCalcApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        // Plugin.AdMob 3.0.2 は MauiProgram の UseAdMob() で
        // Android 側の初期化(MobileAds.Initialize)も自動で行うため、
        // MainActivity 側で特別な記述は不要。
    }
}
