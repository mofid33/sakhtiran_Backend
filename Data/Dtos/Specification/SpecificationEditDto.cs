using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecificationEditDto
    {
        public int SpecId { get; set; }
        [Required]
        [MaxLength(50)]
        public string SpecTitle { get; set; }
        public int FkSpecGroupId { get; set; }
        public bool IsSelectable { get; set; }
        public bool IsMultiSelect { get; set; }
        public bool IsMultiLineText { get; set; }
        public bool IsMultiSelectInFilter { get; set; }
        public bool IsKeySpec { get; set; }
        public bool IsRequired { get; set; }
        public bool? Status { get; set; }

        public List<CategorySpecificationAddDto> TCategorySpecification { get; set; }
        public List<SpecificationOptionsDto> TSpecificationOptions { get; set; }
        public List<SpecificationOptionsDto> EditOptions { get; set; }
        public List<int> Gcsid { get; set; }
        public List<int> OptionId { get; set; }
    }
}