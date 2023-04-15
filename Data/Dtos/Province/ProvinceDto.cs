namespace MarketPlace.API.Data.Dtos.Province
{
    public class  ProvinceDto
    {
        public int  ProvinceId { get; set; }
        public string  ProvinceName { get; set; }
        public int FkCountryId { get; set; }
        public bool Status { get; set; }
    }
}