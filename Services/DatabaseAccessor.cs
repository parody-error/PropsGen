using PropsGen.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace PropsGen.Services
{
    internal class DatabaseAccessor : IDatabaseAccessor
    {
        private static readonly string DB_INSTANCE = ".\\SQLSERVER2022";
        private static readonly string DB_MASTER = "master";

        private static readonly string ERROR_INVALID_DATABASE_NAME = "An invalid database name was supplied.";
        private static readonly string ERROR_INVALID_ENTITY_ID = "An invalid entity ID was supplied.";

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        public IEnumerable<string> GetDatabaseNames( out string error )
        {
            error = string.Empty;

            var databaseNames = new List<string>();

            try
            {
                using ( var connection = new SqlConnection( GetConnectionString( DB_MASTER ) ) )
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
            catch ( Exception ex )
            {
                error = ex.Message;
            }

            return databaseNames;
        }

        public Entity GetLaunchedEntity( string databaseName, out string error )
        {
            error = string.Empty;

            if ( string.IsNullOrEmpty( databaseName ) )
            {
                error = ERROR_INVALID_DATABASE_NAME;
                return new Entity();
            }

            var entity = new Entity();

            try
            {
                using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
                {
                    connection.Open();

                    string query = @"select top(1) ENTITY_ID, LOCKED_BY from ENTITY_LOCK_INFO where LOCKED_BY is not null;";

                    var command = new SqlCommand( query, connection );
                    var result = command.ExecuteReader();

                    if ( result is null || !result.HasRows )
                        return entity;

                    if ( result.Read() )
                    {
                        entity.EntityID = result.GetGuid( 0 );
                        entity.EntityName = result.GetString( 1 );
                    }

                    connection.Close();
                }
            }
            catch ( Exception ex )
            {
                error = ex.Message;
            }

            return entity;
        }

        public string GetProps( string databaseName, Guid entityID, out string error )
        {
            error = string.Empty;

            if ( string.IsNullOrEmpty( databaseName ) )
            {
                error = ERROR_INVALID_DATABASE_NAME;
                return string.Empty;
            }

            if ( entityID == Guid.Empty )
            {
                error = ERROR_INVALID_ENTITY_ID;
                return string.Empty;
            }

            var props = new Props();

            try
            {
                using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
                {
                    connection.Open();

                    string query = @"select EUR, S_G, H_2_S, C_O_2 from GAS_PROPERTIES where ENTITY_ID = @entityID;";

                    var command = new SqlCommand( query, connection );
                    command.Parameters.Add( "@entityID", SqlDbType.UniqueIdentifier );
                    command.Parameters["@entityID"].Value = entityID;

                    var result = command.ExecuteReader();

                    if ( result is null || !result.HasRows || result.FieldCount != Props.FIELD_COUNT )
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

                // Populate props with some default parameters
                props.parameters.temperature = 530.0;
                props.parameters.pressure = 3000.0;
            }
            catch ( Exception ex )
            {
                error = ex.Message;
            }

            return JsonSerializer.Serialize( props, _jsonSerializerOptions );
        }

        private string GetConnectionString( string databaseName )
        {
            return $@"Data Source={DB_INSTANCE};DATABASE={databaseName};Integrated Security=True";
        }
    }
}
