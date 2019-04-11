using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TxoutSet.Common;
using TxoutSet.Publisher.DataHolders;

namespace TxoutSet.Publisher.HostedServices
{
    public class AggregateHostedService : BaseAsyncService
    {
        public AggregateHostedService(Zonfig zonfig)
        {
            _zonfig = zonfig;
        }

        private readonly Zonfig _zonfig;

        internal override Task[] InitializeTasks()
        {
            return new[]
            {
                CreateLoopTask(GroupDataAndTweet)
            };
        }

        public static AsyncManualResetEvent Signal { get; set; } = new AsyncManualResetEvent();
        public static AggregatedDataset AggregatedData { get; set; } = new AggregatedDataset();
        async Task GroupDataAndTweet()
        {
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(Cancellation))
            {
                try
                {
                    // await until we get signal that data collection started, then wait 60 seconds to group calls
                    await Signal.WaitAsync(Cancellation);
                    await Task.Delay(TimeSpan.FromSeconds(_zonfig.AggregationBeforeSecs), Cancellation);

                    AggregatedData.Tweetout(_zonfig);

                    // timeout after tweet - we won't be accepting data
                    await Task.Delay(TimeSpan.FromSeconds(_zonfig.AggregationAfterSecs), Cancellation);
                    AggregatedData.Clear();
                    Signal.Reset();
                }
                catch (OperationCanceledException) when (timeout.IsCancellationRequested)
                {
                }
            }
        }
    }
}
