using System.Threading.Tasks;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public class MyMiddleware
{
    private readonly RequestDelegate _next;

    public MyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ISettingRepository _settingRepository)
    {
        var language = "";
        var currency = "";
        //language
        try
        {
            language = context.Request.Headers["Language"];
            if (string.IsNullOrWhiteSpace(language))
            {
                context.Request.Headers["Language"] = await _settingRepository.GetDefaultLanguage();
            }
            else
            {
                if (language != LanguageEnum.En.ToString() && language != LanguageEnum.Ar.ToString() && language != LanguageEnum.Fa.ToString())
                {
                    context.Request.Headers["Language"] = await _settingRepository.GetDefaultLanguage();
                }
            }
        }
        catch (System.Exception)
        {
            context.Request.Headers["Language"] = await _settingRepository.GetDefaultLanguage();
        }

        //currency
        try
        {
            currency = context.Request.Headers["Currency"];
            if (string.IsNullOrWhiteSpace(currency))
            {
                context.Request.Headers["Currency"] = await _settingRepository.GetDefaultCurrency();
            }
            else
            {
                if (currency != CurrencyEnum.USD.ToString() && currency != CurrencyEnum.BHD.ToString() && currency != CurrencyEnum.TMN.ToString())
                {
                    context.Request.Headers["Currency"] = await _settingRepository.GetDefaultCurrency();
                }
            }
        }
        catch (System.Exception)
        {
            context.Request.Headers["Currency"] = await _settingRepository.GetDefaultCurrency();
        }
        await _next(context);
    }
}

public static class MyMiddlewareExtensions
{
    public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<MyMiddleware>();
    }
}
