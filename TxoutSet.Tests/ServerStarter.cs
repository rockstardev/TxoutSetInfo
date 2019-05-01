

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
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
                .UseStartup<Startup>()
                .Build();

            _webHost.RunAsync();
        }
    }
}