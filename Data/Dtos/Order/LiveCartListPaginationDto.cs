using System;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class LiveCartListPaginationDto
    {
        public int CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal Rate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}