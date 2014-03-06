using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using GooglemapsClustering.Clustering.Utility;

namespace GooglemapsClustering.Web
{    
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Init Google Maps Clustering
            GmcInit.Init(HttpContext.Current.Server.MapPath("~") + @"App_Data\Points.csv");

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
        }

        static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}