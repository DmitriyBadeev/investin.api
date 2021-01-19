﻿using System;
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
    public class BondInfoTests
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

        private AssetType _bondType = new AssetType()
        {
            Id = 1,
            Name = SeedFinanceData.BOND_ASSET_TYPE
        };

        [SetUp]
        public void Setup()
        {
            var mockHttp = new MockHttpMessageHandler();
            var client = mockHttp.ToHttpClient();
            TestHelpers.MockBondData(mockHttp);
            TestHelpers.MockCouponsData(mockHttp);
            var stockMarketAPI = new StockMarketAPI(client);
            _stockMarketData = new StockMarketData(stockMarketAPI);
            
            var context = TestHelpers.GetMockFinanceDbContext(); 
            _financeDataService = new FinanceDataService(context);
        }

        [Test]
        public async Task GetAllPrice()
        {
            var bond = Getbond();

            var price = await bond.GetAllPrice();

            Assert.AreEqual(318849, price);
        }

        [Test]
        public void AmortizationDate()
        {
            var bond = (BondInfo)Getbond();
            var amortizedBond = (BondInfo)GetAmortizedBond();

            Assert.AreEqual(new DateTime(2022, 7, 20), bond.AmortizationDate);
            Assert.AreEqual(new DateTime(2019, 12, 11), amortizedBond.AmortizationDate);
        }

        [Test]
        public void HasAmortized()
        {
            var bond = (BondInfo)Getbond();
            var amortizedBond = (BondInfo)GetAmortizedBond();

            Assert.IsFalse(bond.HasAmortized);
            Assert.IsTrue(amortizedBond.HasAmortized);
        }

        [Test]
        public void PaymentsData()
        {
            var bond = (BondInfo)Getbond();
            var amortizedBond = (BondInfo)GetAmortizedBond();

            Assert.AreEqual(6, bond.PaymentsData.Count);
            Assert.AreEqual(5, amortizedBond.PaymentsData.Count);
        }

        [Test]
        public void GetSumPayments()
        {
            var bond = (BondInfo)Getbond();
            var amortizedBond = (BondInfo)GetAmortizedBond();

            Assert.AreEqual(3790 * 3, bond.GetSumPayments());
            Assert.AreEqual(113564 * 2, amortizedBond.GetSumPayments());
        }

        private AssetInfo Getbond()
        {
            var operations = new List<AssetOperation>()
            {
                new AssetOperation()
                {
                    Id = 1,
                    Ticket = "SU26209RMFS5",
                    Amount = 2,
                    PaymentPrice = 103180,
                    PortfolioId = 1,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    AssetType = _bondType,
                    AssetTypeId = _bondType.Id,
                    Date = new DateTime(2020, 2, 7)
                },
                new AssetOperation()
                {
                    Id = 2,
                    Ticket = "SU26209RMFS5",
                    Amount = 1,
                    PortfolioId = 1,
                    PaymentPrice = 103230,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    AssetType = _bondType,
                    AssetTypeId = _bondType.Id,
                    Date = new DateTime(2020, 4, 4)
                }
            };
            
            var payments = new List<Payment>()
            {
                new Payment()
                {
                    PortfolioId = 1,
                    Ticket = "SU26209RMFS5",
                    Amount = 3,
                    Date = DateTime.Now,
                    PaymentValue = 11370
                },
            };
            _financeDataService.EfContext.Payments.AddRange(payments);
            _financeDataService.EfContext.SaveChanges();
            
            var stockInfo = new BondInfo(_stockMarketData, _financeDataService, "SU26209RMFS5");
            foreach (var assetOperation in operations)
            {
                stockInfo.RegisterOperation(assetOperation);
            }

            return stockInfo;
        }

        private AssetInfo GetAmortizedBond()
        {
            var operations = new List<AssetOperation>()
            {
                new AssetOperation()
                {
                    Id = 1,
                    Ticket = "SU26210RMFS3",
                    PortfolioId = 1,
                    Amount = 2,
                    PaymentPrice = 101180,
                    AssetAction = _buyAction,
                    AssetActionId = _buyAction.Id,
                    AssetType = _bondType,
                    AssetTypeId = _bondType.Id,
                    Date = new DateTime(2018, 2, 7)
                },
            };
            
            var payments = new List<Payment>()
            {
                new Payment()
                {
                    PortfolioId = 1,
                    Ticket = "SU26210RMFS3",
                    Amount = 2,
                    Date = DateTime.Now,
                    PaymentValue = 227128
                },
            };
            _financeDataService.EfContext.Payments.AddRange(payments);
            _financeDataService.EfContext.SaveChanges();
            
            var stockInfo = new BondInfo(_stockMarketData, _financeDataService,"SU26210RMFS3");
            foreach (var assetOperation in operations)
            {
                stockInfo.RegisterOperation(assetOperation);
            }

            return stockInfo;
        }
    }
}