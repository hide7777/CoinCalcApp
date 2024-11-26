using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoinCalcApp.ViewModels
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) { return false; }
            field = value;
            //�v���p�e�B�ύX��ʒm
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); return true;
        }
    }
}
