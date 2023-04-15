using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Brand
{
    public class BrandDto
    {
        public int BrandId { get; set; }
        public string BrandTitle { get; set; }
        public string BrandLogoImage { get; set; }
        public string Description { get; set; }
        public bool? IsAccepted { get; set; }
        public List<CategoryBrandDto> TCategoryBrand { get; set; }
    }
}