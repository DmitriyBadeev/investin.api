using System.Collections.Generic;
using InvestIn.Core.Interfaces;

namespace InvestIn.Core.Entities.Finance
{
    public class PortfolioType : IEntityBase
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string IconUrl { get; set; }

        public List<Portfolio> Portfolios { get; set; }
    }
}