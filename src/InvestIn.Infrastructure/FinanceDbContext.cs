using Microsoft.EntityFrameworkCore;
using InvestIn.Core.Entities.Finance;
using InvestIn.Core.Entities.Reports;

namespace InvestIn.Infrastructure
{
    public class FinanceDbContext : DbContext
    {
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<AssetOperation> AssetOperations { get; set; }
        public DbSet<AssetAction> AssetActions { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<CurrencyOperation> CurrencyOperations { get; set; }
        public DbSet<CurrencyAction> CurrencyActions { get; set; }
        
        public DbSet<PortfolioType> PortfolioTypes { get; set; }
        
        public DbSet<DailyPortfolioReport> DailyPortfolioReports { get; set; }
    
        public DbSet<Payment> Payments { get; set; }
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
        {
        }
    }
}
