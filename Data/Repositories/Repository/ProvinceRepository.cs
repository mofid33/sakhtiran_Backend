using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class ProvinceRepository : IProvinceRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public ProvinceRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }

        public async Task<TProvince> ProvinceAdd(TProvince Province)
        {
            try
            {
                Province.ProvinceName = JsonExtensions.JsonAdd(Province.ProvinceName, header);
                Province.Status = true;
                await _context.TProvince.AddAsync(Province);
                await _context.SaveChangesAsync();
                Province.ProvinceName = JsonExtensions.JsonGet(Province.ProvinceName, header);
                return Province;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TProvince> ProvinceEdit(TProvince Province)
        {
            try
            {
                var data = await _context.TProvince.FirstOrDefaultAsync(x => x.ProvinceId == Province.ProvinceId);
                Province.ProvinceName = JsonExtensions.JsonEdit(Province.ProvinceName, data.ProvinceName, header);
                _context.Entry(data).CurrentValues.SetValues(Province);
                _context.Entry(data).Property(x => x.Status).IsModified = false;
                await _context.SaveChangesAsync();
                Province.ProvinceName = JsonExtensions.JsonGet(Province.ProvinceName, header);
                return Province;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TProvince>> ProvinceDelete(int id)
        {
            try
            {
                var data = await _context.TProvince.FirstOrDefaultAsync(x => x.ProvinceId == id);
                if (data == null)
                {
                    return new RepRes<TProvince>(Message.ProvinceNotFoundById, false, null);
                }

                var hasRelation = await _context.TCustomerAddress.AsNoTracking().AnyAsync(x => x.FkProvinceId == id);
                if (hasRelation)
                {
                    return new RepRes<TProvince>(Message.ProvinceCantDelete, false, null);
                }
                // hasRelation = await _context.TOrder.AsNoTracking().AnyAsync(x => x.AdFkProvinceId == id);
                // if (hasRelation)
                // {
                //     return new RepRes<TProvince>(Message.ProvinceCantDelete, false, null);
                // }
                _context.TProvince.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TProvince>(Message.Successfull, true, data);

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ProvinceExist(int id)
        {
            try
            {
                var result = await _context.TProvince.AsNoTracking().AnyAsync(x => x.ProvinceId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<ProvinceGetDto>> ProvinceGetAll(PaginationDto pagination)
        {
            try
            {
                return await _context.TProvince
                    .Where(x => (pagination.Id == 0 ? true : (x.FkCountryId == pagination.Id)) && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.ProvinceName, header.Language).Contains(pagination.Filter))))
                    .OrderByDescending(x => x.Status)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkCountry)
                    .Select(x => new ProvinceGetDto()
                    {
                        ProvinceId = x.ProvinceId,
                        ProvinceName = JsonExtensions.JsonValue(x.ProvinceName, header.Language),
                        FkCountryId = x.FkCountryId,
                        Status = x.Status,
                        CountryTitle = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                    })
                    .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> ProvinceGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TProvince
                    .AsNoTracking()
                    .CountAsync(x => (pagination.Id == 0 ? true : (x.FkCountryId == pagination.Id)) && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.ProvinceName, header.Language).Contains(pagination.Filter))));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<ProvinceGetDto> GetProvinceById(int ProvinceId)
        {
            try
            {
                var data = await _context.TProvince
                .Include(x => x.FkCountry)
                .Select(x => new ProvinceGetDto()
                {
                    ProvinceId = x.ProvinceId,
                    Status = x.Status,
                    ProvinceName = JsonExtensions.JsonValue(x.ProvinceName, header.Language),
                    FkCountryId = x.FkCountryId,
                    CountryTitle = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language)
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProvinceId == ProvinceId);
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
                var ProvinceIds = new List<int>();
                foreach (var item in accept)
                {
                    ProvinceIds.Add(item.Id);
                }
                var data = await _context.TProvince.Include(x => x.TCity).Where(x => ProvinceIds.Contains(x.ProvinceId)).ToListAsync();
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Status = accept[0].Accept;
                    foreach (var item in data[i].TCity)
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