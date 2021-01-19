using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InvestIn.Core.Entities.Finance;
using InvestIn.Core.Interfaces;

namespace InvestIn.Infrastructure.Services
{
    public static class SeedFinanceData
    {
        public static string BUY_ACTION = "Покупка";
        public static string SELL_ACTION = "Продажа";
        
        public static string REFILL_ACTION = "Пополнение";
        public static string WITHDRAWAL_ACTION = "Вывод";
        
        public static string STOCK_ASSET_TYPE = "Акция";
        public static string FOND_ASSET_TYPE = "Фонд";
        public static string BOND_ASSET_TYPE = "Облигация";

        public static string RUB_CURRENCY_ID = "SUR";
        public static string RUB_CURRENCY_NAME = "Рубль";

        public static string SBER_TYPE = "СберБанк Инвестор";
        public static string TINKOFF_TYPE = "Тинькофф Инвестиции";
    }

    public class SeedFinanceDataService : ISeedDataService
    {
        private readonly ILogger<SeedFinanceDataService> _logger;
        private readonly FinanceDbContext _context;

        public SeedFinanceDataService(ILogger<SeedFinanceDataService> logger, FinanceDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void Initialise()
        {
            _logger.LogInformation("Seeding database");

            if (_context.Database.IsSqlServer())
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _logger.LogInformation("Migrating database");
                    _context.Database.Migrate();
                    _logger.LogInformation("Database has migrated successfully");
                }
            }
            
            AddActions();
            AddAssetTypes();
            AddPortfolioTypes();
            
            _logger.LogInformation("Database has seeded successfully");
        }

        private void AddActions()
        {
            if (!_context.AssetActions.Any())
            {
                _logger.LogInformation("Add asset actions");
                var buy = new AssetAction()
                {
                    Name = SeedFinanceData.BUY_ACTION
                };

                var sell = new AssetAction()
                {
                    Name = SeedFinanceData.SELL_ACTION
                };

                _context.AssetActions.AddRange(buy, sell);
                _context.SaveChanges();
                _logger.LogInformation("Added asset actions successfully");
            }

            if (!_context.CurrencyActions.Any())
            {
                _logger.LogInformation("Adding currency actions");
                var refill = new CurrencyAction()
                {
                    Name = SeedFinanceData.REFILL_ACTION
                };

                var withdrawal = new CurrencyAction()
                {
                    Name = SeedFinanceData.WITHDRAWAL_ACTION
                };

                _context.CurrencyActions.AddRange(refill, withdrawal);
                _context.SaveChanges();
                _logger.LogInformation("Added currency actions successfully");
            }
        }

        private void AddAssetTypes()
        {
            if (!_context.AssetTypes.Any())
            {
                _logger.LogInformation("Adding asset types");
                var stock = new AssetType()
                {
                    Name = SeedFinanceData.STOCK_ASSET_TYPE
                };

                var fond = new AssetType()
                {
                    Name = SeedFinanceData.FOND_ASSET_TYPE
                };

                var bond = new AssetType()
                {
                    Name = SeedFinanceData.BOND_ASSET_TYPE
                };

                _context.AssetTypes.AddRange(stock, fond, bond);
                _context.SaveChanges();
                _logger.LogInformation("Added asset types successfully");
            }
        }

        private void AddPortfolioTypes()
        {
            if (!_context.PortfolioTypes.Any())
            {
                _logger.LogInformation("Adding portfolio types");

                var sber = new PortfolioType()
                {
                    Name = SeedFinanceData.SBER_TYPE,
                    IconUrl = "https://storage.badeev.info/icons/sber.svg"
                };

                var tinkoff = new PortfolioType()
                {
                    Name = SeedFinanceData.TINKOFF_TYPE,
                    IconUrl = "https://storage.badeev.info/icons/tinkoff.svg"
                };
                
                _context.PortfolioTypes.AddRange(sber, tinkoff);
                _context.SaveChanges();
                
                _logger.LogInformation("Added portfolio types successfully");
            }
        }
    }
}