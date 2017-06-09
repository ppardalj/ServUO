﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

using Parameters = System.Collections.Generic.Dictionary<string, string>;

namespace Server.Engines.RestApi
{
    public class HttpRouter
    {
        private Dictionary<Route, Type> m_RoutingMap;

        public HttpRouter()
        {
            m_RoutingMap = new Dictionary<Route, Type>();
        }

        public void RegisterController(Route route, Type controllerType)
        {
            m_RoutingMap[route] = controllerType;
            Console.WriteLine("Rest Api: Registering route {0}", route);
        }

        public void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                string uri = context.Request.RawUrl.Split('?').First();

                // Select the route that matches the uri
                var route = m_RoutingMap.Keys.FirstOrDefault(r => r.IsMatch(uri));
                if (route == null)
                    throw new NotFoundException("Could not find route");

                // Get the controller
                var controllerType = m_RoutingMap[route];
                var controller = (BaseController)Activator.CreateInstance(controllerType);
                if (controller == null)
                    throw new NotFoundException("Could not instantiate controller");

                // Get the matched parameters
                var parameters = route.GetMatchedParameters(uri);

                // Call the handler
                controller.AccessCheck(parameters, context);
                var response = controller.HandleRequest(parameters, context);

                // Serialize the response
                var jsonResponse = JsonSerialize(response);

                // Write the serialized data into the output stream
                context.Response.ContentType = "application/json";
                byte[] outputBuffer = Encoding.ASCII.GetBytes(jsonResponse);
                context.Response.OutputStream.Write(outputBuffer, 0, outputBuffer.Length);
            }
            catch (NotFoundException e)
            {
                Console.WriteLine("Rest Api: Not found: {0}", context.Request.RawUrl);
                context.Response.StatusCode = 404; // Not found
            }
            catch (AccessDeniedException e)
            {
                Console.WriteLine("Rest Api: Access denied: {0}", e);
                context.Response.StatusCode = 401; // Unauthorized
            }
            catch (Exception e)
            {
                Console.WriteLine("Rest Api: Unexpected error: {0}", e);
                context.Response.StatusCode = 500;
            }
        }

        private string JsonSerialize(object o)
        {
            var sb = new StringBuilder();
            var json = new JavaScriptSerializer();

            json.Serialize(o, sb);

            return sb.ToString();
        }
    }
}