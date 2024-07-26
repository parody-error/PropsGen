using Prism.Commands;

namespace PropsGen.ViewModels
{
    internal class ConnectionViewModel : ViewModelBase
    {
        public bool IsConnected { get; private set; } = false;

        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand DisconnectCommand { get; }

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
            IsConnected = true;
            UpdateCommandState();
        }

        private void ExecuteDisconnect()
        {
            IsConnected = false;
            UpdateCommandState();
        }

        private void UpdateCommandState()
        {
            ConnectCommand.RaiseCanExecuteChanged();
            DisconnectCommand.RaiseCanExecuteChanged();
        }
    }
}
