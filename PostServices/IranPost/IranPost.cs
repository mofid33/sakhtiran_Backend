using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;


namespace MarketPlace.API.PostServices.IranPost
{
    public class IranPost
    {

        public IranPost()
        {

        }

        public async static Task<double> executedIranPost(double weight, string state, string city, string tip, string cod, decimal price, string shCode)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://gateway.post.ir/Deliver.asmx");
                byte[] bytes;
                var requestXml = "<soap:Envelope xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>"+
                                "<soap:Body>"
                                  + "<GetPrice xmlns='http://Gateway.Post.IR/'>"
                                  +" <Username>royan123</Username>"
                                  + " <Password>123456</Password>"
                                  + " <Weight>"+weight+"</Weight>"
                                  + " <Price>"+price+"</Price>"
                                  + " <Shcode>"+shCode+"</Shcode>"
                                  + " <State>"+state+"</State>"
                                  + " <City>"+city+"</City>"
                                  + " <Tip>"+tip+"</Tip>"
                                  + " <Cod>"+cod+"</Cod>"
                                  + " <Showtype>"+1+"</Showtype>"
                                  + " </GetPrice>"
                               +" </soap:Body> " 
                               + "</soap:Envelope>" ;
                bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = await request.GetRequestStreamAsync();
                await requestStream.WriteAsync(bytes, 0, bytes.Length);
                requestStream.Close();
               // Task<HttpWebResponse> response;
                var response = await request.GetResponseAsync();
           
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    return 0;
        
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


    }
}