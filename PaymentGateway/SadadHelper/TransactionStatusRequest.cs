using Newtonsoft.Json;

namespace MarketPlace.API.PaymentGateway.PaypalHelper
{
    class TransactionStatusRequest : BaseRequest
    {
        /// <summary>
        /// Transaction Reference number of the invoice to check status
        /// </summary>
        [JsonProperty("transaction-reference")]
        public string TransactionReference { get; set; }
    }
}
