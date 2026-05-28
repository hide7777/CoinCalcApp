using CoinCalcApp.Domain.Models;

namespace CoinCalcApp.Domain.Services;

public sealed class FeeCalculator : IFeeCalculator
{
    /// <summary>
    /// 「最安ルート」として推奨してよい上限の取引回数。
    /// これを超える(=多くの ATM 投入が必要な)選択肢は最安候補から除外する。
    /// </summary>
    public const int PracticalMaxTransactions = 3;

    private readonly IBankFeeRepository _repository;

    public FeeCalculator(IBankFeeRepository repository)
    {
        _repository = repository;
    }

    public CalculationResult Calculate(CoinSet coins)
    {
        var totalCount  = coins.TotalCount;
        var totalAmount = coins.TotalAmount;

        var fees = _repository.GetAll()
            .Select(rule => rule.Apply(coins))
            .ToList();

        // 枚数 0 のときは推奨を出さない(全行 0 円なので意味がない)
        BankFee? cheapest = null;
        if (totalCount > 0)
        {
            // 取引回数が現実的な選択肢のみを最安候補にする。
            // 例えば 5,060 枚で ATM 51 回投入が「無料だから最安」になるのを防ぐ。
            var practical = fees
                .Where(f => f.RequiredTransactions <= PracticalMaxTransactions)
                .ToList();

            // すべて閾値超過の場合(理論上は窓口があるので発生しないが、念のためフォールバック)
            var candidates = practical.Count > 0 ? practical : fees;

            cheapest = candidates
                .OrderBy(f => f.Fee)
                .ThenBy(f => f.RequiredTransactions)
                .First();
        }

        return new CalculationResult(totalAmount, totalCount, fees, cheapest);
    }
}
