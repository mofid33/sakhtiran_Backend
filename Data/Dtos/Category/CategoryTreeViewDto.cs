namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryTreeViewDto
    {
        public CategoryTreeViewDto()
        {
            this.CanClick = true;
        }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public int? FkParentId { get; set; }
        public bool hasCHild { get; set; }
        public int PriorityNumber { get; set; }
        public string CategoryPath { get; set; }
        public bool CanClick { get; set; }
        public bool? ToBeDisplayed { get; set; }
        public bool? IsActive { get; set; }
        public bool? HaveWebPage { get; set; }
        public bool HaveSpecificationGroup { get; set; }
        public int ProductCount { get; set; }

    }
}