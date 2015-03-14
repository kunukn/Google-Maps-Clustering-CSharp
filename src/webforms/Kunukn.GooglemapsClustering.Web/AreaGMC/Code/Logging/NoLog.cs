using System;
using System.Reflection;
using Kunukn.GooglemapsClustering.Web.AreaGMC.Code.Contract;

namespace Kunukn.GooglemapsClustering.Web.AreaGMC.Code.Logging
{
    public class NoLog : ILog2
    {       
        public void Error(MethodBase m, string s){}
        public void Error(MethodBase m, Exception e){}
        public void Info(MethodBase m, string s){}        
    }
}