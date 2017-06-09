using System;

namespace Server.Engines.RestApi
{
    public class AccessDenied : Exception
    {
        public AccessDenied( string message )
            : base( message )
        {
        }
    }
}
