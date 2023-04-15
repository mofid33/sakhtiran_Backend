using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.ShippingMethod;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopActivityCityGetDto
    {
        public int Id { get; set; }
        public int FkShopId { get; set; }
        public int? FkCityId { get; set; }
        public int FkProviceId { get; set; }
        public string CityTitle { get; set; }
        public string ProvinceTitle { get; set; }
        public int FkShippingMethodId { get; set; }
        public string ShippingMethodTitle { get; set; }
        public int? PostTimeoutDayByShop { get; set; }
        public bool? ReturningAllowed { get; set; }
        public decimal? ShippingPriceFewerBaseWeight { get; set; }
        public decimal? ShippingPriceMoreBaseWeight { get; set; }
        public List<ShippingMethodFormDto> ShippingMethodList { get; set; }
    }
}