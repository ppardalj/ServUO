using System;

namespace Server.Engines.RestApi
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException(string message)
            : base(message)
        {
        }
    }
}
