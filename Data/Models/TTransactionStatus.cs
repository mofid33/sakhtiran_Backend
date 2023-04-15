using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TTransactionStatus
    {
        public TTransactionStatus()
        {
            TUserTransaction = new HashSet<TUserTransaction>();
        }

        public int StatusId { get; set; }
        public string StatusTitle { get; set; }

        public virtual ICollection<TUserTransaction> TUserTransaction { get; set; }
    }
}
