namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopActivityCountryDto
    {
        public int Id { get; set; }
        public int FkCountryId { get; set; }
        public int FkShopId { get; set; }
        public int FkShippingMethodId { get; set; }
        public bool ReturningAllowed { get; set; }
    }
}