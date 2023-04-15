using System;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopBalancePagination
    {
        public int ShopId { get; set; }
        public int Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}