using System.Collections.Generic;

namespace Kunukn.GooglemapsClustering.Clustering.Data.Json
{
    public class JsonKnnReply : JsonReplyBase
    {
        public List<GmcPDist> Nns { get; set; } // nearest neighbors        
        public string Data { get; set; }
        
        public JsonKnnReply()
        {
            Nns = new List<GmcPDist>();            
            Data = "";
        }
    }
}
