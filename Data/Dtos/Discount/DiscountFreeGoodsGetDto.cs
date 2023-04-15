using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountFreeGoodsGetDto
    {
        public int FreeGoodsId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public List<GoodsVarietyGetDto> Varity { get; set; }
        public string SerialNumber { get; set; }
        public string ProductTitle { get; set; }
        public string varietyTitle { get; set; }
        public string Image { get; set; }
        public string GoodsCode { get; set; }
        public double Quantity { get; set; }
    }
}