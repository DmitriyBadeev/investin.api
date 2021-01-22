﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using InvestIn.Finance.Services.Services;
using InvestIn.Infrastructure.Services;
using RichardSzalay.MockHttp;

namespace InvestIn.Finance.Services.Test.ServicesTests
{
    [TestFixture]
    public class PortfolioServiceTests
    {
        private PortfolioService _portfolioService;
        private FinanceDataService _financeDataService;
        private BalanceService _balanceService;

        [SetUp]
        public void Setup()
        {
            var mockHttp = new MockHttpMessageHandler();
            var client = mockHttp.ToHttpClient();

            var stockMarketApi = new StockMarketAPI(client);
            var stockMarketData = new StockMarketData(stockMarketApi);
            
            var context = TestHelpers.GetMockFinanceDbContext();
            _financeDataService = new FinanceDataService(context);
          
            var assetFactory = new AssetsFactory(_financeDataService, stockMarketData);
            _balanceService = new BalanceService(_financeDataService);
            TestHelpers.SeedApp(context);
            _portfolioService = new PortfolioService(_financeDataService, _balanceService, assetFactory);

            TestHelpers.MockStockData(mockHttp);
            TestHelpers.MockFondData(mockHttp);
            TestHelpers.MockBondData(mockHttp);

            TestHelpers.MockCouponsData(mockHttp);
            TestHelpers.MockDividendData(mockHttp);
            
            TestHelpers.SeedOperations2(context);
        }

        [Test]
        public void GetPortfolioTypes()
        {
            var types = _portfolioService.GetPortfolioTypes();
            
            Assert.AreEqual(2, types.Count());
        }

        [Test]
        public async Task CreateTest()
        {
            var sberType = _portfolioService
                .GetPortfolioTypes()
                .FirstOrDefault(t => t.Name == SeedFinanceData.SBER_TYPE);
            
            var name = "Тестовый портфель 2";
            var result = await _portfolioService.CreatePortfolio(name, "1", sberType.Id);

            Assert.IsTrue(result.IsSuccess);
            
            var containsPortfolio = await 
                _financeDataService.EfContext.Portfolios.AnyAsync(p => p.Name == name && p.UserId == "1");
            
            Assert.IsTrue(containsPortfolio);
            
            var wrongTypeResult = await _portfolioService.CreatePortfolio("Неправильный тип", "1", 999);
            
            Assert.IsFalse(wrongTypeResult.IsSuccess);
        }

        [Test]
        public async Task CreateTest__SameName()
        {
            var type1 = _portfolioService
                .GetPortfolioTypes()
                .FirstOrDefault(t => t.Name == SeedFinanceData.SBER_TYPE);
            
            var type2 = _portfolioService
                .GetPortfolioTypes()
                .FirstOrDefault(t => t.Name == SeedFinanceData.TINKOFF_TYPE);
            
            var name = "Тестовый портфель 2";

            var result1 = await _portfolioService.CreatePortfolio(name, "1", type1.Id);
            Assert.IsTrue(result1.IsSuccess);

            var result2 = await _portfolioService.CreatePortfolio(name, "1", type1.Id);
            Assert.IsFalse(result2.IsSuccess);

            var result3 = await _portfolioService.CreatePortfolio(name, "2", type2.Id);
            Assert.IsTrue(result3.IsSuccess);
        }

        [Test]
        public async Task RemovePortfolio()
        {
            var result1 = await _portfolioService.RemovePortfolio(12, "1");
            Assert.IsFalse(result1.IsSuccess);

            var result2 = await _portfolioService.RemovePortfolio(12, "2");
            Assert.IsTrue(result2.IsSuccess);
            
            var result3 = await _portfolioService.RemovePortfolio(99, "2");
            Assert.IsFalse(result3.IsSuccess);
            
            var portfolios = _portfolioService.GetPortfolios("2");
            Assert.AreEqual(0, portfolios.Count());
        }

        [Test]
        public async Task GetPortfoliosTest()
        {
            var portfolios1 = _portfolioService.GetPortfolios("1");
            var portfolios2 = _portfolioService.GetPortfolios("2");

            Assert.AreEqual(2, portfolios1.Count());
            Assert.AreEqual(1, portfolios2.Count());
        }

