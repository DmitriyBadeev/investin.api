using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Entities;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Infrastructure.Services;

namespace InvestIn.Finance.Services.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly FinanceDataService _financeData;
        private readonly IBalanceService _balanceService;
        private readonly IAssetsFactory _assetsFactory;
        
        public PortfolioService(FinanceDataService financeData, IBalanceService balanceService, 
            IAssetsFactory assetsFactory)
        {
            _financeData = financeData;
            _balanceService = balanceService;
            _assetsFactory = assetsFactory;
        }

        public IEnumerable<InvestIn.Core.Entities.Finance.Portfolio> GetPortfolios(string userId)
        {
            return _financeData.EfContext.Portfolios
                .Include(p => p.PortfolioType)
                .Where(p => p.UserId == userId);
        }
        
        public IEnumerable<PortfolioType> GetPortfolioTypes()
        {
            return _financeData.EfContext.PortfolioTypes;
        }

        public async Task<OperationResult> CreatePortfolio(string name, string userId, int typeId)
        {
            var containsSameNamePortfolio = 
                await _financeData.EfContext.Portfolios.AnyAsync(p => p.Name == name && p.UserId == userId);
            
            if (containsSameNamePortfolio)
            {
                return new OperationResult()
                {
                    IsSuccess = false,
                    Message = "Порфель с таким именем у Вас уже существует"
                };
            }

            var portfolioType = await _financeData.EfContext.PortfolioTypes.FindAsync(typeId);

            if (portfolioType == null)
            {
                return new OperationResult()
                {
                    IsSuccess = false,
                    Message = "Задан несуществующий тип"
                };
            }
            
            var portfolio = new InvestIn.Core.Entities.Finance.Portfolio()
            {
                Name = name,
                UserId = userId,
                PortfolioTypeId = typeId,
                PortfolioType = portfolioType
            };
            
            await _financeData.EfContext.Portfolios.AddAsync(portfolio);
            await _financeData.EfContext.SaveChangesAsync();

            return new OperationResult()
            {
                IsSuccess = true,
                Message = $"Портфель {name} создан успешно"
            };
        }

        public async Task<OperationResult<int>> GetCost(int portfolioId, string userId)
        {
            var portfolio = await _financeData.EfContext.Portfolios.FindAsync(portfolioId);
            
            var validationResult = CommonValidate<int>(portfolioId, userId, portfolio);
            if (validationResult != null)
            {
                return validationResult;
            }

            var paperPriceResult = await GetPaperPrice(portfolioId, userId);
            if (!paperPriceResult.IsSuccess)
            {
                return paperPriceResult;
            }

            var portfolioBalanceResult = await _balanceService.GetBalance(portfolioId, userId);

            var cost = paperPriceResult.Result + portfolioBalanceResult.Result;

            return new OperationResult<int>()
            {
                IsSuccess = true,
                Message = "Суммарная стоимость портфеля",
                Result = cost
            };
        }

        public async Task<OperationResult<int>> GetPaperPrice(int portfolioId, string userId)
        {
            var portfolio = await _financeData.EfContext.Portfolios.FindAsync(portfolioId);
            
            var validationResult = CommonValidate<int>(portfolioId, userId, portfolio);
            if (validationResult != null)
            {
                return validationResult;
            }
            
            var portfolioData = GetPortfolioData(portfolioId);

            var sumPrice = 0;
            foreach (var asset in portfolioData.Assets)
            {
                var price = await asset.GetAllPrice();

                sumPrice += price;
            }
            
            return new OperationResult<int>()
            {
                IsSuccess = true,
                Message = $"Бумажная стоимость портфеля {portfolio.Name}",
                Result = sumPrice
            };
        }
        
        public async Task<OperationResult<ValuePercent>> GetPaperProfit(int portfolioId, string userId)
        {
            var portfolio = await _financeData.EfContext.Portfolios.FindAsync(portfolioId);
            
            var validationResult = CommonValidate<ValuePercent>(portfolioId, userId, portfolio);
            if (validationResult != null)
            {
                return validationResult;
            }

            var portfolioData = GetPortfolioData(portfolioId);

            var sumProfit = 0;
            foreach (var asset in portfolioData.Assets)
            {
                var profit = await asset.GetPaperProfit();

                sumProfit += profit;
            }

            var investingSum = _balanceService.GetInvestSum(portfolioId, userId);
            var percent = FinanceHelpers.DivWithOneDigitRound(sumProfit, investingSum);

            return new OperationResult<ValuePercent>()
            {
                IsSuccess = true,
                Message = $"Бумажная прибыль портфеля {portfolio.Name}",
                Result = new ValuePercent()
                {
                    Value = sumProfit,
                    Percent = percent
                }
            };
        }
        
        public async Task<OperationResult> AddPaymentInPortfolio(int portfolioId, string userId, string ticket, int amount, 
            int paymentValue, DateTime date)
        {
            var portfolio = await _financeData.EfContext.Portfolios.FindAsync(portfolioId);
            
            var validationResult = CommonValidate(portfolioId, userId, portfolio);
            if (validationResult != null)
            {
                return validationResult;
            }
            
            var payment = new Payment()
            {
                Ticket = ticket,
                Amount = amount,
                Date = date,
                PaymentValue = paymentValue,
                PortfolioId = portfolio.Id,
                Portfolio = portfolio
            };

            await _financeData.EfContext.Payments.AddAsync(payment);
            await _financeData.EfContext.SaveChangesAsync();
            
            return new OperationResult()
            {
                IsSuccess = true,
                Message = $"Выплата для {ticket} произведена успешно"
            };
        }

        public async Task<OperationResult<List<Payment>>> GetPortfolioPayments(int portfolioId, string userId)
        {
            var portfolio = await _financeData.EfContext.Portfolios.FindAsync(portfolioId);

            var validationResult = CommonValidate<List<Payment>>(portfolioId, userId, portfolio);
            if (validationResult != null)
            {
                return validationResult;
            }

            var payments = await _financeData.EfContext.Payments
                .Where(p => p.PortfolioId == portfolioId)
                .ToListAsync();

            return new OperationResult<List<Payment>>()
            {
                IsSuccess = true,
                Message = $"Выплаты для портфеля {portfolio.Name}",
                Result = payments
            };
        }

        public async Task<OperationResult<ValuePercent>> GetPortfolioPaymentProfit(int portfolioId, string userId)
        {
            var result = await GetPortfolioPayments(portfolioId, userId);

            if (!result.IsSuccess)
            {
                return new OperationResult<ValuePercent>()
                {
                    IsSuccess = result.IsSuccess,
                    Message = result.Message,
                    Result = null
                };
            }

            var payments = result.Result;

            var profit = payments.Aggregate(0, (sum, payment) => sum + payment.PaymentValue);
            var investingSum = _balanceService.GetInvestSum(portfolioId, userId);
            
            var percent = FinanceHelpers.DivWithOneDigitRound(profit, investingSum);
            
            return new OperationResult<ValuePercent>()
            {
                IsSuccess = true,
                Message = "Дивидендная прибыль",
                Result = new ValuePercent()
                {
                    Value = profit,
                    Percent = percent
                }
            };
        }
        
        public IEnumerable<StockInfo> GetStocks(int portfolioId, string userId)
        {
            var portfolio = GetPortfolioData(portfolioId);

            if (portfolio == null || portfolio.UserId != userId)
            {
                yield break;
            }

            foreach (var asset in portfolio.Assets)
            {
                var type = asset.GetType();

                if (type.Name == "StockInfo")
                    yield return (StockInfo)asset;
            }
        }

        public IEnumerable<FondInfo> GetFonds(int portfolioId, string userId)
        {
            var portfolio = GetPortfolioData(portfolioId);

            if (portfolio == null || portfolio.UserId != userId)
            {
                yield break;
            }

            foreach (var asset in portfolio.Assets)
            {
                var type = asset.GetType();

                if (type.Name == "FondInfo")
                    yield return (FondInfo)asset;
            }
        }

        public IEnumerable<BondInfo> GetBonds(int portfolioId, string userId)
        {
            var portfolio = GetPortfolioData(portfolioId);

            if (portfolio == null || portfolio.UserId != userId)
            {
                yield break;
            }

            foreach (var asset in portfolio.Assets)
            {
                var type = asset.GetType();

                if (type.Name == "BondInfo")
                    yield return (BondInfo)asset;
            }
        }
        
        private PortfolioData GetPortfolioData(int portfolioId)
        {
            var userPortfolio = _financeData.EfContext.Portfolios
                .Find(portfolioId);

            if (userPortfolio == null)
            {
                return null;
            }
            
            var portfolioData = new PortfolioData
            {
                Id = userPortfolio.Id,
                Name = userPortfolio.Name,
                UserId = userPortfolio.UserId,
                Assets = _assetsFactory.Create(portfolioId)
            };

            return portfolioData;
        }

        private OperationResult CommonValidate(int portfolioId, string userId,
            InvestIn.Core.Entities.Finance.Portfolio portfolio)
        {
            var validationResult = CommonValidate<int>(portfolioId, userId, portfolio);

            if (validationResult == null) 
                return null;
            
            return new OperationResult()
            {
                Message = validationResult.Message,
                IsSuccess = validationResult.IsSuccess
            };
        }
        
        private OperationResult<TResult> CommonValidate<TResult>(int portfolioId, string userId, 
            InvestIn.Core.Entities.Finance.Portfolio portfolio)
        {
            if (portfolio == null)
            {
                return new OperationResult<TResult>()
                {
                    IsSuccess = false,
                    Message = $"Портфель с id={portfolioId} не найден"
                };
            }

            if (portfolio.UserId != userId)
            {
                return new OperationResult<TResult>()
                {
                    IsSuccess = false,
                    Message = $"Портфель с id={portfolioId} вам не принадлежит"
                };
            }

            return null;
        }
    }
}