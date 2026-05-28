namespace CoinCalcApp.ViewModels;

/// <summary>
/// CalcPage の銀行手数料行の表示モデル。
/// 1 行 = 1 銀行 1 チャネル。
/// </summary>
public sealed class BankFeeRowVm
{
    /// <summary>「ゆうちょ銀行 ATM」などの行ラベル。</summary>
    public string Label { get; init; } = "";

    /// <summary>「¥6,050」「無料」「¥16,830(51回投入)」などの値表示。</summary>
    public string ValueText { get; init; } = "";

    /// <summary>取引回数が多すぎて非現実的かどうか(将来的なスタイル切替用)。</summary>
    public bool IsImpractical { get; init; }
}
