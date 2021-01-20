using System.Collections.Generic;
using InvestIn.Core.Entities.Finance;
using InvestIn.Finance.Services.Entities;

namespace InvestIn.Finance.Services.DTO
{
    public class PortfolioData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }

        public Portfolio PortfolioEntity { get; set; } 

        public IEnumerable<AssetInfo> Assets { get; set; }
    }
}
