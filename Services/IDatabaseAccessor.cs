namespace PropsGen.Services
{
    internal interface IDatabaseAccessor
    {
        IEnumerable<string> GetDatabaseNames( out string error );

        string GetProps( out string error );
    }
}
