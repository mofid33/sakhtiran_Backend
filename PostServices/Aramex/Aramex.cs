using System;
using System.Threading.Tasks;
using AramexRateCalculator;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace MarketPlace.API.PostServices.Aramex
{
    public class Aramex
    {

        public Aramex()
        {

        }

        public async static Task<double> executedAramexPost(double weight, string fromCountryIso, string toCountryIso, string fromCityName , string toCityName , string currency)
        {
            try
            {
                RateCalculatorRequest _RateRequest = new RateCalculatorRequest();

                _RateRequest.ClientInfo = new ClientInfo();
                _RateRequest.ClientInfo.AccountCountryCode = string.Empty;
                _RateRequest.ClientInfo.AccountEntity = string.Empty;
                _RateRequest.ClientInfo.AccountNumber = string.Empty;
                _RateRequest.ClientInfo.AccountPin = string.Empty;
                _RateRequest.ClientInfo.UserName = "ajyalecommerce@gmail.com";
                _RateRequest.ClientInfo.Password = "Ajyal@2020";
                _RateRequest.ClientInfo.Version = "v1.0";

                _RateRequest.Transaction = new Transaction();
                _RateRequest.Transaction.Reference1 = "001";

                _RateRequest.OriginAddress = new Address();
                _RateRequest.OriginAddress.City = fromCityName;
                _RateRequest.OriginAddress.CountryCode = fromCountryIso;

                _RateRequest.DestinationAddress = new Address();
                _RateRequest.DestinationAddress.City = toCityName;
                _RateRequest.DestinationAddress.CountryCode = toCountryIso;

                _RateRequest.ShipmentDetails = new ShipmentDetails();
                _RateRequest.ShipmentDetails.ProductGroup = fromCountryIso == toCountryIso ? "DOM" : "EXP";
                _RateRequest.ShipmentDetails.ProductType =  fromCountryIso == toCountryIso ? "OND" : "PPX" ;
                _RateRequest.ShipmentDetails.PaymentType = "P";
                _RateRequest.Money = new Money();
                _RateRequest.Money.CurrencyCode = currency;
                _RateRequest.ShipmentDetails.ActualWeight = new Weight();
                _RateRequest.ShipmentDetails.ActualWeight.Value = weight;
                _RateRequest.ShipmentDetails.ActualWeight.Unit = "KG";

                _RateRequest.ShipmentDetails.ChargeableWeight = new Weight();
                _RateRequest.ShipmentDetails.ChargeableWeight.Value = weight ;
                _RateRequest.ShipmentDetails.ChargeableWeight.Unit = "KG";

                _RateRequest.ShipmentDetails.NumberOfPieces = 1;
                Service_1_0Client _Client = new Service_1_0Client();
                //   bool _HasErrors;
                //   Money _TotalAmount;
                var _Notifications = await _Client.CalculateRateAsync(_RateRequest);
                 
                return _Notifications.TotalAmount.Value;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


    }
}