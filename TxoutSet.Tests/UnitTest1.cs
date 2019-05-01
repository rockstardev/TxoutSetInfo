using System;
using System.Threading.Tasks;
using Xunit;

namespace TxoutSet.Tests
{
    public class UnitTest1
    {
        public UnitTest1()
        {
            _publisher = new ServerStarter("http://localhost:11001");
            _publisher.Start("publisher.appsettings.json");
        }

        private readonly ServerStarter _publisher;

        [Fact]
        public async Task CanStartServer()
        {
            var resp = await _publisher.Client.GetAsync("/");
            var str = await resp.Content.ReadAsStringAsync();

            Assert.Equal("Ack", str);
        }
    }
}
