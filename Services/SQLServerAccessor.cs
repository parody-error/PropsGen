using PropsGen.Models;
using System.Data.SqlClient;
using System.Text.Json;

namespace PropsGen.Services
{
    internal class SQLServerAccessor : IDatabaseAccessor
    {
        public bool Connect()
        {
            return true;
        }

        public bool Disconnect()
        {
            return true;
        }

        public string GetProps()
        {
            //#SB: populate from database
            var props = new Props();
            props.GasProps.EUR = 1500;
            props.GasProps.S_G = 0.1;
            props.GasProps.H_2_S = 0.5;
            props.GasProps.C_O_2 = 0.4;

            return JsonSerializer.Serialize( props );
        }

        private bool ExecuteCommnd( string sql, out string error )
        {
            error = string.Empty;

            try
            {
                using ( var connection = new SqlConnection( GetConnectionString() ) )
                {
                    connection.Open();

                    connection.Close();
                }
            }
            catch ( Exception ex )
            {
                error = ex.Message;
            }

            return string.IsNullOrEmpty( error );
        }

        private string GetConnectionString()
        {
            //#SB: use member variables
            return $@"Data Source=OBERON\SQLSERVER2022;DATABASE=TestDatabase;Integrated Security=True";
        }
    }
}
