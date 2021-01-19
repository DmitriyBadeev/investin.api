using System;
using InvestIn.Core.Entities.Finance;
using InvestIn.Core.Interfaces;

namespace InvestIn.Core.Entities.Reports
{
    public class DailyPortfolioReport : IEntityBase
    {
        public int Id { get; set; }
        
        public int Cost { get; set; }

        public int Profit { get; set; }

        public DateTime Time { get; set; }

        public int PortfolioId { get; set; }

        public Portfolio Portfolio { get; set; }
    }
}