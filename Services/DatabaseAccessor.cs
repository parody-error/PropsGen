using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using PropsGen.Models;

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
                props.basicReservoir = GetReservoirProps( databaseName, entityID );
                props.gas = GetGasProps( databaseName, entityID );
                props.oil = GetOilProps( databaseName, entityID );
                props.water = GetWaterProps( databaseName, entityID );
                props.relativePermeability = GetRelativePermeabilityProps( databaseName, entityID );

                props.parameters = GetParameters();
            }
            catch ( Exception ex )
            {
                error = ex.Message;
            }

            return string.IsNullOrEmpty( error ) ? JsonSerializer.Serialize( props, _jsonSerializerOptions ) : error;
        }

        private static Guid GetLaunchedEntityID( string databaseName )
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

        private static string GetLaunchedEntityName( string databaseName, Guid entityID )
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

        private static ReservoirProps GetReservoirProps( string databaseName, Guid entityID )
        {
            var reservoirProps = new ReservoirProps();

            using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
            {
                connection.Open();

                string query = Queries.RESERVOIR_PROPS;

                var command = new SqlCommand( query, connection );
                command.Parameters.Add( "@entityID", SqlDbType.UniqueIdentifier );
                command.Parameters[ "@entityID" ].Value = entityID;

                using ( var result = command.ExecuteReader() )
                {
                    if ( result is null || !result.HasRows || result.FieldCount != ReservoirProps.FIELD_COUNT )
                        throw new Exception( ERROR_READING_DATA );

                    if ( result.Read() )
                    {
                        int index = 0;

                        reservoirProps.temperature = GetDouble( result, index++, 580.0 );
                        reservoirProps.pressure = GetDouble( result, index++, 3000.0 );
                        reservoirProps.netPay = GetDouble( result, index++, 100.0 );
                        reservoirProps.porosity = GetDouble( result, index++, 0.10 );
                        reservoirProps.gasSaturation = GetDouble( result, index++, 0.10 );
                        reservoirProps.oilSaturation = GetDouble( result, index++, 0.40 );
                        reservoirProps.waterSaturation = GetDouble( result, index++, 0.50 );
                        reservoirProps.initialFormationCompressibility = GetDouble( result, index++, 1e-9 );
                    }
                }

                connection.Close();
            }

            return reservoirProps;
        }

        private static GasProps GetGasProps( string databaseName, Guid entityID )
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

                        gasProps.pvtCorrelation = GetInt( result, index++, 0 );
                        gasProps.viscosityCorrelation = GetInt( result, index++, 0 );
                        gasProps.gasType = GetInt( result, index++, 0 );
                        gasProps.rvCorrelation = GetInt( result, index++, 0 );
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

        private static OilProps GetOilProps( string databaseName, Guid entityID )
        {
            var oilProps = new OilProps();

            using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
            {
                connection.Open();

                string query = Queries.OIL_PROPS;

                var command = new SqlCommand( query, connection );
                command.Parameters.Add( "@entityID", SqlDbType.UniqueIdentifier );
                command.Parameters[ "@entityID" ].Value = entityID;

                using ( var result = command.ExecuteReader() )
                {
                    if ( result is null || !result.HasRows || result.FieldCount != OilProps.FIELD_COUNT )
                        throw new Exception( ERROR_READING_DATA );

                    if ( result.Read() )
                    {
                        int index = 0;

                        oilProps.pvtCorrelation = GetInt( result, index++, 0 );
                        oilProps.viscosityCorrelation = GetInt( result, index++, 0 );
                        oilProps.apiGravity = GetDouble( result, index++, 30.0 );
                        oilProps.initialSaturationPressure = GetDouble( result, index++, 2500.0 );
                        oilProps.initialSolutionGasOilRatio = GetDouble( result, index++, 0.0 );
                    }
                }

                connection.Close();
            }

            return oilProps;
        }

        private static WaterProps GetWaterProps( string databaseName, Guid entityID )
        {
            var waterProps = new WaterProps();

            using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
            {
                connection.Open();

                string query = Queries.WATER_PROPS;

                var command = new SqlCommand( query, connection );
                command.Parameters.Add( "@entityID", SqlDbType.UniqueIdentifier );
                command.Parameters[ "@entityID" ].Value = entityID;

                using ( var result = command.ExecuteReader() )
                {
                    if ( result is null || !result.HasRows || result.FieldCount != WaterProps.FIELD_COUNT )
                        throw new Exception( ERROR_READING_DATA );

                    if ( result.Read() )
                    {
                        int index = 0;

                        waterProps.generalCorrelation = GetInt( result, index++, 0 );
                        waterProps.specificGravity = GetDouble( result, index++, 1.0 );
                        waterProps.salinity = GetDouble( result, index++, 0.0 );
                        waterProps.isSaturated = GetBool( result, index++, false );
                    }
                }

                connection.Close();
            }

            return waterProps;
        }

        private static RelativePermeabilityProps GetRelativePermeabilityProps( string databaseName, Guid entityID )
        {
            var relPermProps = new RelativePermeabilityProps();

            using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
            {
                connection.Open();

                string query = Queries.RELATIVE_PERMEABILITY_PROPS;

                var command = new SqlCommand( query, connection );
                command.Parameters.Add( "@entityID", SqlDbType.UniqueIdentifier );
                command.Parameters[ "@entityID" ].Value = entityID;

                using ( var result = command.ExecuteReader() )
                {
                    if ( result is null || !result.HasRows || result.FieldCount != RelativePermeabilityProps.FIELD_COUNT )
                        throw new Exception( ERROR_READING_DATA );

                    if ( result.Read() )
                    {
                        int index = 0;

                        relPermProps.twoPhaseCorrelation = GetInt( result, index++, 0 );
                        relPermProps.threePhaseCorrelation = GetInt( result, index++, 0 );
                        relPermProps.Swirr = GetDouble( result, index++, 0.2 );
                        relPermProps.Sgc = GetDouble( result, index++, 0.05 );
                        relPermProps.Sorg = GetDouble( result, index++, 0.2 );
                        relPermProps.Sorw = GetDouble( result, index++, 0.2 );
                        relPermProps.krw_Sgc = GetDouble( result, index++, 1.0 );
                        relPermProps.krg_Swirr = GetDouble( result, index++, 1.0 );
                        relPermProps.krg_Sorg = GetDouble( result, index++, 0.5 );
                        relPermProps.kro_Swirr = GetDouble( result, index++, 1.0 );
                        relPermProps.krw_Sorw = GetDouble( result, index++, 0.6 );
                        relPermProps.nw = GetDouble( result, index++, 3.0 );
                        relPermProps.ng = GetDouble( result, index++, 1.5 );
                        relPermProps.nog = GetDouble( result, index++, 2.5 );
                        relPermProps.now = GetDouble( result, index++, 2.0 );
                    }
                }

                connection.Close();
            }

            return relPermProps;
        }

        private static Parameters GetParameters()
        {
            return new Parameters()
            {
                // Populate with some default parameters
                temperature = 530.0,
                pressure = 3000.0
            };
        }

        private static string GetConnectionString( string databaseName )
        {
            return $@"Data Source={DB_INSTANCE};DATABASE={databaseName};Integrated Security=True";
        }

        private static bool GetBool( SqlDataReader? reader, int index, bool defaultValue )
        {
            if ( reader is null )
                return defaultValue;

            return reader.IsDBNull( index ) ? defaultValue : reader.GetInt32( index ) == 1;
        }

        private static int GetInt( SqlDataReader? reader, int index, int defaultValue )
        {
            if ( reader is null )
                return defaultValue;

            return reader.IsDBNull( index ) ? defaultValue : reader.GetInt32( index );
        }

        private static double GetDouble( SqlDataReader? reader, int index, double defaultValue )
        {
            if ( reader is null )
                return defaultValue;

            return reader.IsDBNull( index ) ? defaultValue : reader.GetDouble( index );
        }
    }
}
