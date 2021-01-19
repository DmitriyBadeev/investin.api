﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.DTO;

namespace InvestIn.Finance.Services.Interfaces
{
    public interface IMarketService
    {
        IEnumerable<AssetOperation> GetAllAssetOperations(int portfolioId);
        List<PaymentData> GetAllFuturePayments(string userId);
        Task<AssetPrices> GetAllAssetPrices(string userId);
        Task<OperationResult> BuyAsset(int portfolioId, string ticket, int price, int amount,
            int assetTypeId, DateTime date);
        Task<OperationResult> SellAsset(int portfolioId, string ticket, int price, int amount,
            int assetTypeId, DateTime date);
    }
}