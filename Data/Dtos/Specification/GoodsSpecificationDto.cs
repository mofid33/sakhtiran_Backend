using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Specification
{
    public class GoodsSpecificationDto
    {
        public int Gsid { get; set; }
        public int FkGoodsId { get; set; }
        public int FkSpecId { get; set; } 
        public string SpecValueText { get; set; } 
        public List<GoodsSpecificationOptionsDto> TGoodsSpecificationOptions { get; set; }
    }
}