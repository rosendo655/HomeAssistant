using EmbedIO.Routing;
using EmbedIO.WebApi;
using IoTService.Models;
using IoTService.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Api.Controller
{
    public class DataRegisterController : EmbedIO.WebApi.WebApiController
    {

        private static Dictionary<int, string> metricTokens = new Dictionary<int, string>();
        public DataRegisterController() : base()
        {

        }


        [Route(EmbedIO.HttpVerbs.Get, "/sendData/{metricId}/{token}/{value}")]
        public async Task<string> InsertData(int metricId , string token , float value)
        {
            if (metricTokens.ContainsKey(metricId))
            {
                if (metricTokens[metricId] != token)
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else
                {
                    await InsertData(metricId, value);
                    await VerifyWatches(metricId, value);
                }

            }
            else
            {
                var keypair = await DataService.DataRegisters.MetricToken(metricId);
                if (keypair.Key == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else if (keypair.Value != token)
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else
                {
                    metricTokens.Add(keypair.Key, keypair.Value);
                    await InsertData(metricId, value);
                    await VerifyWatches(metricId, value);
                }
            }



            return "OK";
        }

        private static async Task InsertData(int metricId, float value)
        {
            DataStore row = new DataStore()
            {
                Date = DateTime.UtcNow,
                MetricId = metricId,
                Value = value
            };
            await DataService.DataRegisters.InsertDataRegister(row);
        }

        private static async Task VerifyWatches(int metricId, float value)
        {
            IEnumerable<Watch> watches = await DataService.Watches.WatchesByMetric(metricId);

            if (watches.Count() > 0)
            {
                int chatid = await DataService.Users.UserIdByMetric(metricId);
                foreach (Watch watch in watches)
                {
                    if (watch.Sent == 0 && JintService.EvaluateExpression(watch.Expression, value))
                    {
                        await TelegramBotManager.SendMessageSameKeyboard(chatid, watch.Message);
                        watch.Sent = 1;
                        await DataService.Watches.UpdateWatch(watch);
                    }
                }
            }

        }
    }
}
