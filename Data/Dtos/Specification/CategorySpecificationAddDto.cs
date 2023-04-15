namespace MarketPlace.API.Data.Dtos.Specification
{
    public class CategorySpecificationAddDto
    {
        public int Gcsid { get; set; }
        public int FkCategoryId { get; set; }
        public int FkSpecId { get; set; }
        public string CategoryPath { get; set; }
    }
}