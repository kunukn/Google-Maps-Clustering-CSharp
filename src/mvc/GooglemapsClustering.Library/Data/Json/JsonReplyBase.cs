using Newtonsoft.Json;

namespace GooglemapsClustering.Clustering.Data.Json
{
    public abstract class JsonReplyBase
    {
        public string Msec { get; set; } // server-time msec             
        public string Ok { get; set; } // operation result
        public string EMsg { get; set; } // error message        
        public string Debug { get; set; }  // info for developer
		
		[JsonProperty(Order = -2)] // make appear first in json response		
        public bool? Cache { get; set; }  // is cached data

        protected JsonReplyBase()
        {            
            Ok = "1";			
        }
    }
}
