using System;

namespace InvestIn.Finance.Services.DTO
{
    public class PaymentData
    {
        public string Name { get; set; }

        public string Ticket { get; set; }

        public int PaymentValue { get; set; }

        public int Amount { get; set; }

        public int AllPayment { get; set; }

        public DateTime RegistryCloseDate { get; set; }

        public string CurrencyId { get; set; }
    }
}
