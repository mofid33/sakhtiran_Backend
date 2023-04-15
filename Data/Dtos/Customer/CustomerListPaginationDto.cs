namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerListPaginationDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int CountryId { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public string Phone { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}