namespace MarketPlace.API.Data.Dtos.Brand
{
    public class CategoryBrandGetDto
    {
        public int BrandCategoryId { get; set; }
        public int FkCategoryId { get; set; }
        public int FkBrandId { get; set; }
        public string CategoryTitle { get; set; }
    }
}