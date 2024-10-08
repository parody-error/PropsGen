﻿using Prism.Commands;
using PropsGen.Services;
using System.Windows;
using System.Windows.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PropsGen.ViewModels
{
    internal class PropsViewModel : ViewModelBase
    {
        private readonly int QUERY_INTERVAL_IN_SECONDS = 2;

        public DelegateCommand GetPropsCommand { get; }
        private bool CanExecuteGetProps => !string.IsNullOrEmpty( DatabaseName ) && EntityID != Guid.Empty;

        public DelegateCommand CopyPropsCommand { get; }
        private bool CanExecuteCopyProps => !string.IsNullOrEmpty( PropsJSON );

        public string DatabaseName { get; set; } = string.Empty;

        private string _propsJSON = string.Empty;
        public string PropsJSON
        {
            get => _propsJSON;
            set
            {
                _propsJSON = value;
                CopyPropsCommand.RaiseCanExecuteChanged();
            }
        }

        private Guid _entityID = Guid.Empty;
        public Guid EntityID
        {
            get => _entityID;
            private set
            {
                if ( _entityID != value )
                {
                    _entityID = value;
                    GetPropsCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _entityName = string.Empty;
        public string EntityName
        {
            get => _entityName;
            private set
            {
                if ( string.Compare( _entityName, value, StringComparison.OrdinalIgnoreCase ) != 0 )
                {
                    _entityName = value;
                    OnPropertyChanged( nameof( EntityName ) );
                }
            }
        }

        private DispatcherTimer? _queryTimer = null;

        public PropsViewModel()
        {
            GetPropsCommand = new DelegateCommand(
                ExecuteGetProps,
                () => CanExecuteGetProps
            );

            CopyPropsCommand = new DelegateCommand(
                ExecuteCopyProps,
                () => CanExecuteCopyProps
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
            if ( !CanExecuteGetProps )
                return;

            var databaseAccessor = DatabaseAccessorFactory.GetDatabaseAccessor();
            if ( databaseAccessor is null )
                return;

            var json = databaseAccessor.GetProps( DatabaseName, EntityID, out string error );
            PropsJSON = !string.IsNullOrEmpty( error ) ? error : json;

            OnPropertyChanged( nameof( PropsJSON ) );
        }

        private void ExecuteCopyProps()
        {
            if ( !CanExecuteCopyProps )
                return;

            Clipboard.SetText( PropsJSON );
        }

        private void UpdateSelectedEntity()
        {
            if ( string.IsNullOrEmpty( DatabaseName ) )
                return;

            var databaseAccessor = DatabaseAccessorFactory.GetDatabaseAccessor();
            if ( databaseAccessor is null )
                return;

            var entity = databaseAccessor.GetLaunchedEntity( DatabaseName, out string error );
            if ( !string.IsNullOrEmpty( error ) )
                return;

            EntityID = entity.EntityID;
            EntityName = entity.EntityName;
        }
    }
}
