namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class OrderReturningAddDto
    {
        public long FkOrderItemId { get; set; }
        public int FkReturningReasonId { get; set; }
        public int FkReturningActionId { get; set; }
        public string RequestComment { get; set; }
        public double Quantity { get; set; }
    }
}