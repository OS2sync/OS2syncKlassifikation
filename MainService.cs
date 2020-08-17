using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace StsKlassifikation
{
    public class MainService
    {
        private IWebHost webHost;
        private bool stopRequested;
        private ILogger logger;

        public void Start()
        {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("logsettings.json")
                .AddJsonFile("appsettings.json")
                // add optional development config without copying it to output dir
                .AddJsonFile(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "", "appsettings.development.json"), true)
                .Build();

            // configure logging
            logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            // set up web host
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(configuration.GetValue<string>("OData:Urls", "http://*:80;https://*:443"))
                .UseConfiguration(configuration)
                .UseStartup<Startup>();

            // create and run host)
            webHost = webHostBuilder.Build();            
            webHost.Services.GetRequiredService<IApplicationLifetime>()
                .ApplicationStopped.Register(() =>
                {
                    if (!stopRequested)
                        Stop();
                });

            webHost.Start();

            logger.Information("STS Klassifikation Service started");
            var serverAddresses = webHost.ServerFeatures.Get<IServerAddressesFeature>()?.Addresses;
            foreach (var address in serverAddresses ?? Array.Empty<string>())
            {
                logger.Information($"Listening on: {address}");
            }
        }

        public void Stop()
        {
            stopRequested = true;
            webHost?.Dispose();
            logger.Information("STS Klassifikation Service stopped");
        }
    }
}
