namespace CoinCalcApp.Services.Purchase;

/// <summary>
/// 課金に関する定数。プロダクト ID は App Store Connect と Google Play Console の両方で
/// この同じ ID で登録する必要がある。
///
/// 命名規約:Apple は逆ドメイン形式を推奨、Google も同じ ID なら共通管理しやすい。
/// </summary>
internal static class PurchaseConfig
{
    /// <summary>
    /// 「広告を消す」買い切り商品の Product ID。
    /// App Store Connect / Google Play Console の双方で同じ ID で In-App Purchase を作成する。
    /// </summary>
    public const string AdFreeProductId = "jp.auctor.coincalcapp.adfree";

    /// <summary>
    /// AdFree 購入状態をローカルに保存するための Preferences キー。
    /// 起動時に IPurchaseService がこれを読んで初期値とする(オフライン即時判定用)。
    /// </summary>
    public const string AdFreeOwnedKey = "purchase.adFree.owned";
}
