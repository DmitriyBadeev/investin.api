using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Infrastructure.Services;

namespace InvestIn.Finance.Services.Services
{
    public class MarketService : IMarketService
    {
        private readonly FinanceDataService _financeDataService;
        private readonly IAssetsFactory _assetsFactory;
        private readonly IBalanceService _balanceService;
        private List<PortfolioData> _portfolios;

        public MarketService(FinanceDataService financeDataService, IAssetsFactory assetsFactory, 
            IBalanceService balanceService)
        {
            _financeDataService = financeDataService;
            _assetsFactory = assetsFactory;
            _balanceService = balanceService;
        }

        public IEnumerable<AssetOperation> GetAllAssetOperations(string userId)
        {
            return _financeDataService.EfContext.AssetOperations
                .Include(o => o.Portfolio)
                .Include(o => o.AssetAction)
                .Include(o => o.AssetType)
                .Where(o => o.Portfolio.UserId == userId);
        }

        public async Task<OperationResult> BuyAsset(int portfolioId, string ticket, int price, int amount,
            int assetTypeId, DateTime date)
        {
            var portfolio = await _financeDataService.EfContext.Portfolios.FindAsync(portfolioId);
            var buyAction =
                await _financeDataService.EfContext.AssetActions.FirstOrDefaultAsync(a =>
                    a.Name == SeedFinanceData.BUY_ACTION);
            var assetType = await _financeDataService.EfContext.AssetTypes.FindAsync(assetTypeId);

            if (!CommonValidate(price, amount, assetType, portfolio, out var message))
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = message
                };
            }

            var currentBalanceResult = await _balanceService.GetBalance(portfolioId, portfolio.UserId);
            if (price > currentBalanceResult.Result)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "Недостаточно средств"
                };
            }

            var assetOperation = new AssetOperation
            {
                Portfolio = portfolio,
                PortfolioId = portfolioId,
                AssetAction = buyAction,
                AssetActionId = buyAction.Id,
                AssetType = assetType,
                AssetTypeId = assetType.Id,
                Date = date,
                PaymentPrice = price,
                Ticket = ticket,
                Amount = amount
            };

            await _financeDataService.EfContext.AssetOperations.AddAsync(assetOperation);
            await _financeDataService.EfContext.SaveChangesAsync();

            GetPortfoliosData(portfolio.UserId, true);

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Актив куплен успешно"
            };
        }

        public async Task<OperationResult> SellAsset(int portfolioId, string ticket, int price, int amount,
            int assetTypeId, DateTime date)
        {
            var portfolio = await _financeDataService.EfContext.Portfolios.FindAsync(portfolioId);
            var sellAction =
                await _financeDataService.EfContext.AssetActions.FirstOrDefaultAsync(a =>
                    a.Name == SeedFinanceData.SELL_ACTION);
            var assetType = await _financeDataService.EfContext.AssetTypes.FindAsync(assetTypeId);

            if (!CommonValidate(price, amount, assetType, portfolio, out var message))
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = message
                };
            }

            if (!HasAsset(portfolioId, amount, ticket, portfolio.UserId))
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "Такого количества активов нет в наличии"
                };
            }

            var assetOperation = new AssetOperation
            {
                Portfolio = portfolio,
                PortfolioId = portfolioId,
                AssetAction = sellAction,
                AssetActionId = sellAction.Id,
                AssetType = assetType,
                AssetTypeId = assetType.Id,
                Date = date,
                PaymentPrice = price,
                Ticket = ticket,
                Amount = amount
            };

            await _financeDataService.EfContext.AssetOperations.AddAsync(assetOperation);
            await _financeDataService.EfContext.SaveChangesAsync();

            GetPortfoliosData(portfolio.UserId, true);

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Актив продан успешно"
            };
        }

        public List<PaymentData> GetAllFuturePayments(string userId)
        {
            var portfolios = GetPortfoliosData(userId);
            var payments = new List<PaymentData>();

            foreach (var portfolio in portfolios)
            {
                foreach (var assetInfo in portfolio.Assets)
                {
                    payments.AddRange(assetInfo.GetFuturePayments());
                }
            }

            return payments;
        }
        
        public async Task<AssetPrices> GetAllAssetPrices(string userId)
        {
            var portfolios = GetPortfoliosData(userId);

            var assetPrices = new AssetPrices();
            foreach (var portfolio in portfolios)
            {
                foreach (var asset in portfolio.Assets)
                {
                    var type = asset.GetType();

                    switch (type.Name)
                    {
                        case "StockInfo":
                            assetPrices.StockPrice += await asset.GetAllPrice();
                            break;
                        case "FondInfo":
                            assetPrices.FondPrice += await asset.GetAllPrice();
                            break;
                        case "BondInfo":
                            assetPrices.BondPrice += await asset.GetAllPrice();
                            break;
                    }
                }
            }

            return assetPrices;
        }

        private bool HasAsset(int portfolioId, int amount, string ticket, string userId)
        {
            var portfolio = GetPortfoliosData(userId).Find(p => p.Id == portfolioId);

            var asset = portfolio.Assets.FirstOrDefault(a => a.Ticket == ticket);

            if (asset == null) return false;
            if (asset.Amount < amount) return false;

            return true;
        }

        private List<PortfolioData> GetPortfoliosData(string userId, bool isForceUpdate = false)
        {
            if (_portfolios != null && !isForceUpdate) return _portfolios;
            
            var portfolios = new List<PortfolioData>();
            var userPortfolios = _financeDataService.EfContext.Portfolios
                .Where(p => p.UserId == userId);

            foreach (var userPortfolio in userPortfolios)
            {
                var portfolioData = new PortfolioData
                {
                    Id = userPortfolio.Id,
                    Name = userPortfolio.Name,
                    UserId = userPortfolio.UserId,
                    Assets = _assetsFactory.Create(userPortfolio.Id)
                };

                portfolios.Add(portfolioData);
            }

            _portfolios = portfolios;
            return portfolios;

        }

        private static bool CommonValidate(int price, int amount, AssetType assetType,
            Portfolio portfolio, out string message)
        {
            if (assetType == null)
            {
                message = "Тип актива не найден";
                return false;
            }

            if (portfolio == null)
            {
                message = "Портфель не найден";
                return false;
            }

            if (price < 0 || amount <= 0)
            {
                message = "Некорректные данные";
                return false;
            }

            message = "";
            return true;
        }
    }
}
