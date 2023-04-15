namespace MarketPlace.API.Data.Dtos.Variation
{
    public class VariationParameterValuesDto
    {
        public int ValueId { get; set; }
        public int FkParameterId { get; set; }
        public string Value { get; set; }
    }
}