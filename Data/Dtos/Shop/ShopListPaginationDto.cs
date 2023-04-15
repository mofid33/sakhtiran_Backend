using System;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopListPaginationDto
    {
        public string StoreName { get; set; }
        public string Email { get; set; }
        public int PlanId { get; set; }
        public int CategoryId { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string PhoneNumber { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}