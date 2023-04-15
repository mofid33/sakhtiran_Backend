using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecPagination
    {
        public int GroupId { get; set; }
        public int CategoryId { get; set; }
        public List<int> CatChilds { get; set; }
        public string Filter { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}