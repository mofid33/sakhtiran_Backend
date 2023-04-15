using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class NoVariationGoodsProviderGetDto
    {
        public GoodsProviderGetDto GoodsProvider { get; set; }
        public bool? IsAccepted { get; set; }

    }
}