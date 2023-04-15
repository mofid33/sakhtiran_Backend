using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class GuaranteeRepository : IGuaranteeRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public GuaranteeRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TGuarantee> GuaranteeAdd(TGuarantee Guarantee)
        {
            try
            {
                Guarantee.GuaranteeTitle = JsonExtensions.JsonAdd(Guarantee.GuaranteeTitle, header);
                Guarantee.Description = JsonExtensions.JsonAdd(Guarantee.Description, header);
                await _context.TGuarantee.AddAsync(Guarantee);
                await _context.SaveChangesAsync();
                Guarantee.GuaranteeTitle = JsonExtensions.JsonGet(Guarantee.GuaranteeTitle, header);
                Guarantee.Description = JsonExtensions.JsonGet(Guarantee.Description, header);
                return Guarantee;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TGuarantee>> GuaranteeDelete(int id)
        {
            try
            {
                var data = await _context.TGuarantee.Include(x => x.TCategoryGuarantee).FirstOrDefaultAsync(x => x.GuaranteeId == id);
                if (data == null)
                {
                    return new RepRes<TGuarantee>(Message.GuaranteeNotFoundById, false, null);
                }
                var hasRelation = await _context.TGoodsProvider.AsNoTracking().AnyAsync(x => x.FkGuaranteeId == id);
                if (hasRelation)
                {
                    return new RepRes<TGuarantee>(Message.GuaranteeCantDelete, false, null);
                }
                _context.RemoveRange(data.TCategoryGuarantee);
                await _context.SaveChangesAsync();
                _context.TGuarantee.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TGuarantee>(Message.Successfull, true, null);
            }
            catch (System.Exception)
            {
                return new RepRes<TGuarantee>(Message.GuaranteeDelete, false, null);
            }
        }

        public async Task<TGuarantee> GuaranteeEdit(TGuarantee Guarantee)
        {
            try
            {
                var data = await _context.TGuarantee.Include(x => x.TCategoryGuarantee).FirstOrDefaultAsync(x => x.GuaranteeId == Guarantee.GuaranteeId);
                _context.RemoveRange(data.TCategoryGuarantee);
                Guarantee.GuaranteeTitle = JsonExtensions.JsonEdit(Guarantee.GuaranteeTitle, data.GuaranteeTitle, header);
                Guarantee.Description = JsonExtensions.JsonEdit(Guarantee.Description, data.Description, header);
                _context.Entry(data).CurrentValues.SetValues(Guarantee);
                await _context.SaveChangesAsync();
                var newGuaranteeCats = Guarantee.TCategoryGuarantee;
                foreach (var item in newGuaranteeCats)
                {
                    item.FkGuaranteeId = Guarantee.GuaranteeId;
                    item.CategoryGuaranteeId = 0;
                }
                await _context.TCategoryGuarantee.AddRangeAsync(newGuaranteeCats);
                await _context.SaveChangesAsync();
                Guarantee.TCategoryGuarantee = newGuaranteeCats;
                Guarantee.GuaranteeTitle = JsonExtensions.JsonGet(Guarantee.GuaranteeTitle, header);
                Guarantee.Description = JsonExtensions.JsonGet(Guarantee.Description, header);
                return Guarantee;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> GuaranteeExist(int id)
        {
            try
            {
                var result = await _context.TGuarantee.AsNoTracking().AnyAsync(x => x.GuaranteeId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<GuaranteeDto>> GuaranteeGetAll(PaginationDto pagination)
        {
            try
            {
                if (pagination.shopId != 0)
                {
                    return await _context.TGuarantee
                    .Include(x => x.TCategoryGuarantee)
                    .ThenInclude(v => v.FkCategory)
                    .Where(x => (pagination.ChildIds.Count > 0 ? x.TCategoryGuarantee.Any(t => pagination.ChildIds.Contains(t.FkCategoryId)) : true) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language).Contains(pagination.Filter))) &&
                    x.IsAccepted == true)
                    .OrderBy(x => x.GuaranteeTitle)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new GuaranteeDto()
                    {
                        GuaranteeId = x.GuaranteeId,
                        GuaranteeTitle = JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language),
                        IsAccepted = x.IsAccepted,
                        Description = JsonExtensions.JsonValue(x.Description, header.Language),
                        TCategoryGuarantee = x.TCategoryGuarantee.Select(g => new CategoryGuaranteeDto {
                          CategoryGuaranteeId = g.CategoryGuaranteeId,
                          CategoryTitle = JsonExtensions.JsonValue(g.FkCategory.CategoryTitle, header.Language),
                          FkCategoryId = g.FkCategoryId,
                          FkGuaranteeId = g.FkGuaranteeId
                        }).ToList()
                    })
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    return await _context.TGuarantee
                    .Include(x => x.TCategoryGuarantee)
                    .ThenInclude(v => v.FkCategory)
                    .Where(x => (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language).Contains(pagination.Filter))) &&
                    (pagination.ActiveNumber == 1 ? x.IsAccepted == true : 
                    (pagination.ActiveNumber == 0 ? x.IsAccepted == false : 
                    (pagination.ActiveNumber == -1 ? x.IsAccepted == null : 
                    true))
                    ))
                    .OrderBy(x => x.GuaranteeTitle)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new GuaranteeDto()
                    {
                        GuaranteeId = x.GuaranteeId,
                        GuaranteeTitle = JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language),
                        IsAccepted = x.IsAccepted,
                        Description = JsonExtensions.JsonValue(x.Description, header.Language),
                        TCategoryGuarantee = x.TCategoryGuarantee.Select(g => new CategoryGuaranteeDto {
                          CategoryGuaranteeId = g.CategoryGuaranteeId,
                          CategoryTitle = JsonExtensions.JsonValue(g.FkCategory.CategoryTitle, header.Language),
                          FkCategoryId = g.FkCategoryId,
                          FkGuaranteeId = g.FkGuaranteeId
                        }).ToList()
                    })
                    .AsNoTracking().ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GuaranteeGetAllCount(PaginationDto pagination)
        {
            if (pagination.shopId != 0)
            {
                return await _context.TGuarantee
                .Include(x => x.TCategoryGuarantee)
                .AsNoTracking()
                .CountAsync(x => (pagination.ChildIds.Count > 0 ? x.TCategoryGuarantee.Any(t => pagination.ChildIds.Contains(t.FkCategoryId)) : true) &&
                (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language).Contains(pagination.Filter))) &&
                x.IsAccepted == true);

            }
            else
            {
                return await _context.TGuarantee
                .AsNoTracking()
                .CountAsync(x => (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language).Contains(pagination.Filter))) &&
                (pagination.Active == true ? x.IsAccepted == null : x.IsAccepted != null));
            }
        }

        public async Task<bool> ChangeAccept(AcceptNullDto accept)
        {
            try
            {
                var data = await _context.TGuarantee.FindAsync(accept.Id);
                bool? accepted = null;
                if(accept.Accept == 0) {
                    accepted = false;
                }
                else if(accept.Accept == 1) {
                    accepted = true;
                } else {
                    accepted = null;
                }

                data.IsAccepted = accepted;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<GuaranteeGetOneDto> GetGuaranteeById(int guaranteeId)
        {
            try
            {
                var data = await _context.TGuarantee
                .Include(x => x.TCategoryGuarantee).ThenInclude(t => t.FkCategory)
                .Select(x => new GuaranteeGetOneDto()
                {
                    GuaranteeId = x.GuaranteeId,
                    GuaranteeTitle = JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language),
                    Description = JsonExtensions.JsonValue(x.Description, header.Language),
                    IsAccepted = x.IsAccepted,
                    TCategoryGuarantee = x.TCategoryGuarantee.Select(t => new CategoryGuaranteeGetDto()
                    {
                        CategoryGuaranteeId = t.CategoryGuaranteeId,
                        FkCategoryId = t.FkCategoryId,
                        FkGuaranteeId = t.FkGuaranteeId,
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language)
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GuaranteeId == guaranteeId);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GuaranteeFormDto>> GetGuaranteeForWebsite(List<int> catIds)
        {
            try
            {
                var brands = await _context.TGuarantee
                .Include(x => x.TCategoryGuarantee)
                .Where(x => x.IsAccepted == true &&
                (catIds.Count > 0 ? x.TCategoryGuarantee.Any(t => catIds.Contains(t.FkCategoryId)) : true)
                )
                .Select(x => new GuaranteeFormDto()
                {
                    GuaranteeId = x.GuaranteeId,
                    GuaranteeTitle = JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language),
                })
                .AsNoTracking()
                .ToListAsync();

                return brands;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> AcceptShopGuaranteeAdding()
        {
            try
            {
                return await _context.TSetting.AsNoTracking().Select(x=>x.SysAutoActiveGuarantee).FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}