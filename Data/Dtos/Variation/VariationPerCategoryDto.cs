namespace MarketPlace.API.Data.Dtos.Variation
{
    public class VariationPerCategoryDto
    {
        public long Id { get; set; }
        public int FkCategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public int FkParameterId { get; set; }
    }
}