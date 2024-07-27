using Prism.Commands;
using PropsGen.Services;

namespace PropsGen.ViewModels
{
    internal class ConnectionViewModel : ViewModelBase
    {
        public bool IsConnected => _databaseAccessor is not null;

        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand DisconnectCommand { get; }

        private IDatabaseAccessor? _databaseAccessor = null;

        public ConnectionViewModel()
        {
            ConnectCommand = new DelegateCommand(
                ExecuteConnect,
                () => { return !IsConnected; }
            );

            DisconnectCommand = new DelegateCommand(
                ExecuteDisconnect,
                () => { return IsConnected; }
            );
        }

        private void ExecuteConnect()
        {
            _databaseAccessor = DatabaseAccessorFactory.GetDatabaseAccessor();
            if ( !_databaseAccessor.Connect() )
                _databaseAccessor = null;

            UpdateCommandState();
        }

        private void ExecuteDisconnect()
        {
            _databaseAccessor?.Disconnect();
            _databaseAccessor = null;

            UpdateCommandState();
        }

        private void UpdateCommandState()
        {
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        }
    }
}
