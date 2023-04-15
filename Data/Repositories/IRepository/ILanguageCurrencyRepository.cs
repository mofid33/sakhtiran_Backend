using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Currency;
using MarketPlace.API.Data.Dtos.Language;
using MarketPlace.API.Data.Dtos.Pagination;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface ILanguageCurrencyRepository
    {
        Task<List<LanguageDto>> GetAllLanguage();
        Task<List<CurrencyDto>> GetAllCurrency();
        Task<bool> ChangeDefaultLanguage(int languageId);
        Task<bool> ChangeDefaultCurrency(int currencyId);
        Task<bool> RatesAgainstOneDollar(int currencyId, double rate);
    }
}