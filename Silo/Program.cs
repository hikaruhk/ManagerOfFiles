using CommandLine;
using Microsoft.Extensions.Configuration;
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
        public static async Task Main(string[] args)
        {
            var result = Parser.Default
                .ParseArguments<Options>(args)
                .MapResult(r => r, e => throw new ArgumentException("Unable to parse commands!"));

            var configs = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var siloHostBuilder = result switch
            {
                Options { Environment: EnvironmentType.SqlServerCluster } => 
                    new SiloHostBuilder()
                        .UseAdoNetClustering(options =>
                        {
                            options.ConnectionString = configs["connectionString"];
                            options.Invariant = configs["connectionName"];
                        }),
                _ => new SiloHostBuilder().UseLocalhostClustering()
            };

            var siloBuilder = siloHostBuilder
                .ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                .UseDashboard(options => { options.Port = 7777; })
                .ConfigureServices(c => c.AddHttpClient())
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = configs["clusterId"];
                    options.ServiceId = configs["serviceId"];
                })
                .ConfigureEndpoints(
                    IPAddress.Loopback,
                    siloPort: configs.GetValue("siloPort", 7778),
                    gatewayPort: configs.GetValue("gatewayPort", 7779))
                .ConfigureLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Warning));

            using var host = siloBuilder.Build();
            await host.StartAsync();
            Console.ReadLine();
        }
    }
}
