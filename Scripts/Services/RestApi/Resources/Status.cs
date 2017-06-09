using System;
using System.Linq;
using System.Net;

using Server;
using Server.Accounting;
using Server.Guilds;
using Server.Network;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
    [Path("/status")]
    public class StatusController : BaseController
    {
        public override object HandleRequest(Parameters parameters, HttpListenerContext context)
        {
            var onlineCount = NetState.Instances.Count;
            var accountCount = Accounts.GetAccounts().Count(a => /*!a.Banned && */a.Count > 0);
            var playerCount = World.Mobiles.Values.Count(m => m.Player);
            var guildCount = Guild.List.Count;

            return new
            {
                Status = new
                {
                    Online = onlineCount,
                    Accounts = accountCount,
                    Characters = playerCount,
                    Guilds = guildCount,
                }
            };
        }
    }
}
