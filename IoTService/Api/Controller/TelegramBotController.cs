using EmbedIO.Routing;
using EmbedIO.WebApi;
using IoTService.Models;
using IoTService.Services;
using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Api.Controller
{
    public class TelegramBotController : EmbedIO.WebApi.WebApiController
    {
        private static Dictionary<int, ChatSession> chats = new Dictionary<int, ChatSession>();

        public TelegramBotController() : base()
        {

        }

        [Route(EmbedIO.HttpVerbs.Post ,"/message")]
        public async Task<dynamic> ProcessMesage([FormData] NameValueCollection content)
        {
            //return "ok";
            var postcontent = content.AllKeys;
            JObject obj = JObject.Parse(postcontent[0]);

            var chat_id = obj["message"]["chat"]["id"].ToObject<int>();
            var text = obj["message"]["text"].ToObject<string>();
            var userName = obj["message"]["from"]["username"].ToObject<string>();

            ChatSession chat_session = null;
            if(chats.ContainsKey(chat_id))
            {
                chat_session = chats[chat_id];
            }
            else
            {
                chat_session = new ChatSession(chat_id,userName);
                chats.Add(chat_id, chat_session);
            }
            chat_session.Dump("chat");

            chat_session.AddMessage(text);

            await ChatSessionHandler.HandleSession(chat_session);

            return "ok";
        }
    }
}
