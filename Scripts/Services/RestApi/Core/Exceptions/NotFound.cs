using System;

namespace Server.Engines.RestApi
{
    public class NotFound : Exception
    {
        public NotFound( string message )
            : base( message )
        {
        }
    }
}
