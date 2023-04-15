using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.OrderCancelingReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class OrderCancelingReasonRepository : IOrderCancelingReasonRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public OrderCancelingReasonRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TOrderCancelingReason> OrderCancelingReasonAdd(TOrderCancelingReason OrderCancelingReason)
        {
            try
            {
                OrderCancelingReason.ReasonTitle = JsonExtensions.JsonAdd(OrderCancelingReason.ReasonTitle,header);
                OrderCancelingReason.Status = true;
                await _context.TOrderCancelingReason.AddAsync(OrderCancelingReason);
                await _context.SaveChangesAsync();
                OrderCancelingReason.ReasonTitle = JsonExtensions.JsonGet(OrderCancelingReason.ReasonTitle,header);
                return OrderCancelingReason;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TOrderCancelingReason>> OrderCancelingReasonDelete(int id)
        {
            try
            {
                var existInOrderCanseling = await _context.TOrderCanceling.AsNoTracking().AnyAsync(x=>x.FkCancelingReasonId == id);
                if(existInOrderCanseling)
                {
                    return new RepRes<TOrderCancelingReason>(Message.OrderCancelingReasonCantDelete,false,null);
                }
                var data = await _context.TOrderCancelingReason.FindAsync(id);
                _context.TOrderCancelingReason.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TOrderCancelingReason>(Message.Successfull,true,null);
            }
            catch (System.Exception)
            {
                return new RepRes<TOrderCancelingReason>(Message.OrderCancelingReasonDelete,false,null);
            }
        }

        public async Task<TOrderCancelingReason> OrderCancelingReasonEdit(TOrderCancelingReason OrderCancelingReason)
        {
            try
            {
                var data = await _context.TOrderCancelingReason.FindAsync(OrderCancelingReason.ReasonId);
                OrderCancelingReason.ReasonTitle = JsonExtensions.JsonEdit(OrderCancelingReason.ReasonTitle, data.ReasonTitle, header);
                _context.Entry(data).CurrentValues.SetValues(OrderCancelingReason);
                _context.Entry(data).Property(x => x.Status).IsModified = false;
                await _context.SaveChangesAsync();
                OrderCancelingReason.ReasonTitle = JsonExtensions.JsonGet(OrderCancelingReason.ReasonTitle, header);
                return OrderCancelingReason;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> OrderCancelingReasonExist(int id)
        {
            try
            {
                var result = await _context.TOrderCancelingReason.AsNoTracking().AnyAsync(x => x.ReasonId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<OrderCancelingReasonDto>> OrderCancelingReasonGetAllForm()
        {
            try
            {
                var OrderCancelingReason = await _context.TOrderCancelingReason
                .OrderByDescending(x => x.ReasonId)
                .Select(x=> new OrderCancelingReasonDto(){
                    ReasonId = x.ReasonId,
                    ReasonTitle  = JsonExtensions.JsonValue(x.ReasonTitle, header.Language),
                    Status = x.Status
                })
                .AsNoTracking().ToListAsync();
                return OrderCancelingReason;
            }
            catch (System.Exception)
            {
                return null;
            }
        }        
        
        public async Task<List<OrderCancelingReasonDto>> OrderCancelingReasonGetAll(PaginationDto pagination)
        {
            try
            {
                var OrderCancelingReason = await _context.TOrderCancelingReason
                .Where(x => string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.ReasonTitle, header.Language).Contains(pagination.Filter)))
                .OrderByDescending(x => x.ReasonId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x=> new OrderCancelingReasonDto(){
                    ReasonId = x.ReasonId,
                    ReasonTitle  = JsonExtensions.JsonValue(x.ReasonTitle, header.Language),
                    Status = x.Status
                })  
                .AsNoTracking().ToListAsync();
                return OrderCancelingReason;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> OrderCancelingReasonGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TOrderCancelingReason
                .AsNoTracking()
                .CountAsync(x => string.IsNullOrWhiteSpace(pagination.Filter) ? (true) : (JsonExtensions.JsonValue(x.ReasonTitle, header.Language).Contains(pagination.Filter)));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> ChangeAccept(AcceptDto accept)
        {
            try
            {
                var data = await _context.TOrderCancelingReason.FindAsync(accept.Id);
                data.Status = accept.Accept;
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