using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Country;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Enums;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class CountryRepository : ICountryRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public CountryRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }

        public async Task<TCountry> CountryAdd(TCountry Country)
        {
            try
            {
                if (Country.Vat > 100)
                {
                    Country.Vat = 100;
                }
                else if (Country.Vat < 0)
                {
                    Country.Vat = 0;
                }
                Country.Status = true;
                Country.CountryTitle = JsonExtensions.JsonAdd(Country.CountryTitle, header);
                await _context.TCountry.AddAsync(Country);
                await _context.SaveChangesAsync();
                Country.CountryTitle = JsonExtensions.JsonGet(Country.CountryTitle, header);
                return Country;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TCountry> CountryEdit(TCountry Country)
        {
            try
            {
                var data = await _context.TCountry.FirstOrDefaultAsync(x => x.CountryId == Country.CountryId);
                if (Country.Vat > 100)
                {
                    Country.Vat = 100;
                }
                else if (Country.Vat < 0)
                {
                    Country.Vat = 0;
                }
                if (data.Vat != Country.Vat && Country.Vat != 0)
                {
                    //get all goodsprovider  that vatfree = false and the shop of them is in this country
                    var shopIds = await _context.TShop.Where(x => x.FkCountryId == Country.CountryId).Select(x => x.ShopId).ToListAsync();
                    var GoodsProvider = await _context.TGoodsProvider.Where(x => x.Vatfree == false && shopIds.Contains(x.FkShopId)).ToListAsync();
                    foreach (var item in GoodsProvider)
                    {
                        item.Vatamount = (item.Price - (item.DiscountAmount == null ? (decimal)0 : (decimal)item.DiscountAmount) * Country.Vat) / 100;
                    }
                }
                Country.CountryTitle = JsonExtensions.JsonEdit(Country.CountryTitle, data.CountryTitle, header);
                _context.Entry(data).CurrentValues.SetValues(Country);
                _context.Entry(data).Property(x => x.Status).IsModified = false;
                _context.Entry(data).Property(x => x.FlagUrl).IsModified = false;
                _context.Entry(data).Property(x => x.Iso2).IsModified = false;
                _context.Entry(data).Property(x => x.Iso3).IsModified = false;
                _context.Entry(data).Property(x => x.PhoneCode).IsModified = false;
                await _context.SaveChangesAsync();
                Country.CountryTitle = JsonExtensions.JsonGet(Country.CountryTitle, header);
                return Country;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TCountry>> CountryDelete(int id)
        {
            try
            {
                var data = await _context.TCountry.FirstOrDefaultAsync(x => x.CountryId == id);
                if (data == null)
                {
                    return new RepRes<TCountry>(Message.CountryNotFoundById, false, null);
                }
                var hasRelation = await _context.TCity.AsNoTracking().AnyAsync(x => x.FkCountryId == id);
                if (hasRelation)
                {
                    return new RepRes<TCountry>(Message.CountryCantDelete, false, null);
                }
                hasRelation = await _context.TCustomerAddress.AsNoTracking().AnyAsync(x => x.FkCountryId == id);
                if (hasRelation)
                {
                    return new RepRes<TCountry>(Message.CountryCantDelete, false, null);
                }
                hasRelation = await _context.TOrder.AsNoTracking().AnyAsync(x => x.AdFkCountryId == id);
                if (hasRelation)
                {
                    return new RepRes<TCountry>(Message.CountryCantDelete, false, null);
                }
                _context.TCountry.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TCountry>(Message.Successfull, true, data);

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> CountryExist(int id)
        {
            try
            {
                var result = await _context.TCountry.AsNoTracking().AnyAsync(x => x.CountryId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<CountryDto>> CountryGetAll(PaginationDto pagination)
        {
            try
            {
                var rate = (decimal)1.00;
                if (header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var data = await _context.TCountry
                    .Where(x => (pagination.Active != null ? x.Status == (bool)pagination.Active : true) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CountryTitle, header.Language).Contains(pagination.Filter))))
                    .OrderByDescending(x => x.Status)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new CountryDto()
                    {
                        CountryId = x.CountryId,
                        CountryTitle = JsonExtensions.JsonValue(x.CountryTitle, header.Language),
                        FlagUrl = x.FlagUrl,
                        Vat = decimal.Round((decimal)x.Vat, 2, MidpointRounding.AwayFromZero)  ,
                        Status = x.Status
                    })
                    .AsNoTracking().ToListAsync();
                return data;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> CountryGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TCountry
                    .AsNoTracking()
                    .CountAsync(x => (pagination.Active != null ? x.Status == (bool)pagination.Active : true) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CountryTitle, header.Language).Contains(pagination.Filter))));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<CountryDto> GetCountryById(int CountryId)
        {
            try
            {
                var data = await _context.TCountry
                .Select(x => new CountryDto()
                {
                    CountryId = x.CountryId,
                    CountryTitle = JsonExtensions.JsonValue(x.CountryTitle, header.Language),
                    FlagUrl = x.FlagUrl,
                    Vat = decimal.Round((decimal)x.Vat, 2, MidpointRounding.AwayFromZero),
                    Status = x.Status
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CountryId == CountryId);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ChangeAccept(List<AcceptDto> accept)
        {
            try
            {
                var countryIds = new List<int>() ;
                foreach (var item in accept)
                {
                     countryIds.Add(item.Id);
                }
                var data = await _context.TCountry.Include(x => x.TProvince).Where(x=>countryIds.Contains(x.CountryId)).ToListAsync();
                for (int i = 0; i < data.Count; i++)
                {
                   data[i].Status = accept[0].Accept ;
                    foreach (var item in data[i].TProvince)
                    {
                        item.Status = accept[0].Accept;
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
    }
}