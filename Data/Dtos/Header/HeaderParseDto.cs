using System;
using MarketPlace.API.Data.Enums;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Header
{
    public class HeaderParseDto
    {
        public HeaderParseDto(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                Language = "$." + httpContextAccessor.HttpContext.Request.Headers["Language"];
                if(httpContextAccessor.HttpContext.Request.Headers["Language"] == LanguageEnum.En.ToString())
                {
                    LanguageNum = LanguageEnum.En;
                }
                else if(httpContextAccessor.HttpContext.Request.Headers["Language"] == LanguageEnum.Ar.ToString())
                {
                    LanguageNum = LanguageEnum.Ar;
                }
                else if(httpContextAccessor.HttpContext.Request.Headers["Language"] == LanguageEnum.Fa.ToString())
                {
                    LanguageNum = LanguageEnum.Fa;
                }
                else
                {

                    Language = "$.En";
                    LanguageNum = LanguageEnum.En;
                }
            }
            catch (System.Exception)
            {
                Language = "$.En";
                LanguageNum = LanguageEnum.En;
            }
            try
            {
                Currency = httpContextAccessor.HttpContext.Request.Headers["Currency"];
                if(httpContextAccessor.HttpContext.Request.Headers["Currency"] == CurrencyEnum.USD.ToString())
                {
                    CurrencyNum = CurrencyEnum.USD;
                }
                else if(httpContextAccessor.HttpContext.Request.Headers["Currency"] == CurrencyEnum.BHD.ToString())
                {
                     CurrencyNum = CurrencyEnum.BHD;
                }                
                else if(httpContextAccessor.HttpContext.Request.Headers["Currency"] == CurrencyEnum.TMN.ToString())
                {
                     CurrencyNum = CurrencyEnum.TMN;
                }
                else
                {
                    Currency = "USD";
                    CurrencyNum = CurrencyEnum.USD;
                }
            }
            catch (System.Exception)
            {
                Currency = "USD";
                CurrencyNum = CurrencyEnum.USD;
            }

            try
            {
                Platform = httpContextAccessor.HttpContext.Request.Headers["platform"];
                if(Platform != PlatformEnum.android.ToString() && Platform != PlatformEnum.ios.ToString())
                {
                    Platform = PlatformEnum.windows.ToString();
                }
               
            }
            catch (System.Exception)
            {
                Platform = PlatformEnum.windows.ToString();
            }


        }

        public string Language { get; set; }
        public string Currency { get; set; }
        public string Platform { get; set; }
        public LanguageEnum LanguageNum { get; set; }
        public CurrencyEnum CurrencyNum { get; set; }
    }
}