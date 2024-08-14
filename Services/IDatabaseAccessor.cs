using PropsGen.Models;

namespace PropsGen.Services
{
    internal interface IDatabaseAccessor
    {
        IEnumerable<string> GetDatabaseNames( out string error );

        //#SB: supply database name
        Entity GetLaunchedEntity( out string error );

        //#SB: supply database name, entity ID
        string GetProps( out string error );
    }
}
