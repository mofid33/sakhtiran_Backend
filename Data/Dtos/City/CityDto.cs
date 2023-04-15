namespace MarketPlace.API.Data.Dtos.City
{
    public class CityDto
    {
        public int CityId { get; set; }
        public string CityTitle { get; set; }
        public int FkCountryId { get; set; }
        public int FkProvinceId { get; set; }
        public bool Status { get; set; }
    }
}