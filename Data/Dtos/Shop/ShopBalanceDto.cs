using System;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopBalanceDto
    {
        public long TransactionId { get; set; }
        public int FkTransactionTypeId { get; set; }
        public string TransactionTypeTitle { get; set; }
        public int FkApprovalStatusId { get; set; }
        public string ApprovalStatusTitle { get; set; }
        public string TransactionDateTime { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }
}