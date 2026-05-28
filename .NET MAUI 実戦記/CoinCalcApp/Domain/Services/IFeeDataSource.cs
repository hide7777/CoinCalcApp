namespace CoinCalcApp.Domain.Services;

/// <summary>
/// 手数料データ(fees.json)の読み込み元を抽象化する。
/// 現在はアプリ同梱ファイル(BundledFeeDataSource)が実装、
/// 将来は HTTP 経由でリモート取得する RemoteFeeDataSource を追加可能。
/// </summary>
public interface IFeeDataSource
{
    /// <summary>JSON 文字列を返す。失敗時は例外を投げる。</summary>
    Task<string> LoadJsonAsync(CancellationToken cancellationToken = default);
}
