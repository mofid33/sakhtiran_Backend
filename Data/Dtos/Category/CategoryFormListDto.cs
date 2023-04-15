using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryFormListDto
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string IconUrl { get; set; }
        public string Parents { get; set; }
        public string CategoryPath { get; set; }
    }
}