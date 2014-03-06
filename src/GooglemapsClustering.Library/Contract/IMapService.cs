using GooglemapsClustering.Clustering.Data.Json;

namespace GooglemapsClustering.Clustering.Contract
{
    public interface IMapService
    {
        JsonMarkersReply GetMarkers(string s);
        JsonMarkersReply GetMarkers(JsonGetMarkersInput s);        
        JsonMarkerInfoReply GetMarkerInfo(string s);
        JsonInfoReply Info();
    }
}
