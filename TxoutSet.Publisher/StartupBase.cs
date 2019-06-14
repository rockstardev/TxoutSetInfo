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
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using TxoutSet.Publisher.DataHolders;
using TxoutSet.Publisher.HostedServices;

namespace TxoutSet.Publisher
{
    public abstract class StartupBase
    {
        public StartupBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        internal static ServiceProvider Di { get; private set; }
        internal static T GetService<T>()
        {
            return Di.GetService<T>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var appConfig = new Zonfig();
            Configuration.Bind("Zonfig", appConfig);
            services.AddSingleton(appConfig);

            services.AddSingleton(new AggregationState(appConfig));
            services.AddTransient(typeof(AggregatedDataset));
            services.AddTransient<ILogger>(a =>
            {
                return NLog.LogManager.GetLogger("TxoutSet.Publisher") as ILogger;
            });

            services.AddHostedService<AggregateHostedService>();

            ConfigureExtraServices(services);

            Di = services.BuildServiceProvider();
        }

        public abstract void ConfigureExtraServices(IServiceCollection services);

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
