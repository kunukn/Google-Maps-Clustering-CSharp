using System;

namespace Kunukn.GooglemapsClustering.Clustering.Utility
{
    public static class ExceptionUtil
    {
        public static string GetException(Exception ex)
        {
            return string.Format("Msg:{0}\nStacktrace:{1}\nInnerExc:{2}", 
                ex.Message, ex.StackTrace, ex.InnerException);
        }
    }
}
