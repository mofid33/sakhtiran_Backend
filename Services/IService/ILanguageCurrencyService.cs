using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Currency;
using MarketPlace.API.Data.Dtos.Language;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;
namespace MarketPlace.API.Services.IService
{
    public interface ILanguageCurrencyService
    {
        Task<ApiResponse<List<LanguageDto>>> GetAllLanguage();
        Task<ApiResponse<List<CurrencyDto>>> GetAllCurrency();
        Task<ApiResponse<bool>> ChangeDefaultLanguage(int languageId);
        Task<ApiResponse<bool>> ChangeDefaultCurrency(int currencyId);
        Task<ApiResponse<bool>> RatesAgainstOneDollar(int currencyId,double rate);

    }
}