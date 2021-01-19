using System.Threading.Tasks;
using InvestIn.Finance.Services.DTO;

namespace InvestIn.Finance.Services.Interfaces
{
    public interface ISearchService
    {
        Task<SearchData> Search(string code);
        Task<AssetData> GetAssetData(string ticket, string userId);
    }
}