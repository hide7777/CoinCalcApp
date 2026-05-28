using CoinCalcApp.Domain.Rules;

namespace CoinCalcApp.Domain.Services;

/// <summary>
/// 銀行手数料ルールのリポジトリ。
/// 現在はインメモリ実装だが、Phase 1b 以降はリモート JSON 取得型に差し替える。
/// </summary>
public interface IBankFeeRepository
{
    IReadOnlyList<BankFeeRule> GetAll();
}
