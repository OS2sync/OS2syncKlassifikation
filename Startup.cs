using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using StsKlassifikation.DBContext;
using StsKlassifikation.Model;
using StsKlassifikation.Quartz;
using StsKlassifikation.Service;
using System;
using System.Collections.Generic;

namespace StsKlassifikation
{
    public class Startup : IStartup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(configuration);
            services.AddDbContext<ClassificationContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlserver")));
            services.AddSingleton<ClassificationService>();
            services.AddSingleton<FacetService>();
            services.AddSingleton<KlasseService>();
            services.AddSingleton<MainService>();
            services.AddSingleton<MainService>();

            // Add Quartz services
            services.AddSingleton<IJobFactory, MicrosoftDependencyInjectionJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHostedService<QuartzHostedService>();

            // Add Sync Job
            services.AddSingleton<SyncJob>();
            var cron = configuration.GetValue<string>("Schedule:Cron", "0 0 0/4 ? * *");// every 4 hours, on the hour
            var enabled = configuration.GetValue<bool>("Schedule:Enabled", true);
            var runOnStartup = configuration.GetValue<bool>("Schedule:RunOnStartup", true);
            services.AddSingleton(new JobSchedule(typeof(SyncJob), cron, enabled, runOnStartup));

            // configure logging
            var logger = new LoggerConfiguration().ReadFrom.Configuration(configuration);
            services.AddLogging(builder => builder.AddSerilog(logger.CreateLogger()));

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                var maxExpansionDepth = configuration.GetValue<int>("OData:MaxExpansionDepth", 10);
                options.Filters.Add(new EnableQueryAttribute() { MaxExpansionDepth = maxExpansionDepth });
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOData();
            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<Facet>("Facetter");
            modelBuilder.EntitySet<Klasse>("Klasser");
            modelBuilder.EntitySet<Klassifikation>("Klassifikationer");
            var model = modelBuilder.GetEdmModel();

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
                routeBuilder.Expand().Select().Filter().Count().OrderBy().MaxTop(null);
                routeBuilder.MapODataServiceRoute("odata", "odata", containerBuilder =>
                {
                    containerBuilder.AddService(Microsoft.OData.ServiceLifetime.Scoped, sp => model);
                    containerBuilder.AddService<IODataPathHandler>(Microsoft.OData.ServiceLifetime.Singleton, sp => new DefaultODataPathHandler());
                    containerBuilder.AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, sp => ODataRoutingConventions.CreateDefaultWithAttributeRouting("odata", routeBuilder));
                });
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ClassificationContext>())
                {
                    context.Database.Migrate();
                }

            }
        }
    }
}
