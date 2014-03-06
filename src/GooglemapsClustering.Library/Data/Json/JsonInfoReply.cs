using System.Collections.Generic;

namespace GooglemapsClustering.Clustering.Data.Json
{
    public class JsonInfoReply
    {
        public int DbSize { get; set; } // size of p in database
        public IList<P> Points { get; set; }
        
        public JsonInfoReply()
        {
            Points = new List<P>();            
        }          
    }
}
