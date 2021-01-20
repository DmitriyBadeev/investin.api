using System.Collections.Generic;
using System.Threading.Tasks;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Entities;

namespace InvestIn.Finance.Services.Interfaces
{
    public interface IAggregatePortfolioService
    {
        Task<OperationResult<List<Payment>>> AggregatePayments(IEnumerable<int> portfolioIds, string userId);

        Task<OperationResult<List<PaymentData>>> AggregateFuturePayments(IEnumerable<int> portfolioIds, string userId);

        Task<OperationResult<ValuePercent>> AggregatePaymentProfit(IEnumerable<int> portfolioIds, string userId);

        Task<OperationResult<ValuePercent>> AggregatePaperProfit(IEnumerable<int> portfolioIds, string userId);

        Task<OperationResult<int>> AggregateCost(IEnumerable<int> portfolioIds, string userId);

        IEnumerable<StockInfo> AggregateStocks(IEnumerable<int> portfolioIds, string userId);

        IEnumerable<FondInfo> AggregateFonds(IEnumerable<int> portfolioIds, string userId);

        IEnumerable<BondInfo> AggregateBonds(IEnumerable<int> portfolioIds, string userId);
    }
}