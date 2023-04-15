using System;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class ShopOrderListDto
    {
        public long ItemId { get; set; }
        public int GoodsId { get; set; }
        public string GoodsImage { get; set; }
        public string GoodsTitle { get; set; }
        public string GoodsCode { get; set; }
        public string SerialNumber { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string Date { get; set; }
        public double? ItemCount { get; set; }
        public decimal? Price { get; set; }
        public decimal? Total { get; set; }
        public int StatusId { get; set; }
        public string StatusTitle { get; set; }
        public string StatusColor { get; set; }
    }
}