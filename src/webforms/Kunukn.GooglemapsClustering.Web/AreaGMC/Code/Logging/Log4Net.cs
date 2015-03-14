using System;
using System.Reflection;
using Kunukn.GooglemapsClustering.Web.AreaGMC.Code.Contract;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace Kunukn.GooglemapsClustering.Web.AreaGMC.Code.Logging
{
    public class Log4Net : ILog2
    {       
        public void Error(MethodBase m, string s)
        {
            if (m == null) m = MethodBase.GetCurrentMethod();
            log4net.LogManager.GetLogger(m.DeclaringType).Error(s);            
        }

        public void Error(MethodBase m, Exception e)
        {
            if (m == null) m = MethodBase.GetCurrentMethod();
            log4net.LogManager.GetLogger(m.DeclaringType).Error(e);  
        }

        public void Info(MethodBase m, string s)
        {
            if (m == null) m = MethodBase.GetCurrentMethod();
            log4net.LogManager.GetLogger(m.DeclaringType).Info(s);
        }        
    }
}