using System.Web.Mvc;
using GooglemapsClustering.Clustering.Contract;
using GooglemapsClustering.Clustering.Data.Json;
using GooglemapsClustering.Clustering.Service;

namespace GooglemapsClustering.Web.Controllers
{
    public class JsonController : BaseController
    {
        private readonly IMapService _mapService; // Clustering API

        public JsonController()
        {
            _mapService = new MapService();
        }

        public ActionResult Index()
        {
            return Json("json service", JsonRequestBehavior.AllowGet);
        }

        public ContentResult GetMarkers(string id)
        {            
            JsonMarkersReply data = _mapService.GetMarkers(id);            
            return JsonMin(data);            
        }

        public ActionResult GetMarkerInfo(string id)
        {
            JsonMarkerInfoReply data = _mapService.GetMarkerInfo(id);
            return JsonMin(data);            
        }        
    }
}
