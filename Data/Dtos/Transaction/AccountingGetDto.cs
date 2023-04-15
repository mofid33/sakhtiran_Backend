
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class AccountingGetDto
    {
        public List<TransactionGetDto> Transaction { get; set; }
        public int Count { get; set; }
        public double Total { get; set; }
    }
}