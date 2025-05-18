using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FreakyControlsSample.DTO
{
    public class CollectionDataItem : INotifyPropertyChanged
    {
        private string? _Name;
        private bool _IsVisible;

        public string? Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }

        public bool IsVisible
        {
            get => _IsVisible;
            set => SetProperty(ref _IsVisible, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
