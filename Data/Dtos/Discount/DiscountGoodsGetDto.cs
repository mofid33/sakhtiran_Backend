using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountGoodsGetDto
    {
        public int AssingedGoodsId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public bool Allowed { get; set; }
        public List<GoodsVarietyGetDto> Varity { get; set; }
        public string SerialNumber { get; set; }
        public string ProductTitle { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string GoodsCode { get; set; }
    }
}