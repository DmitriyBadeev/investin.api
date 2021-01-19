using System;
using System.Threading;
using System.Threading.Tasks;
using InvestIn.BackgroundTasks.Base;
using InvestIn.Core.Entities.Reports;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvestIn.BackgroundTasks
{
    public class DailyReportHostedService : ScheduledHostedServiceBase
    {
        private readonly ILogger<DailyReportHostedService> _logger;

        public DailyReportHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<DailyReportHostedService> logger) 
            : base(serviceScopeFactory, logger)
        {
            _logger = logger;
        }

        protected override async Task ProcessInScopeAsync(IServiceProvider serviceProvider, CancellationToken token)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                _logger.LogInformation("Starting write finance report for the day...");
                var financeData = scope.ServiceProvider.GetService<FinanceDataService>();
                var portfolioService = scope.ServiceProvider.GetService<IPortfolioService>();

                foreach (var portfolio in financeData.EfContext.Portfolios)
                {
                    var cost = await portfolioService.GetCost(portfolio.Id, portfolio.UserId);
                    var profit = await portfolioService.GetPaperProfit(portfolio.Id, portfolio.UserId);
                    if (cost.IsSuccess && profit.IsSuccess)
                    {
                        if (cost.Result != 0)
                        {
                            var dailyReport = new DailyPortfolioReport
                            {
                                Cost = cost.Result,
                                Profit = profit.Result.Value,
                                Time = DateTime.Today,
                                Portfolio = portfolio,
                                PortfolioId = portfolio.Id
                            };

                            await financeData.EfContext.DailyPortfolioReports.AddAsync(dailyReport, token);
                        }
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong for writing a daily finance report for a portfolio with id={portfolio.Id}");
                    }
                }

                await financeData.EfContext.SaveChangesAsync(token);
            }
        }
        
        //MOSCOW TIME
        protected override string Schedule => "0 21 * * *";
        protected override string DisplayName => "Daily report service";
        
        //!DEBUG
        protected override bool IsExecuteOnServerRestart => false;
    }
}