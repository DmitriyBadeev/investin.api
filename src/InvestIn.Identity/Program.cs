using System;
using InvestIn.Core.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvestIn.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            
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
                    logger.LogCritical($"Error creating/seeding database - {ex.Message}", ex);
                }
            }
                
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
