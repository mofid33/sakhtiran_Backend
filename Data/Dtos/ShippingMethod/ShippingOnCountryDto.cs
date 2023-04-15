namespace MarketPlace.API.Data.Dtos.ShippingMethod
{
    public class ShippingOnCountryDto
    {
        public int Id { get; set; }
        public int FkCountryId { get; set; }
        public string CountryTitle { get; set; }
        public int FkShippingMethodId { get; set; }
        public int PostTimeoutDay { get; set; }
        public decimal? ShippingPriceFewerBaseWeight { get; set; }
        public decimal? ShippingPriceMoreBaseWeight { get; set; }
    }
}