namespace CoinCalcApp.Services.Ads;

/// <summary>
/// AdMob 広告ユニット ID のコンパイル時定数。
///
/// Debug ビルド: Google 公式のテスト広告 ID(収益化されない、開発用)
/// Release ビルド: AdMob 管理画面で発行した本番広告ユニット ID
///
/// 切り分けは #if DEBUG で行う。Debug 実機テストや Visual Studio Designer プレビュー時に
/// 本番 ID にリクエストが飛ぶのを避ける目的。Release 時は本番 ID で収益化される。
///
/// 注:AdMob "App ID"(Info.plist / AndroidManifest.xml に書く方)はここでは扱わない。
/// それらは SDK 起動時にプラットフォーム側で静的に読まれるためコードから変更できない。
/// それらの本番値も Release ビルド前に差し替え済みである必要がある。
///   - Platforms/iOS/Info.plist の GADApplicationIdentifier
///   - Platforms/Android/AndroidManifest.xml の com.google.android.gms.ads.APPLICATION_ID
/// </summary>
internal static class AdConfig
{
#if DEBUG
    // ─────────── Google 公式のテスト広告 ID(本番収益化されない、開発用) ───────────
    // 参考: https://developers.google.com/admob/ios/test-ads
    //      https://developers.google.com/admob/android/test-ads
    public const string BannerAdUnitId_iOS         = "ca-app-pub-3940256099942544/2934735716";
    public const string BannerAdUnitId_Android     = "ca-app-pub-3940256099942544/6300978111";

    public const string InterstitialAdUnitId_iOS     = "ca-app-pub-3940256099942544/4411468910";
    public const string InterstitialAdUnitId_Android = "ca-app-pub-3940256099942544/1033173712";
#else
    // ─────────── 本番広告ユニット ID(AdMob 管理画面で発行) ───────────
    // Publisher ID: pub-7063940532480643
    // 取得日: 2026-05-21
    // 「広告配信を制限しています(要審査)」状態でも ID 自体は使用可能。
    // App Store/Google Play のデベロッパーサイト URL が反映されて AdMob クローラが
    // app-ads.txt を検証完了すると「制限つき配信」が解除されてフル収益化される。
    public const string BannerAdUnitId_iOS         = "ca-app-pub-7063940532480643/3882110696";
    public const string BannerAdUnitId_Android     = "ca-app-pub-7063940532480643/2548847720";

    public const string InterstitialAdUnitId_iOS     = "ca-app-pub-7063940532480643/3690539005";
    public const string InterstitialAdUnitId_Android = "ca-app-pub-7063940532480643/4785191884";
#endif

    /// <summary>現在のプラットフォームに合わせたバナー広告 ID。</summary>
    public static string BannerAdUnitId =>
#if IOS
        BannerAdUnitId_iOS;
#elif ANDROID
        BannerAdUnitId_Android;
#else
        ""; // Windows / MacCatalyst では広告非対応
#endif

    /// <summary>現在のプラットフォームに合わせたインタースティシャル広告 ID。</summary>
    public static string InterstitialAdUnitId =>
#if IOS
        InterstitialAdUnitId_iOS;
#elif ANDROID
        InterstitialAdUnitId_Android;
#else
        "";
#endif
}
