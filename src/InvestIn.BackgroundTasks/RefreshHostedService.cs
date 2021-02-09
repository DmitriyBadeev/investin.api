using System;
using System.Threading;
using System.Threading.Tasks;
using InvestIn.BackgroundTasks.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvestIn.BackgroundTasks
{
    public class RefreshHostedService : ScheduledHostedServiceBase
    {
        private readonly ILogger<RefreshHostedService> _logger;

        public RefreshHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<RefreshHostedService> logger) 
            : base(serviceScopeFactory, logger)
        {
            _logger = logger;
        }

        protected override Task ProcessInScopeAsync(IServiceProvider serviceProvider, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override string Schedule => "* * * * *";
        protected override string DisplayName => "Refresh process service";
    }
}