using InvestIn.Core.Interfaces;

namespace InvestIn.Core.Entities.Finance
{
    public class Asset : IEntityBase
    {
        public int Id { get; set; }

        public string Ticket { get; set; }

        public string ShortName { get; set; }

        public string MarketFullName { get; set; }

        public string FullName { get; set; }

        public string LatName { get; set; }

        public AssetType AssetType { get; set; }

        public int AssetTypeId { get; set; }

        public int LotSize { get; set; }

        public long IssueSize { get; set; }

        public double PrevClosePrice { get; set; }

        public long Capitalization { get; set; }

        public string Description { get; set; }

        public string Sector { get; set; }
        
        public double Price { get; set; }

        public double PriceChange { get; set; }
        
        public string UpdateTime { get; set; }
    }
}