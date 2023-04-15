using System;
using System.Collections.Generic;
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
    public class Ubex
    {

        public Ubex()
        {

        }

        public async static Task<double> executedUbexPost(double weight,double length ,double width ,double height, decimal price, double qty)
        {
            try
            {
                var client = new RestClient("https://ubex-clients.apis.delivery/api/shipments/shipment-rate");
                var request = new RestRequest();
                request.AddParameter(
                    "token","eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczpcL1wvdWJleC1jbGllbnRzLmFwaXMuZGVsaXZlcnlcL2FwaVwvdjJcL2FjY291bnRcL2xvZ2luIiwiaWF0IjoxNjE1MjEzMDUyLCJuYmYiOjE2MTUyMTMwNTIsImp0aSI6ImlKQlNPd3JSYUF1Z1ZnMzUiLCJzdWIiOjExMzg1NCwicHJ2IjoiODdlMGFmMWVmOWZkMTU4MTJmZGVjOTcxNTNhMTRlMGIwNDc1NDZhYSJ9.cvA15Hwu_seNouc-W9tFJhgtrZhkbPms7YDOhGEaAds",
                    ParameterType.QueryString);

                  var param = new UbexRequestDto();
                  var pieces = new UbexRequestDto.Pieces();
                  param.pieces = new List<UbexRequestDto.Pieces>();
                  param.area = "df2dab77-8e9c-491c-82f8-00880f84bb2e";
                  param.country = "BH";
                  param.base_product = "d282d62f-69e0-42e3-abfd-64dcd29f26ec";
                  param.parcel_value = price;
                  param.parcel_currency = "BHD";
                  param.declared_value = price;
                  param.declared_currency = "BHD";
                  pieces.height = height.ToString();
                  pieces.width= width.ToString();
                  pieces.weight = weight.ToString();
                  pieces.length = length.ToString();
                  pieces.qty = qty.ToString();
                  pieces.value = price.ToString();
                  param.pieces.Add(pieces);
                  request.AddJsonBody(param);

                var response = await client.PostAsync<UbexResponseDto>(request);

             //   var resource = JsonConvert.DeserializeObject<UbexResponseDto>(response.Content);
                if(response.status == 200) { 
                return Convert.ToDouble(response.charges);
                } else { 
                   return 0 ;
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


    }
}