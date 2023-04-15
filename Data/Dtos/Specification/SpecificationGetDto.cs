using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecificationGetDto
    {
        public int SpecId { get; set; }
        public string SpecTitle { get; set; }
        public int? FkSpecGroupId { get; set; }
        public bool? IsMultiSelectInFilter { get; set; }
        public bool IsSelectable { get; set; }
        public bool IsMultiSelect { get; set; }
        public bool IsMultiLineText { get; set; }
        public bool IsKeySpec { get; set; }
        public bool IsRequired { get; set; }
        public bool? Status { get; set; }

        public List<SpecificationOptionsDto> TSpecificationOptions { get; set; }
        public List<CategorySpecificationGetDto> TCategorySpecification { get; set; }
        public List<GoodsSpecificationDto> TGoodsSpecification { get; set; }
    }
}