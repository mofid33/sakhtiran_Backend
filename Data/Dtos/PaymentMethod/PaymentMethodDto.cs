namespace MarketPlace.API.Data.Dtos.PaymentMethod
{
    public class PaymentMethodDto
    {
        public int MethodId { get; set; }
        public string MethodTitle { get; set; }
        public string MethodImageUrl { get; set; }
        public bool? Active { get; set; }
    }
}