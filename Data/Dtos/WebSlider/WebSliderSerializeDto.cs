using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.WebSlider
{
    public class WebSliderSerializeDto
    {
        [Required]
        public string WebSlider { get; set; }
        public IFormFile SliderImg { get; set; }
        public IFormFile ResponsiveImage { get; set; }

    }
}