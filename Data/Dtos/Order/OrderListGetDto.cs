using System.Collections.Generic;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class OrderListGetDto
    {
        public List<OrderListDto> Order { get; set; }
        public int OrderCount { get; set; }
        public decimal? ShipingCost { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? ComissionPrice { get; set; }
        public double? Count { get; set; }
    }
}