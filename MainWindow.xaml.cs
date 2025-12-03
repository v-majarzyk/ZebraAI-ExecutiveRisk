// MainWindow.xaml.cs
using System.Windows;
using System.Diagnostics;
using System.Windows.Navigation;
using ZebraAI_ExecutiveRisk.ViewModels;
using System.ComponentModel;
using System.Windows.Controls; 
// Removed using System.Windows.Media.Animation;

namespace ZebraAI_ExecutiveRisk
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Set the DataContext to an instance of the MainViewModel.
            this.DataContext = new MainViewModel();
        }

        // Handles the hyperlink click logic
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}