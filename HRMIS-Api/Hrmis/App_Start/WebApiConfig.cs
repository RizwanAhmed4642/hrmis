
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Owin.Security.OAuth;

namespace Hrmis
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
           config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();
            var enableCorsAttribute = new EnableCorsAttribute(ConfigurationManager.AppSettings["hrBase"],
                                                  "Origin, Content-Type, Authorization, Accept",
                                                  "GET, PUT, POST, DELETE, OPTIONS");
            //EnableCorsAttribute enableCorsAttribute = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(enableCorsAttribute);

            // Json Formatter Settings
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
                    = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling
                    = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
