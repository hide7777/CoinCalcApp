namespace CoinCalcApp.Services.Dialog;

/// <summary>
/// ダイアログ表示の抽象化。ViewModel が Page クラスへ直接依存しないようにするためのもの。
/// (Prism の IPageDialogService の代わり。)
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// アラートダイアログ(キャンセル/閉じるボタンのみ)を表示する。
    /// </summary>
    Task DisplayAlertAsync(string title, string message, string cancel);
}
