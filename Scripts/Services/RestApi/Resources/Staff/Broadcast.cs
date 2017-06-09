using System;
using System.Net;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
	[Path( "/staff/broadcast" )]
	public class BroadcastController : BaseController
	{
		public override object HandleRequest( Parameters parameters, HttpListenerContext context )
		{
			if ( context.Request.HttpMethod != "POST" )
				throw new NotSupportedException();

			var request = GetRequestData<BroadcastRequest>( context );
			var broadcast = request.Broadcast;

			Commands.CommandHandlers.BroadcastMessage( AccessLevel.Player, 0x482, string.Format( "Staff message from {0}:", broadcast.Name ) );
			Commands.CommandHandlers.BroadcastMessage( AccessLevel.Player, 0x482, broadcast.Message );

			return request;
		}

		private class BroadcastRequest
		{
			public Broadcast Broadcast { get; set; }
		}

		private class Broadcast
		{
			public string Name { get; set; }
			public string Message { get; set; }
		}
	}
}
