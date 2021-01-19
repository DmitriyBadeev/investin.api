﻿using System.Collections.Generic;
using System.Text.Json;

namespace InvestIn.Finance.Services.DTO.Responses
{
    public class Securities
    {
        public List<string> columns { get; set; }
        public List<List<JsonElement>> data { get; set; }
    }

    public class Marketdata
    {
        public List<string> columns { get; set; }
        public List<List<JsonElement>> data { get; set; }
    }

    public class AssetResponse
    {
        public Securities securities { get; set; }
        public Marketdata marketdata { get; set; }
    }
}
