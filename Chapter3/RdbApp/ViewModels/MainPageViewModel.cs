using RdbApp.ViewModels;

namespace RdbApp.ViewModels;

public class MainPageViewModel : BindableBase
{
    /// <summary>
    ///ページサービス保存用の定数
    /// </summary>        
    protected IPageDialogService PageDialogService { get; private set; }

    /// <summary>
    ///コンストラクタ
    /// </summary>        
    public MainPageViewModel(IPageDialogService pageDialogService)
    {
        PageDialogService = pageDialogService;
        CountCommand = new DelegateCommand(OnCountCommandExecuted);
    }

    public string Title => "RdbApp";

    public DelegateCommand CountCommand { get; }

    /// <summary>
    ///ボタン押下時の処理
    /// </summary>        
    private async void OnCountCommandExecuted()
    {
        try {
            //Oracleの場合
            await PageDialogService.DisplayAlertAsync("開始", "ORACLEに接続確認します", "OK");
            OracleClass or = new OracleClass();
            or.execute();
            await PageDialogService.DisplayAlertAsync("結果", "成功しました", "OK");

            //PostgreSQLの場合
            await PageDialogService.DisplayAlertAsync("開始", "PostgreSQLに接続確認します", "OK");
            PostgreSQLClass pg = new PostgreSQLClass();
            pg.select();
            await PageDialogService.DisplayAlertAsync("結果", "成功しました", "OK");

            //SQLServerの場合
            await PageDialogService.DisplayAlertAsync("開始", "SQLServerに接続確認します", "OK");
            SQLServerClass ss = new SQLServerClass();
            ss.select();
            await PageDialogService.DisplayAlertAsync("結果", "成功しました", "OK");

            //MySQLの場合
            await PageDialogService.DisplayAlertAsync("開始", "MySQLに接続確認します", "OK");
            MySQLClass ms = new MySQLClass();
            ms.select();
            await PageDialogService.DisplayAlertAsync("結果", "成功しました", "OK");
        }
        catch (Exception e)
        {
            await PageDialogService.DisplayAlertAsync("異常が発生しました", e.ToString(), "OK");
            System.Diagnostics.Debug.WriteLine("ERROR:" + e.ToString());
        }
    }
}
