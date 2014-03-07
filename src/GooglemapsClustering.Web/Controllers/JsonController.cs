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

        public JsonResult Index()
        {
            return Json("json service", JsonRequestBehavior.AllowGet);
        }

        public ContentResult GetMarkers(JsonGetMarkersInput input)
        {
            return JsonMin(_mapService.GetMarkers(input));
        }

        public ContentResult GetMarkerInfo(string id)
        {            
            return JsonMin(_mapService.GetMarkerInfo(id));
        }

        public ContentResult Info(string id)
        {            
            return JsonMin(_mapService.Info());
        }
    }
}
