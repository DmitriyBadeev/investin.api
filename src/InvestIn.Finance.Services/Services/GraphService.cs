using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestIn.Finance.Services.DTO.Graphs;
using InvestIn.Finance.Services.Interfaces;
using InvestIn.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace InvestIn.Finance.Services.Services
{
    public class GraphService : IGraphService
    {
        private readonly IStockMarketData _marketData;
        private readonly FinanceDataService _financeDataService;

        public GraphService(IStockMarketData marketData, FinanceDataService financeDataService)
        {
            _marketData = marketData;
            _financeDataService = financeDataService;
        }

        public async Task<List<StockCandle>> StockCandles(string ticket, DateTime from, CandleInterval interval)
        {
            var data = await _marketData.GetStockCandleData(ticket, from, interval);

            var openIndex = data.candles.columns.IndexOf("open");
            var closeIndex = data.candles.columns.IndexOf("close");
            var highIndex = data.candles.columns.IndexOf("high");
            var lowIndex = data.candles.columns.IndexOf("low");
            var valueIndex = data.candles.columns.IndexOf("value");
            var volumeIndex = data.candles.columns.IndexOf("volume");
            var dateTimeIndex = data.candles.columns.IndexOf("begin");

            var candles = new List<StockCandle>();

            foreach (var candleData in data.candles.data)
            {
                candles.Add(new StockCandle()
                {
                    Open = candleData[openIndex].GetDouble(),
                    Close = candleData[closeIndex].GetDouble(),
                    High = candleData[highIndex].GetDouble(),
                    Low = candleData[lowIndex].GetDouble(),
                    Value = candleData[valueIndex].GetDouble(),
                    Volume = candleData[volumeIndex].GetDouble(),
                    DateTime = DateTime.Parse(candleData[dateTimeIndex].GetString()),
                });
            }

            return candles;
        }

        public List<TimeValue> PortfolioCostGraph(int portfolioId, string userId)
        {
            var reports = _financeDataService.EfContext.DailyPortfolioReports
                .Include(r => r.Portfolio)
                .Where(r => r.PortfolioId == portfolioId && r.Portfolio.UserId == userId);
            
            var graphData = new List<TimeValue>();
            
            foreach (var report in reports)
            {
                var cost = report.Cost;
                var datestamp = report.Time.MillisecondsTimestamp();
                
                var dateValue = new TimeValue
                {
                    Value = cost, 
                    Date = datestamp,
                };
                graphData.Add(dateValue);
            }

            return graphData;
        }
    }
}
