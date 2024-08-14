using PropsGen.Models;

namespace PropsGen.Services
{
    internal interface IDatabaseAccessor
    {
        IEnumerable<string> GetDatabaseNames( out string error );

        Entity GetLaunchedEntity( string databaseName, out string error );

        string GetProps( string databaseName, Guid entityID, out string error );
    }
}
