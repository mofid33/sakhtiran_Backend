using System;

namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class TransactionGetDto
    {
        public long TransactionId { get; set; }
        public int FkTransactionTypeId { get; set; }
        public string TransactionTypeTitle { get; set; }
        public Guid FkUserId { get; set; }
        public string ShopTitle { get; set; }
        public long? FkOrderId { get; set; }
        public long? FkOrderItemId { get; set; }
        public int? FkReturningId { get; set; }
        public int FkApprovalStatusId { get; set; }
        public string ApprovalStatusTitle { get; set; }
        public string TransactionDateTime { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }
}