using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TReturningReason
    {
        public TReturningReason()
        {
            TOrderReturning = new HashSet<TOrderReturning>();
        }

        public int ReasonId { get; set; }
        public string ReasonTitle { get; set; }
        public string ReturnCondition { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<TOrderReturning> TOrderReturning { get; set; }
    }
}
