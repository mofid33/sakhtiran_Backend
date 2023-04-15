using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TTransactionType
    {
        public TTransactionType()
        {
            TUserTransaction = new HashSet<TUserTransaction>();
        }

        public int TransactionTypeId { get; set; }
        public string TransactionTypeTitle { get; set; }
        public string Comment { get; set; }
        public bool? ApprovalRequire { get; set; }
        public string Kind { get; set; }

        public virtual ICollection<TUserTransaction> TUserTransaction { get; set; }
    }
}
