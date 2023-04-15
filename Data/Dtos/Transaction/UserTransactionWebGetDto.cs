using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class UserTransactionWebGetDto
    {
        public decimal Credit { get; set; }
        public int Count { get; set; }
        public List<UserTransactionGetDto> TransactionList { get; set; }
    }
}