﻿using System.Collections.Generic;
using System.Text.Json;

namespace InvestIn.Finance.Services.DTO.Responses
{
    public class Coupons
    {
        public List<string> columns { get; set; }
        public List<List<JsonElement>> data { get; set; }
    }

    public class Amortizations
    {
        public List<string> columns { get; set; }
        public List<List<JsonElement>> data { get; set; }
    }

    public class CouponsResponse
    {
        public Coupons coupons { get; set; }
        public Amortizations amortizations { get; set; }
    }
}
