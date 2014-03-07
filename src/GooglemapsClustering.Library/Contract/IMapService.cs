using GooglemapsClustering.Clustering.Data.Json;

namespace GooglemapsClustering.Clustering.Contract
{
    public interface IMapService
    {        
        JsonMarkersReply GetMarkers(JsonGetMarkersInput input);        
        JsonMarkerInfoReply GetMarkerInfo(string id);
        JsonInfoReply Info();
    }
}
