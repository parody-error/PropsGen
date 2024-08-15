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
				using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
				{
					connection.Open();

					GetGasProps( connection, props, entityID, out error );

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

		private Guid GetLaunchedEntityID( string databaseName )
		{
			Guid entityID = Guid.Empty;

			using ( var connection = new SqlConnection( GetConnectionString( databaseName ) ) )
			{
				connection.Open();

				string query = @"select ENTITY_ID from ENTITY_LOCK_INFO where LOCKED_BY is not null;";

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

				string query =
@"select
  COALESCE( W.WELL_NAME, W.DLS, UCG.CUSTOM_GROUP_NAME, E.ENTITY_NAME ) as ENTITY_NAME
from ENTITY E
  left join WELL W on (W.WELL_ID = E.FACILITY_ID)
  left join USER_CUSTOM_GROUPS UCG on (UCG.ENTITY_ID = E.ENTITY_ID)
where
  E.ENTITY_ID = @entityId;";

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

		private bool GetGasProps( SqlConnection connection, Props props, Guid entityID, out string error )
		{
			error = string.Empty;

			try
			{
				string query = @"select EUR, S_G, H_2_S, C_O_2 from GAS_PROPERTIES where ENTITY_ID = @entityID;";

				var command = new SqlCommand( query, connection );
				command.Parameters.Add( "@entityID", SqlDbType.UniqueIdentifier );
				command.Parameters[ "@entityID" ].Value = entityID;

				var result = command.ExecuteReader();

				if ( result is null || !result.HasRows || result.FieldCount != GasProps.FIELD_COUNT )
				{
					error = ERROR_READING_DATA;
					return false;
				}

				if ( result.Read() )
				{
					props.gas.EUR = result.GetDouble( 0 );
					props.gas.S_G = result.GetDouble( 1 );
					props.gas.H_2_S = result.GetDouble( 2 );
					props.gas.C_O_2 = result.GetDouble( 3 );
				}
			}
			catch ( Exception ex )
			{
				error = ex.Message;
			}

			return string.IsNullOrEmpty( error );
		}

		private string GetConnectionString( string databaseName )
		{
			return $@"Data Source={DB_INSTANCE};DATABASE={databaseName};Integrated Security=True";
		}
	}
}
