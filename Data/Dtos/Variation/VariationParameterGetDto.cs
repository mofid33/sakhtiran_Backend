using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Variation
{
    public class VariationParameterGetDto
    {
        public int ParameterId { get; set; }
        public string ParameterTitle { get; set; }
        public string VariationParameterTitle { get; set; }
        public string VariationPerCategoryTitle { get; set; }
        public bool ValuesHaveImage { get; set; }
        public List<VariationParameterValuesDto> TVariationParameterValues { get; set; }
        public List<VariationPerCategoryDto> TVariationPerCategory { get; set; }

    }
}