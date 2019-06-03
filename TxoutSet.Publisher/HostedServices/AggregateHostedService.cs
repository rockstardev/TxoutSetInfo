﻿using Nito.AsyncEx;
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
        public AggregateHostedService(AggregationState state, Zonfig zonfig)
        {
            _state = state;
            _zonfig = zonfig;
        }

        private readonly AggregationState _state;
        private readonly Zonfig _zonfig;

        internal override Task[] InitializeTasks()
        {
            return new[]
            {
                CreateLoopTask(GroupDataAndTweet)
            };
        }

        async Task GroupDataAndTweet()
        {
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(Cancellation))
            {
                try
                {
                    // clean AggregatedData every 10 seconds
                    await Task.Delay(TimeSpan.FromSeconds(10), Cancellation);

                    _state.Cleanup();
                }
                catch (OperationCanceledException) when (timeout.IsCancellationRequested)
                {
                }
            }
        }
    }
}
