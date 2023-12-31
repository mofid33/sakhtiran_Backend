using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MarketPlace.API.PaymentGateway.PaypalHelper
{
    /// <summary>
    /// SADDED Payment Client for creating and verifying payments.
    /// </summary>
    public class SaddedClient
    {

        public IConfiguration configuration { get; }

        public SaddedClient (IConfiguration configuration)
        {

            this.configuration = configuration;

        }

        /// <summary>
        /// This function creates an invoice link based on the given parameters as an invoice from the program
        /// </summary>
        /// <param name="invoice">All paramters that are related to invoice</param>
        /// <returns>Link to the invoice of SADDED. The customer should be redirected to this link.</returns>
        public InvoiceResponse createInvoiceLink(CreateInvoiceRequest invoice)
        {
            SetEssentialParameters(invoice);
            InvoiceResponse validationResponse = IsValidCreateInvoiceLinkRequest(invoice);
            if(validationResponse.ErrorCode == 0)
            {
                string uri = configuration["Sadad:BaseUrl"] + EndPoints.InvoiceCreate;
                string response = HttpPost(uri, JsonConvert.SerializeObject(invoice));
                return JsonConvert.DeserializeObject<InvoiceResponse>(response);
            }
            return validationResponse;
        }

        /// <summary>
        /// This function checks if the status of the transaction is completed or not. 
        /// </summary>
        /// <param name="transactionReference">This is the transaction reference which was returned in invoice url generation response</param>
        /// <returns>Status of the invoice. All status codes are given in the documentation.</returns>
        public InvoiceResponse checkTransactionStatus(string transactionReference)
        {
            TransactionStatusRequest transactionStatusRequest = new TransactionStatusRequest()
            {
                ApiKey = configuration["Sadad:ApiKey"],
                BranchId = Convert.ToInt64(configuration["Sadad:BranchId"]),
                VendorId = Convert.ToInt64(configuration["Sadad:VendorId"]),
                TerminalId = Convert.ToInt64(configuration["Sadad:TerminalId"]),
                TransactionReference = transactionReference
            };
            string uri = configuration["Sadad:BaseUrl"] + EndPoints.InvoiceStatus;
            string response = HttpPost(uri, JsonConvert.SerializeObject(transactionStatusRequest));
            return JsonConvert.DeserializeObject<InvoiceResponse>(response);
        }


        /// <summary>
        /// This function carry out Http Post requests 
        /// </summary>
        /// <param name="uri">Url to be called</param>
        /// <param name="data">request body data</param>
        /// <returns>Response from server.</returns>
        private string HttpPost(string uri, string data)
        {
            try
            {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = "multipart/form-data";
            request.Method = "POST";

            using (Stream requestBody = request.GetRequestStream())
                requestBody.Write(dataBytes, 0, dataBytes.Length);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                return reader.ReadToEnd();
            }
            catch (System.Exception)
            {
                
                throw;
            }

        }

        /// <summary>
        /// This function validates invoice create request 
        /// </summary>
        /// <param name="invoice">All paramters that are related to invoice</param>
        /// <returns>InvoiceResponse containing error messages if any</returns>
        private InvoiceResponse IsValidCreateInvoiceLinkRequest(CreateInvoiceRequest invoice)
        {
            List<string> listOfErrors = new List<string>();
            if (invoice.Amount <= 0)
                listOfErrors.Add("Amount must be greater than zero");

            if (invoice.Date == DateTime.MinValue)
                listOfErrors.Add("Date is missing");

            if (string.IsNullOrEmpty(invoice.Email) && invoice.NotificationMode.Equals((int)NotificationMode.Email))
                listOfErrors.Add("Email is missing");

            if(!string.IsNullOrEmpty(invoice.Email))
            { 
                MailAddress mailAddress = new MailAddress(invoice.Email);
                if (mailAddress.Address != invoice.Email && invoice.NotificationMode.Equals((int)NotificationMode.Email))
                    listOfErrors.Add("Email address is invalid");
            }
            if (string.IsNullOrEmpty(invoice.Msisdn) && invoice.NotificationMode.Equals((int)NotificationMode.SMS))
                listOfErrors.Add("Msisdn is missing");
            
            if (invoice.NotificationMode.Equals((int)NotificationMode.Online) && string.IsNullOrEmpty(invoice.Msisdn) && string.IsNullOrEmpty(invoice.Email))
                listOfErrors.Add("Msisdn or Email is missing");

            if (string.IsNullOrEmpty(invoice.CustomerName))
                listOfErrors.Add("CustomerName is missing");

            if (invoice.NotificationMode.Equals((int)NotificationMode.Online))
            {
                if (string.IsNullOrEmpty(invoice.SuccessUrl))
                    listOfErrors.Add("SuccessUrl is missing");
                if (string.IsNullOrEmpty(invoice.ErrorUrl))
                    listOfErrors.Add("ErrorUrl is missing");
            }

            InvoiceResponse validationResponse = new InvoiceResponse()
            {
                ErrorCode = 0
            };

            if(listOfErrors.Count != 0)
            {
                validationResponse.ErrorCode = 102; // missing parameters error code
                validationResponse.ErrorMessage = string.Join(" | ", listOfErrors.ToArray());
            }

            return validationResponse;
        }

        /// <summary>
        /// This function sets apikey, branchid, terminalid and vendorid if not set by user. 
        /// </summary>
        /// <param name="invoice">All paramters that are related to invoice</param>
        /// <returns>void</returns>
        private void SetEssentialParameters(CreateInvoiceRequest invoice)
        {
            if (string.IsNullOrEmpty(invoice.ApiKey))
                invoice.ApiKey = configuration["Sadad:ApiKey"];
            if (invoice.BranchId == 0)
                invoice.BranchId = Convert.ToInt64(configuration["Sadad:BranchId"]);
            if (invoice.TerminalId == 0)
                invoice.TerminalId = Convert.ToInt64(configuration["Sadad:TerminalId"]);
            if (invoice.VendorId == 0)
                invoice.VendorId = Convert.ToInt64(configuration["Sadad:VendorId"]);
        }


    }
}
