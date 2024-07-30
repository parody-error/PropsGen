using System.Diagnostics;

namespace PropsGen.Services
{
    internal class hldbAccessor : IDatabaseAccessor
    {
        public bool Connect()
        {
            Trace.WriteLine( "Connecting to database" );
            return true;
        }

        public bool Disconnect()
        {
            Trace.WriteLine( "Disconnecting from database" );
            return true;
        }

        public string GetProps( out string error )
        {
            error = string.Empty;
            return "{}";
        }
    }
}
