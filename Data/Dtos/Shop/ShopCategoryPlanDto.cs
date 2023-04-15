namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopCategoryPlanDto
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public decimal? Commission { get; set; }
    }
}