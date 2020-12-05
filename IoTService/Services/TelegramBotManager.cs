 using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Services
{
    public class TelegramBotManager
    {
        private static string _baseDir => "https://api.telegram.org/bot[TOKEN]";
        private static string _updateWebHookDir => _baseDir + "/setWebhook";
        private static string _sendMessageDir => _baseDir + "/sendMessage";


        public static async Task<bool> UpdateWebHook(string url)
        {
            var response = await HttpClientService.Post(_updateWebHookDir, new { url });
            //string text = await response.Content.ReadAsStringAsync();
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public static async Task<bool> SendMessage(int chat_id , string text, IKeyboard reply_markup = null)
        {
            
            reply_markup = reply_markup ?? IKeyboard.Remove;
            var response = await HttpClientService.Post(_sendMessageDir, new { chat_id , text, reply_markup });
            string content = await response.Content.ReadAsStringAsync();
            return response.StatusCode == System.Net.HttpStatusCode.OK;

        }

        public static async Task<bool> SendMessageSameKeyboard(int chat_id, string text)
        {

            var response = await HttpClientService.Post(_sendMessageDir, new { chat_id, text });
            string content = await response.Content.ReadAsStringAsync();
            return response.StatusCode == System.Net.HttpStatusCode.OK;

        }


    }
}
