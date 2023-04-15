using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TStockOperation
    {
        public int OperationId { get; set; }
        public int FkOperationTypeId { get; set; }
        public int FkStockId { get; set; }
        public long? FkOrderItem { get; set; }
        public DateTime OperationDate { get; set; }
        public double OperationStockCount { get; set; }
        public decimal? SaleUnitPrice { get; set; }
        public string OperationComment { get; set; }

        public virtual TStockOperationType FkOperationType { get; set; }
        public virtual TOrderItem FkOrderItemNavigation { get; set; }
        public virtual TGoodsProvider FkStock { get; set; }
    }
}
