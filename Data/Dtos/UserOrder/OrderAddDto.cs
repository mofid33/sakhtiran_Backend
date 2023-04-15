namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class OrderAddDto
    {
        public int GoodsId { get; set; }
        public int ProviderId { get; set; }
        public int Number { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public int? ProvinceId { get; set; }
        public bool? OneClick { get; set; }
    }
}