        [Test]
        public async Task AddPaymentInPortfolio()
        {
            var result1 = await _portfolioService.AddPaymentInPortfolio(10, "1","SBER", 30, 
                60000, DateTime.Now);
            
            var result2 = await _portfolioService.AddPaymentInPortfolio(999, "1", "SBER", 30, 
                60000, DateTime.Now);
            
            var result3 = await _portfolioService.AddPaymentInPortfolio(10, "2", "SBER", 30, 
                60000, DateTime.Now);
            
            Assert.IsTrue(result1.IsSuccess, "Некорректное добавление выплаты");
            Assert.IsFalse(result2.IsSuccess, "Добавление выплаты в несуществеющий портфель");
            Assert.IsFalse(result3.IsSuccess, "Добавление выплаты в портфель, который не принадлежит пользователю");
        }

        [Test]
        public async Task GetPortfolioPayments()
        {
            var result1 = await _portfolioService.GetPortfolioPayments(10, "1");
            var result2 = await _portfolioService.GetPortfolioPayments(10, "2");
            
            Assert.IsTrue(result1.IsSuccess, "Неуспешное выполнение операции");
            Assert.IsFalse(result2.IsSuccess, "Получение выплат у чужого пользователя");
            Assert.AreEqual(3, result1.Result.Count, "Неверное количество выплат");
        }

        [Test]
        public void GetUserPayments()
        {
            var result1 = _portfolioService.GetUserPayments("1");
            var result2 = _portfolioService.GetUserPayments("2");
            
            Assert.AreEqual(4, result1.Count());
            Assert.AreEqual(1, result2.Count());
        }

