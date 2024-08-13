namespace PropsGen.Services
{
    internal interface IDatabaseAccessor
    {
        //#SB: probably don't actually need these.
        bool Connect();
        bool Disconnect();

        IEnumerable<string> GetDatabaseNames();

        string GetProps( out string error );
    }
}
