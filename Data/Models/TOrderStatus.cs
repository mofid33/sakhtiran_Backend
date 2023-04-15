using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TOrderStatus
    {
        public TOrderStatus()
        {
            TOrder = new HashSet<TOrder>();
            TOrderItem = new HashSet<TOrderItem>();
            TOrderLog = new HashSet<TOrderLog>();
        }

        public int StatusId { get; set; }
        public string StatusTitle { get; set; }
        public string Comment { get; set; }
        public bool? AllowCancelOrder { get; set; }
        public bool? AllowChangeProvider { get; set; }
        public bool? AllowCustomerEditOrder { get; set; }
        public bool? AllowReturnOrderItem { get; set; }
        public bool? ShopAvailable { get; set; }
        public bool? AdminAvailable { get; set; }
        public string Color { get; set; }

        public virtual ICollection<TOrder> TOrder { get; set; }
        public virtual ICollection<TOrderItem> TOrderItem { get; set; }
        public virtual ICollection<TOrderLog> TOrderLog { get; set; }
    }
}
