using Prism.Commands;
using PropsGen.Services;

namespace PropsGen.ViewModels
{
    internal class ConnectionViewModel : ViewModelBase
    {
        public delegate void OnConnectedAction( string databaseName );
        public event OnConnectedAction? OnConnectedEvent;

        public delegate void OnCancelAction();
        public event OnCancelAction? OnCancelEvent;

        private string _databaseName = string.Empty;
        public string DatabaseName
        {
            get => _databaseName;
            set
            {
                _databaseName = value;

                UpdateCommandState();
            }
        }

        public ICollection<string> Databases { get; }

        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public ConnectionViewModel()
        {
            var databaseAccessor = DatabaseAccessorFactory.GetDatabaseAccessor();
            Databases = databaseAccessor.GetDatabaseNames( out string error ).ToList();

            _databaseName = Databases.FirstOrDefault( name => !string.IsNullOrEmpty( name ) ) ?? string.Empty;

            ConnectCommand = new DelegateCommand(
                ExecuteConnect,
                () => { return !string.IsNullOrEmpty( _databaseName ); }
            );

            CancelCommand = new DelegateCommand(
                ExecuteCancel,
                () => { return true; }
            );
        }

        private void ExecuteConnect()
        {
            OnConnectedEvent?.Invoke( _databaseName );
        }

        private void ExecuteCancel()
        {
            OnCancelEvent?.Invoke();
        }

        private void UpdateCommandState()
        {
            ConnectCommand.RaiseCanExecuteChanged();
        }
    }
}
