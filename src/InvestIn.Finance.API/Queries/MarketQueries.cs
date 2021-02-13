using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Interfaces;

namespace InvestIn.Finance.API.Queries
{
    [ExtendObjectType(Name = "Queries")]
    public class MarketQueries
    {
        [Authorize]
        public IEnumerable<Asset> GetSectorsAssets([Service] IMarketService marketService, string type, string[] sectors)
        {
            return marketService.GetMarketAssets(type, sectors);
        }
        
        [Authorize]
        public IEnumerable<Asset> GetAssets([Service] IMarketService marketService, string type)
        {
            return marketService.GetMarketAssets(type);
        }

        [Authorize]
        public async Task<OperationResult<Asset>> GetAsset([Service] IMarketService marketService, string ticket)
        {
            return await marketService.GetAssetInfo(ticket);
        }
    }
}