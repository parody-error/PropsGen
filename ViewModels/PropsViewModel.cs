using Prism.Commands;
using PropsGen.Services;
using System.Diagnostics;

namespace PropsGen.ViewModels
{
    internal class PropsViewModel : ViewModelBase
    {
        public DelegateCommand GetPropsCommand { get; }

        private IDatabaseAccessor? _databaseAccessor = null;

        public PropsViewModel()
        {
            GetPropsCommand = new DelegateCommand(
                ExecuteGetProps,
                () => { return true; }
            );
        }

        private void ExecuteGetProps()
        {
            //#SB: refactor this, this probably belongs in some shared class.
            _databaseAccessor = DatabaseAccessorFactory.GetDatabaseAccessor();

            Trace.WriteLine( _databaseAccessor.GetProps() );
        }
    }
}
