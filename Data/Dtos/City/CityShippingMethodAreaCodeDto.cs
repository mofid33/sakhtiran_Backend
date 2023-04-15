namespace MarketPlace.API.Data.Dtos.City
{
    public class CityShippingMethodAreaCodeDto
    {
        public long PostAreaCodeId { get; set; }
        public int FkCityId { get; set; }
        public int FkShippingMethodId { get; set; }
        public int Code { get; set; }
        public int? StateCode { get; set; }

        public string ShippingMethodTitle { get; set; }
        public string CityTitle { get; set; }
    }
}