using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopActivityCityEditDto
    {
        public List<ShopActivityCityDto> ShopActivityCity { get; set; }
        public List<int> Ids { get; set; }
        public int ShopId { get; set; }
    }
}