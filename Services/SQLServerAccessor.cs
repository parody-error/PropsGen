using PropsGen.Models;
using System.Data.SqlClient;
using System.Text.Json;

namespace PropsGen.Services
{
    internal class SQLServerAccessor : IDatabaseAccessor
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        public bool Connect()
        {
            return true;
        }

        public bool Disconnect()
        {
            return true;
        }

        //#SB: probably move to a common class.
        public string GetProps( out string error )
        {
            error = string.Empty;

            var props = new Props();

            try
            {
                using ( var connection = new SqlConnection( GetConnectionString() ) )
                {
                    connection.Open();

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
            return $@"Data Source=OBERON\SQLSERVER2022;DATABASE=TestDatabase;Integrated Security=True";
        }
    }
}
