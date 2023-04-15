using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecificationCatGroupDto
    {
        public int SpecId { get; set; }
        public string SpecTitle { get; set; }
        public int FkSpecGroupId { get; set; }
        public bool IsSelectable { get; set; }
        public bool IsMultiSelect { get; set; }
        public bool IsMultiLineText { get; set; }
        public bool? IsMultiSelectInFilter { get; set; }
        public bool IsKeySpec { get; set; }
        public bool IsRequired { get; set; }
        public bool? Status { get; set; }
        public string SpecGroupTitle { get; set; }
        public string CategoryTitle { get; set; }
        public List<CategorySpecDto> Category { get; set; }
    }
}