using System.Collections.Generic;
using InvestIn.Core.Interfaces;

namespace InvestIn.Core.Entities.Finance
{
    public class AssetType : IEntityBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<AssetOperation> AssetOperations { get; set; }
        
        public List<Asset> Assets { get; set; }
    }
}