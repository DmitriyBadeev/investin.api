using System.Collections.Generic;
using InvestIn.Core.Entities.Reports;
using InvestIn.Core.Interfaces;

namespace InvestIn.Core.Entities.Finance
{
    public class Portfolio : IEntityBase
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public int? PortfolioTypeId { get; set; }

        public PortfolioType PortfolioType { get; set; }

        public List<AssetOperation> AssetOperations { get; set; }
        
        public List<Payment> Payments { get; set; }

        public List<DailyPortfolioReport> DailyPortfolioReports { get; set; }
    }
}
