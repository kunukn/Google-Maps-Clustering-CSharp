using System;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace GooglemapsClustering.Web.Controllers
{
    public abstract class BaseController : Controller
    {     
        public ContentResult JsonMin(object o)
        {            
            try
            {                                
                return Content(GetJsonMin(o), "application/json", new UTF8Encoding());
            }
            catch (Exception ex)
            {
                return Content(GetJsonDefault(new { Exception = ex.StackTrace }), 
					"application/json", new UTF8Encoding());
            }
        }
        public ContentResult JsonDefault(object o)
        {
            try
            {                                
               return Content(GetJsonDefault(o), "application/json", new UTF8Encoding());                               
            }
            catch (Exception ex)
            {
                return Content(GetJsonDefault(new { Exception = ex.StackTrace }), 
					"application/json", new UTF8Encoding());
            }
        }



        protected static string GetJsonMin(object o)
        {
            return JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            });
        }
        protected static string GetJsonDefault(object o)
        {
            return JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            });
        }        
    }
}