using AppTrackingTransparency;
using CoreFoundation;
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

            // App Tracking Transparency (ATT)
            // ─────────────────────────────────────────────────────────────
            // 本アプリは AdMob でパーソナライズ広告(IDFA)を使用するため、
            // App Store ガイドライン 2.1 上、トラッキングに使うデータを収集する「前」に
            // ATT の許可ダイアログを必ず表示する必要がある。
            //
            // 重要:RequestTrackingAuthorization は、アプリが「アクティブ」状態に
            //       なる前に呼ぶとダイアログが表示されず即時に既定値が返ることがある。
            //       そのため FinishedLaunching 内で即時に呼ばず、わずかに遅延させて
            //       メインキューで実行する。許可結果が確定してから広告 SDK を起動する。
            RequestTrackingAuthorizationThenStartAds();

            return result;
        }

        private void RequestTrackingAuthorizationThenStartAds()
        {
            // iOS 14 未満には ATT が存在しないため、そのまま広告 SDK を起動する。
            if (!OperatingSystem.IsIOSVersionAtLeast(14))
            {
                MobileAds.SharedInstance.Start(_ => { });
                return;
            }

            // アプリがアクティブになるのを待ってからダイアログを要求する(約0.5秒遅延)。
            NSTimer.CreateScheduledTimer(0.5, repeats: false, _ =>
            {
                ATTrackingManager.RequestTrackingAuthorization(status =>
                {
                    // status の値に関わらず広告 SDK は起動する。
                    //   Authorized                        → IDFA 取得可・パーソナライズ広告
                    //   Denied / Restricted / NotDetermined → 非パーソナライズ広告
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        MobileAds.SharedInstance.Start(_ => { });
                    });
                });
            });
        }
    }
}
