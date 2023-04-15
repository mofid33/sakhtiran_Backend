namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class OrderCancelingAddDto
    {
        public long FkOrderId { get; set; }
        public long FkOrderItemId { get; set; }
        public int FkCancelingReasonId { get; set; }
        public string Comment { get; set; }
    }
}