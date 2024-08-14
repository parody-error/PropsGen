using PropsGen.Models;
using System.Data.SqlClient;
using System.Text.Json;

namespace PropsGen.Services
{
    internal class DatabaseAccessor : IDatabaseAccessor
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        public IEnumerable<string> GetDatabaseNames( out string error )
        {
            error = string.Empty;

            var databaseNames = new List<string>();

            try
            {
                using ( var connection = new SqlConnection( GetConnectionString() ) )
                {
                    connection.Open();

                    string query = @"select name from sys.databases where name not in ('master', 'tempdb', 'model', 'msdb') order by name;";

                    var command = new SqlCommand( query, connection );
                    var result = command.ExecuteReader();

                    if ( result is null || !result.HasRows )
                        return databaseNames;

                    while ( result.Read() )
                    {
                        databaseNames.Add( result.GetString( 0 ) );
                    }

                    connection.Close();
                }
            }
            catch( Exception ex )
            {
                error = ex.Message;
            }

            return databaseNames;
        }

        public string GetLaunchedEntity( out string error )
        {
            error = string.Empty;

            string entityName = string.Empty;

            try
            {
                using ( var connection = new SqlConnection( GetConnectionString() ) )
                {
                    connection.Open();

                    string query = @"select top(1) ENTITY_ID from ENTITY_LOCK_INFO where LOCKED_BY is not null;";

                    var command = new SqlCommand( query, connection );
                    var result = command.ExecuteReader();

                    if ( result is null || !result.HasRows )
                        return entityName;

                    if ( result.Read() )
                    {
                        entityName = result.GetString( 0 );
                    }

                    connection.Close();
                }
            }
            catch ( Exception ex )
            {
                error = ex.Message;
            }

            return entityName;
        }

        public string GetProps( out string error )
        {
            error = string.Empty;

            var props = new Props();

            try
            {
                using ( var connection = new SqlConnection( GetConnectionString() ) )
                {
                    connection.Open();

                    //#SB: probably split up into multiple functions (oil, gas, ....)
                    string query = @"select EUR, S_G, H_2_S, C_O_2 from GAS_PROPERTIES;";

                    var command = new SqlCommand( query, connection );
                    var result = command.ExecuteReader();

                    if( result is null || !result.HasRows || result.FieldCount != Props.FIELD_COUNT )
                        return string.Empty;

                    if ( result.Read() )
                    {
                        props.GasProps.EUR = result.GetDouble( 0 );
                        props.GasProps.S_G = result.GetDouble( 1 );
                        props.GasProps.H_2_S = result.GetDouble( 2 );
                        props.GasProps.C_O_2 = result.GetDouble( 3 );
                    }
                
                    connection.Close();
                }
            }
            catch ( Exception ex )
            {
                error = ex.Message;
            }

            return JsonSerializer.Serialize( props, _jsonSerializerOptions );
        }

        private string GetConnectionString()
        {
            //#SB: use member variables
            return $@"Data Source=.\SQLSERVER2022;DATABASE=TestDatabase;Integrated Security=True";
        }
    }
}
