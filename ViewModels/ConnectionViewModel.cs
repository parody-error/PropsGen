using Prism.Commands;

namespace PropsGen.ViewModels
{
    internal class ConnectionViewModel : ViewModelBase
    {
        public delegate void OnConnectedAction( string databaseName );
        public event OnConnectedAction? OnConnectedEvent = null;

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

        //#SB: get from the database
        public ICollection<string> Databases { get; } = new List<string>() { "Red", "Yellow", "Blue" };

        public DelegateCommand ConnectCommand { get; }

        public ConnectionViewModel()
        {
            ConnectCommand = new DelegateCommand(
                ExecuteConnect,
                () => { return !string.IsNullOrEmpty( _databaseName ); }
            );
        }

        private void ExecuteConnect()
        {
            OnConnectedEvent?.Invoke( _databaseName );
        }

        private void UpdateCommandState()
        {
            ConnectCommand.RaiseCanExecuteChanged();
        }
    }
}
