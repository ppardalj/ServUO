using System;
using System.Net;

using Server.Accounting;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
    [Path("/accounts/{username}/summary")]
    public class AccountSummaryController : BaseProtectedController
    {
        public override AccessLevel RequiredAccessLevel { get { return AccessLevel.Player; } }

        public override object HandleRequest(Parameters parameters, HttpListenerContext context)
        {
            var username = parameters["username"];
            var acct = Accounts.GetAccount(username) as Account;

            if (acct == null)
                throw new NotFoundException("Account not found");

            var myAccount = GetAccount(context);
            if (myAccount.AccessLevel <= AccessLevel.Player && myAccount != acct)
                throw new AccessDeniedException("Cannot see other player account summary");

            string email = acct.GetTag("email");
            if (email == null)
                email = "";

            var age = DateTime.Now - acct.Created;

            return new
            {
                AccountSummary = new
                {
                    Username = acct.Username,
                    Email = email,
                    //Created = acct.Created.ToUnixTime(), // TODO: add created date
                    Gametime = (int)acct.TotalGameTime.TotalSeconds,
                    Age = (int)age.TotalSeconds
                }
            };
        }
    }
}
