namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopSliderDto
    {
        public int SliderId { get; set; }
        public int FkShopId { get; set; }
        public string ImageUrl { get; set; }
        public bool Status { get; set; }
    }
}