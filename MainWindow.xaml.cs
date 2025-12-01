// MainWindow.xaml.cs
using System.Windows;
using System.Diagnostics;
using System.Windows.Navigation;
using ZebraAI_ExecutiveRisk.ViewModels;

namespace ZebraAI_ExecutiveRisk
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Set the DataContext to an instance of the MainViewModel.
            // This is the CRUCIAL step that connects the XAML (View) 
            // to the C# Logic (ViewModel).
            this.DataContext = new MainViewModel();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
