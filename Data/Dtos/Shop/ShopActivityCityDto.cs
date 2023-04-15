namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopActivityCityDto
    {
        public int Id { get; set; }
        public int FkShopId { get; set; }
        public int? FkCityId { get; set; }
        public int fkProviceId { get; set; }
        public int FkShippingMethodId { get; set; }
        public int? PostTimeoutDayByShop { get; set; }
        public bool ReturningAllowed { get; set; }
        public double? ShippingPriceFewerBaseWeight { get; set; }
        public double? ShippingPriceMoreBaseWeight { get; set; }
    }
}