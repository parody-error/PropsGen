using Prism.Commands;
using PropsGen.Services;

namespace PropsGen.ViewModels
{
    internal class PropsViewModel : ViewModelBase
    {
        public DelegateCommand GetPropsCommand { get; }
        public string PropsJSON { get; private set; } = string.Empty;

        public string EntityName { get; private set; } = string.Empty;

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

            var json = _databaseAccessor.GetProps( out string error );
            PropsJSON = !string.IsNullOrEmpty( error ) ? error : json;

            OnPropertyChanged( nameof( PropsJSON ) );
        }
    }
}
