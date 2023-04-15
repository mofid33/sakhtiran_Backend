namespace MarketPlace.API.PaymentGateway.PaypalHelper
{
    public class PayPalAccessToken
    {
        public string scope { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string app_id { get; set; }
        public string expires_in { get; set; }
        public string nonce { get; set; }
        
    }
}