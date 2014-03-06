using GooglemapsClustering.Clustering.Data.Json;

namespace GooglemapsClustering.Clustering.Contract
{
    public interface IMapService
    {
        JsonMarkersReply GetMarkers(string s);
        JsonMarkerInfoReply GetMarkerInfo(string s);
        JsonInfoReply Info();
    }
}
