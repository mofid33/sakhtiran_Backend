using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Shop;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class OrderListDto
    {
        public long OrderId { get; set; }
        public int FkOrderStatusId { get; set; }
        public string OrderStatusTitle { get; set; }
        public string PlacedDateTime { get; set; }
        public int FkCustomerId { get; set; }
        public string CustomerName { get; set; }
        public string StatusColor { get; set; }
        public string ShopString { get; set; }
        public List<ShopFormDto> Shops { get; set; }
        public bool PaymentStatus { get; set; }
        public decimal? FinalPrice { get; set; }
    }
}