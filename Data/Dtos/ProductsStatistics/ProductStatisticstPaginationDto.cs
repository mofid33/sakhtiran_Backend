using System;

namespace MarketPlace.API.Data.Dtos.ProductsStatistics
{
    public class ProductStatisticsPaginationDto
    {
        public int CategoryId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int ProductId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}