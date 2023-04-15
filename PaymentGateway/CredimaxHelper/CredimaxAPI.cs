using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Helper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarketPlace.API.PaymentGateway.CredimaxHelper
{
    public class CredimaxAPI
    {

        public IConfiguration configuration { get; }

        public CredimaxAPI(IConfiguration configuration)
        {
            this.configuration = configuration;

        }


        public async Task<CredimaxDto> executedAuth(decimal total, string currency, PayOrderDto order)
        {
            try
            {
                CredimaxDto auth = await GetAuthCrediMaxAsync( total, currency, order);

                return auth;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<CredimaxDto> executedPayment(decimal total, string currency, long orderId, long transactionId)
        {
            try
            {
                return await PaymentCrediMaxAsync(total, currency, orderId, transactionId);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private async Task<CredimaxDto> GetAuthCrediMaxAsync(
               decimal total, string currency, PayOrderDto order)
        {
            try
            {
                string body = String.Empty;
                 string requestUrl =  configuration["Credimax:urlAPI"] +  "/merchant/"+configuration["Credimax:MerchantId"]+"/order/"+order.OrderId+
               "/transaction/"+order.PaymentId ;
                // Create the web request
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;

                //http verb
                request.Method = "PUT";

                //content type, json, form, etc
                request.ContentType = "application/json; charset=iso-8859-1";
                string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(configuration["Credimax:UserName"] + ":" + configuration["Credimax:Password"]));
                request.Headers.Add("Authorization", "Basic " + credentials);
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("apiOperation", "AUTHORIZE");
                nvc.Add("sourceOfFunds.type", "CARD");
                nvc.Add("sourceOfFunds.provided.card.number", order.CardNumber);
                nvc.Add("sourceOfFunds.provided.card.expiry.month", order.CardMonth);
                nvc.Add("sourceOfFunds.provided.card.expiry.year", order.CardYear);
                nvc.Add("sourceOfFunds.provided.card.securityCode", order.SecurityCode);
                nvc.Add("order.amount", total.ToString());
                nvc.Add("order.currency", currency);
                
                var Payload = Extentions.BuildJsonFromNVC(nvc);
                // Create a byte array of the data we want to send
                byte[] utf8bytes = Encoding.UTF8.GetBytes(Payload);
                byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);

                // Set the content length in the request headers
                request.ContentLength = iso8859bytes.Length;

                // Write request data
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(iso8859bytes, 0, iso8859bytes.Length);
                }
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                    body = reader.ReadToEnd();
                }

                 CredimaxDto CredimaxDto =
                JsonConvert.DeserializeObject<CredimaxDto>(body);
                return CredimaxDto;

            }
            catch (System.Exception)
            {
               var res = new CredimaxDto();
               res.response.gatewayCode = "error";
               res.error.explanation = "An error has occurred";
               return res ;
            }
        }


        private async Task<CredimaxDto> PaymentCrediMaxAsync(
               decimal total, string currency, long orderId, long transactionId)
        {
            try
            {

                string body = String.Empty;
                 string requestUrl =  configuration["Credimax:urlAPI"] +  "/merchant/"+configuration["Credimax:MerchantId"]+"/order/"+orderId+
               "/transaction/"+transactionId ;
                // Create the web request
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;

                //http verb
                request.Method = "PUT";

                //content type, json, form, etc
                request.ContentType = "application/json; charset=iso-8859-1";
                string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(configuration["Credimax:UserName"] + ":" + configuration["Credimax:Password"]));
                request.Headers.Add("Authorization", "Basic " + credentials);
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("apiOperation", "CAPTURE");
                nvc.Add("transaction.amount", total.ToString());
                nvc.Add("transaction.currency", currency);
                
                var Payload = Extentions.BuildJsonFromNVC(nvc);
                // Create a byte array of the data we want to send
                byte[] utf8bytes = Encoding.UTF8.GetBytes(Payload);
                byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);

                // Set the content length in the request headers
                request.ContentLength = iso8859bytes.Length;

                // Write request data
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(iso8859bytes, 0, iso8859bytes.Length);
                }
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                    body = reader.ReadToEnd();
                }

                 CredimaxDto CredimaxDto =
                JsonConvert.DeserializeObject<CredimaxDto>(body);
                return CredimaxDto;

            }
            catch (System.Exception)
            {
               var res = new CredimaxDto();
               res.response.gatewayCode = "error";
               res.error.explanation = "An error has occurred";
               return res ;
            }
        }



    }
}