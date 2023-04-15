namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class CustomerAddWithdrawalRequestDto
    {
        public string RequestText { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
    }
}