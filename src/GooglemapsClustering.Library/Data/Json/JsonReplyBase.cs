namespace GooglemapsClustering.Clustering.Data.Json
{
    public abstract class JsonReplyBase
    {
        public string Msec { get; set; } // server-time msec             
        public string Ok { get; set; } // operation result
        public string EMsg { get; set; } // error message        
        public string Debug { get; set; }  // info for developer
        public bool? Cache { get; set; }  // is cached data

        protected JsonReplyBase()
        {            
            Ok = "1";
			//EMsg = string.Empty;
        }
    }
}
