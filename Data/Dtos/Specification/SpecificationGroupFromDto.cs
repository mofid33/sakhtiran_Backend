namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecificationGroupFromDto
    {
        public string SpecGroupTitle { get; set; }
        public int CatSpecGroupId { get; set; }
        public int FkCategoryId { get; set; }
        public int FkSpecGroupId { get; set; }
        public int PriorityNumber { get; set; }
    }
}