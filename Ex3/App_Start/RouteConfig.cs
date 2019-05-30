using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ex3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "Map/{action}/{id}",
                defaults: new { controller = "Map", action = "LocationDisplay", id = UrlParameter.Optional }
                );

            routes.MapRoute("LocationDisplay", "display/{arg1}/{arg2}", 
                defaults: new {controller = "Map", action = "LocationDisplay" });

            routes.MapRoute("RouteDisplay", "display/{arg1}/{arg2}/{arg3}",
                defaults: new { controller = "Map", action = "RouteDisplay" });
        }

    }
}
