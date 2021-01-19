using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using InvestIn.Finance.API.Mutations.InputTypes;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Interfaces;

namespace InvestIn.Finance.API.Mutations
{
    [ExtendObjectType(Name = "Mutations")]
    public class PortfolioMutations
    {
        [Authorize]
        public async Task<OperationResult> CreatePortfolio([CurrentUserIdGlobalState] string userId,
            [Service] IPortfolioService portfolioService, string name, int portfolioType)
        {
            var result = await portfolioService.CreatePortfolio(name, userId, portfolioType);

            return result;
        }

        [Authorize]
        public async Task<OperationResult> AddPaymentInPortfolio([CurrentUserIdGlobalState] string userId,
            [Service] IPortfolioService portfolioService, PaymentInput paymentInput)
        {
            var result = await portfolioService.AddPaymentInPortfolio(paymentInput.PortfolioId, userId,
                paymentInput.Ticket, paymentInput.Amount, paymentInput.PaymentValue, paymentInput.Date);

            return result;
        }
    }
}