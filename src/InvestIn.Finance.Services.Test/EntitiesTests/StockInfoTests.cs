using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.Entities;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Finance.Services.Services;
using InvestIn.Infrastructure.Services;
using RichardSzalay.MockHttp;

namespace InvestIn.Finance.Services.Test.EntitiesTests
{
    [TestFixture]
    public class StockInfoTests
    {
        private IStockMarketData _stockMarketData;
        private FinanceDataService _financeDataService;
        
        private AssetAction _buyAction = new AssetAction()
        {
            Id = 1,
            Name = SeedFinanceData.BUY_ACTION
        };

        private AssetAction _sellAction = new AssetAction()
        {
            Id = 1,
            Name = SeedFinanceData.SELL_ACTION
        };

        private AssetType _stockType = new AssetType()
        {
            Id = 1,
            Name = SeedFinanceData.STOCK_ASSET_TYPE
        };

        [SetUp]
        public void Setup()
        { 
            var mockHttp = new MockHttpMessageHandler();
            var client = mockHttp.ToHttpClient();
            TestHelpers.MockStockData(mockHttp);
            TestHelpers.MockDividendData(mockHttp);
            var stockMarketAPI = new StockMarketAPI(client);
            _stockMarketData = new StockMarketData(stockMarketAPI);
            
            var context = TestHelpers.GetMockFinanceDbContext(); 
            _financeDataService = new FinanceDataService(context);
            MockPortfolios();
        }

        [Test]
        public async Task GetPrice1()
        {
            var stockInfo = GetYNDXStock();
            var price = await stockInfo.GetPrice();

            Assert.AreEqual(4347.20, price);
        }

        [Test]
        public async Task GetPrice2()
        {
            var stockInfo = GetSBERStock();
            var price = await stockInfo.GetPrice();

            Assert.AreEqual(226.58, price);
        }

        [Test]
        public async Task GetPaperProfit1()
        {
            var stockInfo = GetYNDXStock();
            var profit = await stockInfo.GetPaperProfit();

            Assert.AreEqual(434720 - 312430, profit);
        }

        [Test]
        public async Task GetPaperProfit2()
        {
            var stockInfo = GetSBERStock();
            var profit = await stockInfo.GetPaperProfit();

            Assert.AreEqual(-1134228, profit);
        }

        [Test]
        public async Task NameAsset()
        {
            var stockInfo = GetSBERStock();
            var name = await stockInfo.GetName();

            Assert.AreEqual("Сбербанк".Length, name.Length);
        }

        [Test]
        public void GetDividendsData()
        {
            var stockSBERInfo = GetSBERStock();
            var stockYNDXInfo = GetYNDXStock();

            Assert.AreEqual(7, stockSBERInfo.PaymentsData.Count);
            Assert.AreEqual(0, stockYNDXInfo.PaymentsData.Count);

            Assert.AreEqual("RUB", stockSBERInfo.PaymentsData[0].CurrencyId);
            Assert.AreEqual(320, stockSBERInfo.PaymentsData[0].PaymentValue);
            Assert.AreEqual(new DateTime(2014, 6, 17), stockSBERInfo.PaymentsData[0].RegistryCloseDate);
            Assert.AreEqual("SBER", stockSBERInfo.PaymentsData[0].Ticket);
        }

        [Test]
        public void PaymentsData()
        {
            var stockSBERInfo = GetSBERStock();
            var stockYNDXInfo = GetYNDXStock();

            Assert.AreEqual(1, stockSBERInfo.GetFuturePayments().Count);
            Assert.AreEqual(1870, stockSBERInfo.GetFuturePayments()[0].PaymentValue);
            Assert.AreEqual(0, stockYNDXInfo.GetFuturePayments().Count);
        }

        [Test]
        public void GetPaidPayments()
        {
            var stockSBERInfo = GetSBERStock();
            var stockYNDXInfo = GetYNDXStock();

            Assert.AreEqual(2, stockSBERInfo.GetPaidPayments().Count);
            Assert.AreEqual(0, stockYNDXInfo.GetPaidPayments().Count);
        }

        [Test]
        public void GetSumPayments()
        {
            var stockSBERInfo = GetSBERStock();
            var stockYNDXInfo = GetYNDXStock();

            Assert.AreEqual(3600 + 6400, stockSBERInfo.GetSumPayments());
            Assert.AreEqual(0, stockYNDXInfo.GetSumPayments());
        }

        [Test]
        public void GetPriceChange()
        {
            var stockSBERInfo = GetSBERStock();
            var stockYNDXInfo = GetYNDXStock();

            Assert.AreEqual(-26, stockSBERInfo.GetPriceChange().Result);
            Assert.AreEqual(82, stockYNDXInfo.GetPriceChange().Result);
        }

        [Test]
        public void GetPaperProfitPercent()
        {
            var stockSBERInfo = GetSBERStock();
            var stockYNDXInfo = GetYNDXStock();

            Assert.AreEqual(-92.6, stockSBERInfo.GetPaperProfitPercent().Result);
            Assert.AreEqual(39.1, stockYNDXInfo.GetPaperProfitPercent().Result);
        }

        [Test]
        public void GetUpdateTime()
        {
            var stockSBERInfo = GetSBERStock();
            var stockYNDXInfo = GetYNDXStock();

            Assert.AreEqual("16:14:14", stockSBERInfo.GetUpdateTime().Result);
            Assert.AreEqual("16:03:39", stockYNDXInfo.GetUpdateTime().Result);
        }

        [Test]
        public void GetNearestPayment()
        {
            var stockSBERInfo = GetSBERStock();
            var stockYNDXInfo = GetYNDXStock();

            Assert.AreEqual(1870, stockSBERInfo.GetNearestPayment().PaymentValue);
            Assert.AreEqual(null, stockYNDXInfo.GetNearestPayment());
        }
        
