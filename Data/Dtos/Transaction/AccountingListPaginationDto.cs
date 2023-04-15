using System;

namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class AccountingListPaginationDto
    {
        public int ShopId { get; set; }
        public int Type { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}