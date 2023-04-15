using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Country
{
    public class CountrySerializeDto
    {
        public string Country { get; set; }
        public IFormFile Image { get; set; }
    }
}