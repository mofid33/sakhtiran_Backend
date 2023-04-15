using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecificationGroupGetForGoodsDto
    {
        public int SpecGroupId { get; set; }
        public string SpecGroupTitle { get; set; }
        public List<SpecificationGetDto> Specification { get; set; }
        public List<CategoryFormGetDto> Category { get; set; }
    }
}