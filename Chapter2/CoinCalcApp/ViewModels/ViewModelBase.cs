using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace CoinCalcApp.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, IInitializeAsync, IDestructible, INavigatedAware
    {
        protected IRegionNavigationService RegionNavigationService { get; private set; } = null!;
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionNavigationService = navigationContext.NavigationService;
            System.Diagnostics.Debug.WriteLine("▲OnNavigatedTo▲");
            var flag = (bool?)navigationContext.Parameters["IsNewOpen"];
            if (flag == true)
            {
                /* 新しく開いたとき */
                System.Diagnostics.Debug.WriteLine("▲New Open▲");
            }
            else
            {
                /* 戻ってきたとき */
                System.Diagnostics.Debug.WriteLine("▲Come Back▲");
            }
        }
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            System.Diagnostics.Debug.WriteLine("▲OnNavigatedFrom▲");
        }


        protected INavigationService NavigationService { get; private set; }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
        
        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            System.Diagnostics.Debug.WriteLine("▲OnNavigatedFrom▲");
            //throw new NotImplementedException();
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            System.Diagnostics.Debug.WriteLine("▲OnNavigatedTo▲");

            //throw new NotImplementedException();
        }
        

        Task IInitializeAsync.InitializeAsync(INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

        void IInitialize.Initialize(INavigationParameters parameters)
        {
            //throw new NotImplementedException();
        }

        void IDestructible.Destroy()
        {
            //throw new NotImplementedException();
        }
    }
}
