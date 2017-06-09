using System;
using System.Net;

using Server;
using Server.Accounting;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
    public abstract class BaseProtectedController : BaseController
    {
        public abstract AccessLevel RequiredAccessLevel { get; }

        public override void AccessCheck(Parameters parameters, HttpListenerContext context)
        {
            var account = GetAccount(context);
            if (account == null)
                throw new AccessDeniedException("Unexistant account");

            var identity = (HttpListenerBasicIdentity)context.User.Identity;
            if (!account.CheckPassword(identity.Password))
                throw new AccessDeniedException("Invalid credentials");

            if (account.AccessLevel < RequiredAccessLevel)
                throw new AccessDeniedException("Insufficient permissions");
        }

        protected IAccount GetAccount(HttpListenerContext context)
        {
            return Accounts.GetAccount(context.User.Identity.Name);
        }
    }
}
