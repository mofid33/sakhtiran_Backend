namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopFormDto
    {
        public ShopFormDto()
        {
        }

        public ShopFormDto(int shopId, string shopTitle)
        {
            this.ShopId = shopId;
            this.ShopTitle = shopTitle;
        }
        public int ShopId { get; set; }
        public string ShopTitle { get; set; }
    }
}