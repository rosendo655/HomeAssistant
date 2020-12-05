using EmbedIO;
using EmbedIO.Routing;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Api.Controller
{
    public class IoTDataController : EmbedIO.WebApi.WebApiController
    {
        public IoTDataController():base()
        {
            
        }

        [Route(HttpVerbs.Get, "/data")]
        public async Task<string> GetData()
        {
            return "Hola mundo";
        }
    }
}
