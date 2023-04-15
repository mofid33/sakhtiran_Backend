namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsVarietyDto
    {
        public int VarietyId { get; set; }
        public int FkProviderId { get; set; }
        public int FkGoodsId { get; set; }
        public int FkVariationParameterId { get; set; }
        public int FkVariationParameterValueId { get; set; }
        public string ImageUrl { get; set; }
    }
}