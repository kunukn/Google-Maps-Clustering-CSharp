namespace GooglemapsClustering.Clustering.Data.Json
{
	public class JsonInfoReply
	{
		public int DbSize { get; set; } // count of p in database
		public P FirstPoint { get; set; }

		public override string ToString()
		{
			return string.Format("DbSize {0}, FirstPoint {1}", DbSize, FirstPoint);
		}
	}
}
