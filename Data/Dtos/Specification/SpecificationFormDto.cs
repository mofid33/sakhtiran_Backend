namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecificationFormDto
    {
        public int SpecId { get; set; }
        public string SpecTitle { get; set; }
        public int FkSpecGroupId { get; set; }
        public int? PriorityNumber { get; set; }
    }
}