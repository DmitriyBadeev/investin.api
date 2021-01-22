using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.DTO;

namespace InvestIn.Finance.Services.Interfaces
{
    public interface IBalanceService
    {
        Task<OperationResult<int>> AggregateBalance(IEnumerable<int> portfolioIds, string userId);
        Task<OperationResult<int>> GetBalance(int portfolioId, string userId);

        IEnumerable<CurrencyOperation> GetAllCurrencyOperations(string userId);
        
        Task<OperationResult> RefillBalance(int portfolioId, int price, DateTime date);

        Task<OperationResult> WithdrawalBalance(int portfolioId, int price, DateTime date);

        int GetInvestSum(int portfolioId, string userId);

        int GetAggregateInvestSum(IEnumerable<int> portfolioIds, string userId);
    }
}