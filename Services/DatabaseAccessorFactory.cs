namespace PropsGen.Services
{
    internal static class DatabaseAccessorFactory
    {
        internal static IDatabaseAccessor GetDatabaseAccessor()
        {
            return new SQLServerAccessor();
        }
    }
}
