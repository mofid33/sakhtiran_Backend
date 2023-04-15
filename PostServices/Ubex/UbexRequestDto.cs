using System.Collections.Generic;

namespace MarketPlace.API.PaymentGateway.CredimaxHelper
{
    public class UbexRequestDto
    {
        public string area { get; set; }
        public string country { get; set; }
        public string base_product { get; set; }
        public decimal parcel_value { get; set; }
        public string parcel_currency { get; set; }
        public decimal declared_value{ get; set; }
        public string declared_currency{ get; set; }

        public List<Pieces> pieces { get; set; }

        public class Pieces
        {
          public string weight{ get; set; }
          public string length{ get; set; }
          public string width{ get; set; }
          public string height{ get; set; }
          public string qty{ get; set; }
          public string value{ get; set; }
        }


    }
}

