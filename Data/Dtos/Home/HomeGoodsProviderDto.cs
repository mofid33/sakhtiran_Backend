using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.Home
{
    public class HomeGoodsProviderDto
    {
        public int ProviderId { get; set; }
        public int FkShopId { get; set; }
        public string ShopTitle { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkGuaranteeId { get; set; }
        public string GuaranteeTitle { get; set; }
        public string VendorUrl { get; set; }
        public bool HaveGuarantee { get; set; }
        public int? GuaranteeMonthDuration { get; set; }
        public bool Vatfree { get; set; }
        public decimal Price { get; set; }
        public decimal? Vatamount { get; set; }
        public int? PostTimeoutDayByShop { get; set; }
        public bool? ReturningAllowed { get; set; }
        public int? MaxDeadlineDayToReturning { get; set; }
        public bool? HasInventory { get; set; }
        public bool? Microstore { get; set; }
        public double? InventoryCount { get; set; }
        public int ShippingPossibilities { get; set; }
        public string ShippingDesc { get; set; }
        public string ShippingImage { get; set; }
        public string PostTimeoutDay { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? FinalPrice { get; set; }
        public double? ShopSurvey { get; set; }
        public int ShopSurveyCount { get; set; }

        public List<GoodsVarietyGetDto> TGoodsVariety { get; set; }
    }
}