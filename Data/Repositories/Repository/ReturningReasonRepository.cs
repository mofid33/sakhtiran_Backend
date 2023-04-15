using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.ReturningReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class ReturningReasonRepository : IReturningReasonRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public ReturningReasonRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TReturningReason> ReturningReasonAdd(TReturningReason ReturningReason)
        {
            try
            {
                ReturningReason.ReasonTitle = JsonExtensions.JsonAdd(ReturningReason.ReasonTitle,header);
                ReturningReason.ReturnCondition = JsonExtensions.JsonAdd(ReturningReason.ReturnCondition,header);
                ReturningReason.Status = true;
                await _context.TReturningReason.AddAsync(ReturningReason);
                await _context.SaveChangesAsync();
                ReturningReason.ReasonTitle = JsonExtensions.JsonGet(ReturningReason.ReasonTitle,header);
                ReturningReason.ReturnCondition = JsonExtensions.JsonGet(ReturningReason.ReturnCondition,header);
                return ReturningReason;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TReturningReason>> ReturningReasonDelete(int id)
        {
            try
            {
                var existInOrderCanseling = await _context.TOrderReturning.AsNoTracking().AnyAsync(x=>x.FkReturningReasonId == id);
                if(existInOrderCanseling)
                {
                    return new RepRes<TReturningReason>(Message.ReturningReasonCantDelete,false,null);
                }
                var data = await _context.TReturningReason.FindAsync(id);
                _context.TReturningReason.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TReturningReason>(Message.Successfull,true,null);
            }
            catch (System.Exception)
            {
                return new RepRes<TReturningReason>(Message.ReturningReasonDelete,false,null);
            }
        }

        public async Task<TReturningReason> ReturningReasonEdit(TReturningReason ReturningReason)
        {
            try
            {
                var data = await _context.TReturningReason.FindAsync(ReturningReason.ReasonId);
                ReturningReason.ReasonTitle = JsonExtensions.JsonEdit(ReturningReason.ReasonTitle, data.ReasonTitle, header);
                ReturningReason.ReturnCondition = JsonExtensions.JsonEdit(ReturningReason.ReturnCondition, data.ReturnCondition, header);
                _context.Entry(data).CurrentValues.SetValues(ReturningReason);
                _context.Entry(data).Property(x => x.Status).IsModified = false;
                await _context.SaveChangesAsync();
                ReturningReason.ReasonTitle = JsonExtensions.JsonGet(ReturningReason.ReasonTitle, header);
                ReturningReason.ReturnCondition = JsonExtensions.JsonGet(ReturningReason.ReturnCondition, header);
                return ReturningReason;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ReturningReasonExist(int id)
        {
            try
            {
                var result = await _context.TReturningReason.AsNoTracking().AnyAsync(x => x.ReasonId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<ReturningReasonDto>> ReturningReasonGetAllForm()
        {
            try
            {
                var ReturningReason = await _context.TReturningReason
                .OrderByDescending(x => x.ReasonId)
                .Select(x=> new ReturningReasonDto(){
                    ReasonId = x.ReasonId,
                    ReasonTitle  = JsonExtensions.JsonValue(x.ReasonTitle, header.Language),
                    ReturnCondition  = JsonExtensions.JsonValue(x.ReturnCondition, header.Language),
                    Status = x.Status
                })
                .AsNoTracking().ToListAsync();
                return ReturningReason;
            }
            catch (System.Exception)
            {
                return null;
            }
        }        
        
        public async Task<List<ReturningReasonDto>> ReturningReasonGetAll(PaginationDto pagination)
        {
            try
            {
                var ReturningReason = await _context.TReturningReason
                .Where(x => string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.ReasonTitle, header.Language).Contains(pagination.Filter)))
                .OrderByDescending(x => x.ReasonId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x=> new ReturningReasonDto(){
                    ReasonId = x.ReasonId,
                    ReasonTitle  = JsonExtensions.JsonValue(x.ReasonTitle, header.Language),
                    ReturnCondition  = JsonExtensions.JsonValue(x.ReturnCondition, header.Language),
                    Status = x.Status
                })
                .AsNoTracking().ToListAsync();
                return ReturningReason;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        
        public async Task<bool> ChangeAccept(AcceptDto accept)
        {
            try
            {
                var data = await _context.TReturningReason.FindAsync(accept.Id);
                data.Status = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<int> ReturningReasonGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TReturningReason
                .AsNoTracking()
                .CountAsync(x => string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.ReasonTitle, header.Language).Contains(pagination.Filter)));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }
    }
}