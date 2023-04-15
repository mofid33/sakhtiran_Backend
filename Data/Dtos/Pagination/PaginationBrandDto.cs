using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Pagination
{
    public class PaginationBrandDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int Id { get; set; }
        public string Filter { get; set; }
        public List<int> BrandIds { get; set; }

    }
}