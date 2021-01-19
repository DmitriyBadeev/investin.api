using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Interfaces;

namespace InvestIn.Finance.API.Queries
{
    [ExtendObjectType(Name = "Queries")]
    public class BalanceQueries
    {
        [Authorize]
        public async Task<OperationResult<int>> AggregateBalance([CurrentUserIdGlobalState] string userId,
            [Service] IBalanceService balanceService, IEnumerable<int> portfolioIds)
        {
            return await balanceService.AggregateBalance(portfolioIds, userId);
        }
        
        [Authorize]
        public int AggregateInvestSum([CurrentUserIdGlobalState] string userId,
            [Service] IBalanceService balanceService, IEnumerable<int> portfolioIds)
        {
            return balanceService.GetAggregateInvestSum(portfolioIds, userId);
        }
    }
}