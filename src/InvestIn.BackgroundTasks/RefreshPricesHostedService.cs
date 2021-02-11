using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvestIn.BackgroundTasks.Base;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvestIn.BackgroundTasks
{
    public class RefreshPricesHostedService : ScheduledHostedServiceBase
    {
        private readonly ILogger<RefreshPricesHostedService> _logger;

        public RefreshPricesHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<RefreshPricesHostedService> logger) 
            : base(serviceScopeFactory, logger)
        {
            _logger = logger;
        }

        protected override Task ProcessInScopeAsync(IServiceProvider serviceProvider, CancellationToken token)
        {
            _logger.LogInformation("Refresh prices");

            using (var scope = serviceProvider.CreateScope())
            {
                var financeData = scope.ServiceProvider.GetService<FinanceDataService>();
                var stockMarketData = scope.ServiceProvider.GetService<IStockMarketData>();
                var assets = financeData.EfContext.Assets
                    .Include(a => a.AssetType)
                    .ToList();
                
                foreach (var asset in assets)
                {
                    if (asset.AssetType.Name == SeedFinanceData.STOCK_ASSET_TYPE)
                    {
                        var data = stockMarketData.GetStockData(asset.Ticket).GetAwaiter().GetResult();
                        var priceReport = stockMarketData.GetPrice(data);

                        asset.Capitalization = (long)priceReport.Price * asset.IssueSize;
                        asset.Price = priceReport.Price;
                        asset.PriceChange = priceReport.Percent;
                        asset.UpdateTime = priceReport.Time;
                    }

                    if (asset.AssetType.Name == SeedFinanceData.FOND_ASSET_TYPE)
                    {
                        var data = stockMarketData.GetFondData(asset.Ticket).GetAwaiter().GetResult();
                        var priceReport = stockMarketData.GetPrice(data);

                        asset.Price = priceReport.Price;
                        asset.PriceChange = priceReport.Percent;
                        asset.UpdateTime = priceReport.Time;
                    }

                    if (asset.AssetType.Name == SeedFinanceData.BOND_ASSET_TYPE)
                    {
                        var data = stockMarketData.GetBondData(asset.Ticket).GetAwaiter().GetResult();
                        var priceReport = stockMarketData.GetPrice(data);

                        asset.Price = priceReport.Price;
                        asset.PriceChange = priceReport.Percent;
                        asset.UpdateTime = priceReport.Time;
                    }
                }

                financeData.EfContext.SaveChanges();
            }
            
            return Task.CompletedTask;
        }

        protected override string Schedule => "3 8-20 * * 1-5";
        protected override string DisplayName => "Refresh process service";
        
        //!DEBUG
        protected override bool IsExecuteOnServerRestart => false;
    }
}