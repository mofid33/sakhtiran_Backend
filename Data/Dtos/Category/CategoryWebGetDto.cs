using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Brand;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryWebGetDto
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public bool HaveWebPage { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int? FkParentId { get; set; }
        public int? GoodsCount { get; set; }

        public List<WebsiteBrandDto> WebsiteBrand { get; set; }
        public List<CategoryWebGetDto> Childs { get; set; }

    }
}