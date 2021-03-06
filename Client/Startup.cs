﻿using Client.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

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
            var orleansConfig = Configuration.GetSection("OrleansConfig");

            var client = new ClientBuilder()
                .UseLocalhostClustering(gatewayPort: orleansConfig.GetValue<int>("gatewayPort"))
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = orleansConfig["clusterId"];
                    options.ServiceId = orleansConfig["serviceId"];
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

                client
                    .Connect(async ex =>
                    {
                        Console.WriteLine(ex);
                        Console.WriteLine("Retrying...");
                        await Task.Delay(3000);

                        return true;
                    })
                    .Wait();

            return client;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var orleansClient = CreateOrleansClient();

            services.AddOptions();
            services.AddSingleton(orleansClient);
            services.AddMvc();
            services.Configure<ClientConfiguration>(Configuration.GetSection("ManagerOfFilesConfigs"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
