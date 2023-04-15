using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;

namespace MarketPlace.API.Data.Dtos.Dashboard
{
    public class GarenteeRequestDto
    {
        public int GuaranteeId { get; set; }
        public string GuaranteeTitle { get; set; }
        public string Description { get; set; }
        public List<CategoryFormGetDto> Category { get; set; }
    }
}