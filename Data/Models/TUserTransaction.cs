using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TUserTransaction
    {
        public long TransactionId { get; set; }
        public int FkTransactionTypeId { get; set; }
        public Guid FkUserId { get; set; }
        public long? FkOrderId { get; set; }
        public long? FkOrderItemId { get; set; }
        public int? FkReturningId { get; set; }
        public int FkApprovalStatusId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }

        public virtual TTransactionStatus FkApprovalStatus { get; set; }
        public virtual TOrder FkOrder { get; set; }
        public virtual TOrderItem FkOrderItem { get; set; }
        public virtual TOrderReturning FkReturning { get; set; }
        public virtual TTransactionType FkTransactionType { get; set; }
        public virtual TUser FkUser { get; set; }
    }
}
