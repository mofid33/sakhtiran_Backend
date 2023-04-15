
namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopCategoryDto
    {
        public int ShopCategoryId { get; set; }
        public int FkShopId { get; set; }
        public int FkCategoryId { get; set; }

    }
}