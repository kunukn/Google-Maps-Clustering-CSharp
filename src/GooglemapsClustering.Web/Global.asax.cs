using System.Web.Mvc;
using System.Web.Routing;
using System.Web;

namespace GooglemapsClustering.Web
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RegisterRoutes(RouteTable.Routes);

			// DI Container
			new Bootstrap().Configure(filePathToPoints:
				string.Concat(HttpContext.Current.Server.MapPath("~"), @"App_Data\Points.csv"));
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