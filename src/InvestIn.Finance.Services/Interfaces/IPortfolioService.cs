using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Entities;

namespace InvestIn.Finance.Services.Interfaces
{
    public interface IPortfolioService
    {
        IEnumerable<PortfolioType> GetPortfolioTypes();
        
        Task<OperationResult> CreatePortfolio(string name, string userId, int typeId);

        Task<OperationResult> RemovePortfolio(int portfolioId, string userId);

        IEnumerable<Portfolio> GetPortfolios(string userId);

        Task<OperationResult> AddPaymentInPortfolio(int portfolioId, string userId, string ticket, int amount,
            int paymentValue, DateTime date);

        Task<OperationResult<List<Payment>>> GetPortfolioPayments(int portfolioId, string userId);

        Task<OperationResult<List<PaymentData>>> GetFuturePortfolioPayments(int portfolioId, string userId);

        Task<OperationResult<ValuePercent>> GetPortfolioPaymentProfit(int portfolioId, string userId);

        Task<OperationResult<ValuePercent>> GetPaperProfit(int portfolioId, string userId);

        Task<OperationResult<int>> GetCost(int portfolioId, string userId);

        Task<OperationResult<int>> GetPaperPrice(int portfolioId, string userId);

        IEnumerable<StockInfo> GetStocks(int portfolioId, string userId);
        
        IEnumerable<FondInfo> GetFonds(int portfolioId, string userId);
        
        IEnumerable<BondInfo> GetBonds(int portfolioId, string userId);
    }
}