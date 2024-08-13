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

            var dataContext = new MainViewModel();
            DataContext = dataContext;

            var connectionPrompt = new ConnectionDialog();
            var result = connectionPrompt.ShowDialog() ?? false;

            if ( !result )
            {
                Close();
            }
        }
    }
}
