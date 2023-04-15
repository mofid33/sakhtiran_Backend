using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Variation
{
    public class VariationParameterDto
    {
        public int ParameterId { get; set; }
        public string ParameterTitle { get; set; }
        public bool ValuesHaveImage { get; set; }
        public List<VariationPerCategoryDto> TVariationPerCategory { get; set; }
    }
}