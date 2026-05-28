using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoinCalcApp.ViewModels
{
    /// <summary>
    /// ViewModel の基底クラス。INotifyPropertyChanged を最小実装で提供する。
    /// (Prism 9.0.537 が MAUI 10 と AdMob プラグイン群との互換性に問題があったため、
    ///  Prism から MAUI Shell ナビゲーションへ移行。Prism.BindableBase / INavigationAware
    ///  などへの依存はこのクラスで吸収する。)
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        private string _title = "default";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// 値が変化したときだけ通知する標準の SetProperty パターン。
        /// </summary>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
