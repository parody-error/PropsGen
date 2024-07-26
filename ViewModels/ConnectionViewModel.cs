using PropsGen.Commands;
using System.Diagnostics;
using System.Windows.Input;

namespace PropsGen.ViewModels
{
    internal class ConnectionViewModel : ViewModelBase
    {
        public ICommand OnConnectCommand { get; private set; }

        public ConnectionViewModel()
        {
            OnConnectCommand = new DelegateCommand( OnConnect, OnCanConnect );
        }

        private void OnConnect(object? foo)
        {
            Trace.WriteLine( "Connected!" );
        }

        private bool OnCanConnect(object? foo)
        {
            return true;
        }
    }
}
