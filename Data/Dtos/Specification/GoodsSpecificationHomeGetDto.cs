using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class GoodsSpecificationHomeGetDto
    {
        public int SpecId { get; set; }
        public string SpecTitle { get; set; }
        public string SpecValueText { get; set; }

        public List<SpecificationOptionsDto> SpecificationOptions { get; set; }

    }
}