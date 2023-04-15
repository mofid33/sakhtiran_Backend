namespace MarketPlace.API.Data.Dtos.Specification
{
    public class WebsiteSpecificationOptionDto
    {
        public int OptionId { get; set; }
        public string OptionTitle { get; set; }
        public int FkSpecId { get; set; }
        public int Priority { get; set; }
    }
}