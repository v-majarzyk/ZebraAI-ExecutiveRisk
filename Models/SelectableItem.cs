// Models/SelectableItem.cs
using ZebraAI_ExecutiveRisk.ViewModels;

namespace ZebraAI_ExecutiveRisk.Models
{
    // A wrapper class to hold an item's value and its selection state.
    // Inherits from BaseViewModel so the UI can be notified when IsSelected changes.
    public class SelectableItem<T> : BaseViewModel
    {
        private bool _isSelected;
        
        public T Value { get; set; }
        
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public SelectableItem(T value, bool isSelected = false)
        {
            Value = value;
            IsSelected = isSelected;
        }
    }
}