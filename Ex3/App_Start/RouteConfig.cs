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

            routes.MapRoute("Option1", "display/{ip}/{port}", 
                defaults: new {controller = "Data", action = "Option1"});

            routes.MapRoute("Option2", "display/{ip}/{port}/{samples}",
                defaults: new { controller = "Data", action = "Option2"});
        }
    }
}
