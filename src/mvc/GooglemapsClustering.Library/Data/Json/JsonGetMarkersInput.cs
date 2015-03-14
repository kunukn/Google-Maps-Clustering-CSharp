namespace GooglemapsClustering.Clustering.Data.Json
{
	public class JsonGetMarkersInput
	{
		public string nelat { get; set; }
		public string nelon { get; set; }
		public string swlat { get; set; }
		public string swlon { get; set; }
		public string zoomLevel { get; set; }
		public string filter { get; set; }
		public string w { get; set; } // clientWidth 
		public string h { get; set; } // clientHeight
	}
}
