namespace Kunukn.GooglemapsClustering.Clustering.Data.Json
{
    public abstract class JsonReplyBase
    {
        public long Msec { get; set; } // server-time msec
        public int Rid { get; set; } // for async mismatch check        
        public string Ok { get; set; } // operation result
        public string EMsg { get; set; } // error message
        
        protected JsonReplyBase()
        {
            Rid = 1;  // ReplyId
            Ok = "1";
            EMsg = string.Empty;
        }
    }
}
