using CoinCalcApp.Domain.Models;

namespace CoinCalcApp.Domain.Rules;

/// <summary>
/// 銀行手数料計算ルールの基底レコード。
/// 各銀行・チャネルの計算方式は本クラスを継承して実装する。
/// 将来的に外部 JSON からの読み込みに切り替える際もこの抽象は維持する。
/// </summary>
public abstract record BankFeeRule(string BankId, string DisplayName, string Channel)
{
    /// <summary>
    /// 1 回の取引で投入できる最大枚数(通常硬貨)。null = 制限なし(窓口を想定)。
    /// ATM は通常 100 枚程度の制限がある。
    /// </summary>
    public int? MaxCoinsPerTransaction { get; init; }

    /// <summary>
    /// 1 回の取引で投入できる 500 円硬貨の最大枚数。
    /// null の場合は MaxCoinsPerTransaction と同じ扱い。
    /// 例:三井住友銀行 ATM は通常 100 枚だが 500 円硬貨は 75 枚まで。
    /// </summary>
    public int? MaxFiveHundredYenPerTransaction { get; init; }

    /// <summary>指定枚数に対する手数料(円)を返す。</summary>
    public abstract int Calculate(int coinCount);

    /// <summary>
    /// 必要な取引回数(投入/受付の回数)を返す。
    /// 制限なし(窓口)の場合は枚数があれば 1、0 枚なら 0。
    /// 500 円硬貨に独自制限がある場合は分離して計算する。
    /// </summary>
    public int CalculateRequiredTransactions(CoinSet coins)
    {
        if (coins.TotalCount == 0) return 0;
        if (MaxCoinsPerTransaction is null) return 1; // 窓口は1回でまとめて受付

        var max = MaxCoinsPerTransaction.Value;
        var max500 = MaxFiveHundredYenPerTransaction ?? max;

        if (max500 == max)
        {
            // 500 円硬貨に独自制限なし → 全硬貨をまとめて計数
            return Ceil(coins.TotalCount, max);
        }

        // 500 円硬貨に独自制限あり → 500 円とそれ以外を分けて投入する想定
        var nonFiveHundred = coins.TotalCount - coins.FiveHundred;
        var transactions = 0;
        if (nonFiveHundred > 0) transactions += Ceil(nonFiveHundred, max);
        if (coins.FiveHundred > 0) transactions += Ceil(coins.FiveHundred, max500);
        return transactions;
    }

    /// <summary>計算結果を BankFee として返す。</summary>
    public BankFee Apply(CoinSet coins) =>
        new BankFee(
            BankId,
            DisplayName,
            Channel,
            Calculate(coins.TotalCount),
            CalculateRequiredTransactions(coins));

    private static int Ceil(int numerator, int denominator) =>
        (numerator + denominator - 1) / denominator;
}

/// <summary>
/// 「N 枚以下なら手数料 F 円」という段階を表す。
/// </summary>
public sealed record FeeTier(int MaxCoins, int Fee);

/// <summary>
/// 段階的手数料(枚数の範囲ごとに固定額、超過分は定数バッチで加算)。
/// 主に銀行窓口の手数料計算で利用。
/// </summary>
public sealed record TieredCounterFeeRule(
    string BankId,
    string DisplayName,
    string Channel,
    IReadOnlyList<FeeTier> Tiers,
    int BatchSize,
    int BatchFee
) : BankFeeRule(BankId, DisplayName, Channel)
{
    public override int Calculate(int coinCount)
    {
        foreach (var tier in Tiers)
        {
            if (coinCount <= tier.MaxCoins)
            {
                return tier.Fee;
            }
        }

        var div = Math.DivRem(coinCount, BatchSize, out var rem);
        return rem == 0
            ? BatchFee * div
            : BatchFee * (div + 1);
    }
}

/// <summary>
/// ゆうちょ銀行 ATM 方式。
/// 100 枚単位でバッチ手数料、100 枚未満の端数は段階別に加算する。
/// </summary>
public sealed record YuchoAtmFeeRule(
    string BankId,
    string DisplayName,
    string Channel,
    int BatchSize,
    int BatchFee,
    IReadOnlyList<FeeTier> RemainderTiers
) : BankFeeRule(BankId, DisplayName, Channel)
{
    public override int Calculate(int coinCount)
    {
        var div = Math.DivRem(coinCount, BatchSize, out var rem);
        var fee = div * BatchFee;
        foreach (var tier in RemainderTiers)
        {
            if (rem <= tier.MaxCoins)
            {
                return fee + tier.Fee;
            }
        }
        return fee;
    }
}

/// <summary>
/// 常に無料(三菱UFJ・三井住友・みずほの ATM 等)。
/// </summary>
public sealed record FreeFeeRule(
    string BankId,
    string DisplayName,
    string Channel
) : BankFeeRule(BankId, DisplayName, Channel)
{
    public override int Calculate(int coinCount) => 0;
}
