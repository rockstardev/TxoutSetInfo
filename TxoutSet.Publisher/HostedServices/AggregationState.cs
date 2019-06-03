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
                    var obj = new AggregatedDataset(_zonfig);
                    AggregatedData.Add(set.height, obj);
                }

                AggregatedData[set.height].Add(key, set);
            }
        }

        internal void Cleanup()
        {
            lock (AggregatedData)
            {
                var toRemove = AggregatedData.Where(a => a.Value.Complete).ToList();

                foreach (var rm in toRemove)
                {
                    rm.Value.Dispose();
                    AggregatedData.Remove(rm.Key);
                }
            }
        }
    }
}
