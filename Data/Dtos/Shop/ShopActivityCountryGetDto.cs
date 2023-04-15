using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.ShippingMethod;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopActivityCountryGetDto
    {
        public int Id { get; set; }
        public int FkCountryId { get; set; }
        public string CountryTitle { get; set; }
        public int FkShopId { get; set; }
        public int FkShippingMethodId { get; set; }
        public bool? ReturningAllowed { get; set; }
        public List<ShippingMethodFormDto> ShippingMethodList { get; set; }
    }
}