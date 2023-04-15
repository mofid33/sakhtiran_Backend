using System;

namespace MarketPlace.API.Data.Dtos.WareHouse
{
    public class WareHouseOprationDetailDto
    {
        public int OperationId { get; set; }
        public int FkOperationTypeId { get; set; }
        public string OperationTypeTitle { get; set; }
        public int FkStockId { get; set; }
        public long? FkOrderItem { get; set; }
        public string OperationDate { get; set; }
        public double OperationStockCount { get; set; }
        public decimal? SaleUnitPrice { get; set; }
        public string OperationComment { get; set; }
        public double Balance { get; set; }
    }
}