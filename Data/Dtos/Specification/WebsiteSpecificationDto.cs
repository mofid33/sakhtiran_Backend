using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class WebsiteSpecificationDto
    {
        public int SpecId { get; set; }
        public string SpecTitle { get; set; }
        public bool? IsMultiSelectInFilter { get; set; }
        //public int? PriorityNumber { get; set; }
        public List<WebsiteSpecificationOptionDto> Options { get; set; }
    }
}