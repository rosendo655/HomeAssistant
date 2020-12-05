using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using IoTService.Models;
using IoTService.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Api.Controller
{
    public class DeviceControlController : EmbedIO.WebApi.WebApiController
    {
        private static Dictionary<int, string> controlTokens = new Dictionary<int, string>();
        public DeviceControlController() : base()
        {

        }

        [Route(HttpVerbs.Get, "/control/{controlId}/{token}")]
        public async Task<int> GetControlValue(int controlId , string token)
        {
            if (controlTokens.ContainsKey(controlId))
            {
                if (controlTokens[controlId] != token)
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return -1;
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return await GetControlByControlId(controlId);
                }
            }
            else
            {
                var keypair = await DataService.ControlQueues.ControlToken(controlId);
                if (keypair.Key == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return -1;
                }
                else if (keypair.Value != token)
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return -1;
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    controlTokens.Add(keypair.Key, keypair.Value);
                    return await GetControlByControlId(controlId);
                }
            }

        }

        private static async Task<int> GetControlByControlId(int controlId)
        {
            ControlQueue controlQueue = await DataService.ControlQueues.FirstControlQueue(controlId);
            if(controlQueue == null)
            {
                return -1;
            }
            await DataService.ControlQueues.DeleteControlQueue(controlQueue);
            return controlQueue.Value;
        }
    }
}
