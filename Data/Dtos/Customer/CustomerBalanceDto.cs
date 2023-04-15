using System;

namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerBalanceDto
    {
        public long TransactionId { get; set; }
        public int FkTransactionTypeId { get; set; }
        public string TransactionTypeTitle { get; set; }
        public int FkApprovalStatusId { get; set; }
        public string ApprovalStatusTitle { get; set; }
        public string TransactionDateTime { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Comment { get; set; }
    }
}