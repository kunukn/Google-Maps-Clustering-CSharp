using System;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using Kunukn.GooglemapsClustering.Clustering.Utility;
using Kunukn.GooglemapsClustering.Web.AreaGMC.Code.Contract;
using Kunukn.GooglemapsClustering.Web.AreaGMC.Code.Logging;

namespace Kunukn.GooglemapsClustering.Web
{
    /// <summary>
    /// Author: Kunuk Nykjaer
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        private static ILog2 _log;

        void Application_Start(object sender, EventArgs e)
        {
            _log = new NoLog(); // Log4Net();
           _log.Info(MethodBase.GetCurrentMethod(), "Init");

            // Init Google Maps Clustering
            GmcInit.Init(HttpContext.Current.Server.MapPath("~") + @"\AreaGMC\Files\Points.csv");
           
            RegisterRoutes();
        }

        private static void RegisterRoutes()
        {
            // Default
            RouteTable.Routes.MapPageRoute("default", "", "~/Default.aspx");              
        }

        void Application_End(object sender, EventArgs e)
        {            
        }

        void Application_Error(object sender, EventArgs e)
        {
            var ex = this.Server.GetLastError();

            if (ex.Message == "File does not exist.")
            {
                var msg = string.Format("{0} {1} {2}", 
                    ex.Message, HttpContext.Current.Request.Url, ex.StackTrace);
                _log.Error(MethodBase.GetCurrentMethod(), msg);                
            }
            else
            {
                _log.Error(MethodBase.GetCurrentMethod(), ex);
            }            
        }
    
        protected void Application_BeginRequest(object sender, EventArgs e)
        {            
        }        
    }
}