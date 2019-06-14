using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TxoutSet.Publisher.DataHolders;
using TxoutSet.Publisher.HostedServices;

namespace TxoutSet.Publisher
{
    public class Startup : StartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration)
        { }

        public override void ExtraInjection(IServiceCollection services)
        {
            services.AddTransient<ITweetLog, TweetLog>();
        }
    }
}
