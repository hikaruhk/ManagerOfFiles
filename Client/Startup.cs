using System;
using System.Threading.Tasks;
using Client.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;

namespace Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private IClusterClient CreateOrleansClient()
        {
            var client = new ClientBuilder()
            .UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "ManagerOfFilesSilo";
            })
            .ConfigureLogging(logging => logging.AddConsole())
            .Build();

            client.Connect(async ex =>
            {
                Console.WriteLine(ex);
                Console.WriteLine("Retrying...");
                await Task.Delay(3000);

                return true;
            }).Wait();

            return client;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var orleansClient = CreateOrleansClient();

            services.AddOptions();
            services.AddSingleton(orleansClient);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<ClientConfiguration>(Configuration.GetSection("ManagerOfFilesConfigs"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
