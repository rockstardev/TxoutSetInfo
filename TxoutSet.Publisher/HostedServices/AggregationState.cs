using NLog;
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
        public AggregationState(Zonfig zonfig, ILogger logger)
        {
            _zonfig = zonfig;
            _logger = logger;
        }

        private readonly Zonfig _zonfig;
        private readonly ILogger _logger;

        private object _syncLock = new object();
        public Dictionary<int, AggregatedDataset> AggregatedData { get; set; } = new Dictionary<int, AggregatedDataset>();

        internal void AddSet(string key, TxoutSetInfo set)
        {
            lock (_syncLock)
            {
                if (!AggregatedData.ContainsKey(set.height))
                {
                    var obj = Startup.GetService<AggregatedDataset>();
                    obj.Height = set.height;
                    AggregatedData.Add(set.height, obj);
                    _logger.Debug("Creating dataset for block {Block}, initiated by {ApiKeysName}", obj.Height, key);
                }

                AggregatedData[set.height].Add(key, set);
            }
        }

        internal void TimedTweetout()
        {
            lock (_syncLock)
            {
                var toTrigger = AggregatedData.Where(a => a.Value.RoundTimeout < DateTimeOffset.UtcNow).ToList();
                _logger.Info("TimedTweetout will be triggering {RoundsExpired} sets whose RoundTimeout expired", toTrigger.Count);

                foreach (var t in toTrigger)
                    t.Value.TimedTweetout();
            }
        }

        internal void Cleanup()
        {
            lock (_syncLock)
            {
                var cutoff = DateTimeOffset.UtcNow.AddSeconds(_zonfig.CleanupTweetsAfterSecs);
                var toRemove = AggregatedData.Where(a => a.Value.Completed < cutoff).ToList();

                if (toRemove.Count > 0)
                    _logger.Info("Removing {SetsCount} sets during cleanup", toRemove.Count);

                foreach (var rm in toRemove)
                {
                    rm.Value.Dispose();
                    AggregatedData.Remove(rm.Key);
                }
            }
        }
    }
}
