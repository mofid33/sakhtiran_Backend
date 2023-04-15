using System;

namespace MarketPlace.API.Data.Dtos.Pagination
{
    public class PaginationMessageDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int? ReadOrNot { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}