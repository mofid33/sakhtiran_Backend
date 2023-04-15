using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCurrency
    {
        public TCurrency()
        {
            TPaymentTransaction = new HashSet<TPaymentTransaction>();
        }

        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyTitle { get; set; }
        public double RatesAgainstOneDollar { get; set; }
        public bool DefaultCurrency { get; set; }

        public virtual ICollection<TPaymentTransaction> TPaymentTransaction { get; set; }
    }
}
