namespace MarketPlace.API.Data.Dtos.ReturningReason
{
    public class ReturningReasonDto
    {
        public int ReasonId { get; set; }
        public string ReasonTitle { get; set; }
        public string ReturnCondition { get; set; }
        public bool Status { get; set; }
    }
}