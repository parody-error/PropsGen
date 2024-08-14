using PropsGen.Models;

namespace PropsGen.Services
{
    internal interface IDatabaseAccessor
    {
        IEnumerable<string> GetDatabaseNames( out string error );

        //#SB: supply database name
        Entity GetLaunchedEntity( string databaseName, out string error );

        //#SB: supply database name, entity ID
        string GetProps( string databaseName, out string error );
    }
}
