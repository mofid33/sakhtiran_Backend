using System.IO;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace MarketPlace.API.Services.Service
{
    public class MessageLanguageService : IMessageLanguageService
    {
        public HeaderParseDto header { get; set; }
        public IWebHostEnvironment _hostingEnvironment;


        public MessageLanguageService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostingEnvironment)
        {
            header = new HeaderParseDto(httpContextAccessor);
            _hostingEnvironment = hostingEnvironment;
        }
        public string MessageService(string token)
        {
            string myJsonString;
            if (string.IsNullOrWhiteSpace(_hostingEnvironment.ContentRootPath))
            {
                _hostingEnvironment.ContentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "");
            }

            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Resources/Language/");
            try
            {
                myJsonString = File.ReadAllText(filePath + header.LanguageNum.ToString() + ".json");
            }
            catch (System.Exception)
            {
                myJsonString = File.ReadAllText(filePath + "En.json");
            }
            try
            {
                var myJObject = JObject.Parse(myJsonString);

                return myJObject.SelectToken(token).ToString();
            }
            catch (System.Exception)
            {
                return "";
            }
            
        }


        public string MessageService(string token,string language)
        {
            string myJsonString;
            if (string.IsNullOrWhiteSpace(_hostingEnvironment.ContentRootPath))
            {
                _hostingEnvironment.ContentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "");
            }

            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Resources/Language/");
            try
            {
                myJsonString = File.ReadAllText(filePath + language+ ".json");
            }
            catch (System.Exception)
            {
                myJsonString = File.ReadAllText(filePath + "En.json");
            }
            try
            {
                var myJObject = JObject.Parse(myJsonString);

                return myJObject.SelectToken(token).ToString();
            }
            catch (System.Exception)
            {
                return "";
            }
            
        }
    }
}