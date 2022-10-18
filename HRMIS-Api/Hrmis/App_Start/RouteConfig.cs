using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Hrmis
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("api/{*pathInfo}"); // ignore api routes 
          
            routes.MapPageRoute("NonAPIRoute", "{*url}", "~/wwwroot/index.html");
           
            //routes.MapRoute(
            //    "DefaultHost",
            //    "{action}",
            //     new { controller = "Home", action = "Index" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "", action = "", id = UrlParameter.Optional }
            );


        }
    }
}
