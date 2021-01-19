﻿using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using InvestIn.Core.Interfaces;

namespace InvestIn.Finance.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Running application");
                try
                {
                    var seed = services.GetRequiredService<ISeedDataService>();
                    seed.Initialise();
                }
                catch (Exception ex)
                {
                    logger.LogCritical("Error creating/seeding database - " + ex.Message, ex);
                }
            }

            host.Run();
        }

        public static IWebHostBuilder BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(builder => builder
                    .AddConsole()
                    .AddFile())
                .UseStartup<Startup>();
    }
}
