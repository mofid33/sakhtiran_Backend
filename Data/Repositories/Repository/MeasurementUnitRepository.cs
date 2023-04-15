using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.MeasurementUnit;
using MarketPlace.API.Data.Dtos.Pagination;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class MeasurementUnitRepository : IMeasurementUnitRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public MeasurementUnitRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TMeasurementUnit> MeasurementUnitAdd(TMeasurementUnit MeasurementUnit)
        {
            try
            {
                MeasurementUnit.UnitTitle = JsonExtensions.JsonAdd(MeasurementUnit.UnitTitle,header);
                await _context.TMeasurementUnit.AddAsync(MeasurementUnit);
                await _context.SaveChangesAsync();
                MeasurementUnit.UnitTitle = JsonExtensions.JsonGet(MeasurementUnit.UnitTitle,header);
                return MeasurementUnit;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TMeasurementUnit>> MeasurementUnitDelete(int id)
        {
            try
            {
                var existInOrderCanseling = await _context.TOrderCanceling.AsNoTracking().AnyAsync(x=>x.FkCancelingReasonId == id);
                if(existInOrderCanseling)
                {
                    return new RepRes<TMeasurementUnit>(Message.MeasurementUnitCantDelete,false,null);
                }
                var data = await _context.TMeasurementUnit.FindAsync(id);
                _context.TMeasurementUnit.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TMeasurementUnit>(Message.Successfull,true,null);
            }
            catch (System.Exception)
            {
                return new RepRes<TMeasurementUnit>(Message.MeasurementUnitDelete,false,null);
            }
        }

        public async Task<TMeasurementUnit> MeasurementUnitEdit(TMeasurementUnit MeasurementUnit)
        {
            try
            {
                var data = await _context.TMeasurementUnit.FindAsync(MeasurementUnit.UnitId);
                MeasurementUnit.UnitTitle = JsonExtensions.JsonEdit(MeasurementUnit.UnitTitle, data.UnitTitle, header);
                _context.Entry(data).CurrentValues.SetValues(MeasurementUnit);
                await _context.SaveChangesAsync();
                MeasurementUnit.UnitTitle = JsonExtensions.JsonGet(MeasurementUnit.UnitTitle, header);
                return MeasurementUnit;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> MeasurementUnitExist(int id)
        {
            try
            {
                var result = await _context.TMeasurementUnit.AsNoTracking().AnyAsync(x => x.UnitId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<MeasurementUnitDto>> MeasurementUnitGetAllForm()
        {
            try
            {
                var MeasurementUnit = await _context.TMeasurementUnit
                .OrderByDescending(x => x.UnitId)
                .Select(x=> new MeasurementUnitDto(){
                    UnitId = x.UnitId,
                    UnitTitle  = JsonExtensions.JsonValue(x.UnitTitle, header.Language),
                })
                .AsNoTracking().ToListAsync();
                return MeasurementUnit;
            }
            catch (System.Exception)
            {
                return null;
            }
        }        
        
        public async Task<List<MeasurementUnitDto>> MeasurementUnitGetAll(PaginationDto pagination)
        {
            try
            {
                var MeasurementUnit = await _context.TMeasurementUnit
                .Where(x => string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.UnitTitle, header.Language).Contains(pagination.Filter)))
                .OrderByDescending(x => x.UnitId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x=> new MeasurementUnitDto(){
                    UnitId = x.UnitId,
                    UnitTitle  = JsonExtensions.JsonValue(x.UnitTitle, header.Language),
                })  
                .AsNoTracking().ToListAsync();
                return MeasurementUnit;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> MeasurementUnitGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TMeasurementUnit
                .AsNoTracking()
                .CountAsync(x => string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.UnitTitle, header.Language).Contains(pagination.Filter)));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }
    }
}