using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using InvestIn.Finance.API.Queries;
using InvestIn.Finance.API.Queries.Response;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Interfaces;

namespace InvestIn.Finance.API.Subscriptions
{
    [ExtendObjectType(Name = "Subscriptions")]
    public class ReportSubscriptions
    {
        [Subscribe]
        [Topic]
        public AllPortfoliosReport OnUpdatePortfoliosReport(
            [Service] IMarketService marketService,
            [Service] IBalanceService balanceService,
            [EventMessage] string userId,
            [Service] IAggregatePortfolioService aggregatePortfolioService)
        {
            return QueryGetters.GetAllPortfoliosReport(userId, marketService, balanceService, aggregatePortfolioService);
        }

        [Subscribe]
        [Topic]
        public async Task<List<StockReport>> OnUpdateStockReports(
            [Service] IPortfolioService portfolioService,
            [EventMessage] string userId, int portfolioId)
        {
            return await QueryGetters.GetStockReports(userId, portfolioService, portfolioId);
        }

        [Subscribe]
        [Topic]
        public async Task<List<FondReport>> OnUpdateFondReports(
            [Service] IPortfolioService portfolioService,
            [EventMessage] string userId, int portfolioId)
        {
            return await QueryGetters.GetFondReports(userId, portfolioService, portfolioId);
        }

        [Subscribe]
        [Topic]
        public async Task<List<BondReport>> OnUpdateBondReports(
            [Service] IPortfolioService portfolioService,
            [EventMessage] string userId, int portfolioId)
        {
            return await QueryGetters.GetBondReports(userId, portfolioService, portfolioId);
        }

        [Subscribe]
        [Topic]
        public async Task<AssetPricesReport> OnUpdatePricesReport(
            [Service] IMarketService marketService,
            [EventMessage] string userId)
        {
            return await QueryGetters.GetAllAssetPricesReport(userId, marketService);
        }
    }
}
