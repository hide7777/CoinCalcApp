namespace CoinCalcApp.Domain.Models;

/// <summary>
/// 銀行別・チャネル別(ATM / 窓口)の手数料情報。
/// </summary>
public sealed record BankFee(
    string BankId,
    string DisplayName,
    string Channel,
    int Fee,
    int RequiredTransactions);

/// <summary>
/// 計算結果の集約モデル。
/// CheapestOption は枚数が 0 の場合 null。
/// </summary>
public sealed record CalculationResult(
    int TotalAmount,
    int TotalCount,
    IReadOnlyList<BankFee> BankFees,
    BankFee? CheapestOption);
