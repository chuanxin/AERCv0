using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AERC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "AERC.Controllers" }
            );
            //routes.MapRoute(
            //    name: "Default",
            //    //url: "{controller}/{action}/{id}",
            //    //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //    url: "{AreaName}/{controller}/{action}/{id}",
            //    defaults: new { controller = "Ctrl", action = "Index" }, namespaces: new string[] { "AERC.Areas.Dry.Controllers", }                
            //);
        }
    }
}