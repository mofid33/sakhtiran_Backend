using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class LiveCartDetailDto
    {
        public int FkGoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string SerialNumber { get; set; }
        public string ImageUrl { get; set; }
        public string GoodsCode { get; set; }
        public List<GoodsVarietyGetDto> Varity { get; set; }
        public double? ItemCount { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}