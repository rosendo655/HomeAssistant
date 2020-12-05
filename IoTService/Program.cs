using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using IoTService.Api.Controller;
using IoTService.Services;
using Swan.Logging;
using System;

namespace IoTService
{
    class Program
    {
        public static string MainUrl { get; private set; }
        public static string GrafanaUrl { get; private set; }
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            var url = "http://localhost:9696/";
            string webHookUrl = "";
            if (args.Length > 0)
            {
                MainUrl = webHookUrl = args[0];
                GrafanaUrl = args[1];
            }

            // Our web server is disposable.
            using (var server = CreateWebServer(url))
            {
                // Once we've registered our modules and configured them, we call the RunAsync() method.
                server.RunAsync();

                //var browser = new System.Diagnostics.Process()
                //{
                //    StartInfo = new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }
                //};
                //browser.Start();

                var result = TelegramBotManager.UpdateWebHook(webHookUrl + "/api/message").Result;
                // Wait for any key to be pressed before disposing of our web server.
                // In a service, we'd manage the lifecycle of our web server using
                // something like a BackgroundWorker or a ManualResetEvent.
                Console.ReadKey(true);
            }
        }

        // Create and configure our web server.
        private static WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                // First, we will configure our web server by adding Modules.
                .WithLocalSessionManager()
                .WithWebApi("/api", m => m
                    
                    .WithController<TelegramBotController>()
                    .WithController<DataRegisterController>()
                    .WithController<DeviceControlController>()
                )
                
                .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

            // Listen for state changes.
            server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();

            return server;
        }
    }
}
