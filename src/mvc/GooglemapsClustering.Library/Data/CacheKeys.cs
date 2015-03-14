namespace GooglemapsClustering.Clustering.Data
{
	public static class CacheKeys
	{
		private const string gmcKN = "gmcKN";

		public static string PointsDatabase = string.Concat(gmcKN, "PointsDatabase");

		public static string GetMarkers(int i)
		{
			return string.Concat(gmcKN, "GetMarkers", i);
		}
		public static string GetMarkerInfo(int i)
		{
			return string.Concat(gmcKN, "GetMarkerInfo", i);
		}
	}
}
