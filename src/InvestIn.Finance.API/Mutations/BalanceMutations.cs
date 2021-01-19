using System.Threading.Tasks;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using InvestIn.Finance.API.Mutations.InputTypes;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Interfaces;

namespace InvestIn.Finance.API.Mutations
{
    [ExtendObjectType(Name = "Mutations")]
    public class BalanceMutations
    {
        private readonly IBalanceService _balanceService;

        public BalanceMutations(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [Authorize]
        public async Task<OperationResult> RefillBalance(RefillBalanceInput refillBalanceInput)
        {
            var result = await _balanceService.RefillBalance(refillBalanceInput.PortfolioId, refillBalanceInput.Price,
                refillBalanceInput.Date);

            return result;
        }

        [Authorize]
        public async Task<OperationResult> WithdrawalBalance(WithdrawalBalanceInput withdrawalBalanceInput)
        {
            var result = await _balanceService.WithdrawalBalance(withdrawalBalanceInput.PortfolioId, withdrawalBalanceInput.Price,
                withdrawalBalanceInput.Date);

            return result;
        }
    }
}