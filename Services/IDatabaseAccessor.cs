namespace PropsGen.Services
{
    internal interface IDatabaseAccessor
    {
        bool Connect();
        bool Disconnect();

        string GetProps( out string error );
    }
}
