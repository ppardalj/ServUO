﻿using System;
using System.Linq;
using System.Net;

using Server;
using Server.Network;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
	[Path( "/players" )]
	public class PlayerListLocator : BaseLocator
	{
		public override BaseResource Locate( Parameters parameters )
		{
			return new PlayerListResource();
		}
	}

	public class PlayerListResource : BaseResource
	{
		public static BaseResource Acquire( Parameters parameters )
		{
			return new PlayerListResource();
		}

		public PlayerListResource()
		{
		}

		public override object HandleRequest( HttpListenerContext context )
		{
			var clients = NetState.Instances;

			var onlinePlayers = clients
				.Select( client => client.Mobile )
				.Where( m => m != null );

			return new
			{
				Players = onlinePlayers.Select( m => new
				{
					Player = new
					{
						Name = m.Name,
						Serial = m.Serial.Value,
					}
				} )
			};
		}
	}
}
