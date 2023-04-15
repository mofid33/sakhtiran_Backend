using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Currency;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Language;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class LanguageCurrencyRepository : ILanguageCurrencyRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public LanguageCurrencyRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<bool> ChangeDefaultCurrency(int currencyId)
        {
            try
            {
                var data = await _context.TCurrency.ToListAsync();
                if (!data.Any(x => x.CurrencyId == currencyId))
                {
                    return false;
                }
                foreach (var item in data)
                {
                    item.DefaultCurrency = false;
                    if (item.CurrencyId == currencyId)
                    {
                        item.DefaultCurrency = true;
                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChangeDefaultLanguage(int languageId)
        {
            try
            {
                var data = await _context.TLanguage.ToListAsync();
                if (!data.Any(x => x.LanguageId == languageId))
                {
                    return false;
                }
                foreach (var item in data)
                {
                    item.DefaultLanguage = false;
                    if (item.LanguageId == languageId)
                    {
                        item.DefaultLanguage = true;
                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<CurrencyDto>> GetAllCurrency()
        {
            try
            {
                return await _context.TCurrency
                    .OrderByDescending(x => x.CurrencyId)
                    .Select(x => new CurrencyDto()
                    {
                        CurrencyId = x.CurrencyId,
                        CurrencyCode = x.CurrencyCode,
                        CurrencyTitle = JsonExtensions.JsonValue(x.CurrencyTitle,header.Language),
                        RatesAgainstOneDollar = x.RatesAgainstOneDollar,
                        DefaultCurrency = x.DefaultCurrency
                    })
                    .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<LanguageDto>> GetAllLanguage()
        {
            try
            {
                return await _context.TLanguage
                .OrderByDescending(x => x.LanguageId)
                .Select(x => new LanguageDto()
                {
                    LanguageId = x.LanguageId,
                    LanguageCode = x.LanguageCode,
                    LanguageTitle = JsonExtensions.JsonValue(x.LanguageTitle,header.Language),
                    JsonFile = x.JsonFile,
                    IsRtl = x.IsRtl,
                    DefaultLanguage = x.DefaultLanguage,
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> RatesAgainstOneDollar(int currencyId, double rate)
        {
            try
            {
                var data = await _context.TCurrency.FindAsync(currencyId);
                if (data == null)
                {
                    return false;
                }
                data.RatesAgainstOneDollar = rate;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}