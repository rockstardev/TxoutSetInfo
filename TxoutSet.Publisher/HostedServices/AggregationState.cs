using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TxoutSet.Common;
using TxoutSet.Publisher.DataHolders;

namespace TxoutSet.Publisher.HostedServices
{
    public class AggregationState
    {
        public AggregationState(Zonfig zonfig)
        {
            _zonfig = zonfig;
        }

        private readonly Zonfig _zonfig;

        public Dictionary<int, AggregatedDataset> AggregatedData { get; set; } = new Dictionary<int, AggregatedDataset>();

        internal void AddSet(string key, TxoutSetInfo set)
        {
            lock (AggregatedData)
            {
                if (!AggregatedData.ContainsKey(set.height))
                {
                    var obj = Startup.GetService<AggregatedDataset>();
                    obj.Height = set.height;
                    AggregatedData.Add(set.height, obj);
                }

                AggregatedData[set.height].Add(key, set);
            }
        }

        internal void TimedTweetout()
        {
            lock (AggregatedData)
            {
                var toTrigger = AggregatedData.Where(a => a.Value.RoundTimeout < DateTimeOffset.UtcNow);
                foreach (var t in toTrigger)
                    t.Value.TimedTweetout();
            }
        }

        internal void Cleanup()
        {
            lock (AggregatedData)
            {
                var cutoff = DateTimeOffset.UtcNow.AddSeconds(-_zonfig.CleanupTweetsAfterSecs);
                var toRemove = AggregatedData.Where(a => a.Value.Completed < cutoff).ToList();

                foreach (var rm in toRemove)
                {
                    rm.Value.Dispose();
                    AggregatedData.Remove(rm.Key);
                }
            }
        }
    }
}
