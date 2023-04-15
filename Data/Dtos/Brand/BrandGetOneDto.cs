using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Brand
{
    public class BrandGetOneDto
    {
        public int BrandId { get; set; }
        public string BrandTitle { get; set; }
        public string BrandLogoImage { get; set; }
        public string Description { get; set; }
        public bool? IsAccepted { get; set; }
        public List<CategoryBrandGetDto> TCategoryBrand { get; set; }

    }
}