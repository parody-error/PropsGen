using PropsGen.ViewModels;
using PropsGen.Views;
using System.Windows;

namespace PropsGen
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var mainVM = new MainViewModel();
            DataContext = mainVM;

            var connectionPrompt = new ConnectionDialog();
            var result = connectionPrompt.ShowDialog() ?? false;

            if ( result )
            {
                mainVM.PropsViewModel.DatabaseName = connectionPrompt.DatabaseName;
            }
            else
            {
                Close();
            }
        }
    }
}
