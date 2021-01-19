using System.Collections.Generic;
using InvestIn.Core.Interfaces;

namespace InvestIn.Core.Entities.Finance
{
    public class CurrencyAction : IEntityBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<CurrencyOperation> CurrencyOperations { get; set; }
    }
}