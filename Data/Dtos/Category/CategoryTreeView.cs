using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryTreeView
    {
        public CategoryTreeView()
        {
        }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool HasCHild { get; set; }
        public bool Expanded { get; set; }
        public int PriorityNumber { get; set; }
        public int? FkParentId { get; set; }
        public string CategoryPath { get; set; }
        public bool? ToBeDisplayed { get; set; }
        public bool? IsActive { get; set; }
        public int ProductCount { get; set; }

        public List<CategoryTreeView> Child { get; set; }
    }
}