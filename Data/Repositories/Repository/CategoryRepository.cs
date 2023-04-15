using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Token;
using System.Linq.Expressions;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }

        public CategoryRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
            token = new TokenParseDto(httpContextAccessor);
        }
        public async Task<RepRes<TCategory>> CategoryAdd(TCategory category)
        {
            try
            {
                //check  category title duplicate
                if (await _context.TCategory.AsNoTracking().AnyAsync(x => JsonExtensions.JsonValue(x.CategoryTitle, header.Language) == category.CategoryTitle && x.FkParentId == null))
                {
                    return new RepRes<TCategory>(Message.CategoryNameDupplicate, false, null);
                }

                // if (category.CommissionFee != null)
                // {
                //     var rate = 1.0;
                //     if (  header.CurrencyNum != CurrencyEnum.TMN)
                //     {
                //         var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                //         rate = currency == null ? 1.0 : currency.RatesAgainstOneDollar;
                //     }
                //     category.CommissionFee = category.CommissionFee / rate;
                // }

                category.CategoryTitle = JsonExtensions.JsonAdd(category.CategoryTitle, header);
                category.PageTitle = JsonExtensions.JsonAdd(category.PageTitle, header);
                category.MetaTitle = JsonExtensions.JsonAdd(category.MetaTitle, header);
                category.MetaKeywords = JsonExtensions.JsonAdd(category.MetaKeywords, header);
                category.MetaDescription = JsonExtensions.JsonAdd(category.MetaDescription, header);
                var count = await _context.TCategory.AsNoTracking().CountAsync(x => x.FkParentId == category.FkParentId);
                category.PriorityNumber = count + 1;
                await _context.TCategory.AddAsync(category);
                await _context.SaveChangesAsync();

                if (category.FkParentId == null)
                {
                    category.CategoryPath = "/" + category.CategoryId + "/";
                }
                else
                {
                    var parent = await _context.TCategory.Select(x => new { x.CategoryId, x.CategoryPath }).FirstOrDefaultAsync(x => x.CategoryId == category.FkParentId);
                    category.CategoryPath = parent.CategoryPath + category.CategoryId + "/";
                }
                await _context.SaveChangesAsync();

                category.CategoryTitle = JsonExtensions.JsonGet(category.CategoryTitle, header);
                category.PageTitle = JsonExtensions.JsonGet(category.PageTitle, header);
                category.MetaTitle = JsonExtensions.JsonGet(category.MetaTitle, header);
                category.MetaKeywords = JsonExtensions.JsonGet(category.MetaKeywords, header);
                category.MetaDescription = JsonExtensions.JsonGet(category.MetaDescription, header);
                return new RepRes<TCategory>(Message.Successfull, true, category);
            }
            catch (System.Exception)
            {
                return new RepRes<TCategory>(Message.CategoryAdding, false, null);
            }
        }

        public async Task<List<CategoryGetDto>> GetForWebsite()
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TCategory
                    .Include(x => x.InverseFkParent)
                    .Include(x => x.TGoods)
                    .Select(x => new CategoryGetDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        FkParentId = x.FkParentId,
                        PriorityNumber = x.PriorityNumber,
                        HasCHild = x.InverseFkParent.Any(),
                        ToBeDisplayed = x.ToBeDisplayed,
                        IsActive = x.IsActive,
                        CategoryPath = x.CategoryPath,
                        ProductCount = x.TGoods.Count()

                    }).ToListAsync();
                }
                else
                {
                    var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TCategory, bool>>)(x => x.CategoryPath.Contains("/" + k + "/")));
                    return await _context.TCategory
                    .WhereAny(predicates.ToArray())
                    .Include(x => x.InverseFkParent)
                    .Include(x => x.TGoods)
                    .Select(x => new CategoryGetDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        FkParentId = x.FkParentId,
                        PriorityNumber = x.PriorityNumber,
                        HasCHild = x.InverseFkParent.Any(),
                        ToBeDisplayed = x.ToBeDisplayed,
                        IsActive = x.IsActive,
                        CategoryPath = x.CategoryPath,
                        ProductCount = x.TGoods.Count()

                    }).ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> CategoryExist(int id)
        {
            try
            {
                var result = await _context.TCategory.AsNoTracking().AnyAsync(x => x.CategoryId == id);
                return result;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<RepRes<bool>> CategoryEdit(TCategory category)
        {
            try
            {

                //check  category title duplicate
                if (await _context.TCategory.AsNoTracking().AnyAsync(x => x.CategoryId != category.CategoryId && JsonExtensions.JsonValue(x.CategoryTitle, header.Language) == category.CategoryTitle && x.FkParentId == null))
                {
                    return new RepRes<bool>(Message.CategoryNameDupplicate, false, false);
                }

                var data = await _context.TCategory.FindAsync(category.CategoryId);

                if (data.FkParentId == null && category.FkParentId != null)
                {
                    return new RepRes<bool>(Message.CategoryInFirstLevelCantChangeLocation, false, false);
                }

                // if (category.CommissionFee != null)
                // {
                //     var rate = 1.0;
                //     if (  header.CurrencyNum != CurrencyEnum.TMN)
                //     {
                //         var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                //         rate = currency == null ? 1.0 : currency.RatesAgainstOneDollar;
                //     }
                //     category.CommissionFee = category.CommissionFee / rate;
                // }

                category.CategoryTitle = JsonExtensions.JsonEdit(category.CategoryTitle, data.CategoryTitle, header);
                category.PageTitle = JsonExtensions.JsonEdit(category.PageTitle, data.PageTitle, header);
                category.MetaDescription = JsonExtensions.JsonEdit(category.MetaDescription, data.MetaDescription, header);
                category.MetaKeywords = JsonExtensions.JsonEdit(category.MetaKeywords, data.MetaKeywords, header);
                category.MetaTitle = JsonExtensions.JsonEdit(category.MetaTitle, data.MetaTitle, header);
                if (data.FkParentId != category.FkParentId)
                {
                    var allOldData = await _context.TCategory.Where(x => x.PriorityNumber > data.PriorityNumber && x.FkParentId == data.FkParentId).ToListAsync();
                    foreach (var item in allOldData)
                    {
                        item.PriorityNumber = item.PriorityNumber - 1;
                    }
                    var count = await _context.TCategory.AsNoTracking().CountAsync(x => x.FkParentId == category.FkParentId);
                    data.PriorityNumber = count + 1;
                }
                if (category.FkParentId == null)
                {
                    category.CategoryPath = "/" + category.CategoryId + "/";
                }
                else
                {
                    var parent = await _context.TCategory.Select(x => new { x.CategoryId, x.CategoryPath }).FirstOrDefaultAsync(x => x.CategoryId == category.FkParentId);
                    category.CategoryPath = parent.CategoryPath + category.CategoryId + "/";
                }
                _context.Entry(data).CurrentValues.SetValues(category);

                await _context.SaveChangesAsync();




                // category.CategoryTitle = JsonExtensions.JsonGet(category.CategoryTitle, header);
                // category.PageTitle = JsonExtensions.JsonGet(category.PageTitle, header);
                // category.MetaTitle = JsonExtensions.JsonGet(category.MetaTitle, header);
                // category.MetaKeywords = JsonExtensions.JsonGet(category.MetaKeywords, header);
                // category.MetaDescription = JsonExtensions.JsonGet(category.MetaDescription, header);

                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.CategoryEditing, false, false);
            }
        }

        public async Task<List<CategoryTreeViewDto>> CategoryGetOne(int parentId)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TCategory
                    .Where(x => parentId == 0 ? x.FkParentId == null : x.FkParentId == parentId)
                    .Include(x => x.InverseFkParent)
                    .Include(x => x.TGoods)
                    .Include(x => x.TCategorySpecificationGroup)
                    .OrderBy(c => c.PriorityNumber)
                    .Select(x => new CategoryTreeViewDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        ProductCount = x.TGoods.Count(),
                        FkParentId = x.FkParentId,
                        PriorityNumber = x.PriorityNumber,
                        hasCHild = x.InverseFkParent.Any(),
                        ToBeDisplayed = x.ToBeDisplayed,
                        IsActive = x.IsActive,
                        CategoryPath = x.CategoryPath,
                        HaveWebPage = x.HaveWebPage,
                        HaveSpecificationGroup = x.TCategorySpecificationGroup.Any()
                    }).AsNoTracking().ToListAsync();
                }
                else
                {
                    var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TCategory, bool>>)(x => x.CategoryPath.Contains("/" + k + "/")));
                    return await _context.TCategory
                    .WhereAny(predicates.ToArray())
                    .Where(x => parentId == 0 ? x.FkParentId == null : x.FkParentId == parentId)
                    .Include(x => x.InverseFkParent)
                    .Include(x => x.TGoods)
                    .OrderBy(c => c.PriorityNumber)
                    .Select(x => new CategoryTreeViewDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        FkParentId = x.FkParentId,
                        PriorityNumber = x.PriorityNumber,
                        hasCHild = x.InverseFkParent.Any(),
                        ToBeDisplayed = x.ToBeDisplayed,
                        IsActive = x.IsActive,
                        CategoryPath = x.CategoryPath,
                        HaveWebPage = x.HaveWebPage,
                        ProductCount = x.TGoods.Count()
                    }).AsNoTracking().ToListAsync();
                }

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
                var categoriesId = new List<int>();
                foreach (var item in accept)
                {
                    categoriesId.Add(item.Id);
                }
                var data = await _context.TCategory.Where(x => categoriesId.Contains(x.CategoryId)).ToArrayAsync();
                for (int i = 0; i < data.Length; i++)
                {
                    if (accept[0].Accept == false)
                    {
                        // var catids = Extentions.GetParentIds(data[i].CategoryPath);
                        // var goods = await _context.TGoods.Where(x => catids.Contains(x.FkCategoryId)).ToListAsync();
                        // foreach (var item in goods)
                        // {
                        //     item.ToBeDisplayed = false;
                        // }
                        data[i].IsActive = false;
                    }
                    else
                    {
                        data[i].IsActive = true;
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


        public async Task<bool> ChangeDisplay(AcceptDto accept)
        {
            try
            {
                var data = await _context.TCategory.FindAsync(accept.Id);
                data.ToBeDisplayed = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<RepRes<TCategory>> CategoryDelete(int id)
        {
            try
            {
                var hasChild = await _context.TCategory.AsNoTracking().AnyAsync(x => x.FkParentId == id);
                if (hasChild)
                {
                    return new RepRes<TCategory>(Message.CategoryHasChildCantDelete, false, null);
                }
                var hasRWithSpecification = await _context.TCategorySpecification.AsNoTracking().AnyAsync(x => x.FkCategoryId == id);
                if (hasRWithSpecification)
                {
                    return new RepRes<TCategory>(Message.CategoryHasSpecCantDelete, false, null);
                }
                var hasRWithSpecificationGroup = await _context.TCategorySpecificationGroup.AsNoTracking().AnyAsync(x => x.FkCategoryId == id);
                if (hasRWithSpecification)
                {
                    return new RepRes<TCategory>(Message.CategoryHasSpecGroupCantDelete, false, null);
                }
                var hasRWithGoods = await _context.TGoods.AsNoTracking().AnyAsync(x => x.FkCategoryId == id);
                if (hasRWithGoods)
                {
                    return new RepRes<TCategory>(Message.CategoryHasGoodsCantDelete, false, null);
                }
                var hasRWithSurveyQuestions = await _context.TGoodsSurveyQuestions.AsNoTracking().AnyAsync(x => x.FkCategoryId == id);
                if (hasRWithSurveyQuestions)
                {
                    return new RepRes<TCategory>(Message.CategoryHasSurveyQuestionsCantDelete, false, null);
                }
                var data = await _context.TCategory.FindAsync(id);
                var otherChild = await _context.TCategory.Where(x => x.FkParentId == data.FkParentId && x.PriorityNumber > data.PriorityNumber).ToListAsync();
                foreach (var item in otherChild)
                {
                    item.PriorityNumber = item.PriorityNumber - 1;
                }
                _context.TCategory.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TCategory>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return new RepRes<TCategory>(Message.CategoryDelete, false, null);
            }
        }

        public async Task<CategoryGetDto> GetCategoryById(int CategoryId)
        {
            try
            {
                var child = await _context.TCategory.FirstOrDefaultAsync(t => t.CategoryId == CategoryId);
                return await _context.TCategory.Include(x => x.InverseFkParent)
                .Select(x => new CategoryGetDto()
                {
                    CategoryId = x.CategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                    IconUrl = x.IconUrl,
                    ImageUrl = x.ImageUrl,
                    FkParentId = x.FkParentId,
                    PriorityNumber = x.PriorityNumber,
                    HasCHild = x.InverseFkParent.Any(),
                    ToBeDisplayed = x.ToBeDisplayed,
                    IsActive = x.IsActive,
                    CategoryPath = x.CategoryPath,
                    HaveWebPage = x.HaveWebPage
                }).FirstOrDefaultAsync(x => child.FkParentId == null ? (x.CategoryId == child.CategoryId) : (x.CategoryId == child.FkParentId));
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<int>> GetCategoriesChilds(int parentId)
        {
            try
            {
                return await _context.TCategory.Where(x => x.CategoryPath.Contains("/" + parentId + "/"))
                .Select(x => x.CategoryId).Distinct().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<List<int>> GetCategoriesDirectChilds(int parentId)
        {
            try
            {
                return await _context.TCategory.Where(x => x.FkParentId == parentId)
                .Select(x => x.CategoryId).Distinct().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<List<int>> GetCategoriesChildsTrueStatus(int parentId)
        {
            try
            {
                return await _context.TCategory.Where(x => x.IsActive == true  &&  x.CategoryPath.Contains("/" + parentId + "/"))
                .Select(x => x.CategoryId).Distinct().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<int>> GetParentsAndChildsId(int CategoryId)
        {
            try
            {
                return await _context.TCategory.Where(x => x.IsActive == true && (x.CategoryPath.Contains("/" + CategoryId + "/") ||
                (Extentions.GetParentIds(_context.TCategory.Find(CategoryId).CategoryPath).Contains(x.CategoryId))
                ))
                .Select(x => x.CategoryId).Distinct().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<TCategory>> GetCategoryByIds(List<int> CategoryIds)
        {
            try
            {
                var data = await _context.TCategory.Where(x => CategoryIds.Contains(x.CategoryId)).Distinct().ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<CategoryTreeViewFilterDto> GetChildCategoryForWebsite(int CategoryId)
        {
            try
            {
                return await _context.TCategory
                .Where(x => x.CategoryId == CategoryId && x.IsActive == true && x.ToBeDisplayed == true)
                .Include(x => x.InverseFkParent)
                .ThenInclude(c => c.InverseFkParent)
                 .Select(x => new CategoryTreeViewFilterDto()
                 {
                     CategoryId = x.CategoryId,
                     CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                     Child = x.InverseFkParent.Where(t => t.IsActive == true && t.ToBeDisplayed == true).Select(t => new CategoryTreeViewFilterDto()
                     {
                         CategoryId = t.CategoryId,
                         CategoryTitle = JsonExtensions.JsonValue(t.CategoryTitle, header.Language),
                         Child = t.InverseFkParent.Where(b => b.IsActive == true && b.ToBeDisplayed == true).Select(r => new CategoryTreeViewFilterDto()
                         {
                             CategoryId = r.CategoryId,
                             CategoryTitle = JsonExtensions.JsonValue(r.CategoryTitle, header.Language),

                         }).ToList()
                     }).ToList()
                 })
                 .AsNoTracking()
                 .FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<CategoryTreeViewDto>> GetParentCategoryForWebsite(List<int> CategoryId)
        {
            try
            {
                var category = await _context.TCategory
                .Where(x => CategoryId.Contains(x.CategoryId))
                .Select(x => new CategoryTreeViewDto()
                {
                    CategoryId = x.CategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                    IconUrl = x.IconUrl,
                    ImageUrl = x.ImageUrl,
                    FkParentId = x.FkParentId,
                    PriorityNumber = x.PriorityNumber,
                    hasCHild = x.InverseFkParent.Any(),
                    ToBeDisplayed = x.ToBeDisplayed,
                    IsActive = x.IsActive,
                    CategoryPath = x.CategoryPath,
                    HaveWebPage = x.HaveWebPage
                })
                .AsNoTracking()
                .Distinct()
                .ToListAsync();

                return category;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<int>> GetParentCatIds(int childId)
        {
            try
            {
                return await _context.TCategory.Where(x => (Extentions.GetParentIds(_context.TCategory.Find(childId).CategoryPath).Contains(x.CategoryId))
                )
                .Select(x => x.CategoryId).Distinct().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ChangePriority(ChangePriorityDto changePriority)
        {
            try
            {
                int? ParentIdDto;
                if (changePriority.ParentID == 0)
                {
                    ParentIdDto = null;
                }
                else
                {
                    ParentIdDto = changePriority.ParentID;
                }
                var data = await _context.TCategory.FirstAsync(x => x.CategoryId == changePriority.Id && x.FkParentId == ParentIdDto);
                if (data.PriorityNumber > changePriority.PriorityNumber)
                {
                    var allChildOfParent = await _context.TCategory
                    .Where(x => x.FkParentId == ParentIdDto && x.PriorityNumber < data.PriorityNumber && x.PriorityNumber >= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allChildOfParent)
                    {
                        item.PriorityNumber = item.PriorityNumber + 1;
                    }
                }
                else if (data.PriorityNumber < changePriority.PriorityNumber)
                {
                    var allChildOfParent = await _context.TCategory
                    .Where(x => x.FkParentId == ParentIdDto && x.PriorityNumber > data.PriorityNumber && x.PriorityNumber <= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allChildOfParent)
                    {
                        item.PriorityNumber = item.PriorityNumber - 1;
                    }
                }
                else
                {

                }
                data.PriorityNumber = changePriority.PriorityNumber;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<int> GetGoodsCategoryId(int goodsId)
        {
            try
            {
                var id = await _context.TGoods.Select(x => new { x.FkCategoryId, x.GoodsId }).AsNoTracking().FirstOrDefaultAsync(x => x.GoodsId == goodsId);
                if (id == null)
                {
                    return 0;
                }
                return id.FkCategoryId;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<int>> GetShopCatIds(int shopId)
        {
            try
            {
                return await _context.TShopCategory.Where(x => x.FkShopId == shopId).Select(x => x.FkCategoryId).ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ChangeReturning(AcceptDto accept)
        {
            try
            {
                var data = await _context.TCategory.FindAsync(accept.Id);
                data.ReturningAllowed = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChangeAppearInFooter(AcceptDto accept)
        {
            try
            {
                var data = await _context.TCategory.FindAsync(accept.Id);
                data.AppearInFooter = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<CategoryFormDto>> GetFooterForWebsite(int? MaxFooterItem, int? MaxItemPerFooterColumn)
        {
            try
            {
                return await _context.TCategory.Where(x => x.AppearInFooter == true)
                .Include(x => x.InverseFkParent)
                .Select(x => new CategoryFormDto()
                {
                    CategoryId = x.CategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                    IconUrl = x.IconUrl,
                    ImageUrl = x.ImageUrl,
                    HaveWebPage = x.HaveWebPage,
                    Childs = x.InverseFkParent.Select(t => new CategoryFormDto()
                    {
                        CategoryId = t.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(t.CategoryTitle, header.Language),
                        IconUrl = t.IconUrl,
                        ImageUrl = x.ImageUrl
                    }).Take(MaxItemPerFooterColumn == null ? 6 : (int)MaxItemPerFooterColumn).ToList()
                }).Take(MaxFooterItem == null ? 6 : (int)MaxFooterItem).ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<CategoryAddGetDto> CategoryGetForEdit(int categoryId)
        {
            try
            {
                return await _context.TCategory.AsNoTracking()
                .Select(x => new CategoryAddGetDto()
                {
                    CategoryId = x.CategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                    PageTitle = JsonExtensions.JsonValue(x.PageTitle, header.Language),
                    IconUrl = x.IconUrl,
                    ImageUrl = x.ImageUrl,
                    FkParentId = x.FkParentId,
                    PriorityNumber = x.PriorityNumber,
                    ToBeDisplayed = x.ToBeDisplayed,
                    IsActive = x.IsActive,
                    AppearInFooter = x.AppearInFooter,
                    CommissionFee = x.CommissionFee,
                    CategoryParentPath = x.FkParent.CategoryPath,
                    ParentTitle = JsonExtensions.JsonValue(x.FkParent.CategoryTitle, header.Language),
                    MetaDescription = JsonExtensions.JsonValue(x.MetaDescription, header.Language),
                    MetaKeywords = JsonExtensions.JsonValue(x.MetaKeywords, header.Language),
                    MetaTitle = JsonExtensions.JsonValue(x.MetaTitle, header.Language),
                    ReturningAllowed = x.ReturningAllowed,
                    HaveWebPage = x.HaveWebPage
                })
                .FirstOrDefaultAsync(x => x.CategoryId == categoryId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        // دسته بندی و برند برای وب سایت

        public async Task<List<CategoryWebGetDto>> GetCategoryAndBrandForWebsite()
        {
            try
            {
                var setting = await _context.TSetting.Select(x => new { x.SysDisplayCategoriesWithoutGoods, x.SettingId }).FirstOrDefaultAsync();

                if (setting != null && setting.SysDisplayCategoriesWithoutGoods == true)
                {
                    return await _context.TCategory
                    .Include(x => x.InverseFkParent)
                    .Where(x => x.IsActive && x.ToBeDisplayed && x.FkParentId == null)
                    .OrderBy(v => v.PriorityNumber)
                    .Select(x => new CategoryWebGetDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        FkParentId = x.FkParentId,
                        HaveWebPage = x.HaveWebPage,
                        MetaTitle = JsonExtensions.JsonValue(x.MetaTitle, header.Language),
                        MetaDescription = JsonExtensions.JsonValue(x.MetaDescription, header.Language),
                        MetaKeywords = JsonExtensions.JsonValue(x.MetaKeywords, header.Language),
                        WebsiteBrand = _context.TBrand.Include(t => t.TCategoryBrand)
                        .ThenInclude(i => i.FkCategory)
                        .Include(t => t.TGoods)
                        .Where(t => t.TCategoryBrand.Any(i => i.FkCategory.CategoryPath.Contains("/" + x.CategoryId + "/")
                                 && t.BrandLogoImage != null)
                               ).OrderByDescending(v => v.TGoods.Max(b => b.LikedCount))
                               .Take(9)
                        .Select(b => new WebsiteBrandDto
                        {
                            BrandId = b.BrandId,
                            BrandLogoImage = b.BrandLogoImage,
                            BrandTitle = JsonExtensions.JsonValue(b.BrandTitle, header.Language),
                        }).ToList(),
                        Childs = x.InverseFkParent.OrderBy(v => v.PriorityNumber).Where(t => t.IsActive && t.ToBeDisplayed).Select(b => new CategoryWebGetDto
                        {
                            CategoryId = b.CategoryId,
                            CategoryTitle = JsonExtensions.JsonValue(b.CategoryTitle, header.Language),
                            IconUrl = b.IconUrl,
                            ImageUrl = b.ImageUrl,
                            HaveWebPage = b.HaveWebPage,
                            FkParentId = b.FkParentId,
                            MetaTitle = JsonExtensions.JsonValue(b.MetaTitle, header.Language),
                            MetaDescription = JsonExtensions.JsonValue(b.MetaDescription, header.Language),
                            MetaKeywords = JsonExtensions.JsonValue(b.MetaKeywords, header.Language)
                        }).Take(10).ToList(),

                    }).ToListAsync();
                }
                else if (setting != null && setting.SysDisplayCategoriesWithoutGoods == false)
                {
                    return await _context.TCategory
                    .Include(x => x.InverseFkParent)
                    .Where(x => x.IsActive && x.FkParentId == null && x.ToBeDisplayed && (_context.TGoods.Include(t => t.FkCategory).Any(t => t.IsAccepted == true && t.ToBeDisplayed == true && t.FkCategory.CategoryPath.Contains("/" + x.CategoryId + "/") == true)))
                    .OrderByDescending(v => v.PriorityNumber)
                    .Select(x => new CategoryWebGetDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        FkParentId = x.FkParentId,
                        HaveWebPage = x.HaveWebPage,
                        MetaTitle = JsonExtensions.JsonValue(x.MetaTitle, header.Language),
                        MetaDescription = JsonExtensions.JsonValue(x.MetaDescription, header.Language),
                        MetaKeywords = JsonExtensions.JsonValue(x.MetaKeywords, header.Language),
                        WebsiteBrand = _context.TBrand.Include(t => t.TCategoryBrand)
                        .ThenInclude(i => i.FkCategory)
                        .Include(t => t.TGoods)
                        .Where(t => t.TCategoryBrand.Any(i => i.FkCategory.CategoryPath.Contains("/" + x.CategoryId + "/")
                                 && t.BrandLogoImage != null)
                               ).OrderByDescending(v => v.TGoods.Max(b => b.LikedCount))
                               .Take(9)
                        .Select(b => new WebsiteBrandDto
                        {
                            BrandId = b.BrandId,
                            BrandLogoImage = b.BrandLogoImage,
                            BrandTitle = JsonExtensions.JsonValue(b.BrandTitle, header.Language),
                        }).ToList(),
                        Childs = x.InverseFkParent.OrderByDescending(v => v.PriorityNumber).Where(b => b.IsActive && b.ToBeDisplayed && (_context.TGoods.Include(t => t.FkCategory).Any(t => t.IsAccepted == true && t.ToBeDisplayed == true && t.FkCategory.CategoryPath.Contains("/" + b.CategoryId + "/") == true))).Select(b => new CategoryWebGetDto
                        {
                            CategoryId = b.CategoryId,
                            CategoryTitle = JsonExtensions.JsonValue(b.CategoryTitle, header.Language),
                            IconUrl = b.IconUrl,
                            ImageUrl = b.ImageUrl,
                            HaveWebPage = b.HaveWebPage,
                            FkParentId = b.FkParentId,
                            MetaTitle = JsonExtensions.JsonValue(b.MetaTitle, header.Language),
                            MetaDescription = JsonExtensions.JsonValue(b.MetaDescription, header.Language),
                            MetaKeywords = JsonExtensions.JsonValue(b.MetaKeywords, header.Language)
                        }).Take(10).ToList(),

                    }).ToListAsync();
                }
                else
                {
                    return null;
                }


            }
            catch (System.Exception)
            {
                return null;
            }
        }
        // دسته بندی برای موبایل

        public async Task<List<CategoryWebGetDto>> GetCategoryForMobile()
        {
            try
            {
                var setting = await _context.TSetting.Select(x => new { x.SysDisplayCategoriesWithoutGoods, x.SettingId }).FirstOrDefaultAsync();

                if (setting != null && setting.SysDisplayCategoriesWithoutGoods == true)
                {
                    return await _context.TCategory
                    .Include(x => x.TGoods)
                    .Include(x => x.InverseFkParent)
                    .ThenInclude(x => x.TGoods)
                    .Where(x => x.IsActive && x.ToBeDisplayed && x.FkParentId == null)
                    .Select(x => new CategoryWebGetDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        FkParentId = x.FkParentId,
                        GoodsCount = _context.TGoods.Include(t => t.FkCategory).Include(t => t.TGoodsProvider).ThenInclude(r => r.FkShop)
                        .Count(t => t.IsAccepted == true && t.ToBeDisplayed == true && 
                        t.TGoodsProvider.Any(n => n.ToBeDisplayed == true && n.ToBeDisplayed == true && n.FkShop.FkStatusId == (int)ShopStatusEnum.Active && n.HasInventory == true)
                       && t.FkCategory.CategoryPath.Contains("/" + x.CategoryId + "/")),
                        HaveWebPage = x.HaveWebPage,
                        Childs = x.InverseFkParent.Where(t => t.IsActive && t.ToBeDisplayed).Select(b => new CategoryWebGetDto
                        {
                            CategoryId = b.CategoryId,
                            CategoryTitle = JsonExtensions.JsonValue(b.CategoryTitle, header.Language),
                            IconUrl = b.IconUrl,
                            ImageUrl = b.ImageUrl,
                            HaveWebPage = b.HaveWebPage,
                            FkParentId = b.FkParentId,
                            GoodsCount = _context.TGoods.Include(t => t.FkCategory).Include(t => t.TGoodsProvider).ThenInclude(r => r.FkShop).Count(g => g.IsAccepted == true && g.ToBeDisplayed == true &&
                        g.TGoodsProvider.Any(n => n.ToBeDisplayed == true && n.ToBeDisplayed == true && n.FkShop.FkStatusId == (int)ShopStatusEnum.Active && n.HasInventory == true)
                            && g.FkCategory.CategoryPath.Contains("/" + b.CategoryId + "/")),
                        }).ToList(),

                    }).ToListAsync();
                }
                else if (setting != null && setting.SysDisplayCategoriesWithoutGoods == false)
                {
                    return await _context.TCategory
                    .Include(x => x.TGoods)
                    .Include(x => x.InverseFkParent)
                    .ThenInclude(x => x.TGoods)
                    .Where(x => x.IsActive && x.FkParentId == null && x.ToBeDisplayed && (_context.TGoods.Include(t => t.FkCategory).Any(t => t.IsAccepted == true && t.ToBeDisplayed == true && t.FkCategory.CategoryPath.Contains("/" + x.CategoryId + "/") == true)))
                    .Select(x => new CategoryWebGetDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        FkParentId = x.FkParentId,
                        HaveWebPage = x.HaveWebPage,
                        GoodsCount =  _context.TGoods.Include(t => t.FkCategory).Include(t => t.TGoodsProvider).ThenInclude(r => r.FkShop).Count(t => t.IsAccepted == true && t.ToBeDisplayed == true && 
                        t.TGoodsProvider.Any(n => n.ToBeDisplayed == true && n.ToBeDisplayed == true && n.FkShop.FkStatusId == (int)ShopStatusEnum.Active && n.HasInventory == true) &&
                        t.FkCategory.CategoryPath.Contains("/" + x.CategoryId + "/")),
                        Childs = x.InverseFkParent.Where(b => b.IsActive && b.ToBeDisplayed).Select(b => new CategoryWebGetDto
                        {
                            CategoryId = b.CategoryId,
                            CategoryTitle = JsonExtensions.JsonValue(b.CategoryTitle, header.Language),
                            IconUrl = b.IconUrl,
                            ImageUrl = b.ImageUrl,
                            HaveWebPage = b.HaveWebPage,
                            FkParentId = b.FkParentId,
                            GoodsCount =  _context.TGoods.Include(t => t.FkCategory).Include(t => t.TGoodsProvider).ThenInclude(r => r.FkShop).Count(t => t.IsAccepted == true && t.ToBeDisplayed == true && 
                        t.TGoodsProvider.Any(n => n.ToBeDisplayed == true && n.ToBeDisplayed == true && n.FkShop.FkStatusId == (int)ShopStatusEnum.Active && n.HasInventory == true) &&
                            t.FkCategory.CategoryPath.Contains("/" + b.CategoryId + "/")),
                        }).ToList(),

                    }).ToListAsync();
                }
                else
                {
                    return null;
                }


            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<CategoryWebDto> GetCategoryAndBrandForCategoryPageWebsite(int categoryId)
        {
            try
            {
                var setting = await _context.TSetting.Select(x => new { x.SysDisplayCategoriesWithoutGoods, x.SettingId }).FirstOrDefaultAsync();

                if (setting != null && setting.SysDisplayCategoriesWithoutGoods == true)
                {
                    return await _context.TCategory
                    .Include(x => x.InverseFkParent)
                    .Select(x => new CategoryWebDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        MetaDescription = JsonExtensions.JsonValue(x.MetaDescription, header.Language),
                        MetaKeywords = JsonExtensions.JsonValue(x.MetaKeywords, header.Language),
                        MetaTitle = JsonExtensions.JsonValue(x.MetaTitle, header.Language),
                        PageTitle = JsonExtensions.JsonValue(x.PageTitle, header.Language),
                        WebsiteBrand = _context.TBrand.Include(t => t.TCategoryBrand).ThenInclude(i => i.FkCategory).Where(t => t.TCategoryBrand.Any(i => i.FkCategory.CategoryPath.Contains("/" + x.CategoryId + "/"))).Select(b => new WebsiteBrandDto
                        {
                            BrandId = b.BrandId,
                            BrandLogoImage = b.BrandLogoImage,
                            BrandTitle = JsonExtensions.JsonValue(b.BrandTitle, header.Language),
                        }).ToList(),
                        Childs = x.InverseFkParent.Where(t => t.IsActive && t.ToBeDisplayed).Select(b => new CategoryWebsiteDto
                        {
                            CategoryId = b.CategoryId,
                            CategoryTitle = JsonExtensions.JsonValue(b.CategoryTitle, header.Language),
                            IconUrl = b.IconUrl,
                            ImageUrl = b.ImageUrl,
                            HaveWebPage = b.HaveWebPage,
                            FkParentId = b.FkParentId,
                        }).ToList(),
                    }).FirstOrDefaultAsync(x => x.CategoryId == categoryId);
                }
                else if (setting != null && setting.SysDisplayCategoriesWithoutGoods == false)
                {
                    return await _context.TCategory
                    .Include(x => x.InverseFkParent)
                    .Where(x => x.IsActive && x.ToBeDisplayed)
                    .Select(x => new CategoryWebDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        ImageUrl = x.ImageUrl,
                        MetaDescription = JsonExtensions.JsonValue(x.MetaDescription, header.Language),
                        MetaKeywords = JsonExtensions.JsonValue(x.MetaKeywords, header.Language),
                        MetaTitle = JsonExtensions.JsonValue(x.MetaTitle, header.Language),
                        PageTitle = JsonExtensions.JsonValue(x.PageTitle, header.Language),
                        WebsiteBrand = _context.TBrand.Include(t => t.TCategoryBrand).ThenInclude(i => i.FkCategory).Where(t => t.TCategoryBrand.Any(i => i.FkCategory.CategoryPath.Contains("/" + x.CategoryId + "/"))).Select(b => new WebsiteBrandDto
                        {
                            BrandId = b.BrandId,
                            BrandLogoImage = b.BrandLogoImage,
                            BrandTitle = JsonExtensions.JsonValue(b.BrandTitle, header.Language),
                        }).ToList(),
                        Childs = x.InverseFkParent.Where(b => b.IsActive && b.ToBeDisplayed && (_context.TGoods.Include(t => t.FkCategory).Any(t => t.IsAccepted == true && t.ToBeDisplayed == true && t.FkCategory.CategoryPath.Contains("/" + b.CategoryId + "/") == true))).Select(b => new CategoryWebsiteDto
                        {
                            CategoryId = b.CategoryId,
                            CategoryTitle = JsonExtensions.JsonValue(b.CategoryTitle, header.Language),
                            IconUrl = b.IconUrl,
                            ImageUrl = b.ImageUrl,
                            HaveWebPage = b.HaveWebPage,
                            FkParentId = b.FkParentId,
                        }).ToList(),

                    }).FirstOrDefaultAsync(x => x.CategoryId == categoryId);
                }
                else
                {
                    return null;
                }


            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<CategorySettingPathDto>> GetFooter()
        {
            try
            {
                return await _context.TCategory.Where(x => x.AppearInFooter == true)
                .Select(x => new CategorySettingPathDto()
                {
                    CategoryId = x.CategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                    CategoryPath = x.CategoryPath
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> CanAddNewCategoryInFooter()
        {
            try
            {
                var count = await _context.TCategory.CountAsync(x => x.AppearInFooter == true);
                var setting = await _context.TSetting.Select(x => x.FooterMaxItem).FirstOrDefaultAsync();
                if (count < setting)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<CategoryGetDto>> GetAllCategoryGrid(CategoryPaginationDto categoryPagination)
        {
            try
            {
                return await _context.TCategory
                .Where(x =>
                (categoryPagination.Childs.Count > 0 ? categoryPagination.Childs.Contains(x.CategoryId) : true) &&
                (categoryPagination.Display == null ? true : x.ToBeDisplayed == categoryPagination.Display) &&
                (string.IsNullOrWhiteSpace(categoryPagination.Title) ? true : x.CategoryTitle.Contains(categoryPagination.Title)) &&
                (categoryPagination.ReturnAllowed == null ? true : x.ReturningAllowed == categoryPagination.ReturnAllowed) &&
                (categoryPagination.Status == null ? true : x.IsActive == categoryPagination.Status) &&
                (categoryPagination.ShowInFooter == null ? true : x.AppearInFooter == categoryPagination.ShowInFooter) &&
                (categoryPagination.SpeciallyWebPage == null ? true : x.HaveWebPage == categoryPagination.SpeciallyWebPage)
                )
                .OrderBy(x => x.FkParentId)
                .Skip(categoryPagination.PageSize * (categoryPagination.PageNumber - 1)).Take(categoryPagination.PageSize)
                .Include(c => c.FkParent)
                .Select(x => new CategoryGetDto()
                {
                    AppearInFooter = x.AppearInFooter,
                    CategoryId = x.CategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                    CommissionFee = x.CommissionFee,
                    HaveWebPage = x.HaveWebPage,
                    IsActive = x.IsActive,
                    ReturningAllowed = x.ReturningAllowed,
                    ToBeDisplayed = x.ToBeDisplayed,
                    Location = x.FkParent == null ? null : JsonExtensions.JsonValue(x.FkParent.CategoryTitle, header.Language)


                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetAllCategoryGridCount(CategoryPaginationDto categoryPagination)
        {
            return await _context.TCategory
            .CountAsync(x =>
            (categoryPagination.Childs.Count > 0 ? categoryPagination.Childs.Contains(x.CategoryId) : true) &&
            (categoryPagination.Display == null ? true : x.ToBeDisplayed == categoryPagination.Display) &&
            (string.IsNullOrWhiteSpace(categoryPagination.Title) ? true : x.CategoryTitle.Contains(categoryPagination.Title)) &&
            (categoryPagination.ReturnAllowed == null ? true : x.ReturningAllowed == categoryPagination.ReturnAllowed) &&
            (categoryPagination.Status == null ? true : x.IsActive == categoryPagination.Status) &&
            (categoryPagination.ShowInFooter == null ? true : x.AppearInFooter == categoryPagination.ShowInFooter) &&
            (categoryPagination.SpeciallyWebPage == null ? true : x.HaveWebPage == categoryPagination.SpeciallyWebPage)
            );
        }
    }
}