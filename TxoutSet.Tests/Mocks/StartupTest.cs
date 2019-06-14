using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using TxoutSet.Publisher;
using TxoutSet.Publisher.DataHolders;
using TxoutSet.Tests.Mocks;

namespace TxoutSet.Tests.Mocks
{
    public class StartupTest : StartupBase
    {
        public StartupTest(IConfiguration configuration) : base(configuration)
        { }

        public override void ExtraInjection(IServiceCollection services)
        {
            services.AddTransient<ITweetLog, TweetLogMock>();
        }
    }
}
