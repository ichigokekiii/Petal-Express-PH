using System.Web.Mvc;
using System.Web.Routing;

namespace Petal_Express_PH
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            // Custom route for OrdersController
            routes.MapRoute(
                name: "Orders",
                url: "Orders/{action}/{id}",
                defaults: new { controller = "Orders", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
