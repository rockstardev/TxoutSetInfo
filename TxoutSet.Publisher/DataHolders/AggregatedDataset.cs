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
        public DateTimeOffset? Completed { get; private set; }
        public long TweetId { get; private set; }

        public DateTimeOffset RoundTimeout { get; set; } = DateTimeOffset.MaxValue;
        public void Add(string senderKey, TxoutSetInfo set)
        {
            lock (_syncLock)
            {
                // already tweeted out this round, now add consensus tweet here
                if (Completed.HasValue)
                {
                    if (!Sets.ContainsKey(senderKey))
                    {
                        Tweet.PublishTweetInReplyTo(senderKey, TweetId);
                    }
                    Sets.Add(senderKey, set);
                    return;
                }


                if (Sets.ContainsKey(senderKey))
                    Sets[senderKey] = set;
                else
                    Sets.Add(senderKey, set);

                if (Sets.Count == _zonfig.ApiKeys.Count)
                {
                    if (RoundTimeout != DateTimeOffset.MaxValue)
                        RoundTimeout = DateTimeOffset.MaxValue;

                    tweetout();
                }
                else
                {
                    // start task that will tweet after timeout if other results don't arrive
                    if (RoundTimeout == DateTimeOffset.MaxValue)
                    {
                        RoundTimeout = DateTimeOffset.UtcNow.AddSeconds(_zonfig.AggregationRoundSecs);
                    }
                }
            }
        }

        public void TimedTweetout()
        {
            lock (_syncLock)
            {
                tweetout();
            }
        }

        private void tweetout()
        {
            var list = new List<Dataset>();
            foreach (var set in Sets)
            {
                var entry = list.SingleOrDefault(a => a.Set.hash_serialized_2 == set.Value.hash_serialized_2);
                if (entry != null)
                    entry.AddSource(set.Key);
                else
                {
                    entry = new Dataset(set.Key, set.Value);
                    list.Add(entry);
                }
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

            Completed = DateTimeOffset.UtcNow;
        }

        private void tweetResult(string tweetText, string consensusTweet)
        {
            Auth.SetUserCredentials(_zonfig.ConsumerKey, _zonfig.ConsumerSecret, _zonfig.UserAccessToken, _zonfig.UserAccessSecret);
            var res = Tweet.PublishTweet(tweetText);
            TweetId = res.Id;
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
