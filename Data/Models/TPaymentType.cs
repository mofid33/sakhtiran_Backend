using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TPaymentType
    {
        public TPaymentType()
        {
            TOrder = new HashSet<TOrder>();
        }

        public int PaymentTypeId { get; set; }
        public string PaymentTypeTitle { get; set; }

        public virtual ICollection<TOrder> TOrder { get; set; }
    }
}
