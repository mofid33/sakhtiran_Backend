using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.PupupItem;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class PupupRepository : IPupupRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }

        public PupupRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TPopupItem> PupupItemAdd(TPopupItem brand)
        {
            try
            {
                brand.Title = JsonExtensions.JsonAdd(brand.Title, header);
                brand.PopupImageUrl = JsonExtensions.JsonAdd(brand.PopupImageUrl, header);
                await _context.TPopupItem.AddAsync(brand);
                await _context.SaveChangesAsync();

                if (brand.Status)
                {
                    var data = await _context.TPopupItem.ToListAsync();

                    foreach (var item in data)
                    {
                        if (item.PopupId != brand.PopupId)
                        {
                            item.Status = false;
                        }

                    }

                    await _context.SaveChangesAsync();
                }

                brand.Title = JsonExtensions.JsonGet(brand.Title, header);
                return brand;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TPopupItem>> PupupItemDelete(int id)
        {
            try
            {
                var data = await _context.TPopupItem.FirstOrDefaultAsync(x => x.PopupId == id);
                if (data == null)
                {
                    return new RepRes<TPopupItem>(Message.PopupItemNotFoundById, false, null);
                }
                _context.TPopupItem.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TPopupItem>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TPopupItem> PupupItemEdit(TPopupItem PopupItem)
        {
            try
            {
                var data = await _context.TPopupItem.FirstOrDefaultAsync(x => x.PopupId == PopupItem.PopupId);
                PopupItem.Title = JsonExtensions.JsonEdit(PopupItem.Title, data.Title, header);
                PopupItem.PopupImageUrl = JsonExtensions.JsonEdit(PopupItem.PopupImageUrl, data.PopupImageUrl, header);
                _context.Entry(data).CurrentValues.SetValues(PopupItem);

                await _context.SaveChangesAsync();


                if (PopupItem.Status)
                {
                    var alldata = await _context.TPopupItem.ToListAsync();

                    foreach (var item in alldata)
                    {
                        if (item.PopupId != PopupItem.PopupId)
                        {
                            item.Status = false;
                        }

                    }

                    await _context.SaveChangesAsync();
                }


                return PopupItem;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> PupupItemExist(int id)
        {
            try
            {
                var result = await _context.TPopupItem.AsNoTracking().AnyAsync(x => x.PopupId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<PupupItemDto>> PupupItemGetAll(PaginationDto pagination)
        {
            try
            {
                return await _context.TPopupItem
                .OrderBy(x => x.Title)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new PupupItemDto()
                {
                    PopupId = x.PopupId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    PopupImageUrl = JsonExtensions.JsonValue(x.PopupImageUrl, header.Language),
                    FkCategoryId = x.FkCategoryId,
                    FkTDiscountPlanId = x.FkTDiscountPlanId,
                    JustNewGoods = x.JustNewGoods,
                    JustShowOnce = x.JustShowOnce,
                    Status = x.Status
                })
                .AsNoTracking().ToListAsync();

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> PupupItemGetAllCount(PaginationDto pagination)
        {
            try
            {
                return await _context.TPopupItem
                .AsNoTracking()
                .CountAsync();


            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        public async Task<PupupItemDto> GetPupupItemById(int popupId)
        {
            try
            {
                var data = await _context.TPopupItem
                .Select(x => new PupupItemDto()
                {
                    PopupId = x.PopupId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    PopupImageUrl = JsonExtensions.JsonValue(x.PopupImageUrl, header.Language),
                    FkCategoryId = x.FkCategoryId,
                    FkTDiscountPlanId = x.FkTDiscountPlanId,
                    JustNewGoods = x.JustNewGoods,
                    JustShowOnce = x.JustShowOnce,
                    Status = x.Status
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PopupId == popupId);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<PupupItemDto> GetWebsitePupup()
        {
            try
            {
                var data = await _context.TPopupItem
                .Select(x => new PupupItemDto()
                {
                    PopupId = x.PopupId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    PopupImageUrl = JsonExtensions.JsonValue(x.PopupImageUrl, header.Language),
                    FkCategoryId = x.FkCategoryId,
                    FkTDiscountPlanId = x.FkTDiscountPlanId,
                    JustNewGoods = x.JustNewGoods,
                    JustShowOnce = x.JustShowOnce,
                    Status = x.Status
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Status == true);
                return data;
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

                var data = await _context.TPopupItem.ToListAsync();

                foreach (var item in data)
                {
                    if (item.PopupId == accept.Id)
                    {
                        item.Status = accept.Accept;
                        continue;
                    }
                    if (accept.Accept)
                    {
                        item.Status = false;
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