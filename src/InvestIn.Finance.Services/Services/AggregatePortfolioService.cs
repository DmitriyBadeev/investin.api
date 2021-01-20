﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Entities;
using InvestIn.Finance.Services.Interfaces;

namespace InvestIn.Finance.Services.Services
{
    public class AggregatePortfolioService : IAggregatePortfolioService
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IBalanceService _balanceService;

        public AggregatePortfolioService(IPortfolioService portfolioService, IBalanceService balanceService)
        {
            _portfolioService = portfolioService;
            _balanceService = balanceService;
        }

        public async Task<OperationResult<List<Payment>>> AggregatePayments(IEnumerable<int> portfolioIds, string userId)
        {
            var payments = new List<Payment>();

            var ids = portfolioIds.ToList();
            foreach (var portfolioId in ids)
            {
                var result = await _portfolioService.GetPortfolioPayments(portfolioId, userId);

                if (!result.IsSuccess)
                {
                    return result;
                }
                
                payments.AddRange(result.Result);
            }

            return new OperationResult<List<Payment>>()
            {
                IsSuccess = true,
                Message = $"Выплаты для портфелей(я) c id={string.Join(", ", ids)}",
                Result = payments
            };
        }

        public async Task<OperationResult<List<PaymentData>>> AggregateFuturePayments(IEnumerable<int> portfolioIds, string userId)
        {
            var payments = new List<PaymentData>();
            
            var ids = portfolioIds.ToList();
            foreach (var portfolioId in ids)
            {
                var result = await _portfolioService.GetFuturePortfolioPayments(portfolioId, userId);

                if (!result.IsSuccess)
                {
                    return result;
                }
                
                payments.AddRange(result.Result);
            }
            
            return new OperationResult<List<PaymentData>>()
            {
                IsSuccess = true,
                Message = $"Будущие выплаты для портфелей(я) c id={string.Join(", ", ids)}",
                Result = payments
            };
        }

        public async Task<OperationResult<ValuePercent>> AggregatePaymentProfit(IEnumerable<int> portfolioIds, string userId)
        {
            var sumProfit = 0;
            var sumInvest = 0;
            
            var ids = portfolioIds.ToList();
            foreach (var portfolioId in ids)
            {
                var resultProfit = await _portfolioService.GetPortfolioPaymentProfit(portfolioId, userId);
                var resultInvestSum = _balanceService.GetInvestSum(portfolioId, userId);
                
                if (!resultProfit.IsSuccess)
                {
                    return resultProfit;
                }

                sumProfit += resultProfit.Result.Value;
                sumInvest += resultInvestSum;
            }
            
            var percent = FinanceHelpers.DivWithOneDigitRound(sumProfit, sumInvest);

            return new OperationResult<ValuePercent>()
            {
                IsSuccess = true,
                Message = $"Суммарная дивидендная прибыль портфелей(я) c id={string.Join(", ", ids)}",
                Result = new ValuePercent()
                {
                    Value = sumProfit,
                    Percent = percent
                }
            };
        }

        public async Task<OperationResult<ValuePercent>> AggregatePaperProfit(IEnumerable<int> portfolioIds, string userId)
        {
            var sumProfit = 0;
            var sumInvest = 0;
            
            var ids = portfolioIds.ToList();
            foreach (var portfolioId in ids)
            {
                var resultProfit = await _portfolioService.GetPaperProfit(portfolioId, userId);
                var resultInvestSum = _balanceService.GetInvestSum(portfolioId, userId);
                
                if (!resultProfit.IsSuccess)
                {
                    return resultProfit;
                }

                sumProfit += resultProfit.Result.Value;
                sumInvest += resultInvestSum;
            }
            
            var percent = FinanceHelpers.DivWithOneDigitRound(sumProfit, sumInvest);

            return new OperationResult<ValuePercent>()
            {
                IsSuccess = true,
                Message = $"Суммарная бумажная прибыль портфелей(я) c id={string.Join(", ", ids)}",
                Result = new ValuePercent()
                {
                    Value = sumProfit,
                    Percent = percent
                }
            };
        }

        public async Task<OperationResult<int>> AggregateCost(IEnumerable<int> portfolioIds, string userId)
        {
            var cost = 0;
            
            var ids = portfolioIds.ToList();
            foreach (var portfolioId in ids)
            {
                var resultCost = await _portfolioService.GetCost(portfolioId, userId);
                
                if (!resultCost.IsSuccess)
                {
                    return resultCost;
                }

                cost += resultCost.Result;
            }
            
            return new OperationResult<int>()
            {
                IsSuccess = true,
                Message = $"Суммарная стоимость портфелей(я) c id={string.Join(", ", ids)}",
                Result = cost
            };
        }

        public IEnumerable<StockInfo> AggregateStocks(IEnumerable<int> portfolioIds, string userId)
        {
            return portfolioIds.SelectMany(portfolioId => _portfolioService.GetStocks(portfolioId, userId));
        }
        
        public IEnumerable<FondInfo> AggregateFonds(IEnumerable<int> portfolioIds, string userId)
        {
            return portfolioIds.SelectMany(portfolioId => _portfolioService.GetFonds(portfolioId, userId));
        }
        
        public IEnumerable<BondInfo> AggregateBonds(IEnumerable<int> portfolioIds, string userId)
        {
            return portfolioIds.SelectMany(portfolioId => _portfolioService.GetBonds(portfolioId, userId));
        }
    }
}