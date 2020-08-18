using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Silo
{
    public class Program
    {
        public static async Task Main(string[] _)
        {
            var siloBuilder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                .UseDashboard(options => { options.Port = 7777; })
                .ConfigureServices(c => c.AddHttpClient())
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "ManagerOfFilesSilo";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Warning));

            using var host = siloBuilder.Build();
            await host.StartAsync();
            Console.ReadLine();
        }
    }
}
