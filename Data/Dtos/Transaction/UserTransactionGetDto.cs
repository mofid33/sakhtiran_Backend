using System;

namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class UserTransactionGetDto
    {
        public long TransactionId { get; set; }
        public string TransactionType { get; set; }
        public int TransactionTypeId { get; set; }
        public string TransactionDateTime { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Comment { get; set; }
    }
}