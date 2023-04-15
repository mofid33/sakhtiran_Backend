namespace MarketPlace.API.Data.Dtos.Province
{
    public class ProvinceGetDto
    {
        public int  ProvinceId { get; set; }
        public string  ProvinceName { get; set; }
        public int FkCountryId { get; set; }
        public string CountryTitle { get; set; }
        public bool Status { get; set; }

    }
}