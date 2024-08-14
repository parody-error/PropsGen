using Prism.Commands;
using PropsGen.Services;
using System.Windows.Threading;

namespace PropsGen.ViewModels
{
    internal class PropsViewModel : ViewModelBase
    {
        private readonly int QUERY_INTERVAL_IN_SECONDS = 2;

        public DelegateCommand GetPropsCommand { get; }

        public string DatabaseName { get; set; } = string.Empty;
        public string PropsJSON { get; private set; } = string.Empty;

        private string _entityID = string.Empty;
        public string EntityID
        {
            get => _entityID;
            private set
            {
                if( string.Compare( _entityID, value, StringComparison.OrdinalIgnoreCase ) != 0)
                {
                    _entityID = value;
                    OnPropertyChanged( nameof( EntityID ) );
                }
            }
        }

        private string _entityName = string.Empty;
        public string EntityName
        {
            get => _entityName;
            private set
            {
                if( string.Compare( _entityName, value, StringComparison.OrdinalIgnoreCase ) != 0 )
                {
                    _entityName = value;
                    OnPropertyChanged( nameof( EntityName ) );
                }
            }
        }

        private IDatabaseAccessor? _databaseAccessor = null;
        private DispatcherTimer? _queryTimer = null;

        public PropsViewModel()
        {
            GetPropsCommand = new DelegateCommand(
                ExecuteGetProps,
                () => { return true; }
            );

            StartTimer();
        }

        private void StartTimer()
        {
            _queryTimer = new DispatcherTimer();
            _queryTimer.Interval = TimeSpan.FromSeconds( QUERY_INTERVAL_IN_SECONDS );
            _queryTimer.Tick += QueryTimerTick;
            _queryTimer.Start();
        }

        private void QueryTimerTick( object? sender, EventArgs e )
        {
            UpdateSelectedEntity();
        }

        private void ExecuteGetProps()
        {
            if ( string.IsNullOrEmpty( DatabaseName ) )
                return;

            //#SB: refactor this, this probably belongs in some shared class, or not a member
            _databaseAccessor = DatabaseAccessorFactory.GetDatabaseAccessor();
            if ( _databaseAccessor is null )
                return;

            var json = _databaseAccessor.GetProps( out string error );
            PropsJSON = !string.IsNullOrEmpty( error ) ? error : json;

            OnPropertyChanged( nameof( PropsJSON ) );
        }

        private void UpdateSelectedEntity()
        {
            //#SB: put common checks in a helper function
            if ( string.IsNullOrEmpty( DatabaseName ) )
                return;

            _databaseAccessor = DatabaseAccessorFactory.GetDatabaseAccessor();
            if( _databaseAccessor is null )
                return;

            var entity = _databaseAccessor.GetLaunchedEntity( out string error );
            if ( !string.IsNullOrEmpty( error ) )
                return;

            EntityID = entity.EntityID;
            EntityName = entity.EntityName;
        }
    }
}
