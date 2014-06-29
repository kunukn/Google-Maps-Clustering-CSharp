using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Data.Json
{
	public class JsonInfoReply : JsonReplyBase
	{
		public int DbSize { get; set; } // count of p in database
		public P FirstPoint { get; set; }	
	}
}
