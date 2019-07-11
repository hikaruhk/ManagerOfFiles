using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Grains;
using Microsoft.Extensions.DependencyInjection;

namespace Silo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var siloBuilder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .UseDashboard(options => { options.Port = 7777; })
                .ConfigureServices(c => c.AddHttpClient())
                .ConfigureApplicationParts(parts => 
                    parts.AddApplicationPart(typeof(ApiDataReaderGrain).Assembly)
                         .WithReferences())
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "ManagerOfFilesSilo";
                })
                .Configure<EndpointOptions>(options =>
                    options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Warning));

            using (var host = siloBuilder.Build())
            {
                await host.StartAsync();

                Console.ReadLine();
            }
        }
    }
}
