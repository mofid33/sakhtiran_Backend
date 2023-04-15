using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Brand
{
    public class BrandSerializeDto
    {
        public string Brand { get; set; }
        public IFormFile Image { get; set; }
    }
}