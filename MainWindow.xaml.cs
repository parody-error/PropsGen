using PropsGen.ViewModels;
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
        }
    }
}
