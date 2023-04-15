namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryWebsiteDto
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public bool HaveWebPage { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public int? FkParentId { get; set; }
    }
}