using GooglemapsClustering.Clustering.Data.Json;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace WebApplication1
{
    public class MarkersController : ApiController
    {
        

        // GET api/<controller>/
        public dynamic Get([ModelBinder]JsonGetMarkersInput input)
        {            
            var result = Map.MapService.GetMarkers(input);
            return result;
        }

        // POST api/<controller>
        //public dynamic Post([FromBody]JsonGetMarkersInput input)
        public dynamic Post([ModelBinder]JsonGetMarkersInput input)
        {
            // http://stackoverflow.com/questions/10863499/asp-net-web-api-model-binding-not-working-like-it-does-in-mvc-3
            var result = Map.MapService.GetMarkers(input);
            return result;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}