using NBitcoin;
using NBitcoin.RPC;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TxoutSet.Common;

namespace TxoutSet.Fetcher
{
    class Program
    {
        private static Zonfig _zonfig;
        static void Main(string[] args)
        {
            var read = File.ReadAllText("zonfig.json");
            _zonfig = JsonConvert.DeserializeObject<Zonfig>(read);

            var uri = new Uri(_zonfig.BitcoindUri);
            var cred = RPCCredentialString.Parse(_zonfig.BitcoindCred);
            var netw = Network.GetNetwork(_zonfig.BitcoindNetwork);

            var rpcClient = new RPCClient(cred, uri, netw);

            var blocks = rpcClient.GetBlockchainInfo();

            if (blocks.Headers > blocks.Blocks)
            {
                Console.WriteLine("Bitcoind still syncing, please try again later...");
                return;
            }

            var resTxoutset = rpcClient.GetTxoutSetInfo();

            var res = new TxoutSetInfo
            {
                bestblock = resTxoutset.Bestblock,
                hash_serialized_2 = resTxoutset.HashSerialized2,
                height = resTxoutset.Height,
                total_amount = resTxoutset.TotalAmount,
                transactions = resTxoutset.Transactions,
                txouts = resTxoutset.Txouts
            };

            if (res == null)
            {
                Console.WriteLine("Can't connect to bitcoind");
            }
            else
            {
                var strBody = JsonConvert.SerializeObject(res);
                //var strBody = "{\"height\":570092,\"bestblock\":\"00000000000000000026960d36e9ffe255e4bde8656a843cea2f32612b1f4b12\",\"transactions\":28714080,\"txouts\":52713092,\"hash_serialized_2\":\"914d9ebf51eac4b5875e87dc2a8ebb0c17fa188dfe4984d3416b20d9a03578fa\",\"total_amount\":17625979.82662823}";
                Console.WriteLine($"Sending to publisher:\n{strBody}");

                //using (var httpClientHandler = new HttpClientHandler())
                //{
                //    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                //    using (var client = new HttpClient(httpClientHandler))
                //    {
                //        var resp = sendTxoutSetInfo(client, strBody);
                //        Console.WriteLine($"Sent to Publisher:\n{strBody}");
                //    }
                //}
            }

            if (_zonfig.ReadlineAtExit)
                Console.ReadLine();
        }


        private static HttpResponseMessage sendTxoutSetInfo(HttpClient client, string strBody)
        {
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_zonfig.PublisherUrl),
                Headers = {
                        { "ApiKey", _zonfig.PublisherApiKey },
                        { "X-Version", "1" }
                    },
                Content = new StringContent(strBody, Encoding.UTF8, "application/json")
            };
            var resp = client.SendAsync(req).Result;
            return resp;
        }
    }
}