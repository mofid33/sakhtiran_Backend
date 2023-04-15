using System;

namespace MarketPlace.API.Data.Dtos.Dashboard
{
    public class RecentOrderDto
    {
        public long ItemId { get; set; }
        public int GoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string GoodsCode { get; set; }
        public string SerialNumber { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public int ShopId { get; set; }
        public string ShopTitle { get; set; }
        public double ItemCount { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? Discount { get; set; }
        public string Date { get; set; }
    }
}