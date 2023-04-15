using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TReturningStatus
    {
        public TReturningStatus()
        {
            TOrderReturning = new HashSet<TOrderReturning>();
            TOrderReturningLog = new HashSet<TOrderReturningLog>();
        }

        public int StatusId { get; set; }
        public string StatusTitle { get; set; }
        public string Description { get; set; }
        public bool? ShopAvailable { get; set; }
        public bool? AdminAvailable { get; set; }

        public virtual ICollection<TOrderReturning> TOrderReturning { get; set; }
        public virtual ICollection<TOrderReturningLog> TOrderReturningLog { get; set; }
    }
}
