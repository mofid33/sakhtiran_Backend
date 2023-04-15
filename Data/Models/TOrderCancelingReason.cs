using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TOrderCancelingReason
    {
        public TOrderCancelingReason()
        {
            TOrderCanceling = new HashSet<TOrderCanceling>();
        }

        public int ReasonId { get; set; }
        public string ReasonTitle { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<TOrderCanceling> TOrderCanceling { get; set; }
    }
}
