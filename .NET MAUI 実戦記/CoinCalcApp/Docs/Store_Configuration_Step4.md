# Step 4 課金実装:ストア登録手順

このドキュメントは、Plugin.InAppBilling 代替の Microsoft 公式パターンで実装した課金機能を
動作させるために、App Store Connect と Google Play Console で行う必要のある作業をまとめたものです。

## 共通プロダクト ID

iOS / Android 共通で以下の Product ID を使用します(`Services/Purchase/PurchaseConfig.cs` で定義):

```
jp.auctor.coincalcapp.adfree
```

両ストアで**完全に同じ ID**で登録してください(大文字小文字も含めて完全一致)。

---

## 1. App Store Connect 側の設定(iOS)

### 1.1 In-App Purchase の作成

1. https://appstoreconnect.apple.com にログイン
2. 「**マイ App**」→「**CoinCalcApp**」を選択
3. 左サイドメニューの「**App 内課金**」→「**管理**」をクリック
4. 「**+**」ボタンをクリックして新規作成
5. 「**消耗型ではない**」(Non-Consumable)を選択 → 「作成」
6. 以下のように入力:

   | 項目 | 値 |
   |---|---|
   | 参照名 | `Remove Ads`(管理用、ユーザーには見えない) |
   | **製品 ID** | `jp.auctor.coincalcapp.adfree` |
   | 価格(価格 Tier) | **¥160**(Tier 1 か、カスタム価格で 160 円を選択) |

7. 「**App 内課金の詳細**」の **「審査メモ」** に以下を記入(審査用):
   ```
   This in-app purchase removes the banner and interstitial ads from the app.
   The app remains fully functional without this purchase; only the ads are hidden.
   No additional features are unlocked.
   ```

8. 「**ローカリゼーション**」(言語ごとの表示名):
   - **日本語**:
     - 表示名:`広告を消す`
     - 説明:`アプリ内に表示される広告(バナー広告とインタースティシャル広告)を非表示にします。`
   - **English (US)**(必須、ストア審査用):
     - Display Name: `Remove Ads`
     - Description: `Hide all ads (banner and interstitial) in the app.`

9. 「**Review Screenshot**」(審査用スクリーンショット)をアップロード:
   - 「広告を消す ¥160」ボタンが見える画面のスクリーンショット(計算結果ページ)
   - 推奨:1242 × 2208 ピクセル以上

10. 「**保存**」をクリック

### 1.2 課金契約・税務情報の確認

「App 内課金」を有効化するには、有料アプリ契約(Paid Applications Agreement)に同意していることが必須です。

1. App Store Connect トップ → 「**ビジネス**」または「**契約 / 税金 / 銀行情報**」
2. **Paid Applications Agreement** がアクティブになっていることを確認
3. 銀行情報・税務情報も全てアクティブに

### 1.3 Sandbox テスター作成

実機・シミュレータでテスト購入するためには、サンドボックステスター用の Apple ID が必要です。

1. App Store Connect → **ユーザーとアクセス** → **サンドボックス** → **テスター**
2. 「**+**」で新規追加
3. 通常の Apple ID と被らないメールアドレスを使う(例:`coincalc-test1@auctor.jp`)
4. パスワードを設定(8 文字以上、要件あり)
5. **国**を「日本」に
6. 作成完了

### 1.4 実機でのテスト購入

1. iPhone の **設定 → App Store → Sandbox アカウント** にサインイン
2. アプリをビルド・実行
3. 「広告を消す ¥160」ボタンタップ
4. Sandbox の購入シートが出る(「[環境: Sandbox]」と表記される)
5. **実際の課金は発生しない**

---

## 2. Google Play Console 側の設定(Android)

### 2.1 アプリの事前準備

In-App Product を登録するためには、アプリが Google Play Console に**少なくとも内部テストトラックにアップロード済み**である必要があります。

1. https://play.google.com/console にログイン
2. 「**CoinCalcApp**」を選択(未作成なら新規作成)
3. 「**テスト**」→「**内部テスト**」で AAB をアップロード
   - **必ず Release ビルドの署名済み AAB**(Debug ビルドでは課金フローがテストできない)
4. 内部テスト用テスターのメールアドレスを「メーリングリスト」に登録

### 2.2 In-App Product の作成

