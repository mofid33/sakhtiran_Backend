using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class ShopOrderListGetDto
    {
        public List<ShopOrderListDto> Order { get; set; }
        public int OrderCount { get; set; }
        public decimal? ShipingCost { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? FinalPrice { get; set; }
        public double? Count { get; set; }
    }
}