using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.City;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class CityRepository : ICityRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public CityRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }

        public async Task<TCity> CityAdd(TCity City)
        {
            try
            {
                City.CityTitle = JsonExtensions.JsonAdd(City.CityTitle, header);
                City.Status = true;
                await _context.TCity.AddAsync(City);
                await _context.SaveChangesAsync();
                City.CityTitle = JsonExtensions.JsonGet(City.CityTitle, header);
                return City;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TCity> CityEdit(TCity City)
        {
            try
            {
                var data = await _context.TCity.FirstOrDefaultAsync(x => x.CityId == City.CityId);
                City.CityTitle = JsonExtensions.JsonEdit(City.CityTitle, data.CityTitle, header);
                _context.Entry(data).CurrentValues.SetValues(City);
                _context.Entry(data).Property(x => x.Status).IsModified = false;
                _context.Entry(data).Property(x => x.Longitude).IsModified = false;
                _context.Entry(data).Property(x => x.Latitude).IsModified = false;
                _context.Entry(data).Property(x => x.IsCapital).IsModified = false;
                await _context.SaveChangesAsync();
                City.CityTitle = JsonExtensions.JsonGet(City.CityTitle, header);
                return City;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TCity>> CityDelete(int id)
        {
            try
            {
                var data = await _context.TCity.FirstOrDefaultAsync(x => x.CityId == id);
                if (data == null)
                {
                    return new RepRes<TCity>(Message.CityNotFoundById, false, null);
                }

                var hasRelation = await _context.TCustomerAddress.AsNoTracking().AnyAsync(x => x.FkCityId == id);
                if (hasRelation)
                {
                    return new RepRes<TCity>(Message.CityCantDelete, false, null);
                }
                hasRelation = await _context.TOrder.AsNoTracking().AnyAsync(x => x.AdFkCityId == id);
                if (hasRelation)
                {
                    return new RepRes<TCity>(Message.CityCantDelete, false, null);
                }
                _context.TCity.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TCity>(Message.Successfull, true, data);

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> CityExist(int id)
        {
            try
            {
                var result = await _context.TCity.AsNoTracking().AnyAsync(x => x.CityId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<CityGetDto>> CityGetAll(PaginationDto pagination)
        {
            try
            {
                return await _context.TCity
                    .Where(x => 
                    (pagination.Id == 0 ? true : (x.FkCountryId == pagination.Id)) &&
                    (pagination.ProvinceId == 0 ? true : (x.FkProvinceId == pagination.ProvinceId)) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CityTitle, header.Language).Contains(pagination.Filter))))
                    .OrderByDescending(x => x.Status)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x=>x.FkCountry)
                    .Include(x=>x.FkProvince)
                    .Select(x => new CityGetDto()
                    {
                        CityId = x.CityId,
                        CityTitle = JsonExtensions.JsonValue(x.CityTitle, header.Language),
                        FkCountryId = x.FkCountryId,
                        FkProvinceId = (int) x.FkProvinceId,
                        Status = x.Status,
                        CountryTitle = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                        ProvinceName = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language)
                    })
                    .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> CityGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TCity
                    .AsNoTracking()
                    .CountAsync(x => 
                    (pagination.Id == 0 ? true : (x.FkCountryId == pagination.Id)) &&
                    (pagination.ProvinceId == 0 ? true : (x.FkProvinceId == pagination.ProvinceId)) 
                    && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CityTitle, header.Language).Contains(pagination.Filter))));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<CityGetDto> GetCityById(int CityId)
        {
            try
            {
                var data = await _context.TCity
                .Include(x => x.FkCountry)
                .Include(x => x.FkProvince)
                .Select(x => new CityGetDto()
                {
                    CityId = x.CityId,
                    Status = x.Status,
                    CityTitle = JsonExtensions.JsonValue(x.CityTitle, header.Language),
                    FkCountryId = x.FkCountryId,
                    FkProvinceId = (int) x.FkProvinceId,
                    CountryTitle = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                    ProvinceName = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language)
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CityId == CityId);
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
                var cityIds = new List<int>() ;
                foreach (var item in accept)
                {
                     cityIds.Add(item.Id);
                }
                var data = await _context.TCity.Where(x=>cityIds.Contains(x.CityId)).ToListAsync();
                for (int i = 0; i < data.Count; i++)
                {
                   data[i].Status = accept[0].Accept ;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }


        public async Task<bool> ShippingMethodAreaCodeExist( long areaId ,int areaCode , int shippingMethodId , int cityId)
        {
            try
            {
                var result = await _context.TShippingMethodAreaCode.AsNoTracking()
                .AnyAsync(x => (areaId == 0 ? true : x.PostAreaCodeId == areaId) &&  x.FkShippingMethodId == shippingMethodId && x.FkCityId == cityId);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<TShippingMethodAreaCode> AddShippingMethodAreaCode(TShippingMethodAreaCode ShippingMethodAreaCode)
        {
            try
            {
                await _context.TShippingMethodAreaCode.AddAsync(ShippingMethodAreaCode);
                await _context.SaveChangesAsync();
                return ShippingMethodAreaCode;
            }
            catch (System.Exception)
            {
                return null;
            }        
        }

        public async Task<TShippingMethodAreaCode> UpdateShippingMethodAreaCode(TShippingMethodAreaCode ShippingMethodAreaCode)
        {
            try{
                var data = await _context.TShippingMethodAreaCode.FirstOrDefaultAsync(x => x.PostAreaCodeId == ShippingMethodAreaCode.PostAreaCodeId);
                _context.Entry(data).CurrentValues.SetValues(ShippingMethodAreaCode);

                await _context.SaveChangesAsync();
                return ShippingMethodAreaCode;
            }
            catch (System.Exception)
            {
                return null;
            }        
        }

        public async Task<RepRes<TShippingMethodAreaCode>> DeleteShippingMethodAreaCode(int id)
        {
            try
            {
                var data = await _context.TShippingMethodAreaCode.FirstOrDefaultAsync(x => x.PostAreaCodeId == id);
                if (data == null)
                {
                    return new RepRes<TShippingMethodAreaCode>(Message.AreaNotFoundById, false, null);
                }

                _context.TShippingMethodAreaCode.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TShippingMethodAreaCode>(Message.Successfull, true, data);

            }
            catch (System.Exception)
            {
                return null;
            }        
        }


        public async Task<List<CityShippingMethodAreaCodeDto>> ShippingMethodGetAll(PaginationDto pagination)
        {
            try
            {
                return await _context.TShippingMethodAreaCode
                    .Where(x => 
                    (x.FkCityId == pagination.Id))
                    .OrderByDescending(x => x.PostAreaCodeId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x=>x.FkCity)
                    .Include(x=>x.FkShippingMethod)
                    .Select(x => new CityShippingMethodAreaCodeDto()
                    {
                        PostAreaCodeId= x.PostAreaCodeId,
                        CityTitle = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                        ShippingMethodTitle = JsonExtensions.JsonValue(x.FkShippingMethod.ShippingMethodTitle, header.Language),
                        FkCityId = x.FkCityId,
                        FkShippingMethodId = x.FkShippingMethodId,
                        Code = x.Code,
                        StateCode = x.StateCode
                    })
                    .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> ShippingMethodGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TShippingMethodAreaCode
                    .AsNoTracking()
                    .CountAsync(x => 
                    (x.FkCityId == pagination.Id));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


    }
}