namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountCustomersGetDto
    {
        public int AssignedCustomerId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkCustomerId { get; set; }
        public bool Allowed { get; set; }
        public string CustomerTitle { get; set; }
    }
}