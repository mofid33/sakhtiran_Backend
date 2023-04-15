using System;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class OrderListPaginationDto
    {
        public long OrderId { get; set; }
        public int CustomerId { get; set; }
        public int GoodsId { get; set; }
        public int ShopId { get; set; }
        public int StatusId { get; set; }
        public string TrackingCode { get; set; }
        public DateTime? PlaceFrom { get; set; }
        public DateTime? PlaceTo { get; set; }
        public int PaymentMethodId { get; set; }
        public int ShippingMethodId { get; set; }
        public decimal Rate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}