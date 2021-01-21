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
    public class AssetMutations
    {
        [Authorize]
        public async Task<OperationResult> BuyAsset([Service] IMarketService marketService, BuyAssetInput buyAssetInput)
        {
            var result = await marketService.BuyAsset(buyAssetInput.PortfolioId, buyAssetInput.Ticket, buyAssetInput.Price,
                buyAssetInput.Amount, buyAssetInput.AssetTypeId, buyAssetInput.Date);

            return result;
        }

        [Authorize]
        public async Task<OperationResult> SellAsset([Service] IMarketService marketService, SellAssetInput sellAssetInput)
        {
            var result = await marketService.SellAsset(sellAssetInput.PortfolioId, sellAssetInput.Ticket, sellAssetInput.Price,
                sellAssetInput.Amount, sellAssetInput.AssetTypeId, sellAssetInput.Date);

            return result;
        }

        [Authorize]
        public async Task<OperationResult> RemovePortfolio(
            [Service] IPortfolioService portfolioService,
            [CurrentUserIdGlobalState] string userId, 
            int portfolioId)
        {
            return await portfolioService.RemovePortfolio(portfolioId, userId);
        }
    }
}