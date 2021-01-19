﻿using System;

namespace InvestIn.Finance.API.Mutations.InputTypes
{
    public class RefillBalanceInput
    {
        public int PortfolioId { get; set; }

        public int Price { get; set; }

        public DateTime Date { get; set; }
    }
}