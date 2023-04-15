namespace MarketPlace.API.Data.Dtos.Specification
{
    public class CategorySpecificationGetDto 
    {
        public int Gcsid { get; set; }
        public int FkCategoryId { get; set; }
        public int FkSpecId { get; set; }
        public string SpecTitle { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryPath { get; set; }
    }
} 