using System;
using System.Linq;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
	public class ApiServer
	{
		public static readonly bool Enabled = Config.Get("RestApi.Enabled", true);
		public static readonly string Domain = Config.Get("RestApi.Domain", "localhost");
		public static readonly int Port = Config.Get("RestApi.Port", 8080);

		public static void Initialize()
		{
			EventSink.ServerStarted += new ServerStartedEventHandler( EventSink_ServerStarted );
		}

		private static void EventSink_ServerStarted()
		{
			if ( !Enabled )
				return;

			var httpServer = new HttpServer( Domain, Port );
			var httpRouter = new HttpRouter();
			Weave( httpRouter );

			var apiServer = new ApiServer( httpServer, httpRouter );
			apiServer.Start();
		}

		private static void Weave( HttpRouter router )
		{
			var types = ScriptCompiler.Assemblies.SelectMany(a => ScriptCompiler.GetTypeCache(a).Types);

			foreach ( var type in types )
			{
				var attr = (PathAttribute) type.GetCustomAttributes( typeof( PathAttribute ), false ).FirstOrDefault();

				if ( attr != null )
					router.RegisterController( new Route( attr.Path ), type );
			}
		}

		private HttpServer _server;
		private HttpRouter _router;

		public ApiServer( HttpServer server, HttpRouter router )
		{
			_server = server;
			_router = router;

			// Bind router to the server
			_server.HandleRequest = _router.ProcessRequest;
		}

		public void Start()
		{
			try
			{
				_server.Start();

				foreach ( string prefix in _server.GetPrefixes() )
					Console.WriteLine( "Rest Api: Listening on {0}", prefix );
			}
			catch ( Exception e )
			{
				Console.WriteLine( "Rest Api: Couldn't start: {0}", e );
			}
		}

		public void Dispose()
		{
			Stop();
		}

		public void Stop()
		{
			_server.Stop();

			Console.WriteLine( "Rest Api: Stopped" );
		}
	}
}
