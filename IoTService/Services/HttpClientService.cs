using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Services
{
    public class HttpClientService
    {
        private static HttpClientService _instance;
        private static HttpClientService Instance => _instance = _instance ?? new HttpClientService();

        private HttpClient client;


        private HttpClientService()
        {
            client = new HttpClient();
        }

        public static Task<HttpResponseMessage> Get(string uri)
        {
            return Instance.client.GetAsync(uri);
        }

        public static Task<HttpResponseMessage> Post<T>(string uri , T data)
        {
            string content = JsonConvert.SerializeObject(data);
            return Instance.client.PostAsync(uri, new StringContent(content, Encoding.UTF8, "application/json"));
        }

    }
}
