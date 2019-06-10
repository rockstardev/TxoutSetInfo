

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TxoutSet.Publisher;

namespace TxoutSet.Tests
{
    public class ServerStarter
    {
        private readonly string _url;
        private IWebHost _webHost;

        public ServerStarter(string url)
        {
            _url = url;

            Client = new HttpClient();
            Client.BaseAddress = new Uri(_url);
        }

        public HttpClient Client { get; }

        public void Start(string configName)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(configName)
                .Build();

            _webHost = WebHost.CreateDefaultBuilder()
                .UseConfiguration(config)
                .UseUrls(_url)
                .UseStartup<StartupTest>()
                .Build();

            _webHost.RunAsync();
        }


        public Task<HttpResponseMessage> PublishTweetMsg(string strBody, string apiKey)
        {
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_url + "/api/publish"),
                Headers = {
                        { "ApiKey", apiKey },
                        { "X-Version", "1" }
                    },
                Content = new StringContent(strBody, Encoding.UTF8, "application/json")
            };

            return Client.SendAsync(req);
        }
    }
}