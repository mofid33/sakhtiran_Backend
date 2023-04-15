using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class SpecificationOptionsDto
    {
        public int OptionId { get; set; }
        [Required]
        [MaxLength(50)]
        public string OptionTitle { get; set; }
        public int FkSpecId { get; set; }
        public int? Priority { get; set; }

        public List<GoodsSpecificationOptionsDto> TGoodsSpecificationOptions { get; set; }
    }
}