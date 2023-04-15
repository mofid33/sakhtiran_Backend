using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopWithdrawalRequest
    {
        public int RequestId { get; set; }
        public int FkShopId { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestText { get; set; }
        public string ResponseText { get; set; }
        public decimal Amount { get; set; }
        public string DocumentUrl { get; set; }
        public bool? Status { get; set; }

        public virtual TShop FkShop { get; set; }
    }
}
