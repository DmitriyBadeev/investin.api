using System;

namespace InvestIn.Finance.API.Mutations.InputTypes
{
    public class PaymentInput
    {
        public int PortfolioId { get; set; }

        public string Ticket { get; set; }

        public int Amount { get; set; }

        public int PaymentValue { get; set; }

        public DateTime Date { get; set; }
    }
}