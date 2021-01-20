using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.DTO.Graphs;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Finance.Services.Services;

namespace InvestIn.Finance.API.Queries
{
    [ExtendObjectType(Name = "Queries")]
    public class GraphQueries
    {
        [Authorize]
        public async Task<List<StockCandle>> StockCandles([Service] IGraphService graphService,
            string ticket, DateTime from, CandleInterval interval)
        {
            return await graphService.StockCandles(ticket, from, interval);
        }

        [Authorize]
        public List<TimeValue> PortfolioCostGraph([Service] IGraphService graphService, 
            [CurrentUserIdGlobalState] string userId, int portfolioId)
        {
            return graphService.PortfolioCostGraph(portfolioId, userId);
        }

        [Authorize]
        public async Task<List<CostGraphData>> AggregatePortfolioCostGraph([Service] IGraphService graphService,
            [CurrentUserIdGlobalState] string userId, int[] portfolioIds)
        {
            return await graphService.AggregatePortfolioCostGraph(portfolioIds, userId);
        }
    }
}