namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopSetting
    {
        public int ShopId { get; set; }
        public bool ShippingPossibilities { get; set; }
        public bool ShippingPermission { get; set; }
        public int? ShippingBaseWeight { get; set; }
    }
}