using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Brand;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryWebDto
    {
        public List<WebsiteBrandDto> WebsiteBrand { get; set; }
        public List<CategoryWebsiteDto> Childs { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public string PageTitle { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
    }
}