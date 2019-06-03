using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using TxoutSet.Common;

namespace TxoutSet.Publisher.DataHolders
{
    public class AggregatedDataset : IDisposable
    {
        public AggregatedDataset(Zonfig zonfig)
        {
            _zonfig = zonfig;
        }
        
        private readonly Zonfig _zonfig;
        private object _syncLock = new object();

        public Dictionary<string, TxoutSetInfo> Sets { get; set; } = new Dictionary<string, TxoutSetInfo>();
        public bool Complete { get; set; }

        private CancellationTokenSource _cts;
        private Task _timeoutTask;
        public void Add(string senderKey, TxoutSetInfo set)
        {
            lock (_syncLock)
            {
                if (Sets.ContainsKey(senderKey))
                    Sets[senderKey] = set;
                else
                    Sets.Add(senderKey, set);

                if (Sets.Count == _zonfig.ApiKeys.Count)
                {
                    if (_cts != null)
                        _cts.Cancel();

                    Tweetout();
                }
                else
                {
                    // start task that will tweet after timeout if other results don't arrive
                    if (_timeoutTask == null)
                    {
                        _cts = new CancellationTokenSource();
                        _timeoutTask = tweetAfterTimeout(_cts.Token);
                    }
                }
            }
        }

        private async Task tweetAfterTimeout(CancellationToken cancel)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_zonfig.AggregationBeforeSecs), cancel);
                Tweetout();
            }
            catch (OperationCanceledException) when (cancel.IsCancellationRequested)
            {
            }
        }

        internal void Tweetout()
        {
            Auth.SetUserCredentials(_zonfig.ConsumerKey, _zonfig.ConsumerSecret, _zonfig.UserAccessToken, _zonfig.UserAccessSecret);

            var list = new List<Dataset>();
            foreach (var set in Sets)
            {
                var entry = list.SingleOrDefault(a => a.Set.hash_serialized_2 == set.Value.hash_serialized_2);
                if (entry == null)
                    list.Add(new Dataset(set.Key, set.Value));
                else
                    entry.AddSource(set.Key);
            }

            foreach (var item in list)
            {
                var tweetText = item.Set.JsonString();
                var consensusTweet = String.Join(", ", item.Consensus);

                if (_zonfig.ConsoleTestTweet)
                    consoleResult(tweetText, consensusTweet);
                else
                    tweetResult(tweetText, consensusTweet);
            }

            Complete = true;
        }

        private void tweetResult(string tweetText, string consensusTweet)
        {
            var res = Tweet.PublishTweet(tweetText);
            Tweet.PublishTweetInReplyTo(consensusTweet, res.Id);
        }

        public List<string> LogConsole = new List<string>();

        private void consoleResult(string tweetText, string consensusTweet)
        {
            Console.WriteLine(tweetText);
            LogConsole.Add(tweetText);
            Console.WriteLine(consensusTweet);
            LogConsole.Add(consensusTweet);
        }

        public void Dispose()
        {
            Sets.Clear();
            LogConsole.Clear();
        }
    }


    public static class UtilEx
    {
        public static string ToSHA256(this string str)
        {
            return BitConverter.ToString(Encoding.UTF8.GetBytes(str).ToSHA256()).Replace("-", "");
        }

        public static byte[] ToSHA256(this byte[] bytes)
        {
            using (var hash = new SHA256Managed())
            {
                return hash.ComputeHash(bytes);
            }
        }

        public static string JsonString(this TxoutSetInfo set)
        {
            var res = JsonConvert.SerializeObject(set, Formatting.Indented);
            res = res.Replace("-1", "xXx");

            var tweet = res.Replace("{", "").Replace("}", "").Replace("  ", "").Trim();
            return tweet;
        }
    }
}
