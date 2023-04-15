using System;
using System.Text;
using System.Threading.Tasks;
using AramexRateCalculator;
using MarketPlace.API.PaymentGateway.CredimaxHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace MarketPlace.API.PostServices.Dhl
{
    public class Dhl
    {

        public Dhl()
        {

        }

        public async static Task<double> executedDhlPost(double weight,double length ,double width ,double height , string originCountryCode , string destinationCountryCode , string originCityName  , string destinationCityName)
        {
            try
            {
                var client = new RestClient("https://api-mock.dhl.com/mydhlapi/rates");
                var request = new RestRequest(Method.GET);
                request.AddParameter(
                    "accountNumber","123456789",
                    ParameterType.QueryString);
                request.AddParameter(
                   "originCountryCode", originCountryCode ,
                   ParameterType.QueryString);
                request.AddParameter(
                   "originCityName ", originCityName ,
                   ParameterType.QueryString);
                request.AddParameter(
                   "destinationCountryCode", destinationCountryCode  ,
                   ParameterType.QueryString);
                request.AddParameter(
                   "destinationCityName", destinationCityName,
                   ParameterType.QueryString);
                request.AddParameter(
                   "weight", weight,
                   ParameterType.QueryString);
                request.AddParameter(
                   "length", length,
                   ParameterType.QueryString);
                request.AddParameter(
                   "width", width,
                   ParameterType.QueryString);
                request.AddParameter(
                   "height", height,
                   ParameterType.QueryString);
                request.AddParameter(
                   "plannedShippingDate", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                   ParameterType.QueryString);
                request.AddParameter(
                   "isCustomsDeclarable", "false",
                   ParameterType.QueryString);
                request.AddParameter(
                   "unitOfMeasurement", "metric",
                   ParameterType.QueryString);
               //  string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("ajyal.bahrain2020" + ":" + "Ajyal@2020"));
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("ajyal.bahrain2020" + ":" + "Ajyal@2020"));

                request.AddHeader("Authorization", "Basic " + encoded);
                IRestResponse response = await client.ExecuteAsync(request);

                var resource = JsonConvert.DeserializeObject<DhlDto>(response.Content);
                return resource.products[0].totalPrice[0].price ;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


    }
}