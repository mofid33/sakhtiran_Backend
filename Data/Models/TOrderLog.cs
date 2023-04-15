using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TOrderLog
    {
        public int LogId { get; set; }
        public long FkOrderId { get; set; }
        public long? FkOrderItemId { get; set; }
        public int? FkStatusId { get; set; }
        public Guid FkUserId { get; set; }
        public string LogComment { get; set; }
        public DateTime LogDateTime { get; set; }

        public virtual TOrder FkOrder { get; set; }
        public virtual TOrderItem FkOrderItem { get; set; }
        public virtual TOrderStatus FkStatus { get; set; }
        public virtual TUser FkUser { get; set; }
    }
}
