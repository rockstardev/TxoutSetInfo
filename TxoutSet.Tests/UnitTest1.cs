using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TxoutSet.Tests
{
    public class UnitTest1
    {
        public UnitTest1()
        {
            _publisher = new ServerStarter(PUBLISHER_ROOT);
            _publisher.Start("publisher.appsettings.json");
        }

        private const string PUBLISHER_ROOT = "http://localhost:11001";
        private readonly ServerStarter _publisher;

        [Fact]
        public async Task CanStartServer()
        {
            var resp = await _publisher.Client.GetAsync("/");
            var str = await resp.Content.ReadAsStringAsync();

            Assert.Equal("Ack", str);
        }

        [Fact]
        public async Task CanPublishTweets()
        {
            var strBody = "{\"height\":570093,\"bestblock\":\"00000000000000000026960d36e9ffe255e4bde8656a843cea2f32612b1f4b12\",\"transactions\":28714080,\"txouts\":52713092,\"hash_serialized_2\":\"914d9ebf51eac4b5875e87dc2a8ebb0c17fa188dfe4984d3416b20d9a03578fa\",\"total_amount\":17625979.82662823}";

            var resp1234 = await _publisher.PublishTweetMsg(strBody, "1234");
            Assert.True(resp1234.IsSuccessStatusCode);
            var resp5678 = await _publisher.PublishTweetMsg(strBody, "5678");
            Assert.True(resp5678.IsSuccessStatusCode);
            var resp9012 = await _publisher.PublishTweetMsg(strBody, "9012");
            Assert.True(resp9012.IsSuccessStatusCode);

            await Task.Delay(500);

            var console = await _publisher.Client.GetStringAsync("/api/ZTest?height=570093");
            var arr = JsonConvert.DeserializeObject<string[]>(console);
            Assert.Equal("\"height\": 570093,\r\n\"bestblock\": \"00000000000000000026960d36e9ffe255e4bde8656a843cea2f32612b1f4b12\",\r\n\"transactions\": 28714080,\r\n\"txouts\": 52713092,\r\n\"hash_serialized_2\": \"914d9ebf51eac4b5875e87dc2a8ebb0c17fa188dfe4984d3416b20d9a03578fa\",\r\n\"total_amount\": 17625979.82662823", arr[0]);
            Assert.Equal("@testdev (0.17.1-win64), @testdev (0.16.3-win64), @testwiz (0.15)", arr[1]);
        }

        [Fact]
        public async Task CanPublishTweetWithTimeout()
        {
            var strBody = "{\"height\":570092,\"bestblock\":\"00000000000000000026960d36e9ffe255e4bde8656a843cea2f32612b1f4b12\",\"transactions\":28714080,\"txouts\":52713092,\"hash_serialized_2\":\"914d9ebf51eac4b5875e87dc2a8ebb0c17fa188dfe4984d3416b20d9a03578fa\",\"total_amount\":17625979.82662823}";

            var resp1234 = await _publisher.PublishTweetMsg(strBody, "1234");
            Assert.True(resp1234.IsSuccessStatusCode);
            var resp5678 = await _publisher.PublishTweetMsg(strBody, "5678");
            Assert.True(resp5678.IsSuccessStatusCode);

            // tester checks if server is still waiting for round to finish
            await Task.Delay(300);
            var arr = await _publisher.ConsoleMessages(570092);
            Assert.Empty(arr);

            // now that timeout expired tester checks if consensus is tweeted out
            await Task.Delay(3000);
            arr = await _publisher.ConsoleMessages(570092);
            Assert.Equal("\"height\": 570092,\r\n\"bestblock\": \"00000000000000000026960d36e9ffe255e4bde8656a843cea2f32612b1f4b12\",\r\n\"transactions\": 28714080,\r\n\"txouts\": 52713092,\r\n\"hash_serialized_2\": \"914d9ebf51eac4b5875e87dc2a8ebb0c17fa188dfe4984d3416b20d9a03578fa\",\r\n\"total_amount\": 17625979.82662823", arr[0]);
            Assert.Equal("@testdev (0.17.1-win64), @testdev (0.16.3-win64)", arr[1]);
        }
    }
}
