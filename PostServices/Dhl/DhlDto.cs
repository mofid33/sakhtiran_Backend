namespace MarketPlace.API.PaymentGateway.CredimaxHelper
{
    public class DhlDto
    {
        public Products[] products { get; set; }

        public class Products
        {
            public TotalPrice[] totalPrice { get; set; }
        }

        public class TotalPrice
        {
            public string currencyType { get; set; }
            public string priceCurrency { get; set; }
            public double price { get; set; }
        }

    }
}

