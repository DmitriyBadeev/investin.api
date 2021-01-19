using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvestIn.Finance.Services.DTO;
using InvestIn.Finance.Services.Services;

namespace InvestIn.Finance.Services.Interfaces
{
    public interface IGraphService
    {
        Task<List<StockCandle>> StockCandles(string ticket, DateTime from, CandleInterval interval);
    }
}