using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopActivityCountryEditDto
    {
        public List<ShopActivityCountryDto> ShopActivityCountry { get; set; }
        public List<int> Ids { get; set; }
        public int ShopId { get; set; }
    }
}