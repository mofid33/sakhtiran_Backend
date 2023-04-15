namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountFreeGoodsDto
    {
        public int FreeGoodsId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public double Quantity { get; set; }
    }
}