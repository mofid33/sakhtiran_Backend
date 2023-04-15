namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountGoodsDto
    {
        public int AssingedGoodsId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public bool Allowed { get; set; }
    }
}