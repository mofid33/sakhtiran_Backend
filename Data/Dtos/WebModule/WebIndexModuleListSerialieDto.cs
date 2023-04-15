using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.WebModule
{
    public class WebIndexModuleListSerialieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IFormFile Image { get; set; }
    }
}