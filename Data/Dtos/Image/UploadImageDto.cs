using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Image
{
    public class UploadImageDto
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }
}