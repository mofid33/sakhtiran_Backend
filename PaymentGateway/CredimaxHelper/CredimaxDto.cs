namespace MarketPlace.API.PaymentGateway.CredimaxHelper
{
    public class CredimaxDto
    {        
        public AuthorizationResponse authorizationResponse { get; set; }
        public string gatewayEntryPoint { get; set; }
        public string merchant { get; set; }
        public string result { get; set; }
        public Response response { get; set; }
        public Transaction transaction { get; set; }
        public Error error { get; set; }

        public class AuthorizationResponse {
          public string  cardLevelIndicator { get; set; }
          public string  cardSecurityCodeError { get; set; }
          public string  commercialCard { get; set; }
          public string  commercialCardIndicator { get; set; }
          public string   date { get; set; }
          public string  posData { get; set; }
          public string  posEntryMode { get; set; }
          public string  processingCode { get; set; }
          public string  responseCode { get; set; }
          public string  returnAci { get; set; }
          public string  stan { get; set; }
          public string  time { get; set; }
          public string  transactionIdentifier { get; set; }
          public string  validationCode { get; set; }       
    }

        public class Response {
             public string acquirerCode {get; set;}
             public string acquirerMessage {get; set;}
             public string gatewayCode {get; set;}
             public CardSecurityCode cardSecurityCode {get; set;}
        }

        public class CardSecurityCode {
             public string acquirerCode {get; set;}
             public string gatewayCode {get; set;}
        }


        public class Transaction {
             public Acquirer acquirer {get; set;}
             public string amount {get; set;}
             public string authenticationStatus {get; set;}
             public string authorizationCode {get; set;}
             public string id {get; set;}
             public string receipt {get; set;}
             public string source {get; set;}
             public string stan {get; set;}
             public string terminal {get; set;}
             public string type {get; set;}
        }
       public class Acquirer {
             public string batch {get; set;}
             public string date {get; set;}
             public string id {get; set;}
             public string merchantId {get; set;}
             public string transactionId {get; set;}
        }
       public class Error {
             public string cause {get; set;}
             public string explanation {get; set;}
             public string field {get; set;}
             public string validationType {get; set;}
        }
    }
}

