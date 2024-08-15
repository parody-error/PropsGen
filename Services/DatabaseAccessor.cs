using PropsGen.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace PropsGen.Services
{
    internal class DatabaseAccessor : IDatabaseAccessor
    {
        private static readonly string DB_INSTANCE = "(localdb)\\HarmonySQL2019";
        private static readonly string DB_MASTER = "master";

        private static readonly string ERROR_INVALID_DATABASE_NAME = "An invalid database name was supplied.";
        private static readonly string ERROR_INVALID_ENTITY_ID = "An invalid entity ID was supplied.";
        private static readonly string ERROR_READING_DATA = "Could not read props data for supplied entity.";

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

                    string query = Queries.DATABASE_NAMES;

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
                entity.EntityID = GetLaunchedEntityID( databaseName );
                entity.EntityName = GetLaunchedEntityName( databaseName, entity.EntityID );
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
                props.gas = GetGasProps( databaseName, entityID );

                //#SB: other props

                props.parameters = GetParameters();
            }
            catch ( Exception ex )
            {
                error = ex.Message;
            }

            return string.IsNullOrEmpty( error ) ? JsonSerializer.Serialize( props, _jsonSerializerOptions ) : error;
        }

        private Guid GetLaunchedEntityID( string databaseName )
        {
            Guid entityID = Guid.Empty;

            using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
            {
                connection.Open();

                string query = Queries.LAUNCHED_ENTITY_ID;

                var command = new SqlCommand( query, connection );
                using ( var result = command.ExecuteReader() )
                {
                    if ( result is null || !result.HasRows )
                        throw new Exception( ERROR_READING_DATA );

                    if ( result.Read() )
                        entityID = result.GetGuid( 0 );
                }

                connection.Close();
            }

            return entityID;
        }

        private string GetLaunchedEntityName( string databaseName, Guid entityID )
        {
            if ( entityID == Guid.Empty )
                return string.Empty;

            string entityName = string.Empty;

            using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
            {
                connection.Open();

                string query = Queries.LAUNCHED_ENTITY_NAME;

                var command = new SqlCommand( query, connection );
                command.Parameters.Add( "@entityID", SqlDbType.UniqueIdentifier );
                command.Parameters[ "@entityID" ].Value = entityID;

                using ( var result = command.ExecuteReader() )
                {
                    if ( result is null || !result.HasRows )
                        throw new Exception( ERROR_READING_DATA );

                    if ( result.Read() )
                        entityName = result.GetString( 0 );
                }

                connection.Close();
            }

            return entityName;
        }

        private GasProps GetGasProps( string databaseName, Guid entityID )
        {
            var gasProps = new GasProps();

            using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
            {
                connection.Open();

                string query = Queries.GAS_PROPS;

                var command = new SqlCommand( query, connection );
                command.Parameters.Add( "@entityID", SqlDbType.UniqueIdentifier );
                command.Parameters[ "@entityID" ].Value = entityID;

                using ( var result = command.ExecuteReader() )
                {
                    if ( result is null || !result.HasRows || result.FieldCount != GasProps.FIELD_COUNT )
                        throw new Exception( ERROR_READING_DATA );

                    if ( result.Read() )
                    {
                        int index = 0;

                        gasProps.pvtCorrelation = result.GetInt32( index++ );
                        gasProps.viscosityCorrelation = result.GetInt32( index++ );
                        gasProps.gasType = result.GetInt32( index++ );
                        gasProps.rvCorrelation = result.GetInt32( index++ );
                        gasProps.separatorSpecificGravity = GetDouble( result, index++, 0.65 );
                        gasProps.CO2 = GetDouble( result, index++, 0.0 );
                        gasProps.N2 = GetDouble( result, index++, 0.0 );
                        gasProps.H2S = GetDouble( result, index++, 0.0 );
                        gasProps.separatorPressure = GetDouble( result, index++, 100.0 );
                        gasProps.separatorTemperature = GetDouble( result, index++, 530.0 );
                        gasProps.condensateGasRatio = GetDouble( result, index++, 100.0 );
                        gasProps.rvOverRvSat = 1.0;
                    }
                }

                connection.Close();
            }

            return gasProps;
        }

        Parameters GetParameters()
        {
            return new Parameters()
            {
                // Populate with some default parameters
                temperature = 530.0,
                pressure = 3000.0
            };
        }

        private string GetConnectionString( string databaseName )
        {
            return $@"Data Source={DB_INSTANCE};DATABASE={databaseName};Integrated Security=True";
        }

        private double GetDouble( SqlDataReader? dataReader, int index, double defaultValue )
        {
            if ( dataReader is null )
                return defaultValue;

            return dataReader.IsDBNull( index ) ? defaultValue : dataReader.GetDouble( index );
        }
    }
}
