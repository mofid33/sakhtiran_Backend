using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TOrderCanceling
    {
        public int CancelingId { get; set; }
        public long FkOrderId { get; set; }
        public long FkOrderItemId { get; set; }
        public int FkCancelingReasonId { get; set; }
        public DateTime CancelDate { get; set; }
        public string Comment { get; set; }

        public virtual TOrderCancelingReason FkCancelingReason { get; set; }
        public virtual TOrder FkOrder { get; set; }
        public virtual TOrderItem FkOrderItem { get; set; }
    }
}
