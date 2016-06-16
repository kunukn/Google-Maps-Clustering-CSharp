using GooglemapsClustering.Clustering.Data.Repository;
using GooglemapsClustering.Clustering.Service;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace WebApplication1
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);            

            /*
             Google Maps Clustering
            */
            var filepath = string.Concat(HttpContext.Current.Server.MapPath("~"), @"App_Data\Points.csv");
            var memcache = new MemCache();
            var pointsDatabase = new PointsDatabase(memcache, filepath);
            var mapService = new MapService(pointsDatabase, memcache);
            Map.MapService = mapService; // Simulate putting data in session, probably dont do this in production where you put data in static class
            

            RegisterRoutes();

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear(); // use JSON
        }

        private static void RegisterRoutes()
        {
            // Default
            RouteTable.Routes.MapPageRoute("default", "", "~/Default.aspx");

            RouteTable.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = System.Web.Http.RouteParameter.Optional }
            );
        }

        void Application_Error(object sender, EventArgs e)
        {
            var exception = this.Server.GetLastError();
        }

    }
}