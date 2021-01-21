using System;

namespace InvestIn.Finance.Services.DTO.Graphs
{
    public class StockCandle
    {
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Value { get; set; }
        public double Volume { get; set; }
        public long Date { get; set; }
    }
}
