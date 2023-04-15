using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;

namespace MarketPlace.API.Data.Dtos.Dashboard
{
    public class BrandRequestDto
    {
        public int BrandId { get; set; }
        public string BrandTitle { get; set; }
        public string BrandLogoImage { get; set; }
        public string Description { get; set; }
        public List<CategoryFormGetDto> Category { get; set; }

    }
}