﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InvestIn.Finance.Services.DTO.Responses;
using InvestIn.Finance.Services.Interfaces;

namespace InvestIn.Finance.Services.Services
{
    public class StockMarketAPI : IStockMarketAPI
    {
        private readonly HttpClient _client;

        public StockMarketAPI(HttpClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse> FindStock(string codeStock)
        {
            var url = $"http://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities/{codeStock}.json?iss.meta=off&iss.only=securities,marketdata";
            return await RequestTo(url);
        }

        public async Task<ApiResponse> FindFond(string codeFond)
        {
            var url = $"http://iss.moex.com/iss/engines/stock/markets/shares/boards/TQTF/securities/{codeFond}.json?iss.meta=off&iss.only=securities,marketdata";
            return await RequestTo(url);
        }

        public async Task<ApiResponse> FindBond(string codeBond)
        {
            var url = $"http://iss.moex.com/iss/engines/stock/markets/bonds/boards/TQOB/securities/{codeBond}.json?iss.meta=off&iss.only=securities,marketdata";
            return await RequestTo(url);
        }

        public async Task<ApiResponse> FindIndex(string codeIndex)
        {
            var url = $"http://iss.moex.com/iss/engines/stock/markets/index/securities/{codeIndex}.json?iss.meta=off&iss.only=securities,marketdata";
            return await RequestTo(url);
        }

        public async Task<ApiResponse> FindCurrency(string codeCurrency)
        {
            var url = $"http://iss.moex.com/iss/engines/currency/markets/selt/boards/CETS/securities/{codeCurrency}.json?iss.meta=off&iss.only=securities,marketdata";
            return await RequestTo(url);
        }

        public async Task<ApiResponse> FindDividends(string codeStock)
        {
            var url = $"http://iss.moex.com/iss/securities/{codeStock}/dividends.json?iss.meta=off";
            return await RequestTo(url);
        }

        public async Task<ApiResponse> FindCoupons(string codeBond, DateTime boughtDate)
        {
            var dateString = boughtDate.ToString("yyyy-MM-dd");
            var url = $"https://iss.moex.com/iss/statistics/engines/stock/markets/bonds/bondization/{codeBond}.json?from={dateString}&iss.only=coupons,amortizations&iss.meta=off";
            return await RequestTo(url);
        }

        public async Task<ApiResponse> FindBrent()
        {
            var url = "http://iss.moex.com/iss/engines/futures/markets/forts/securities/BRV0.json?iss.meta=off&iss.only=securities,marketdata";
            return await RequestTo(url);
        }

        public async Task<ApiResponse> Search(string code)
        {
            var url = $"https://iss.moex.com/iss/securities/{code}.json?iss.meta=off&iss.only=description&description.columns=name,value";
            return await RequestTo(url);
        }

        public async Task<ApiResponse> StockCandles(string code, DateTime from, CandleInterval interval)
        {
            var dateString = from.ToString("yyyy-MM-dd");
            var url = $"http://iss.moex.com/iss/engines/stock/markets/shares/securities/{code}/candles.json?from={dateString}&interval={(int)interval}&iss.meta=off";
            return await RequestTo(url);
        }

        private async Task<ApiResponse> RequestTo(string url)
        {
            var response = await _client.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return new ApiResponse()
                {
                    JsonContent = responseContent,
                    StatusCode = response.StatusCode
                };
            }

            return new ApiResponse()
            {
                JsonContent = "",
                StatusCode = response.StatusCode
            };
        }
    }

    public enum CandleInterval
    {
        Week = 7,
        Day = 24,
        Month = 31,
        Hour = 60
    }
}