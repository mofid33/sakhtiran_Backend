namespace MarketPlace.API.Data.Dtos.CustomerBankCards
{
    public class CustomerBankCardGetDto
    {
        public int BankCardId { get; set; }
        public string BankCardName { get; set; }
        public string BankCardNumber { get; set; }
        public string BankCardMonth { get; set; }
        public string ZipCode { get; set; }
        public int FkCustumerId { get; set; }
        public string BankCardYear { get; set; }
        public int FkPaymentMethodId { get; set; }
        public string PaymentMethodImageName { get; set; }

    }
}