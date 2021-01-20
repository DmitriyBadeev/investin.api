using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvestIn.Core.Entities.Finance;
using InvestIn.Core.Entities.Reports;
using NUnit.Framework;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Finance.Services.Services;
using InvestIn.Finance.Services;
using InvestIn.Infrastructure.Services;
using RichardSzalay.MockHttp;

namespace InvestIn.Finance.Services.Test.ServicesTests
{
    [TestFixture]
    public class GraphServiceTests
    {
        private IGraphService _graphService;
        private MockHttpMessageHandler _mockHttp;
        private FinanceDataService _financeDataService;
        
        [SetUp]
        public void Setup()
        {
            var context = TestHelpers.GetMockFinanceDbContext();
            _financeDataService = new FinanceDataService(context);

            _mockHttp = new MockHttpMessageHandler();
            var client = _mockHttp.ToHttpClient();
            var stockMarketAPI = new StockMarketAPI(client);
            MockData();
            var stockMarketData = new StockMarketData(stockMarketAPI);
            _graphService = new GraphService(stockMarketData, _financeDataService);
        }

        [Test]
        public async Task StockCandles()
        {
            var candles = await _graphService.StockCandles("YNDX", new DateTime(2020, 6, 2), CandleInterval.Day);

            Assert.AreEqual(64, candles.Count);
        }

        [Test]
        public void PortfolioCostGraph()
        {
            var data = _graphService.PortfolioCostGraph(1, "1");
            
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(DateTime.Today.MillisecondsTimestamp(), data[0].Date);
            Assert.AreEqual(1000, data[0].Value);
            
            Assert.AreEqual(DateTime.Today.AddDays(1).MillisecondsTimestamp(), data[1].Date);
            Assert.AreEqual(2000, data[1].Value);
        }

        [Test]
        public async Task AggregatePortfolioCostGraph()
        {
            var data1 = await _graphService.AggregatePortfolioCostGraph(new []{1, 2}, "1");
            var data2 = await _graphService.AggregatePortfolioCostGraph(new []{1}, "1");
            var data3 = await _graphService.AggregatePortfolioCostGraph(new []{1, 2, 3}, "1");
            var data4 = await _graphService.AggregatePortfolioCostGraph(new []{1, 99, 2}, "1");
            
            Assert.AreEqual(2, data1.Count);
            Assert.AreEqual(1, data1[0].PortfolioId);
            Assert.AreEqual(2, data1[1].PortfolioId);
            Assert.AreEqual("TEST 1", data1[0].PortfolioName);
            Assert.AreEqual("TEST 2", data1[1].PortfolioName);
            
            Assert.AreEqual(1, data2.Count);
            Assert.AreEqual(3, data3.Count);
            Assert.AreEqual(0, data3[2].Data.Count);
            
            Assert.AreEqual(2, data4.Count);
        }

        private void MockData()
        {
            TestHelpers.MockCandles(_mockHttp);

            _financeDataService.EfContext.Portfolios.AddRange(new List<Portfolio>
            {
                new Portfolio
                {
                    Id = 1,
                    Name = "TEST 1",
                    UserId = "1",
                },
                new Portfolio
                {
                    Id = 2,
                    Name = "TEST 2",
                    UserId = "1"
                },
                new Portfolio
                {
                    Id = 3,
                    Name = "TEST 2",
                    UserId = "2"
                },
            });

            _financeDataService.EfContext.DailyPortfolioReports.AddRange(new DailyPortfolioReport
            {
                PortfolioId = 1,
                Cost = 1000,
                Profit = 100,
                Time = DateTime.Today,
            }, new DailyPortfolioReport
            {
                PortfolioId = 1,
                Cost = 2000,
                Profit = 200,
                Time = DateTime.Today.AddDays(1),
            }, new DailyPortfolioReport
            {
                PortfolioId = 2,
                Cost = 1000,
                Profit = 100,
                Time = DateTime.Today,
            }, new DailyPortfolioReport
            {
                PortfolioId = 2,
                Cost = 1000,
                Profit = 100,
                Time = DateTime.Today.AddDays(1),
            }, new DailyPortfolioReport
            {
                PortfolioId = 3,
                Cost = 5000,
                Profit = 500,
                Time = DateTime.Today.AddDays(1),
            });

            _financeDataService.EfContext.SaveChanges();
        }
    }
}