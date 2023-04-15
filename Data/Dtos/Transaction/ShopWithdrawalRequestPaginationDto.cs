using System;

namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class ShopWithdrawalRequestPaginationDto
    {
        public int ShopId { get; set; }
        public bool? Status { get; set; }
        public bool SetStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}