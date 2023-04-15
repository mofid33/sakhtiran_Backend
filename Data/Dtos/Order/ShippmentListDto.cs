using System;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class ShippmentListDto
    {
        public long ItemId { get; set; }
        public long FkOrderId { get; set; }
        public int FkShopId { get; set; }
        public int FkGoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string SerialNumber { get; set; }
        public string ImageUrl { get; set; }
        public string GoodsCode { get; set; }
        public int FkVarietyId { get; set; }
        public int? FkShippingMethodId { get; set; }
        public decimal? ShippingCost { get; set; }
        public string ShippingMethodTitle { get; set; }
        public string ShippmentDate { get; set; }
        public string DeliveredDate { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobileNumber { get; set; }

        public string PlacedDateTime { get; set; }

        
        public int shopId { get; set; }
        public string ShopTitle { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }
}