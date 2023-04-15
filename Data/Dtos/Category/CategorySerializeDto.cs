using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategorySerializeDto
    {
        public string Category { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile Icon { get; set; }
    }
}