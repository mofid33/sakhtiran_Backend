using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopCategoryPlanGetDto
    {
        public List<ShopCategoryPlanDto> Category { get; set; }
        public List<ShopCategoryPlanDto> ShopCategory { get; set; }
        public bool HasPlan { get; set; }
        public int? MaxCategory { get; set; }
        public double? Commission { get; set; }
    }
}