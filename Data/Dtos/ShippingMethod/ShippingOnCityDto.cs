namespace MarketPlace.API.Data.Dtos.ShippingMethod
{
    public class ShippingOnCityDto
    {
        public int Id { get; set; }
        public int FkShippingMethodId { get; set; }
        public int FkProviceId { get; set; }
        public int? FkCityId { get; set; }
        public int? PostTimeoutDay { get; set; }
        public string CityTitle { get; set; }
        public string ProvinceTitle { get; set; }
        public decimal? ShippingPriceFewerBaseWeight { get; set; }
        public decimal? ShippingPriceMoreBaseWeight { get; set; }
    }
}