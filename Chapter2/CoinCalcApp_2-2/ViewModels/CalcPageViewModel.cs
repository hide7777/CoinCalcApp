using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CoinCalcApp_2.ViewModels;
public class CalcPageViewModel : ViewModelBase
{
    public CalcPageViewModel(IPageDialogService pageDialogService,INavigationService navigationService)
           : base(pageDialogService, navigationService)
    {
        //画面に表示するタイトル名を設定します
        Title = "計算結果";
    }

    ///画面表示される直前の処理
    public override void OnNavigatedTo(INavigationParameters parameters)
    {
        int one = 0;
        int five = 0;
        int ten = 0;
        int fifty = 0;
        int oneHundred = 0;
        int fiveHundred = 0;

        try
        {
            if (parameters.ContainsKey("one"))
            {
                one = Int32.Parse(parameters.GetValue<string>("one"));
            }

            if (parameters.ContainsKey("five"))
            {
                five = Int32.Parse(parameters.GetValue<string>("five"));
            }

            if (parameters.ContainsKey("ten"))
            {
                ten = Int32.Parse(parameters.GetValue<string>("ten"));
            }

            if (parameters.ContainsKey("fifty"))
            {
                fifty = Int32.Parse(parameters.GetValue<string>("fifty"));
            }

            if (parameters.ContainsKey("oneHundred"))
            {
                oneHundred = Int32.Parse(parameters.GetValue<string>("oneHundred"));
            }

            if (parameters.ContainsKey("fiveHundred"))
            {
                fiveHundred = Int32.Parse(parameters.GetValue<string>("fiveHundred"));
            }

            //合計金額
            int goukei_int = (one * 1) + (five * 5) + (ten * 10) + (fifty * 50) + (oneHundred * 100) + (fiveHundred * 500);
            Goukei = goukei_int.ToString();

            //合計枚数
            int maisu_int = one + five + ten + fifty + oneHundred + fiveHundred;
            Maisu = maisu_int.ToString();

            //ゆうちょ銀行ATM手数料の計算
            int atm_rem; //余り
            int atm_div = Math.DivRem(maisu_int, 100, out atm_rem); //商
            int atm_int = (atm_div * 330);
            switch (atm_rem)
            {
                case int n when n == 0:
                    break;
                case int n when n <= 25:
                    atm_int = atm_int + 110;
                    break;
                case int n when n <= 50:
                    atm_int = atm_int + 220;
                    break;
                case int n when n <= 99:
                    atm_int = atm_int + 330;
                    break;
            }
            Atm1 = atm_int.ToString();

            //ゆうちょ銀行窓口手数料の計算
            int mado1_int = 0;
            switch (maisu_int)
            {
                case int n when n <= 100:
                    mado1_int = 0;
                    break;
                case int n when n <= 500:
                    mado1_int = 550;
                    break;
                case int n when n <= 1000:
                    mado1_int = 1100;
                    break;
                default:
                    int mado_rem; //余り
                    int mado_div = Math.DivRem(maisu_int, 500, out mado_rem); //商
                    if (mado_rem == 0)
                    {
                        mado1_int = (550 * mado_div);
                    }
                    else
                    {
                        mado1_int = (550 * mado_div) + 550;
                    }
                    break;
            }
            Mado1 = mado1_int.ToString();

            //三菱UFJ銀行窓口手数料
            int mado2_int = 0;
            switch (maisu_int)
            {
                case int n when n <= 100:
                    mado2_int = 0;
                    break;
                case int n when n <= 500:
                    mado2_int = 550;
                    break;
                case int n when n <= 1000:
                    mado2_int = 1100;
                    break;
                default:
                    int mado_rem; //余り
                    int mado_div = Math.DivRem(maisu_int, 500, out mado_rem); //商
                    if (mado_rem == 0)
                    {
                        mado2_int = (550 * mado_div);
                    }
                    else
                    {
                        mado2_int = (550 * mado_div) + 550;
                    }
                    break;
            }
            Mado2 = mado2_int.ToString();

            //三井住友銀行窓口手数料
            int mado3_int = 0;
            switch (maisu_int)
            {
                case int n when n <= 300:
                    mado3_int = 0;
                    break;
                case int n when n <= 500:
                    mado3_int = 550;
                    break;
                case int n when n <= 1000:
                    mado3_int = 1100;
                    break;
                default:
                    int mado_rem; //余り
                    int mado_div = Math.DivRem(maisu_int, 500, out mado_rem); //商
                    if (mado_rem == 0)
                    {
                        mado3_int = (550 * mado_div);
                    }
                    else
                    {
                        mado3_int = (550 * mado_div) + 550;
                    }
                    break;
            }
            Mado3 = mado3_int.ToString();

            //みずほ銀行窓口手数料
            int mado4_int = 0;
            switch (maisu_int)
            {
                case int n when n <= 300:
                    mado4_int = 0;
                    break;
                case int n when n <= 500:
                    mado4_int = 550;
                    break;
                case int n when n <= 1000:
                    mado4_int = 1320;
                    break;
                default:
                    int mado_rem; //余り
                    int mado_div = Math.DivRem(maisu_int, 500, out mado_rem); //商
                    if (mado_rem == 0)
                    {
                        mado4_int = (660 * mado_div);
                    }
                    else
                    {
                        mado4_int = (660 * mado_div) + 660;
                    }
                    break;
            }
            Mado4 = mado4_int.ToString();
        }
        catch (Exception e)
        {
            PageDialogService.DisplayAlertAsync("ERROR", e.ToString(), "ERROR");
            System.Diagnostics.Debug.WriteLine("ERROR:" + e.ToString());
        }
    }

    //合計金額
    private string _goukei = "";
    public string Goukei
    {
        get { return _goukei; }

        set { SetProperty(ref _goukei, value); }
    }

    //合計枚数
    private string _maisu = "";
    public string Maisu
    {
        get { return _maisu; }
        set { SetProperty(ref _maisu, value); }
    }

    //郵貯銀行ATM
    private string _atm1 = "";
    public string Atm1
    {
        get { return _atm1; }
        set { SetProperty(ref _atm1, value); }
    }

    //郵貯銀行窓口
    private string _mado1 = "";
    public string Mado1
    {
        get { return _mado1; }
        set { SetProperty(ref _mado1, value); }
    }

    //三菱UFJ銀行窓口
    private string _mado2 = "";
    public string Mado2
    {
        get { return _mado2; }
        set { SetProperty(ref _mado2, value); }
    }

    //三井住友銀行窓口
    private string _mado3 = "";
    public string Mado3
    {
        get { return _mado3; }
        set { SetProperty(ref _mado3, value); }
    }

    //みずほ銀行窓口
    private string _mado4 = "";
    public string Mado4
    {
        get { return _mado4; }
        set { SetProperty(ref _mado4, value); }
    }
}
