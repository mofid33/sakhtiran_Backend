using System;

namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class ShopWithdrawalRequestGetDto
    {
        public int RequestId { get; set; }
        public int FkShopId { get; set; }
        public string ShopTitle { get; set; }
        public string RequestDate { get; set; }
        public string RequestText { get; set; }
        public string ResponseText { get; set; }
        public decimal Amount { get; set; }
        public string DocumentUrl { get; set; }
        public bool? Status { get; set; }
    }
}