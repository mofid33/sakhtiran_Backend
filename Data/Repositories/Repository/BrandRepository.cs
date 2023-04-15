using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class BrandRepository : IBrandRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public BrandRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<TBrand> BrandAdd(TBrand brand)
        {
            try
            {
                brand.BrandTitle = JsonExtensions.JsonAdd(brand.BrandTitle, header);
                brand.Description = JsonExtensions.JsonAdd(brand.Description, header);
                await _context.TBrand.AddAsync(brand);
                await _context.SaveChangesAsync();
                brand.BrandTitle = JsonExtensions.JsonGet(brand.BrandTitle, header);
                brand.Description = JsonExtensions.JsonGet(brand.Description, header);
                return brand;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TBrand>> BrandDelete(int id)
        {
            try
            {
                var data = await _context.TBrand.Include(x => x.TCategoryBrand).FirstOrDefaultAsync(x => x.BrandId == id);
                if (data == null)
                {
                    return new RepRes<TBrand>(Message.BrandNotFoundById, false, null);
                }
                var hasRelation = await _context.TGoods.AsNoTracking().AnyAsync(x => x.FkBrandId == id);
                if (hasRelation)
                {
                    return new RepRes<TBrand>(Message.BrandCantDelete, false, null);
                }
                _context.RemoveRange(data.TCategoryBrand);
                await _context.SaveChangesAsync();
                _context.TBrand.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TBrand>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TBrand> BrandEdit(TBrand brand)
        {
            try
            {
                var data = await _context.TBrand.Include(x => x.TCategoryBrand).FirstOrDefaultAsync(x => x.BrandId == brand.BrandId);
                _context.RemoveRange(data.TCategoryBrand);
                brand.BrandTitle = JsonExtensions.JsonEdit(brand.BrandTitle, data.BrandTitle, header);
                brand.Description = JsonExtensions.JsonEdit(brand.Description, data.Description, header);
                _context.Entry(data).CurrentValues.SetValues(brand);

                await _context.SaveChangesAsync();
                var newBrandCats = brand.TCategoryBrand;
                foreach (var item in newBrandCats)
                {
                    item.FkBrandId = brand.BrandId;
                    item.BrandCategoryId = 0;
                }
                await _context.TCategoryBrand.AddRangeAsync(newBrandCats);
                await _context.SaveChangesAsync();
                brand.TCategoryBrand = newBrandCats;
                brand.BrandTitle = JsonExtensions.JsonGet(brand.BrandTitle, header);
                brand.Description = JsonExtensions.JsonGet(brand.Description, header);
                return brand;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> BrandExist(int id)
        {
            try
            {
                var result = await _context.TBrand.AsNoTracking().AnyAsync(x => x.BrandId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

         public async Task<bool> BrandExistWithTitle(string title)
        {
            try
            {
                var result = await _context.TBrand.AsNoTracking().AnyAsync(x => JsonExtensions.JsonValue(x.BrandTitle, header.Language) == title);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<BrandDto>> BrandGetAll(PaginationDto pagination)
        {
            try
            {
                if (pagination.shopId != 0)
                {
                    return await _context.TBrand
                    .Include(x => x.TCategoryBrand)
                    .ThenInclude(c => c.FkCategory)
                    .Where(x =>
                    (pagination.ChildIds.Count > 0 ? x.TCategoryBrand.Any(t => pagination.ChildIds.Contains(t.FkCategoryId)) : true) &&
                    (pagination.Id != 0 ? x.TCategoryBrand.Any(t => t.FkCategoryId == pagination.Id) : true) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(pagination.Filter))) &&
                    x.IsAccepted == true)
                    .OrderBy(x => x.BrandTitle)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new BrandDto()
                    {
                        BrandId = x.BrandId,
                        BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                        BrandLogoImage = x.BrandLogoImage,
                        Description = JsonExtensions.JsonValue(x.Description, header.Language),
                        IsAccepted = x.IsAccepted,
                        TCategoryBrand = x.TCategoryBrand.Select(c => new CategoryBrandDto
                        {
                            CategoryTitle = JsonExtensions.JsonValue(c.FkCategory.CategoryTitle, header.Language),
                            BrandCategoryId = c.BrandCategoryId,
                            FkCategoryId = c.FkCategoryId,
                            FkBrandId = c.FkBrandId
                        }).ToList()
                    })
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    return await _context.TBrand
                    .Include(b => b.TCategoryBrand)
                    .ThenInclude(c => c.FkCategory)
                    .Where(x => (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(pagination.Filter))) &&
                    (pagination.Id != 0 ? x.TCategoryBrand.Any(t => t.FkCategoryId == pagination.Id) : true) &&
                    (pagination.ActiveNumber == 1 ? x.IsAccepted == true :
                    (pagination.ActiveNumber == 0 ? x.IsAccepted == false :
                    (pagination.ActiveNumber == -1 ? x.IsAccepted == null :
                    true))
                    )

                    )
                    .OrderBy(x => x.BrandTitle)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new BrandDto()
                    {
                        BrandId = x.BrandId,
                        BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                        BrandLogoImage = x.BrandLogoImage,
                        Description = JsonExtensions.JsonValue(x.Description, header.Language),
                        IsAccepted = x.IsAccepted,
                        TCategoryBrand = x.TCategoryBrand.Select(c => new CategoryBrandDto
                        {
                            CategoryTitle = JsonExtensions.JsonValue(c.FkCategory.CategoryTitle, header.Language),
                            BrandCategoryId = c.BrandCategoryId,
                            FkCategoryId = c.FkCategoryId,
                            FkBrandId = c.FkBrandId
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

        public async Task<int> BrandGetAllCount(PaginationDto pagination)
        {
            try
            {
                if (pagination.shopId != 0)
                {
                    return await _context.TBrand
                    .Include(x => x.TCategoryBrand)
                    .AsNoTracking()
                    .CountAsync(x => (pagination.ChildIds.Count > 0 ? x.TCategoryBrand.Any(t => pagination.ChildIds.Contains(t.FkCategoryId)) : true) &&
                    (pagination.Id != 0 ? x.TCategoryBrand.Any(t => t.FkCategoryId == pagination.Id) : true) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(pagination.Filter))) &&
                    x.IsAccepted == true);

                }
                else
                {
                    return await _context.TBrand
                    .AsNoTracking()
                    .CountAsync(x => (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(pagination.Filter))) &&
                    (pagination.Id != 0 ? x.TCategoryBrand.Any(t => t.FkCategoryId == pagination.Id) : true) &&
                    (pagination.Active == null ? true : x.IsAccepted == pagination.Active));
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> ChangeAccept(List<AcceptNullDto> accept)
        {
            try
            {

                var brandIds = new List<int>();
                foreach (var item in accept)
                {
                    brandIds.Add(item.Id);
                }
                var data = await _context.TBrand.Where(x => brandIds.Contains(x.BrandId)).ToListAsync();
                for (int i = 0; i < data.Count; i++)
                {
                    if (accept[0].Accept == 0)
                    {
                        data[i].IsAccepted = false;
                    }
                    else if (accept[0].Accept == 1)
                    {
                        data[i].IsAccepted = true;
                    }
                    else
                    {
                        data[i].IsAccepted = null;
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

        public async Task<List<WebsiteBrandDto>> GetBrandForWebsite(List<int> catIds)
        {
            try
            {
                var brands = await _context.TBrand
                .OrderBy(x => x.BrandTitle)
                .Include(x => x.TCategoryBrand)
                .Include(x => x.TGoods)
                .Where(x => x.IsAccepted == true && x.TGoods.Any(g => g.IsAccepted == true && g.ToBeDisplayed == true) &&
                (catIds.Count > 0 ? x.TCategoryBrand.Any(t => catIds.Contains(t.FkCategoryId)) : true)
                )
                .OrderByDescending(c => c.TGoods.Count(g => g.IsAccepted == true && g.ToBeDisplayed == true))
                .Select(x => new WebsiteBrandDto()
                {
                    BrandId = x.BrandId,
                    BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                    BrandLogoImage = x.BrandLogoImage,
                    GoodsCount = x.TGoods.Count(g => g.IsAccepted == true && g.ToBeDisplayed == true)
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

        public async Task<List<WebsiteBrandDto>> GetBrandForWebsiteWithFillter(PaginationBrandDto pagination, List<int> catIds)
        {
            try
            {
                if (pagination.PageNumber == 0)
                {
                    return await _context.TBrand
                       .Include(x => x.TCategoryBrand)
                       .Include(x => x.TGoods)
                       .ThenInclude(b => b.TGoodsProvider)
                       .Where(x => pagination.BrandIds.Contains(x.BrandId)
                        && x.TGoods.Any(g => g.IsAccepted == true && g.ToBeDisplayed == true
                        && g.TGoodsProvider.Any(b => b.ToBeDisplayed && b.IsAccepted) &&
                        (catIds.Contains(g.FkCategoryId))))
                       .Select(x => new WebsiteBrandDto()
                       {
                           BrandId = x.BrandId,
                           BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                           BrandLogoImage = x.BrandLogoImage,
                           GoodsCount = x.TGoods.Count()
                       })
                       .AsNoTracking().ToListAsync();
                }
                var brands = await _context.TBrand
                .OrderBy(x => x.BrandTitle)
                .Include(x => x.TCategoryBrand)
                .Include(x => x.TGoods)
                .ThenInclude(b => b.TGoodsProvider)
                .Where(x => x.IsAccepted == true
                && x.TGoods.Any(g => g.IsAccepted == true && g.ToBeDisplayed == true
                && g.TGoodsProvider.Any(b => b.ToBeDisplayed && b.IsAccepted) &&
                (catIds.Contains(g.FkCategoryId))) &&
                (catIds.Count > 0 ? x.TCategoryBrand.Any(t => catIds.Contains(t.FkCategoryId)) : true) &&
                (string.IsNullOrWhiteSpace(pagination.Filter) ? true :
                 (JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(pagination.Filter)))
                )
                .OrderByDescending(c => c.TGoods.Count(g => g.IsAccepted == true && g.ToBeDisplayed == true))
                .Select(x => new WebsiteBrandDto()
                {
                    BrandId = x.BrandId,
                    BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                    BrandLogoImage = x.BrandLogoImage,
                    GoodsCount = x.TGoods.Count()
                })
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .AsNoTracking()
                .ToListAsync();


                /// اصافه کردن دیتا بایند شده در سمت کلاینت به دراپ داون به لیست
                if (pagination.BrandIds.Count > 0 && (pagination.PageNumber == 1 || pagination.PageNumber == 0))
                {
                    var notExistDataId = pagination.BrandIds.Where(p => brands.All(p2 => p2.BrandId != p)).ToList();
                    if (notExistDataId.Count > 1 || (notExistDataId.Count == 1 && notExistDataId[0] != 0))
                    {
                        var oldData = await _context.TBrand
                        .Include(x => x.TCategoryBrand)
                        .Include(x => x.TGoods)
                        .ThenInclude(b => b.TGoodsProvider)
                        .Where(x => pagination.BrandIds.Contains(x.BrandId) &&
                        x.TGoods.Any(g => g.IsAccepted == true && g.ToBeDisplayed == true
                        && g.TGoodsProvider.Any(b => b.ToBeDisplayed && b.IsAccepted) &&
                        (catIds.Contains(g.FkCategoryId))))
                        .Select(x => new WebsiteBrandDto()
                        {
                            BrandId = x.BrandId,
                            BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                            BrandLogoImage = x.BrandLogoImage,
                            GoodsCount = x.TGoods.Count()
                        })
                        .AsNoTracking().ToListAsync();
                        if (oldData.Count > 0)
                        {
                            brands.AddRange(oldData);
                        }
                    }
                }
                if (pagination.PageNumber > 1 && pagination.BrandIds.Count > 0)
                {

                    brands = brands.Where(b => !pagination.BrandIds.Contains(b.BrandId)).ToList();
                }
                ///////////////////////

                return brands;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<BrandGetOneDto> GetBrandById(int brandId)
        {
            try
            {
                var data = await _context.TBrand
                .Include(x => x.TCategoryBrand).ThenInclude(t => t.FkCategory)
                .Select(x => new BrandGetOneDto()
                {
                    BrandId = x.BrandId,
                    BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                    BrandLogoImage = x.BrandLogoImage,
                    Description = JsonExtensions.JsonValue(x.Description, header.Language),
                    IsAccepted = x.IsAccepted,
                    TCategoryBrand = x.TCategoryBrand.Select(t => new CategoryBrandGetDto()
                    {
                        BrandCategoryId = t.BrandCategoryId,
                        FkCategoryId = t.FkCategoryId,
                        FkBrandId = t.FkBrandId,
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language)
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BrandId == brandId);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> AcceptShopBrandAdding()
        {
            try
            {
                var setting = await _context.TSetting.AsNoTracking().FirstOrDefaultAsync();
                return setting.SysAutoActiveBrand;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}