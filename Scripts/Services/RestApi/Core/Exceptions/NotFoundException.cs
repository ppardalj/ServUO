using System;

namespace Server.Engines.RestApi
{
    public class NotFoundException : Exception
    {
        public NotFoundException( string message )
            : base( message )
        {
        }
    }
}
