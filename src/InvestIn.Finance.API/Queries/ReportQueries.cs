using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using InvestIn.Finance.API.Queries.Response;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Interfaces;

namespace InvestIn.Finance.API.Queries
{
    [ExtendObjectType(Name = "Queries")]
    public class ReportQueries
    {
        [Authorize]
        public AllPortfoliosReport GetAllPortfoliosReport(
            [CurrentUserIdGlobalState] string userId, 
            [Service] IMarketService marketService, 
            [Service] IBalanceService balanceService, 
            [Service] IAggregatePortfolioService aggregatePortfolioService)
        {
            return QueryGetters.GetAllPortfoliosReport(userId, marketService, balanceService, aggregatePortfolioService);
        }

        [Authorize]
        public async Task<List<StockReport>> GetStockReports([CurrentUserIdGlobalState] string userId,
            [Service] IPortfolioService portfolioService, int portfolioId)
        {
            return await QueryGetters.GetStockReports(userId, portfolioService, portfolioId);
        }

        [Authorize]
        public async Task<List<FondReport>> GetFondReports([CurrentUserIdGlobalState] string userId,
            [Service] IPortfolioService portfolioService, int portfolioId)
        {
            return await QueryGetters.GetFondReports(userId, portfolioService, portfolioId);
        }

        [Authorize]
        public async Task<List<BondReport>> GetBondReports([CurrentUserIdGlobalState] string userId,
            [Service] IPortfolioService portfolioService, int portfolioId)
        {
            return await QueryGetters.GetBondReports(userId, portfolioService, portfolioId);
        }

        [Authorize]
        public async Task<AssetPricesReport> GetAllAssetPricesReport([CurrentUserIdGlobalState] string userId,
            [Service] IMarketService marketService)
        {
            return await QueryGetters.GetAllAssetPricesReport(userId, marketService);
        }

        [Authorize]
        public IEnumerable<PaymentDataReport> GetAllFuturePaymentsReport([CurrentUserIdGlobalState] string userId,
            [Service] IMarketService marketService)
        {
            return QueryGetters.GetAllFuturePaymentsReport(userId, marketService);
        }

        [Authorize]
        public IEnumerable<CommonMarketQuote> GetMarketQuotes([Service] IMarketQuotesService quotesService)
        {
            return QueryGetters.GetMarketQuotes(quotesService);
        }

        [Authorize]
        public async Task<SearchData> SearchAsset([Service] ISearchService searchService, string ticket)
        {
            return await QueryGetters.SearchAsset(searchService, ticket);
        }

        [Authorize]
        public async Task<AssetData> AssetReport([CurrentUserIdGlobalState] string userId, 
            [Service] ISearchService searchService, string ticket)
        {
            return  await QueryGetters.AssetReport(searchService, ticket, userId);
        }
    }
}