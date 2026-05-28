using CoinCalcApp.Domain.Models;

namespace CoinCalcApp.Domain.Services;

/// <summary>
/// 硬貨セットに対し全銀行・全チャネルの手数料を一括算出するサービス。
/// </summary>
public interface IFeeCalculator
{
    CalculationResult Calculate(CoinSet coins);
}