1. 左サイドメニューの「**収益化**」→「**アプリ内アイテム**」→「**管理対象のアイテム**」
2. 「**アイテムを作成**」ボタン
3. 以下のように入力:

   | 項目 | 値 |
   |---|---|
   | **製品 ID** | `jp.auctor.coincalcapp.adfree` |
   | 名前 | `広告を消す` |
   | 説明 | `アプリ内に表示される広告(バナー広告とインタースティシャル広告)を非表示にします。` |
   | デフォルト価格 | **¥160** |

4. 「**保存**」→ ステータスを「**有効**」に変更

### 2.3 ライセンステスター登録

実機でテスト購入するために、テスト用 Google アカウントを登録します。

1. Google Play Console トップ → 左下「**設定**」(歯車アイコン)
2. **ライセンス テスト** を選択
3. テスト用 Gmail アドレスを追加(自分のメインアカウントでも OK)
4. **ライセンス応答**:`RESPOND_NORMALLY` のままでよい

### 2.4 実機でのテスト購入

1. 内部テストトラックにアップロードした AAB を、テスト用 Google アカウントの端末で**Play Store 経由でインストール**(オプトイン URL でインストール必須)
2. アプリ起動
3. 「広告を消す ¥160」ボタンタップ
4. Google Play の購入シートが出て **「これはテスト購入です。請求は発生しません。」** と表示される
5. 購入完了 → 広告が消える

**重要**:Android では `adb install` などのサイドロードではなく、必ず Play Store 経由でインストールしないと購入フローが動作しません。

---

## 3. 動作確認チェックリスト

### iOS

- [ ] App Store Connect で In-App Purchase が「審査の準備が完了」状態
- [ ] Sandbox テスター作成済み
- [ ] 実機 iPhone で Sandbox アカウントにサインイン
- [ ] アプリで「広告を消す ¥160」タップ
- [ ] [環境: Sandbox] と表示された購入シートが出る
- [ ] 購入完了 → バナーとインタースティシャルが消える
- [ ] アプリを再起動 → IsAdFree 状態が保持されている
- [ ] 「購入の復元」タップ → 「広告非表示版が復元されました」アラート

### Android

- [ ] 内部テストトラックに AAB をアップロード済み
- [ ] アプリ内アイテムが「有効」状態
- [ ] ライセンステスターを登録
- [ ] テスター端末で Play Store 経由でインストール
- [ ] アプリで「広告を消す ¥160」タップ
- [ ] 「テスト購入」と表示された購入シートが出る
- [ ] 購入完了 → バナーとインタースティシャルが消える
- [ ] アプリを再起動 → IsAdFree 状態が保持されている
- [ ] 「購入の復元」タップ → 「広告非表示版が復元されました」アラート

---

## 4. 本番リリース前の最終チェック

- [ ] iOS:「App Review Information」に「審査メモ」と「Review Screenshot」が登録済み
- [ ] iOS:「Paid Applications Agreement」がアクティブ
- [ ] Android:「アプリの完全性」(App Signing)が Play App Signing で有効
- [ ] 両ストア:価格 ¥160 で確定
- [ ] AdMob:本番 App ID と広告ユニット ID に差し替え(`Docs/AdMob_Production_Migration.md` 参照)

---

## 5. テスト時のよくあるトラブル

### iOS:「製品が見つからない」エラー

- App Store Connect の Product ID と `PurchaseConfig.AdFreeProductId` が完全一致しているか確認
- In-App Purchase が「準備完了」または「承認済み」状態か
- Bundle ID(`jp.auctor.coincalcapp`)が App Store Connect 上のアプリと一致しているか

### Android:「アイテムは購入できません」エラー

- AAB が**内部テストトラックにアップロード済み**であり、かつ**処理が完了**しているか(数時間かかる場合あり)
- Product ID が完全一致しているか
- テスター端末が**ライセンステスター**として登録されているか
- インストールが **Play Store 経由**か(`adb install` ではダメ)
- アプリの **signingConfig** が AAB と同じ署名であること(Debug ビルドだと Play 経由でも動かない)

### iOS Sandbox:Apple ID を切り替えても反映されない

- 設定 → App Store → Sandbox アカウントを一度サインアウト → 再サインイン
- アプリを完全終了してから起動し直す

### Android:購入後に「広告が消えない」

- BillingClient の接続タイミングを確認
- `MarkAdFreeOwned()` が呼ばれた後、UI スレッドで IsAdFree が更新されているか
- Acknowledge が走っているか(走らないと 3 日後に自動返金される)
