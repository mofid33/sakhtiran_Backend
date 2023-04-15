using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Pagination
{
    public class PaginationDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string Filter { get; set; }
        public int Id { get; set; }
        public int ProvinceId { get; set; } // baraye jostojoye city
        public int Type { get; set; }
        public List<int> ChildIds { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool? Active { get; set; }
        public int? ActiveNumber { get; set; }
        public int shopId { get; set; }
        public string valueId { get; set; }

    }
}