using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Home;

namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class ProfileOrderItemGetDto
    {
        public long ItemId { get; set; }
        public string Title { get; set; }
        public string GoodsImage { get; set; }
        public int GoodsId { get; set; }
        public string StatusTitle { get; set; }
        public int StatusId { get; set; }
        public int? ShippingMethod { get; set; }
        public int? MaxDeadlineDayToReturning { get; set; }
        public int? CustomerRefound { get; set; }
        public double Quantity { get; set; }
        public double Weight { get; set; }
        public string ModelNumber { get; set; }
        public string GoodsCode { get; set; }
        public string ShopName { get; set; }
        public bool ShopSpacialPage { get; set; }
        public string ShopUrl { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Shipping { get; set; }
        public decimal PriceWithDiscount { get; set; }
        public bool? ReturningAllowed { get; set; }
        public bool? CancelingAllowed { get; set; } 
        public bool IsDownloadable { get; set; }
        public string DownloadUrl { get; set; }
        public string OrderStatusPlacedDateTime { get; set; }
        public DateTime? DeliveredDate { get; set; }

    }
}