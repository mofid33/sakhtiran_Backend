using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCustomerBankCard
    {
        public int BankCardId { get; set; }
        public string BankCardName { get; set; }
        public string BankCardNumber { get; set; }
        public string BankCardMonth { get; set; }
        public string ZipCode { get; set; }
        public int FkCustumerId { get; set; }
        public string BankCardYear { get; set; }
        public int FkPaymentMethodId { get; set; }

        public virtual TCustomer FkCustumer { get; set; }
        public virtual TPaymentMethod FkPaymentMethod { get; set; }
    }
}
