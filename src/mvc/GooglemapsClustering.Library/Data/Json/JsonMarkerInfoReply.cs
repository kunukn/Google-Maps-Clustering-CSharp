using System;
using System.Text;
using GooglemapsClustering.Clustering.Data.Geometry;

namespace GooglemapsClustering.Clustering.Data.Json
{
    public class JsonMarkerInfoReply : JsonReplyBase
    {
        public string Id { get; set; }
        public string Content { get; set; }                
        public int Type  { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }                                

        public void BuildContent(P p)
        {
	        if (p == null)
	        {
				Content = "Marker could not be found";
		        return;
	        }

            Id = p.I.ToString();
            Type = p.T;
            Lat = p.Lat;
            Lon = p.Lon;

            var sb = new StringBuilder();
			sb.AppendLine("<div class='gmcKN-marker-info'>");            
            sb.AppendFormat("Time: {0}<br/>",DateTime.Now.ToString("HH:mm:ss"));
            sb.AppendFormat("Id: {0}<br /> Type: {1}<br />", Id, Type);
            sb.AppendFormat("Lat: {0} Lon: {1}", p.Lat, p.Lon);
            sb.AppendLine("</div>");

            Content = sb.ToString();
        }        
    }
}
