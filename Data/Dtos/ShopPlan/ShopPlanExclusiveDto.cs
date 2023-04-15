namespace MarketPlace.API.Data.Dtos.ShopPlan
{
    public class ShopPlanExclusiveDto
    {
        public int ShopPlanExclusiveId { get; set; }
        public int FkShopId { get; set; }
        public int FkPlanId { get; set; }

    }
}