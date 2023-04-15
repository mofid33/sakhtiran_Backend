using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using AutoMapper;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Helper;
using AutoMapper.QueryableExtensions;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.ReturningType;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Enums;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class ReturningTypeRepository : IReturningTypeRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public ReturningTypeRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TReturningAction> ReturningTypeAdd(TReturningAction ReturningType)
        {
            try
            {
                ReturningType.ReturningTypeTitle = JsonExtensions.JsonAdd(ReturningType.ReturningTypeTitle,header);
                await _context.TReturningAction.AddAsync(ReturningType);
                await _context.SaveChangesAsync();
                ReturningType.ReturningTypeTitle = JsonExtensions.JsonGet(ReturningType.ReturningTypeTitle,header);
                return ReturningType;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TReturningAction>> ReturningTypeDelete(int id)
        {
            try
            {
                var existInOrderCanseling = await _context.TOrderReturning.AsNoTracking().AnyAsync(x=>x.FkReturningActionId == id);
                if(existInOrderCanseling)
                {
                    return new RepRes<TReturningAction>(Message.ReturningTypeCantDelete,false,null);
                }
                var data = await _context.TReturningAction.FindAsync(id);
                _context.TReturningAction.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TReturningAction>(Message.Successfull,true,null);
            }
            catch (System.Exception)
            {
                return new RepRes<TReturningAction>(Message.ReturningTypeDelete,false,null);
            }
        }

        public async Task<TReturningAction> ReturningTypeEdit(TReturningAction ReturningType)
        {
            try
            {
                var data = await _context.TReturningAction.FindAsync(ReturningType.ReturningTypeId);
                ReturningType.ReturningTypeTitle = JsonExtensions.JsonEdit(ReturningType.ReturningTypeTitle, data.ReturningTypeTitle, header);
                _context.Entry(data).CurrentValues.SetValues(ReturningType);
                await _context.SaveChangesAsync();
                ReturningType.ReturningTypeTitle = JsonExtensions.JsonGet(ReturningType.ReturningTypeTitle, header);
                return ReturningType;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ReturningTypeExist(int id)
        {
            try
            {
                var result = await _context.TReturningAction.AsNoTracking().AnyAsync(x => x.ReturningTypeId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<ReturningTypeDto>> ReturningTypeGetAllForm()
        {
            try
            {
                var ReturningType = await _context.TReturningAction
                .OrderByDescending(x => x.ReturningTypeId)
                .Select(x=> new ReturningTypeDto(){
                    ReturningTypeId = x.ReturningTypeId,
                    ReturningTypeTitle  = JsonExtensions.JsonValue(x.ReturningTypeTitle, header.Language),
                })
                .AsNoTracking().ToListAsync();
                return ReturningType;
            }
            catch (System.Exception)
            {
                return null;
            }
        }        
        
        public async Task<List<ReturningTypeDto>> ReturningTypeGetAll(PaginationDto pagination)
        {
            try
            {
                var ReturningType = await _context.TReturningAction
                .Where(x => string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.ReturningTypeTitle, header.Language).Contains(pagination.Filter)))
                .OrderByDescending(x => x.ReturningTypeId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x=> new ReturningTypeDto(){
                    ReturningTypeId = x.ReturningTypeId,
                    ReturningTypeTitle  = JsonExtensions.JsonValue(x.ReturningTypeTitle, header.Language),
                })  
                .AsNoTracking().ToListAsync();
                return ReturningType;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> ReturningTypeGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TReturningAction
                .AsNoTracking()
                .CountAsync(x => string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.ReturningTypeTitle, header.Language).Contains(pagination.Filter)));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

















    }
}