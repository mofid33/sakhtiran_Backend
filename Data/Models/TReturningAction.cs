using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TReturningAction
    {
        public TReturningAction()
        {
            TOrderReturning = new HashSet<TOrderReturning>();
        }

        public int ReturningTypeId { get; set; }
        public string ReturningTypeTitle { get; set; }

        public virtual ICollection<TOrderReturning> TOrderReturning { get; set; }
    }
}
