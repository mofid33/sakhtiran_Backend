using System;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class LiveCartListDto
    {
        public long OrderId { get; set; }
        public int FkCustomerId { get; set; }
        public string CustomerName { get; set; }
        public string InitialDateTime { get; set; }
        public double ItemCount { get; set; }
        public double? GoodsCount { get; set; }
        public decimal? FinalPrice { get; set; }
    }
}