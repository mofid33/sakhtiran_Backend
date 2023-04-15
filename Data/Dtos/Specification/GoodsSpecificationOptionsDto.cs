namespace MarketPlace.API.Data.Dtos.Specification
{
    public class GoodsSpecificationOptionsDto
    {
        public int SpecOptionId { get; set; }
        public int FkSpecOptionId { get; set; }
        public int FkGsid { get; set; }
        public string OptionTitle { get; set; }
    }
}