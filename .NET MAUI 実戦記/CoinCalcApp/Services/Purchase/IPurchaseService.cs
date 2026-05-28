using Reactive.Bindings;

namespace CoinCalcApp.Services.Purchase;

/// <summary>
/// 課金状態を管理するサービス。
/// 現在は「広告非表示の買い切り」1 種類のみ対応。
///
/// 実装は Step 4 で Plugin.InAppBilling ベースに差し替え予定。
/// 現在は NoOpPurchaseService(永続化なし、常に未購入扱い)。
/// </summary>
public interface IPurchaseService
{
    /// <summary>
    /// 広告非表示版を購入済みか。
    /// 監視可能(変化時に IAdService 等の購読側へ通知される)。
    /// </summary>
    IReadOnlyReactiveProperty<bool> IsAdFree { get; }

    /// <summary>
    /// 広告非表示の買い切りを購入する。
    /// 成功・失敗・キャンセル等の結果を返す。
    /// </summary>
    Task<PurchaseResult> PurchaseAdFreeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 過去の購入を復元する(機種変更・再インストール時に必要)。
    /// Apple のガイドラインで必須機能。
    /// </summary>
    Task<PurchaseResult> RestorePurchasesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 購入処理の結果。
/// </summary>
public enum PurchaseResult
{
    /// <summary>購入成功または復元成功(IsAdFree が true になった)。</summary>
    Success,

    /// <summary>ユーザーが購入ダイアログをキャンセルした。</summary>
    Canceled,

    /// <summary>復元処理を実行したが、対象の購入が見つからなかった。</summary>
    NothingToRestore,

    /// <summary>ストア接続失敗・ネットワーク不通など、技術的なエラー。</summary>
    Failed,
}
