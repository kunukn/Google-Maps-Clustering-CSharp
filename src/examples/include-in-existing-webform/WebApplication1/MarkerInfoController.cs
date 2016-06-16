using System.Web.Http;

namespace WebApplication1
{
    public class MarkerInfoController : ApiController
    {
        // GET api/<controller>/3039163
        public dynamic Get(string id)
        {            
            var info = Map.MapService.GetMarkerInfo(id);
            return info;
        }

        

        // POST api/<controller>
        public dynamic Post([FromBody]string id)
        {
            var info = Map.MapService.GetMarkerInfo(id);
            return info;
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