using System.ComponentModel;
using System.Windows;
using System.Windows.Controls; // ColumnDefinition, GridLength
using ZebraAI.Client;
using ZebraAI.Desktop.ViewModels;

namespace ZebraAI.Desktop
{
    public partial class MainWindow : Window
    {
        // Resolve at runtime instead of relying on generated fields
        private ColumnDefinition? _leftCol;
        private ColumnDefinition? _splitterCol;

        public MainWindow()
        {
            InitializeComponent(); // requires MainWindow.xaml Build Action = Page

            // Resolve the named columns from XAML safely
            _leftCol     = FindName("LeftCol")     as ColumnDefinition;
            _splitterCol = FindName("SplitterCol") as ColumnDefinition;

            var vm = new MainViewModel(new MockZebraExperimentClient());
            DataContext = vm;

            // Hide left panel at startup
            ToggleLeftPanel(vm.IsSapFilterOpen);

            vm.PropertyChanged += VmOnPropertyChanged;
        }

        private void VmOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.IsSapFilterOpen) && DataContext is MainViewModel vm)
            {
                ToggleLeftPanel(vm.IsSapFilterOpen);
            }
        }

        private void ToggleLeftPanel(bool open)
        {
            if (_leftCol is null || _splitterCol is null) return;

            if (open)
            {
                // show left panel with a sensible starting width; make splitter column visible
                _leftCol.Width     = new GridLength(360);       // users can drag this via GridSplitter
                _splitterCol.Width = GridLength.Auto;           // take the splitter's actual width
            }
            else
            {
                // hide both left and splitter columns
                _leftCol.Width     = new GridLength(0);
                _splitterCol.Width = new GridLength(0);
            }
        }
    }
}
