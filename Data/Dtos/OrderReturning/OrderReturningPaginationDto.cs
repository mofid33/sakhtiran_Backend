using System;

namespace MarketPlace.API.Data.Dtos.OrderReturning
{
    public class OrderReturningPaginationDto
    {
        public int statusId { get; set; }
        public int CustomerId { get; set; }
        public int ShopId { get; set; }
        public int GoodsId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}