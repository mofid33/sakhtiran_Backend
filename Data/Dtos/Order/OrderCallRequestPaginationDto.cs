using System;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class OrderCallRequestPaginationDto
    {
        public string Customer { get; set; }
        public int GoodsId { get; set; }
        public int ShopId { get; set; }
        public int StatusId { get; set; }
        public string TrackingCode { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}