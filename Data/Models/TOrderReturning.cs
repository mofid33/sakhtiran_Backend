using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TOrderReturning
    {
        public TOrderReturning()
        {
            TOrderReturningLog = new HashSet<TOrderReturningLog>();
            TUserTransaction = new HashSet<TUserTransaction>();
        }

        public int ReturningId { get; set; }
        public int FkStatusId { get; set; }
        public long FkOrderId { get; set; }
        public long FkOrderItemId { get; set; }
        public int FkReturningReasonId { get; set; }
        public int FkReturningActionId { get; set; }
        public DateTime RegisterDateTime { get; set; }
        public string RequestComment { get; set; }
        public double Quantity { get; set; }
        public string ProviderComment { get; set; }
        public int? FkShippingMethodId { get; set; }
        public double? ShippingAmount { get; set; }
        public bool? BlockSuccess { get; set; }
        public bool? RefundSuccess { get; set; }

        public virtual TOrder FkOrder { get; set; }
        public virtual TOrderItem FkOrderItem { get; set; }
        public virtual TReturningAction FkReturningAction { get; set; }
        public virtual TReturningReason FkReturningReason { get; set; }
        public virtual TShippingMethod FkShippingMethod { get; set; }
        public virtual TReturningStatus FkStatus { get; set; }
        public virtual ICollection<TOrderReturningLog> TOrderReturningLog { get; set; }
        public virtual ICollection<TUserTransaction> TUserTransaction { get; set; }
    }
}
