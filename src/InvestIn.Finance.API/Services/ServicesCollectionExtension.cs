using InvestIn.Finance.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InvestIn.Finance.API.Services
{
    public static class ServicesCollectionExtension
    {
        public static IServiceCollection AddTimerServices(this IServiceCollection services)
        {
            services.AddSingleton<ITimerService, TimerService>();
            return services;
        }
    }
}
