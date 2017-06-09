﻿using System;
using System.Net;

using Server.Mobiles;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
    [Path("/players/{id}")]
    public class PlayerController : BaseProtectedController
    {
        public override AccessLevel RequiredAccessLevel { get { return AccessLevel.Player; } }

        public override object HandleRequest(Parameters parameters, HttpListenerContext context)
        {
            Serial serial = Convert.ToInt32(parameters["id"]);
            PlayerMobile pm = World.FindMobile(serial) as PlayerMobile;

            if (pm == null)
                throw new NotFoundException("Player not found");

            var account = GetAccount(context);
            if (account.AccessLevel <= AccessLevel.Player)
            {
                if (account != pm.Account)
                    throw new AccessDeniedException("Cannot see other player details");
            }

            return new
            {
                Player = new
                {
                    Name = pm.Name,
                    Serial = pm.Serial,
                    Fame = pm.Fame,
                    Karma = pm.Karma,
                    Kills = pm.Kills,
                    ShortKills = pm.ShortTermMurders,
                }
            };
        }
    }
}