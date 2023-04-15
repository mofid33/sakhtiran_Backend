
using System;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class ShippmentPaginationDto
    {
        public int MethodId { get; set; }
        public int ShopId { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public DateTime? PlacedFrom { get; set; }
        public DateTime? PlacedTo { get; set; }

        public DateTime? ShippmentFrom { get; set; }
        public DateTime? ShippmentTo { get; set; }

        public DateTime? DeliveredFrom { get; set; }
        public DateTime? DeliveredTo { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}