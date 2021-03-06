﻿using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Infrastructure.Services;

namespace InvestIn.Finance.API.Queries
{
    [ExtendObjectType(Name = "Queries")]
    public class OperationQueries
    {
        [Authorize]
        public IEnumerable<AssetOperation> GetAllAssetOperations([CurrentUserIdGlobalState] string userId,
            [Service] IMarketService marketService)
        {
            return marketService.GetAllAssetOperations(userId);
        }

        [Authorize]
        public IEnumerable<CurrencyOperation> GetAllCurrencyOperations([CurrentUserIdGlobalState] string userId,
            [Service] IBalanceService balanceService)
        {
            return balanceService.GetAllCurrencyOperations(userId);
        }

        [Authorize]
        public IEnumerable<AssetType> GetAssetTypes([Service] FinanceDataService financeData)
        {
            return financeData.EfContext.AssetTypes;
        }

        [Authorize]
        public IEnumerable<AssetAction> GetAssetActions([Service] FinanceDataService financeData)
        {
            return financeData.EfContext.AssetActions;
        }

        [Authorize]
        public IEnumerable<CurrencyAction> GetCurrencyActions([Service] FinanceDataService financeData)
        {
            return financeData.EfContext.CurrencyActions;
        }
    }
}