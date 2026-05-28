namespace CoinCalcApp.Services.Dialog;

/// <summary>
/// MAUI 標準の Page.DisplayAlert を Shell の現在ページに対して呼び出す実装。
/// </summary>
public sealed class DialogService : IDialogService
{
    public async Task DisplayAlertAsync(string title, string message, string cancel)
    {
        // Shell ナビゲーション環境下では Shell.Current.CurrentPage が最前面の Page。
        // Shell 未初期化の極端なケースでは現在の Window のルート Page にフォールバック。
        // (MAUI 10 で Application.MainPage が [Obsolete] になったため、Windows コレクション経由でアクセス。)
        var page = Shell.Current?.CurrentPage
                   ?? Application.Current?.Windows?.FirstOrDefault()?.Page;

        if (page is null)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[DialogService] No page available to display alert. title='{title}' message='{message}'");
            return;
        }

        // UI スレッドで実行することを保証(ViewModel のバックグラウンドタスクから呼ばれても安全)
        // MAUI 10 で Page.DisplayAlert が非推奨化されたため、DisplayAlertAsync を使う。
        await MainThread.InvokeOnMainThreadAsync(() =>
            page.DisplayAlertAsync(title, message, cancel));
    }
}
