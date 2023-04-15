namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class ShopWithdrawalRequestDto
    {
        public int RequestId { get; set; }
        public string ResponseText { get; set; }
        public decimal Amount { get; set; }
        public string DocumentUrl { get; set; }
        public bool Status { get; set; }
    }
}