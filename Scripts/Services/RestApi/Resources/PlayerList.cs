using System;
using System.Linq;
using System.Net;

using Server.Network;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
    [Path("/players")]
    public class PlayerListController : BaseController
    {
        public override object HandleRequest(Parameters parameters, HttpListenerContext context)
        {
            var clients = NetState.Instances;

            var onlinePlayers = clients
                .Select(client => client.Mobile)
                .Where(m => m != null);

            return new
            {
                Players = onlinePlayers.Select(m => new
                {
                    Player = new
                    {
                        Name = m.Name,
                        Serial = m.Serial.Value,
                    }
                })
            };
        }
    }
}
