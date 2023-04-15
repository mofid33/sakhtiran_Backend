using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.WebModule
{
    public class WebModuleCollectionsSerializeDto
    {
        [Required]
        public string WebModuleCollections { get; set; }
        public IFormFile Image { get; set; }
        public IFormFile ResponsiveImage { get; set; }
    }
}