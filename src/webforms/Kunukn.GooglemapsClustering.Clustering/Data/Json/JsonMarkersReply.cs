using System.Collections.Generic;

namespace Kunukn.GooglemapsClustering.Clustering.Data.Json
{
    public class JsonMarkersReply : JsonReplyBase
    {                 
        public IList<P> Markers { get; set; } // markers or clusters
        public IList<Line> Polylines { get; set; } // google map draw lines
        public int Count {get { return Markers.Count;} set { }} // returned n markers
        public int Mia { get; set; } // truncated markers due to json restriction (missing in action)
        
        public JsonMarkersReply()
        {
            Markers = new List<P>();
            Polylines = new List<Line>();            
        }
    }
}
