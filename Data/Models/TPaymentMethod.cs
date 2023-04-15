using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TPaymentMethod
    {
        public TPaymentMethod()
        {
            TCustomerBankCard = new HashSet<TCustomerBankCard>();
            TOrder = new HashSet<TOrder>();
            TPaymentTransaction = new HashSet<TPaymentTransaction>();
        }

        public int MethodId { get; set; }
        public string MethodTitle { get; set; }
        public string MethodImageUrl { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<TCustomerBankCard> TCustomerBankCard { get; set; }
        public virtual ICollection<TOrder> TOrder { get; set; }
        public virtual ICollection<TPaymentTransaction> TPaymentTransaction { get; set; }
    }
}
