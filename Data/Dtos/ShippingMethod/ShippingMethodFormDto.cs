namespace MarketPlace.API.Data.Dtos.ShippingMethod
{
    public class ShippingMethodFormDto
    {
        public int Id { get; set; }
        public string ShippingMethodTitle { get; set; }
        public bool CashOnDelivery { get; set; }
        public bool? HaveOnlineService { get; set; }
        public int? BaseWeight { get; set; }
    }
}