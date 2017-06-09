using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Server.Engines.RestApi
{
    public class HttpServer : IDisposable
    {
        private readonly HttpListener m_Listener;

        public HttpServer(string domain = "localhost", int port = 8080)
        {
            m_Listener = new HttpListener();
            m_Listener.Prefixes.Add(string.Format(@"http://{0}:{1}/", domain, port));
            m_Listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
        }

        public IEnumerable<string> GetPrefixes()
        {
            return m_Listener.Prefixes;
        }

        public void Start()
        {
            m_Listener.Start();

            Task.Factory.StartNew(() => HandleRequests());
        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            m_Listener.Stop();
            m_Listener.Close();
        }

        private void HandleRequests()
        {
            while (m_Listener.IsListening)
            {
                var context = m_Listener.GetContext();

                Task.Factory.StartNew(() =>
                {
                    // Enqueue call in a timer in order to preserve thread safety, by ensuring it's executed in core thread
                    Timer.DelayCall(() =>
                    {
                        try
                        {
                            HandleRequest(context);
                        }
                        finally
                        {
                            context.Response.OutputStream.Close();
                        }
                    });
                });
            }
        }

        public Action<HttpListenerContext> HandleRequest;
    }
}
