using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace CoinCalcApp_2.ViewModels
{
    public class ViewModelBase : BindableBase, INavigatedAware
    {
        private string _title="default";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        /// <summary>
        ///ページサービス保存用の定数
        /// </summary>        
        protected IPageDialogService PageDialogService { get; private set; }
        /// <summary>
        ///ナビゲーションサービス保存用の定数
        /// </summary>        
        protected INavigationService NavigationService { get; private set; }
        /// <summary>
        ///コンストラクタ
        /// </summary>        
        public ViewModelBase(IPageDialogService pageDialogService,INavigationService navigationService)
        {
            NavigationService = navigationService;
            PageDialogService = pageDialogService;
        }

        /// <summary>
        ///INavigationServiceのインタフェース実体を定義
        /// </summary>        
        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        /// <summary>
        ///INavigationServiceのインタフェース実体を定義
        /// </summary>        
        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
