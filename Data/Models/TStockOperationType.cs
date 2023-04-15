using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TStockOperationType
    {
        public TStockOperationType()
        {
            TStockOperation = new HashSet<TStockOperation>();
        }

        public int OperationTypeId { get; set; }
        public string OperationTypeTitle { get; set; }
        public string OperationTypeEffect { get; set; }
        public string MiniTitle { get; set; }

        public virtual ICollection<TStockOperation> TStockOperation { get; set; }
    }
}
