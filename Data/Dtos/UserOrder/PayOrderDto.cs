namespace MarketPlace.API.Data.Dtos.UserOrder
{
    public class PayOrderDto
    {
        public int PaymentType { get; set; }
        public string Code { get; set; }
        public string PaymentId { get; set; }
        public string Token { get; set; }
        public string PayerID { get; set; }
        public long OrderId { get; set; }
        public int CurrencyId { get; set; }

        // card info

        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string CardMonth { get; set; }
        public string CardYear { get; set; }
        public string CardZip { get; set; }
        public string SecurityCode { get; set; }


    }
}