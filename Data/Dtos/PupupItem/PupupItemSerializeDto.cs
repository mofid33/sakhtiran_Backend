using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.PupupItem
{
    public class PupupItemSerializeDto
    {
        public string PupupItem { get; set; }
        public IFormFile Image { get; set; }
    }
}