using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GTDoro.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "MainMenu",
                url: "{action}",
                defaults: new { controller = "Layout"},
                constraints: new { action = @"^Admin|Dashboard|Calendar|Reports|Review|MyAccount|Help" }
            );

            routes.MapRoute(
                name: "PTA",
                url: "{controller}/{id}",
                defaults: new { controller = "Home", action = "Details", id = UrlParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Full",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Regular",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index"}
            );

            routes.MapRoute(
               name: "404-PageNotFound",
               url: "{*url}",
               defaults: new { controller = "Base", action = "Http404" }
            );
        }
    }
}
