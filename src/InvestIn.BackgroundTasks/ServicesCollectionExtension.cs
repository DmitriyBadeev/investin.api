using System;
using Microsoft.Extensions.DependencyInjection;

namespace InvestIn.BackgroundTasks
{
    public static class ServicesCollectionExtension
    {

        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<DailyReportHostedService>();
            services.AddHostedService<RefreshHostedService>();
            return services;
        }
    }
}