using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Image
{
    public class UploadTowImageDto
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile ResponsiveImage { get; set; }
    }
}