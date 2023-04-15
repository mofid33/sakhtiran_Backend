namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountShopsDto
    {
        public int AssignedShopId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkShopId { get; set; }
        public bool Allowed { get; set; }
    }
}