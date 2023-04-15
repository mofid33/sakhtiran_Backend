using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Category
{
    public class CategoryPaginationDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public bool? ReturnAllowed { get; set; }
        public bool? ShowInFooter { get; set; }
        public bool? Status { get; set; }
        public bool? Display { get; set; }
        public bool? SpeciallyWebPage { get; set; }
        public List<int> Childs { get; set; }

    }
}