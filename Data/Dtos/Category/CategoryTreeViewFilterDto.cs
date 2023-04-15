using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryTreeViewFilterDto
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public List<CategoryTreeViewFilterDto> Child { get; set; }
        
    }
}