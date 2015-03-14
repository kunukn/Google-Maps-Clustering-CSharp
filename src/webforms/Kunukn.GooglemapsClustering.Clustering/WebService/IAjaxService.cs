using System.ServiceModel;
using System.ServiceModel.Web;
using Kunukn.GooglemapsClustering.Clustering.Data.Json;

namespace Kunukn.GooglemapsClustering.Clustering.WebService
{
    /// <summary>
    /// WCF REST
    /// </summary>
    [ServiceContract]    
    public interface IAjaxService
    {
        #region Post

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "MarkerInfo",
            BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        JsonMarkerInfoReply MarkerInfo(string id, int sendid);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Markers",
            BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        JsonMarkersReply Markers(double nelat, double nelon, double swlat, double swlon,
                                 int zoomlevel, string filter, int sendid);
        #endregion Post



        #region Get

        [OperationContract]
        [WebGet(
            UriTemplate = "GetMarkers/{s}",
            ResponseFormat = WebMessageFormat.Json)
        ]        
        JsonMarkersReply GetMarkers(string s);

        [OperationContract]
        [WebGet(
            UriTemplate = "GetMarkerInfo/{s}",
            ResponseFormat = WebMessageFormat.Json)
        ]
        JsonMarkerInfoReply GetMarkerInfo(string s);
      
        [OperationContract]
        [WebGet(
            UriTemplate = "Knn/{s}",
            ResponseFormat = WebMessageFormat.Json)
        ]
        JsonKnnReply Knn(string s);

        // Debug
        [OperationContract]
        [WebGet(
            UriTemplate = "Info",
            ResponseFormat = WebMessageFormat.Json)
        ]
        JsonInfoReply Info();

        #endregion Get
    }

}