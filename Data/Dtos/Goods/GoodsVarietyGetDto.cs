namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsVarietyGetDto
    {
        public int VarietyId { get; set; }
        public int FkProviderId { get; set; }
        public int FkGoodsId { get; set; }
        public int FkVariationParameterId { get; set; }
        public string ParameterTitle { get; set; }
        public int FkVariationParameterValueId { get; set; }
        public string ValueTitle { get; set; }
        public string ImageUrl { get; set; }
        public bool ValuesHaveImage { get; set; }

    }
}