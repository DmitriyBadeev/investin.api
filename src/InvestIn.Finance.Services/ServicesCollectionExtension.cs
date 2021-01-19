using System.Net.Http;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Finance.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InvestIn.Finance.Services
{
    public static class ServicesCollectionExtension
    {
        public static IServiceCollection AddFinanceServices(this IServiceCollection services)
        {
            services.AddScoped<IStockMarketAPI, StockMarketAPI>();
            services.AddScoped<IStockMarketData, StockMarketData>();
            services.AddScoped<IAssetsFactory, AssetsFactory>();
            services.AddScoped<IBalanceService, BalanceService>();
            services.AddScoped<IMarketService, MarketService>();
            services.AddScoped<IPortfolioService, PortfolioService>();
            services.AddScoped<IAggregatePortfolioService, AggregatePortfolioService>();
            services.AddScoped<IMarketQuotesService, MarketQuotesService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IGraphService, GraphService>();
            services.AddScoped<HttpClient>();
            return services;
        }
    }
}
