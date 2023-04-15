
using System;

namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerCommentPaginationDto
    {        
        public int CategoryId { get; set; }
        public int GoodsId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int CustomerId { get; set; }
        public int? Status { get; set; }
        public int VendorId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}