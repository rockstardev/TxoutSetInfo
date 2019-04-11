using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using TxoutSet.Common;

namespace TxoutSet.Publisher.DataHolders
{
    public class AggregatedDataset
    {
        private object _syncLock = new object();
        public Dictionary<string, Dataset> Sets { get; set; } = new Dictionary<string, Dataset>();

        public void Add(string source, TxoutSetInfo set)
        {
            lock (_syncLock)
            {
                var setHash = set.JsonString().ToSHA256();
                if (Sets.ContainsKey(setHash))
                {
                    Sets[setHash].AddSource(source);
                }
                else
                {
                    Sets.Add(setHash, new Dataset(source, set));
                }
            }
        }

        public void Clear()
        {
            lock (_syncLock)
            {
                Sets.Clear();
            }
        }

        internal void Tweetout(Zonfig _zonfig)
        {
            Auth.SetUserCredentials(_zonfig.ConsumerKey, _zonfig.ConsumerSecret, _zonfig.UserAccessToken, _zonfig.UserAccessSecret);

            foreach (var val in Sets.Values)
                tweetResult(val.Set.JsonString(), val.Consensus);
        }

        private void tweetResult(string tweetText, List<string> consensus)
        {
            var res = Tweet.PublishTweet(tweetText);
            //Console.WriteLine(tweetText);

            var consensusTweet = String.Join(", ", consensus);
            Tweet.PublishTweetInReplyTo(consensusTweet, res.Id);
            //Console.WriteLine(consensusTweet);
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
