using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarketPlace.API.PaymentGateway.PaypalHelper
{
    public class PayPalAPI
    {

        public IConfiguration configuration { get; }

        public PayPalAPI(IConfiguration configuration)
        {
            this.configuration = configuration;

        }
 

        public async Task<PayPalPaymentCreatedResponse> getRedirectUrlToPayPal(decimal total, string currency , bool websiteRedirect , bool mobileRedirect)
        {
            try
            {
                HttpClient http = GetPaypalHttpClient();
                PayPalAccessToken accessToken = await GetPayPalAccessTokenAsync(http);
                PayPalPaymentCreatedResponse createdPayment = await CreatePaypalPaymentAsync(http, accessToken, total, currency , websiteRedirect , mobileRedirect);
                // var result = createdPayment.links.First(x => x.rel == "approval_url").href;
                return createdPayment;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<PayPalPaymentExecutedResponse> executedPayment(string paymentId, string payerId)
        {
            try
            {
                HttpClient http = GetPaypalHttpClient();
                PayPalAccessToken accessToken = await GetPayPalAccessTokenAsync(http);
                return await ExecutePaypalPaymentAsync(http, accessToken, paymentId, payerId);

            }
            catch (System.Exception)
            {

                throw;
            }
        }
        private HttpClient GetPaypalHttpClient()
        {
            try
            {
                string sandbox = configuration["PayPal:urlAPI"];
                var http = new HttpClient
                {
                    BaseAddress = new Uri(sandbox),
                    Timeout = TimeSpan.FromSeconds(30),

                };

                return http;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        private async Task<PayPalAccessToken> GetPayPalAccessTokenAsync(HttpClient http)
        {
            try
            {
                byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes
                ($"{configuration["PayPal:clientId"]}:{configuration["PayPal:secret"]}");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                "/v1/oauth2/token");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(bytes));

                var form = new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials"

                };

                request.Content = new FormUrlEncodedContent(form);
                HttpResponseMessage response = await http.SendAsync(request);
                string content = await response.Content.ReadAsStringAsync();
                PayPalAccessToken accessToken = JsonConvert.DeserializeObject<PayPalAccessToken>
                (content);
                return accessToken;


            }
            catch (System.Exception)
            {

                throw;
            }
        }
        private async Task<PayPalPaymentCreatedResponse> CreatePaypalPaymentAsync(HttpClient http,
                    PayPalAccessToken accessToken, decimal total, string currency , bool websiteRedirect , bool mobileRedirect)
        {
            try
            {

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                "/v1/payments/payment");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                accessToken.access_token);

                var payment = JObject.FromObject(new
                {

                    intent = "sale",
                    redirect_urls = new
                    {
                        return_url = websiteRedirect ? ( mobileRedirect ? configuration["MobilePayPalRedirectUrl:returnUrl"] : configuration["PaymentRedirect:returnUrl"]) :  configuration["PaymentPanelRedirect:returnUrl"],
                        cancel_url = websiteRedirect ? ( mobileRedirect ? configuration["MobilePayPalRedirectUrl:returnUrl"] : configuration["PaymentRedirect:cancelUrl"]) :  configuration["PaymentPanelRedirect:cancelUrl"] 
                    },

                    payer = new { payment_method = "paypal" },
                    transactions = JArray.FromObject(new[]
                   {
                       new {
                           amount = new
                           {
                            total = total ,
                            currency = currency
                           }
                       }
                   })
                });

                request.Content = new StringContent(JsonConvert.SerializeObject(payment),
                Encoding.UTF8, "application/json");

                HttpResponseMessage response = await http.SendAsync(request);
                string content = await response.Content.ReadAsStringAsync();
                PayPalPaymentCreatedResponse payPalPaymentCreated =
                JsonConvert.DeserializeObject<PayPalPaymentCreatedResponse>(content);
                return payPalPaymentCreated;

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        private async Task<PayPalPaymentExecutedResponse> ExecutePaypalPaymentAsync(HttpClient http,
                    PayPalAccessToken accessToken, string paymentId, string payerId)
        {
            try
            {

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                $"/v1/payments/payment/{paymentId}/execute");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                accessToken.access_token);

                var payment = JObject.FromObject(new
                {
                    payer_id = payerId
                });

                request.Content = new StringContent(JsonConvert.SerializeObject(payment),
                Encoding.UTF8, "application/json");

                HttpResponseMessage response = await http.SendAsync(request);
                string content = await response.Content.ReadAsStringAsync();
                PayPalPaymentExecutedResponse executedPayment =
                JsonConvert.DeserializeObject<PayPalPaymentExecutedResponse>(content);
                return executedPayment;

            }
            catch (System.Exception)
            {

                throw;
            }
        }







    }
}