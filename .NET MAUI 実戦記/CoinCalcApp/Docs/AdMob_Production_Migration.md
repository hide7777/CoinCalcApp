# AdMob 本番リリース移行手順

開発時はテスト広告 ID を使っているため、本番リリース前に以下の差し替えが必要です。
すべて差し替えないと、本番ビルドで広告が出ない or 出ても収益化されません。

## 1. AdMob 管理画面で発行するもの

[AdMob](https://admob.google.com/) でアプリを登録し、以下を発行してください。

- **App ID**(iOS / Android で別)
  - 形式例: `ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy`(`~` で区切る)
- **広告ユニット ID**(各種別 × プラットフォームで別)
  - バナー広告(iOS / Android)
  - インタースティシャル広告(iOS / Android)
  - 形式例: `ca-app-pub-xxxxxxxxxxxxxxxx/yyyyyyyyyy`(`/` で区切る)

## 2. ファイルの差し替え箇所

### (A) `Services/Ads/AdConfig.cs`

`#else` ブロック(Release ビルド用)の 4 つの定数を本番 ID に差し替えてください。

```csharp
#else
    public const string BannerAdUnitId_iOS         = "ca-app-pub-XXXXXXXX/XXXXXXXX"; // 本番
    public const string BannerAdUnitId_Android     = "ca-app-pub-XXXXXXXX/XXXXXXXX";
    public const string InterstitialAdUnitId_iOS     = "ca-app-pub-XXXXXXXX/XXXXXXXX";
    public const string InterstitialAdUnitId_Android = "ca-app-pub-XXXXXXXX/XXXXXXXX";
#endif
```

DEBUG ビルドは引き続きテスト ID を使うのでそのままで OK です。

### (B) `Platforms/iOS/Info.plist`

`GADApplicationIdentifier` のテスト App ID を本番 App ID に置き換える。

```xml
<key>GADApplicationIdentifier</key>
<string>ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY</string>  <!-- 本番 -->
```

### (C) `Platforms/Android/AndroidManifest.xml`

`<meta-data>` の `com.google.android.gms.ads.APPLICATION_ID` を本番 App ID に置き換える。

```xml
<meta-data
    android:name="com.google.android.gms.ads.APPLICATION_ID"
    android:value="ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY" /> <!-- 本番 -->
```

## 3. SKAdNetwork(iOS)の更新

`Platforms/iOS/Info.plist` の `SKAdNetworkItems` 配列には現在 Google の SKAdNetwork ID を 1 件だけ入れています。
ストア審査前に [Google 公式ガイド](https://developers.google.com/admob/ios/quick-start#update_your_infoplist) で最新の SKAdNetwork ID 一覧を確認し、必要に応じて追加してください。
(SKAdNetwork ID は広告ネットワークが指定するもので、定期的に追加・更新されます)

## 4. ATT(App Tracking Transparency)説明文(対応済み)

`Platforms/iOS/Info.plist` に `NSUserTrackingUsageDescription` を **すでに追加済み** です。
Plugin.AdMob 3.0.2 はこのキーの存在を必須としています。

文言は日本ストア向けに以下を使用しています(必要なら差し替えてください):
> 「パーソナライズされた広告の表示にこの識別子を使用します。」

なお、ATT プロンプトを実際に**表示してユーザー同意を取る**コード(`ATTrackingManager.RequestTrackingAuthorization`)
は現在実装していません。これがなくても審査・配信は可能ですが、IDFA が取れないため iOS の eCPM は若干下がります。
将来必要になったら起動シーケンスで追加実装してください。

## 5. UMP(GDPR 同意)対応(EU 配信時のみ)

App Store / Google Play での配信地域を **日本のみ** に絞っている場合は UMP 不要です。
EU 圏も対象にする場合は別途同意フォームを実装する必要があります
(Plugin.AdMob の Consent API、または Google UMP SDK を直接呼ぶ)。

## 6. 動作確認チェックリスト

本番 ID 差し替え後、Release ビルドで以下を確認してください。

- [ ] iOS シミュレータ Release ビルドでバナー広告が表示される(テストデバイス登録すれば実機 Debug も可)
- [ ] Android エミュレータ Release ビルドでバナー広告が表示される
- [ ] 計算ボタン押下 → インタースティシャル広告が表示される(初回起動から 24 時間後 + 1 日 2 回まで)
- [ ] AdMob 管理画面のリアルタイム表示でインプレッションが計上されている
- [ ] 「広告を消す ¥160」購入後(Step 4 完了後)、両ページのバナーとインタースティシャルが完全に出ない

## 7. 余談:なぜ App ID は `#if DEBUG` で切り替えていないか

`GADApplicationIdentifier`(iOS Info.plist) と `APPLICATION_ID`(Android Manifest)は
SDK 起動時にプラットフォーム側で静的に読まれるため、C# コードからは制御できません。

MSBuild プロパティとプレースホルダ置換で Debug/Release を分岐させる方法もありますが、
本アプリでは「テスト App ID で開発 → 本番リリース直前に 1 度だけ書き換え」で十分なため、
複雑な切り替え機構は導入していません。