        private void MockPortfolios()
        {
            _financeDataService.EfContext.Portfolios.AddRange(new Portfolio()
            {
                Id = 1,
                Name = "TEST 1",
                UserId = "1"
            }, new Portfolio()
            {
                Id = 2,
                Name = "TEST 2",
                UserId = "1"
            }, new Portfolio()
            {
                Id = 3,
                Name = "TEST 3",
                UserId = "2"
            });

            _financeDataService.EfContext.SaveChanges();
        }

        private AssetInfo GetYNDXStock()
        {
            var operations = new List<AssetOperation>()
            {
                //price = 407800
                //profit = 95370
                new AssetOperation()
                {
                    Id = 1,
                    Ticket = "YNDX",
                    Amount = 2,
                    PaymentPrice = 624860,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    AssetType = _stockType,
                    AssetTypeId = _stockType.Id,
                    Date = new DateTime(2019, 11, 18),
                    PortfolioId = 1
                },
                new AssetOperation()
                {
                    Id = 2,
                    Ticket = "YNDX",
                    Amount = 1,
                    PaymentPrice = 312430,
                    AssetAction = _sellAction,
                    AssetActionId = _sellAction.Id,
                    AssetType = _stockType,
                    AssetTypeId = _stockType.Id,
                    Date = new DateTime(2020, 2, 4),
                    PortfolioId = 1
                },
                new AssetOperation()
                {
                    Id = 2,
                    Ticket = "YNDX",
                    Amount = 1,
                    PaymentPrice = 312430,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    AssetType = _stockType,
                    AssetTypeId = _stockType.Id,
                    Date = new DateTime(2020, 2, 4),
                    PortfolioId = 2
                },
                new AssetOperation()
                {
                    Id = 2,
                    Ticket = "YNDX",
                    Amount = 1,
                    PaymentPrice = 312430,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    AssetType = _stockType,
                    AssetTypeId = _stockType.Id,
                    Date = new DateTime(2020, 2, 4),
                    PortfolioId = 3
                },
            };

            var stockInfo1 = new StockInfo(_stockMarketData, _financeDataService, "YNDX", 1);
            var stockInfo2 = new StockInfo(_stockMarketData, _financeDataService, "YNDX", 2);
            var stockInfo3 = new StockInfo(_stockMarketData, _financeDataService, "YNDX", 3);

            foreach (var assetOperation in operations)
            {
                stockInfo1.RegisterOperation(assetOperation);
                stockInfo2.RegisterOperation(assetOperation);
                stockInfo3.RegisterOperation(assetOperation);
            }


            return stockInfo1;
        }

        private AssetInfo GetSBERStock()
        {
            var operations = new List<AssetOperation>()
            {
                new AssetOperation()
                {
                    Id = 3,
                    Ticket = "SBER",
                    Amount = 3,
                    PaymentPrice = 1012430,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    PortfolioId = 1,
                    AssetType = _stockType,
                    AssetTypeId = _stockType.Id,
                    Date = new DateTime(2018, 1, 4),
                },
                new AssetOperation()
                {
                    Id = 4,
                    Ticket = "SBER",
                    Amount = 1,
                    PortfolioId = 1,
                    PaymentPrice = 212430,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    AssetType = _stockType,
                    AssetTypeId = _stockType.Id,
                    Date = new DateTime(2019, 4, 4)
                },
                new AssetOperation()
                {
                    Id = 3,
                    Ticket = "SBER",
                    Amount = 3,
                    PaymentPrice = 1012430,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    PortfolioId = 2,
                    AssetType = _stockType,
                    AssetTypeId = _stockType.Id,
                    Date = new DateTime(2018, 1, 4)
                },
                new AssetOperation()
                {
                    Id = 3,
                    Ticket = "SBER",
                    Amount = 3,
                    PaymentPrice = 1012430,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    PortfolioId = 3,
                    AssetType = _stockType,
                    AssetTypeId = _stockType.Id,
                    Date = new DateTime(2018, 1, 4)
                },
            };
            
            var payments = new List<Payment>()
            {
                new Payment()
                {
                    PortfolioId = 1,
                    Ticket = "SBER",
                    Amount = 3,
                    Date = DateTime.Now,
                    PaymentValue = 3600
                },
                new Payment()
                {
                    PortfolioId = 1,
                    Ticket = "SBER",
                    Amount = 4,
                    Date = DateTime.Now,
                    PaymentValue = 6400
                },
                new Payment()
                {
                    PortfolioId = 2,
                    Ticket = "SBER",
                    Amount = 4,
                    Date = DateTime.Now,
                    PaymentValue = 6400
                },
                new Payment()
                {
                    PortfolioId = 3,
                    Ticket = "SBER",
                    Amount = 4,
                    Date = DateTime.Now,
                    PaymentValue = 6400
                },
            };
            
            _financeDataService.EfContext.Payments.AddRange(payments);
            _financeDataService.EfContext.SaveChanges();
            
            var stockInfo1 = new StockInfo(_stockMarketData, _financeDataService,"SBER", 1);
            var stockInfo2 = new StockInfo(_stockMarketData, _financeDataService,"SBER", 2);
            var stockInfo3 = new StockInfo(_stockMarketData, _financeDataService,"SBER", 3);
            foreach (var assetOperation in operations)
            {
                stockInfo1.RegisterOperation(assetOperation);
                stockInfo2.RegisterOperation(assetOperation);
                stockInfo3.RegisterOperation(assetOperation);
            }

            return stockInfo1;
        }
    }
}