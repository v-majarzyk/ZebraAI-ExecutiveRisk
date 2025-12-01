// ViewModels/BaseViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZebraAI_ExecutiveRisk.ViewModels
{
    // This class implements INotifyPropertyChanged
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Fix CS8612/CS8618: Mark the event handler as nullable to satisfy the interface.
        public event PropertyChangedEventHandler? PropertyChanged; 

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Fix CS8625/CS8618: Used nameof for clarity, removed unnecessary null checks in SetProperty.
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}