using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InvestIn.Core.Interfaces;

namespace InvestIn.Infrastructure.Services
{
    public static class ServicesCollectionExtension
    {
        public static IServiceCollection AddFinanceInfrastructureServices(this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<FinanceDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<FinanceDataService>();
            services.AddScoped<ISeedDataService, SeedFinanceDataService>();

            return services;
        }
    }
}
