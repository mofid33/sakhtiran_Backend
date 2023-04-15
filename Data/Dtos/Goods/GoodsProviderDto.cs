namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsProviderDto
    {
        public int ProviderId { get; set; }
        public int FkShopId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkGuaranteeId { get; set; }
        public bool HaveGuarantee { get; set; }
        public int? GuaranteeMonthDuration { get; set; }
        public bool Vatfree { get; set; }
        public decimal Price { get; set; }
        public decimal? Vatamount { get; set; }
        public int? PostTimeoutDayByShop { get; set; }
        public bool? ReturningAllowed { get; set; }
        public int? MaxDeadlineDayToReturning { get; set; }
        public int? PostId { get; set; }
        public bool? HasInventory { get; set; }
        public double? InventoryCount { get; set; }
        public int MinimumInventory { get; set; }
        public bool? ToBeDisplayed { get; set; }
        public bool? IsAccepted { get; set; }
    }
}