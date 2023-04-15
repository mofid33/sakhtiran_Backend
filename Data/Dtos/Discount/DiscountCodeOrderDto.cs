using System;

namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountCodeOrderDto
    {
        public string Name { get; set; }
        public string Family { get; set; }
        public string PlacedDateTime { get; set; }
        public long OrderId { get; set; }
        public decimal? DiscountAmount { get; set; }
    }
}