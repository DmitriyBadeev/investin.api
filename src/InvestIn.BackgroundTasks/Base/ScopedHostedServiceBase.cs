using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvestIn.BackgroundTasks.Base
{
    public abstract class ScopedHostedServiceBase : HostedServiceBase
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
 
        protected ScopedHostedServiceBase(IServiceScopeFactory serviceScopeFactory, ILogger logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            Logger = logger;
        }
        
        public string ServiceName => $"[{GetType().Name.ToUpperInvariant()}]";
        
        protected ILogger Logger { get; }
        
        protected override async Task ProcessAsync(CancellationToken token)
        {
            try
            {
                var scope = _serviceScopeFactory.CreateScope();
                await ProcessInScopeAsync(scope.ServiceProvider, token);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, GetType().Name);
                token.ThrowIfCancellationRequested();
            }
        }
        protected abstract Task ProcessInScopeAsync(IServiceProvider serviceProvider, CancellationToken token);
    }
}