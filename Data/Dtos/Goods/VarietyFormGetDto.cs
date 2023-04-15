using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class VarietyFormGetDto
    {
        public int ProviderId { get; set; }
        public int GoodsId { get; set; }
        public List<GoodsVarietyGetDto> Variety { get; set; }
    }
}