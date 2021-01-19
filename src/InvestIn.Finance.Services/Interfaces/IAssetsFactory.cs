using System.Collections.Generic;
using InvestIn.Finance.Services.Entities;

namespace InvestIn.Finance.Services.Interfaces
{
    public interface IAssetsFactory
    {
        List<AssetInfo> Create(int portfolioId);
    }
}