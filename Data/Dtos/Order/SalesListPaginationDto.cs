using System;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class SalesListPaginationDto
    {
        public int StatusId { get; set; }
        public int CustomerId { get; set; }
        public int ShopId { get; set; }
        public int CategoryId { get; set; }
        public int GoodsId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}