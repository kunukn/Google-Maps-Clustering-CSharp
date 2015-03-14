using System;
using System.Web.Mvc;
using GooglemapsClustering.Clustering.Contract;

namespace GooglemapsClustering.Web.Controllers
{
    public class HomeController : BaseController
    {
	    private readonly IMapService _mapService;

		public HomeController()
		{
			throw new Exception("IoC error");
		}
	    public HomeController(IMapService mapService)
	    {
		    _mapService = mapService;
	    }

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Test()
		{
			return JsonDefault(_mapService.Info());
		}
    }
}
