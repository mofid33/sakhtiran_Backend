namespace MarketPlace.API.Data.Dtos.OrderReturning
{
    public class OrderReturningChangeDto
    {
        public int ReturningId { get; set; }
        public int FkStatusId { get; set; }
        public string ProviderComment { get; set; }
        public int? FkShippingMethodId { get; set; }
        public double? ShippingAmount { get; set; }
    }
}