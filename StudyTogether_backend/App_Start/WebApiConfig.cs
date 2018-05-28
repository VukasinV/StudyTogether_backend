using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace StudyTogether_backend
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("*", "*", "*");

            // Web API configuration and services
            config.EnableCors(cors);
            config.Filters.Add(new AuthorizeAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            
        }
    }
}
