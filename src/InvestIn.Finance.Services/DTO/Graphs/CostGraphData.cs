using System.Collections.Generic;

namespace InvestIn.Finance.Services.DTO.Graphs
{
    public class CostGraphData
    {
        public int PortfolioId { get; set; }

        public string PortfolioName { get; set; }

        public List<TimeValue> Data { get; set; }
    }
}