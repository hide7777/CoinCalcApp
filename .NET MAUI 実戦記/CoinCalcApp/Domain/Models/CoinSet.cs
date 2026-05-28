namespace CoinCalcApp.Domain.Models;

/// <summary>
/// 入力された硬貨の枚数セットを表すドメインモデル。
/// 合計金額と合計枚数の派生値を提供する。
/// </summary>
public sealed record CoinSet(
    int One,
    int Five,
    int Ten,
    int Fifty,
    int OneHundred,
    int FiveHundred)
{
    /// <summary>合計金額(円)</summary>
    public int TotalAmount =>
        One * 1
        + Five * 5
        + Ten * 10
        + Fifty * 50
        + OneHundred * 100
        + FiveHundred * 500;

    /// <summary>合計枚数</summary>
    public int TotalCount =>
        One + Five + Ten + Fifty + OneHundred + FiveHundred;
}
