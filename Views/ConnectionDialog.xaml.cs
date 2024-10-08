﻿using PropsGen.ViewModels;
using System.Windows;

namespace PropsGen.Views
{
    public partial class ConnectionDialog : Window
    {
        public string DatabaseName { get; private set; } = string.Empty;

        public ConnectionDialog()
        {
            InitializeComponent();

            var connectionVM = new ConnectionViewModel();
            DataContext = connectionVM;

            connectionVM.OnConnectedEvent += OnDatabaseConnected;
            connectionVM.OnCancelEvent += OnCancel;
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
            DatabaseName = databaseName;
            DialogResult = !string.IsNullOrEmpty( databaseName );
        }

        private void OnCancel()
        {
            DialogResult = false;
        }
    }
}