        [Test]
        public async Task GetFuturePortfolioPayments()
        {
            var result1 = await _portfolioService.GetFuturePortfolioPayments(10, "1");
            var result2 = await _portfolioService.GetFuturePortfolioPayments(11, "1");
            var result3 = await _portfolioService.GetFuturePortfolioPayments(12, "2");
            var result4 = await _portfolioService.GetFuturePortfolioPayments(12, "1");
            
            Assert.IsTrue(result1.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(5, result1.Result.Count, "Неверное количество выплат");
            
            Assert.IsTrue(result2.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(1, result2.Result.Count, "Неверное количество выплат");
            
            Assert.IsTrue(result3.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(0, result3.Result.Count, "Неверное количество выплат");
            
            Assert.IsFalse(result4.IsSuccess, "Получение выплат у чужого пользователя");
            
        }

        [Test]
        public async Task GetPortfolioPaymentProfit()
        {
            var profit1 = await _portfolioService.GetPortfolioPaymentProfit(10, "1");
            var profit2 = await _portfolioService.GetPortfolioPaymentProfit(11, "1");
            var profit3 = await _portfolioService.GetPortfolioPaymentProfit(13, "1");

            Assert.IsTrue(profit1.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(115000, profit1.Result.Value, "Неверная прибыль");
            Assert.AreEqual(5.8, profit1.Result.Percent, "Неверный процент");
            
            Assert.IsTrue(profit2.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(5000, profit2.Result.Value, "Неверная прибыль");
            Assert.AreEqual(0.5, profit2.Result.Percent, "Неверный процент");
            
            Assert.IsFalse(profit3.IsSuccess, "Прибыль в чужом портфеле");
        }

        [Test]
        public async Task GetPaperProfit()
        {
            var profit1 = await _portfolioService.GetPaperProfit(10, "1");
            var profit2 = await _portfolioService.GetPaperProfit(11, "1");
            var profit3 = await _portfolioService.GetPaperProfit(12, "2");
            
            var profit4 = await _portfolioService.GetPaperProfit(12, "1");
            
            Assert.IsTrue(profit1.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(34720 * 2 + 26580 * 2 + 20000 + 6283 - 100000, profit1.Result.Value, "Неверная прибыль");
            Assert.AreEqual(FinanceHelpers.DivWithOneDigitRound(34720 * 2 + 26580 * 2 + 20000 + 6283 - 100000, 2000000), 
                profit1.Result.Percent, "Неверный процент");
            
            Assert.IsTrue(profit2.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(26580, profit2.Result.Value, "Неверная прибыль");
            Assert.AreEqual(FinanceHelpers.DivWithOneDigitRound(26580, 1000000), 
                profit2.Result.Percent, "Неверный процент");
            
            Assert.IsTrue(profit3.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(34720, profit3.Result.Value, "Неверная прибыль");
            Assert.AreEqual(FinanceHelpers.DivWithOneDigitRound(34720, 1500000), profit3.Result.Percent, "Неверный процент");
            
            Assert.IsFalse(profit4.IsSuccess, "Прибыль в чужом портфеле");
        }

        [Test]
        public async Task GetPaperPrice()
        {
            var paperPrice1 = await _portfolioService.GetPaperPrice(10, "1");
            var paperPrice2 = await _portfolioService.GetPaperPrice(11, "1");
            var paperPrice3 = await _portfolioService.GetPaperPrice(12, "2");
            
            var paperPrice4 = await _portfolioService.GetPaperPrice(12, "1");
            
            Assert.IsTrue(paperPrice1.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(434720 * 2 + 22658 * 20 + 101840 + 106283, paperPrice1.Result, "Неверная оценка");

            Assert.IsTrue(paperPrice2.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(22658 * 10, paperPrice2.Result, "Неверная оценка");

            Assert.IsTrue(paperPrice3.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(434720, paperPrice3.Result, "Неверная оценка");
            
            Assert.IsFalse(paperPrice4.IsSuccess, "Прибыль в чужом портфеле");
        }

        [Test]
        public async Task GetCost()
        {
            var cost1 = await _portfolioService.GetCost(10, "1");
            var cost2 = await _portfolioService.GetCost(11, "1");
            var cost3 = await _portfolioService.GetCost(12, "2");
            
            var cost4 = await _portfolioService.GetCost(12, "1");
            
            Assert.IsTrue(cost1.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(434720 * 2 + 22658 * 20 + 101840 + 106283 + 115000 + 600000 - 81840, 
                cost1.Result, "Неверная оценка");

            Assert.IsTrue(cost2.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(22658 * 10 + 5000 + 800000, cost2.Result, "Неверная оценка");

            Assert.IsTrue(cost3.IsSuccess, "Неуспешное выполнение операции");
            Assert.AreEqual(434720 + 10000 + 1100000, cost3.Result, "Неверная оценка");
            
            Assert.IsFalse(cost4.IsSuccess, "Прибыль в чужом портфеле");
        }

        [Test]
        public void GetStocks()
        {
            var stocks1 = _portfolioService.GetStocks(10, "1").Count();
            var stocks2 = _portfolioService.GetStocks(11, "1").Count();
            var stocks3 = _portfolioService.GetStocks(12, "2").Count();
            
            var stocks4 = _portfolioService.GetStocks(12, "1").Count();

            Assert.AreEqual(2, stocks1);
            Assert.AreEqual(1, stocks2);
            Assert.AreEqual(1, stocks3);
            Assert.AreEqual(0, stocks4);
        }
        
        [Test]
        public void GetFonds()
        {
            var stocks1 = _portfolioService.GetFonds(10, "1").Count();
            var stocks2 = _portfolioService.GetFonds(11, "1").Count();
            var stocks3 = _portfolioService.GetFonds(12, "2").Count();
            
            var stocks4 = _portfolioService.GetFonds(12, "1").Count();

            Assert.AreEqual(1, stocks1);
            Assert.AreEqual(0, stocks2);
            Assert.AreEqual(0, stocks3);
            Assert.AreEqual(0, stocks4);
        }
        
        [Test]
        public void GetBonds()
        {
            var stocks1 = _portfolioService.GetBonds(10, "1").Count();
            var stocks2 = _portfolioService.GetBonds(11, "1").Count();
            var stocks3 = _portfolioService.GetBonds(12, "2").Count();
            
            var stocks4 = _portfolioService.GetBonds(12, "1").Count();

            Assert.AreEqual(2, stocks1);
            Assert.AreEqual(0, stocks2);
            Assert.AreEqual(0, stocks3);
            Assert.AreEqual(0, stocks4);
        }
    }
}