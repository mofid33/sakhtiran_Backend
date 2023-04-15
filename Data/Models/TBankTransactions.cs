using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TBankTransactions
    {
        public long TransactionId { get; set; }
        public long? FkOrderId { get; set; }
        public int? FkCustomerId { get; set; }
        public int? FkShopId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public long ReceivedAmount { get; set; }
        public string RefId { get; set; }
        public string ResCode { get; set; }
        public long? SaleReferenceId { get; set; }
        public Guid? CookieId { get; set; }

        public virtual TCustomer FkCustomer { get; set; }
        public virtual TOrder FkOrder { get; set; }
    }
}
