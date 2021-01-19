using System;
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

        private void MockData()
        {
            TestHelpers.MockCandles(_mockHttp);

            _financeDataService.EfContext.Portfolios.Add(new Portfolio
            {
                Id = 1,
                Name = "TEST",
                UserId = "1",
            });
            
            _financeDataService.EfContext.DailyPortfolioReports.AddRange(new []
            {
                new DailyPortfolioReport
                {
                    PortfolioId = 1,
                    Cost = 1000,
                    Profit = 100,
                    Time = DateTime.Today,
                },
                new DailyPortfolioReport
                {
                    PortfolioId = 1,
                    Cost = 2000,
                    Profit = 200,
                    Time = DateTime.Today.AddDays(1),
                },
            });

            _financeDataService.EfContext.SaveChanges();
        }
    }
}