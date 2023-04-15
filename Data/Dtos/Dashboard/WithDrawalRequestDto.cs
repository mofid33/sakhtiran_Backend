using System;

namespace MarketPlace.API.Data.Dtos.Dashboard
{
    public class WithDrawalRequestDto
    {
        public int RequestId { get; set; }
        public int FkShopId { get; set; }
        public string ShopTitle { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestText { get; set; }
        public decimal Amount { get; set; }
    }
}