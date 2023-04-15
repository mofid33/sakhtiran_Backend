using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryFormDto
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool HaveWebPage { get; set; }
        public List<CategoryFormDto> Childs { get; set; }
    }
}