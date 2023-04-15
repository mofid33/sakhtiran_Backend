namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountShopsGetDto
    {
        public int AssignedShopId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkShopId { get; set; }
        public string ShopTitle { get; set; }
        public bool Allowed { get; set; }    }
}