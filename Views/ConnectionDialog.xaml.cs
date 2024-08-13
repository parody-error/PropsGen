using PropsGen.ViewModels;
using System.Windows;

namespace PropsGen.Views
{
    public partial class ConnectionDialog : Window
    {
        public ConnectionDialog()
        {
            InitializeComponent();

            var connectionVM = new ConnectionViewModel();
            DataContext = connectionVM;

            connectionVM.OnConnectedEvent += OnDatabaseConnected;
        }

        public void Dispose()
        {
            var connectionVM = DataContext as ConnectionViewModel;
            if ( connectionVM != null )
            {
                connectionVM.OnConnectedEvent -= OnDatabaseConnected;
            }
        }

        private void OnDatabaseConnected( string databaseName )
        {
            DialogResult = !string.IsNullOrEmpty( databaseName );
        }
    }
}
