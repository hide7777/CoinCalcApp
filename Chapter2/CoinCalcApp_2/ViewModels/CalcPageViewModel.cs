using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinCalcApp_2.ViewModels;
public class CalcPageViewModel : ViewModelBase
{
    public CalcPageViewModel(IPageDialogService pageDialogService,INavigationService navigationService)
           : base(pageDialogService, navigationService)
    {
        //画面に表示するタイトル名を設定します
        Title = "計算結果";
    }
}
