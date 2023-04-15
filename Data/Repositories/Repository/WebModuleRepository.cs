using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Height;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class WebModuleRepository : IWebModuleRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }


        public WebModuleRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<List<WebHomeIndexModuleListDto>> GetModuleCollection(int getType, decimal rate, int? categoryId)
        {
            // getType 1 = for website
            // getType 2 = for adminpanel
            try
            {
                if (getType != 1)
                {
                    rate = (decimal)1.0;
                    if (  header.CurrencyNum != CurrencyEnum.TMN)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                        rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                    }
                }

                if (getType == 1)
                {
                    var data = await _context.WebIndexModuleList
                    .Where(x => x.Status && x.FkCategoryId == categoryId)
                    .Include(m => m.FkModule)
                    .Include(c => c.WebModuleCollections)
                    .Include(c => c.WebModuleCollections)
                    .ThenInclude(w => w.FkCollectionType)
                    .Include(c => c.WebModuleCollections)
                    .OrderBy(x => x.SequenceNumber)
                        .Select(x => new WebHomeIndexModuleListDto()
                        {
                            IModuleId = x.IModuleId,
                            FkModuleId = x.FkModuleId,
                            ModuleTitle = x.FkModule.ModuleTitle,
                            SequenceNumber = x.SequenceNumber,
                            Status = x.Status,
                            SelectedHeight = x.SelectedHeight,
                            BackgroundImageUrl = x.BackgroundImageUrl,
                            IModuleTitle = JsonExtensions.JsonValue(x.ModuleTitle, header.Language),
                            FkCategoryId = x.FkCategoryId,
                            WebModuleCollections = x.WebModuleCollections.OrderBy(t => t.SequenceNumber).Select(t => new WebHomeModuleCollectionsDto()
                            {
                                CollectionId = t.CollectionId,
                                CollectionTitle = JsonExtensions.JsonValue(t.CollectionTitle, header.Language),
                                CollectionTypeTitle = JsonExtensions.JsonValue(t.FkCollectionType.CollectionTypeTitle, header.Language),
                                FkCollectionTypeId = t.FkCollectionTypeId,
                                HaveLink = t.HaveLink,
                                LinkUrl = JsonExtensions.JsonValue(t.LinkUrl, header.Language),
                                ImageUrl = JsonExtensions.JsonValue(t.ImageUrl, header.Language),
                                SequenceNumber = t.SequenceNumber,
                                CriteriaFrom = t.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (t.CriteriaFrom  / rate) : t.CriteriaFrom,
                                CriteriaTo = t.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (t.CriteriaTo  / rate) : t.CriteriaTo,
                                CriteriaType = t.CriteriaType,
                                FkIModuleId = t.FkIModuleId,
                                ResponsiveImageUrl = JsonExtensions.JsonValue(t.ResponsiveImageUrl, header.Language),
                                XitemIds = t.XitemIds
                            }).ToList()
                        })
                    .AsNoTracking()
                    .ToListAsync();
                    return data;
                }
                else
                {
                    var data = await _context.WebIndexModuleList
                    .Where(x => x.FkCategoryId == categoryId)
                    .Include(m => m.FkModule)
                    .Include(c => c.WebModuleCollections)
                    .Include(c => c.WebModuleCollections)
                    .ThenInclude(w => w.FkCollectionType)
                    .Include(c => c.WebModuleCollections)
                    .OrderBy(x => x.SequenceNumber)
                    .Select(x => new WebHomeIndexModuleListDto()
                    {
                        IModuleId = x.IModuleId,
                        FkModuleId = x.FkModuleId,
                        ModuleTitle = x.FkModule.ModuleTitle,
                        SequenceNumber = x.SequenceNumber,
                        Status = x.Status,
                        SelectedHeight = x.SelectedHeight,
                        BackgroundImageUrl = x.BackgroundImageUrl,
                        IModuleTitle = JsonExtensions.JsonValue(x.ModuleTitle, header.Language),
                        FkCategoryId = x.FkCategoryId,
                        WebModuleCollections = x.WebModuleCollections.OrderBy(t => t.SequenceNumber).Select(t => new WebHomeModuleCollectionsDto()
                        {
                            CollectionId = t.CollectionId,
                            CollectionTitle = JsonExtensions.JsonValue(t.CollectionTitle, header.Language),
                            CollectionTypeTitle = JsonExtensions.JsonValue(t.FkCollectionType.CollectionTypeTitle, header.Language) ,
                            FkCollectionTypeId = t.FkCollectionTypeId,
                            HaveLink = t.HaveLink,
                            LinkUrl = JsonExtensions.JsonValue(t.LinkUrl, header.Language),
                            ImageUrl = JsonExtensions.JsonValue(t.ImageUrl, header.Language),
                            SequenceNumber = t.SequenceNumber,
                            CriteriaFrom = t.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (t.CriteriaFrom  / rate) : t.CriteriaFrom,
                            CriteriaTo = t.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (t.CriteriaTo  / rate) : t.CriteriaTo,
                            CriteriaType = t.CriteriaType,
                            FkIModuleId = t.FkIModuleId,
                            XitemIds = t.XitemIds,
                            ResponsiveImageUrl = JsonExtensions.JsonValue(t.ResponsiveImageUrl, header.Language),
                        }).ToList()
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    return data;
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<WebIndexModuleList> WebIndexModuleListAdd(WebIndexModuleList webIndexModuleList)
        {
            try
            {
                webIndexModuleList.ModuleTitle = JsonExtensions.JsonAdd(webIndexModuleList.ModuleTitle, header);

                await _context.WebIndexModuleList.AddAsync(webIndexModuleList);
                await _context.SaveChangesAsync();
                var module = await _context.WebIndexModuleList
                .Where(x => x.IModuleId != webIndexModuleList.IModuleId && x.FkCategoryId == webIndexModuleList.FkCategoryId &&
                 x.SequenceNumber >= webIndexModuleList.SequenceNumber).ToListAsync();
                foreach (var item in module)
                {
                    item.SequenceNumber = item.SequenceNumber + 1;
                }

                if (webIndexModuleList.FkCategoryId != null && webIndexModuleList.FkCategoryId != 0)
                {
                    var category = await _context.TCategory.FirstAsync(x => x.CategoryId == webIndexModuleList.FkCategoryId);
                    category.HaveWebPage = true;
                }

                await _context.SaveChangesAsync();
                webIndexModuleList.ModuleTitle = JsonExtensions.JsonGet(webIndexModuleList.ModuleTitle, header);
                return webIndexModuleList;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public async Task<WebIndexModuleList> WebIndexModuleListEdit(WebIndexModuleList webIndexModuleList)
        {
            try
            {
                var data = await _context.WebIndexModuleList.FirstAsync(x => x.IModuleId == webIndexModuleList.IModuleId);
                data.ModuleTitle = JsonExtensions.JsonAdd(webIndexModuleList.ModuleTitle, header);
                await _context.SaveChangesAsync();
                return webIndexModuleList;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> UploadModuleListImage(string fileName, string title, int Id)
        {
            try
            {
                var data = await _context.WebIndexModuleList.FindAsync(Id);
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    data.BackgroundImageUrl = fileName;
                }
                if (!string.IsNullOrWhiteSpace(title))
                {
                    title = JsonExtensions.JsonEdit(title, data.ModuleTitle, header);
                    data.ModuleTitle = title;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChangePriorityOfWebIndexModuleList(ChangePriorityDto changePriority)
        {
            try
            {
                var data = await _context.WebIndexModuleList.FirstAsync(x => x.IModuleId == changePriority.Id);
                if (data.SequenceNumber > changePriority.PriorityNumber)
                {
                    var allModules = await _context.WebIndexModuleList
                    .Where(x => x.SequenceNumber < data.SequenceNumber && x.SequenceNumber >= changePriority.PriorityNumber && x.FkCategoryId == data.FkCategoryId)
                    .ToListAsync();
                    foreach (var item in allModules)
                    {
                        item.SequenceNumber = item.SequenceNumber + 1;
                    }
                }
                else if (data.SequenceNumber < changePriority.PriorityNumber)
                {
                    var allModules = await _context.WebIndexModuleList
                    .Where(x => x.SequenceNumber > data.SequenceNumber && x.SequenceNumber <= changePriority.PriorityNumber && x.FkCategoryId == data.FkCategoryId)
                    .ToListAsync();
                    foreach (var item in allModules)
                    {
                        item.SequenceNumber = item.SequenceNumber - 1;
                    }
                }
                else
                {

                }
                data.SequenceNumber = changePriority.PriorityNumber;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChangeAcceptOfWebIndexModuleList(AcceptDto changePriority)
        {
            try
            {
                var data = await _context.WebIndexModuleList.FindAsync(changePriority.Id);
                if (changePriority.Accept == true)
                {
                    var countCollection = await _context.WebModuleCollections.CountAsync(x => x.FkIModuleId == data.IModuleId);
                    if (data.FkModuleId == (int)ModuleTypeEnum.ProductList && countCollection < 1)
                    {
                        return false;
                    }
                    if (data.FkModuleId == (int)ModuleTypeEnum.CategoryList && countCollection < 1)
                    {
                        return false;
                    }
                    if (data.FkModuleId == (int)ModuleTypeEnum.Adv1 && countCollection < 1)
                    {
                        return false;
                    }
                    if (data.FkModuleId == (int)ModuleTypeEnum.Adv2 && countCollection < 2)
                    {
                        return false;
                    }
                    if (data.FkModuleId == (int)ModuleTypeEnum.Adv4 && countCollection < 4)
                    {
                        return false;
                    }
                    if (data.FkModuleId == (int)ModuleTypeEnum.Adv6 && countCollection < 6)
                    {
                        return false;
                    }
                }

                data.Status = changePriority.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChangeHeightOfWebIndexModuleList(ChangeHeight changeHeight)
        {
            try
            {
                var data = await _context.WebIndexModuleList.FindAsync(changeHeight.Id);
                data.SelectedHeight = changeHeight.Height;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> WebIndexModuleListExist(int id)
        {
            try
            {
                var exist = await _context.WebIndexModuleList.AsNoTracking().AnyAsync(x => x.IModuleId == id);
                return exist;
            }
            catch (System.Exception)
            {
                return false;

            }
        }

        public async Task<WebIndexModuleList> WebIndexModuleListDelete(int id)
        {
            try
            {
                var data = await _context.WebIndexModuleList.FindAsync(id);
                var Module = await _context.WebIndexModuleList.Where(x => x.FkCategoryId == data.FkCategoryId && x.SequenceNumber > data.SequenceNumber).ToListAsync();
                foreach (var item in Module)
                {
                    item.SequenceNumber = item.SequenceNumber - 1;
                }
                if (data.FkCategoryId != null && data.FkCategoryId != 0)
                {
                    var categoryCount = await _context.WebIndexModuleList.CountAsync(x => x.FkCategoryId == data.FkCategoryId);
                    if (categoryCount == 1)
                    {
                        var category = await _context.TCategory.FirstAsync(x => x.CategoryId == data.FkCategoryId);
                        category.HaveWebPage = false;
                    }
                }
                _context.WebIndexModuleList.Remove(data);
                await _context.SaveChangesAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<WebModuleCollections> WebModuleCollectionsAdd(WebModuleCollections webModuleCollections)
        {
            try
            {
                webModuleCollections.CollectionTitle = JsonExtensions.JsonAdd(webModuleCollections.CollectionTitle, header);
                webModuleCollections.ImageUrl = JsonExtensions.JsonAdd(webModuleCollections.ImageUrl, header);
                webModuleCollections.LinkUrl = JsonExtensions.JsonAdd(webModuleCollections.LinkUrl, header);
                webModuleCollections.ResponsiveImageUrl = JsonExtensions.JsonAdd(webModuleCollections.ResponsiveImageUrl, header);
                if (webModuleCollections.CriteriaType == (int)DiscountTypeId.FixedDiscount)
                {
                    var rate = (decimal)1.00;
                    if (  header.CurrencyNum != CurrencyEnum.TMN)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                        rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                    }
                    if (webModuleCollections.CriteriaFrom != null)
                    {
                        webModuleCollections.CriteriaFrom = webModuleCollections.CriteriaFrom / rate;
                    }
                    if (webModuleCollections.CriteriaTo != null)
                    {
                        webModuleCollections.CriteriaTo = webModuleCollections.CriteriaTo / rate;
                    }
                }
                if (webModuleCollections.HaveLink == true)
                {
                    webModuleCollections.CriteriaTo = null;
                    webModuleCollections.CriteriaFrom = null;
                    webModuleCollections.XitemIds = null;
                    webModuleCollections.CriteriaType = null;
                    webModuleCollections.FkCollectionTypeId = (int)CollectionTypeEnum.AllProduct;
                }
                else
                {
                    webModuleCollections.LinkUrl = null;
                }
                await _context.WebModuleCollections.AddAsync(webModuleCollections);
                await _context.SaveChangesAsync();
                var module = await _context.WebModuleCollections
                .Where(x => x.FkIModuleId == webModuleCollections.FkIModuleId && x.CollectionId != webModuleCollections.CollectionId &&
                 x.SequenceNumber >= webModuleCollections.SequenceNumber).ToListAsync();
                // foreach (var item in module)
                // {
                //     item.SequenceNumber = item.SequenceNumber + 1;
                // }
                await _context.SaveChangesAsync();
                webModuleCollections.CollectionTitle = JsonExtensions.JsonGet(webModuleCollections.CollectionTitle, header);
                webModuleCollections.ImageUrl = JsonExtensions.JsonGet(webModuleCollections.ImageUrl, header);
                webModuleCollections.LinkUrl = JsonExtensions.JsonGet(webModuleCollections.LinkUrl, header);
                webModuleCollections.ResponsiveImageUrl = JsonExtensions.JsonGet(webModuleCollections.ResponsiveImageUrl, header);
                return webModuleCollections;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> WebModuleCollectionsExist(int id)
        {
            try
            {
                var exist = await _context.WebModuleCollections.AsNoTracking().AnyAsync(x => x.CollectionId == id);
                return exist;
            }
            catch (System.Exception)
            {
                return false;
            }
        }


        public async Task<bool> ChangePriorityOfWebModuleCollections(ChangePriorityDto changePriority)
        {
            try
            {
                var data = await _context.WebModuleCollections.FirstAsync(x => x.CollectionId == changePriority.Id && x.FkIModuleId == changePriority.ParentID);
                if (data.SequenceNumber > changePriority.PriorityNumber)
                {
                    var allChildOfParent = await _context.WebModuleCollections
                    .Where(x => x.FkIModuleId == changePriority.ParentID && x.SequenceNumber < data.SequenceNumber && x.SequenceNumber >= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allChildOfParent)
                    {
                        item.SequenceNumber = item.SequenceNumber + 1;
                    }
                }
                else if (data.SequenceNumber < changePriority.PriorityNumber)
                {
                    var allChildOfParent = await _context.WebModuleCollections
                    .Where(x => x.FkIModuleId == changePriority.ParentID && x.SequenceNumber > data.SequenceNumber && x.SequenceNumber <= changePriority.PriorityNumber)
                    .ToListAsync();
                    foreach (var item in allChildOfParent)
                    {
                        item.SequenceNumber = item.SequenceNumber - 1;
                    }
                }
                else
                {

                }
                data.SequenceNumber = changePriority.PriorityNumber;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<WebModuleCollections> WebModuleCollectionsDelete(int id)
        {
            try
            {
                var data = await _context.WebModuleCollections.FindAsync(id);
                var collections = await _context.WebModuleCollections.Where(x => x.FkIModuleId == data.FkIModuleId && x.SequenceNumber > data.SequenceNumber).ToListAsync();
                foreach (var item in collections)
                {
                    item.SequenceNumber = item.SequenceNumber - 1;
                }
                _context.WebModuleCollections.Remove(data);
                await _context.SaveChangesAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<WebModuleCollections> WebModuleCollectionsEdit(WebModuleCollections webModuleCollections)
        {
            try
            {
                if (webModuleCollections.CriteriaType == (int)DiscountTypeId.FixedDiscount)
                {
                    var rate = (decimal)1.00;
                    if (  header.CurrencyNum != CurrencyEnum.TMN)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                        rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                    }
                    if (webModuleCollections.CriteriaFrom != null)
                    {
                        webModuleCollections.CriteriaFrom = (decimal)webModuleCollections.CriteriaFrom / rate;
                    }
                    if (webModuleCollections.CriteriaTo != null)
                    {
                        webModuleCollections.CriteriaTo = (decimal)webModuleCollections.CriteriaTo / rate;
                    }
                }
                var data = await _context.WebModuleCollections.FindAsync(webModuleCollections.CollectionId);
                await _context.SaveChangesAsync();
                webModuleCollections.CollectionTitle = JsonExtensions.JsonEdit(webModuleCollections.CollectionTitle, data.CollectionTitle, header);
                webModuleCollections.ImageUrl = JsonExtensions.JsonEdit(webModuleCollections.ImageUrl, data.ImageUrl, header);
                webModuleCollections.LinkUrl = JsonExtensions.JsonEdit(webModuleCollections.LinkUrl,data.LinkUrl, header);
                webModuleCollections.ResponsiveImageUrl = JsonExtensions.JsonEdit(webModuleCollections.ResponsiveImageUrl, data.ImageUrl, header);
                if (webModuleCollections.HaveLink == true)
                {
                    webModuleCollections.CriteriaTo = null;
                    webModuleCollections.CriteriaFrom = null;
                    webModuleCollections.XitemIds = null;
                    webModuleCollections.CriteriaType = null;
                    webModuleCollections.FkCollectionTypeId = (int)CollectionTypeEnum.AllProduct;
                }
                else
                {
                    webModuleCollections.LinkUrl = null;
                }
                _context.Entry(data).CurrentValues.SetValues(webModuleCollections);
                await _context.SaveChangesAsync();
                webModuleCollections.CollectionTitle = JsonExtensions.JsonGet(webModuleCollections.CollectionTitle, header);
                webModuleCollections.ImageUrl = JsonExtensions.JsonGet(webModuleCollections.ImageUrl, header);
                webModuleCollections.LinkUrl = JsonExtensions.JsonGet(webModuleCollections.LinkUrl, header);
                webModuleCollections.ResponsiveImageUrl = JsonExtensions.JsonGet(webModuleCollections.ResponsiveImageUrl, header);
                return webModuleCollections;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        //// get collection type
        public async Task<List<WebCollectionType>> GetWebCollectionType(bool slider)
        {
            try
            {
                if (slider)
                {
                    var collectionType = await _context.WebCollectionType
                    .Where(x => x.ForCustomer == false)
                    .AsNoTracking().ToListAsync();
                    for (int i = 0; i < collectionType.Count; i++)
                    {
                        collectionType[i].CollectionTypeTitle = JsonExtensions.JsonGet(collectionType[i].CollectionTypeTitle, header);
                    }
                    return collectionType;
                }
                else
                {
                    var collectionType = await _context.WebCollectionType.AsNoTracking().ToListAsync();
                    for (int i = 0; i < collectionType.Count; i++)
                    {
                        collectionType[i].CollectionTypeTitle = JsonExtensions.JsonGet(collectionType[i].CollectionTypeTitle, header);
                    }
                    return collectionType;
                }

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<WebModuleCollectionsAddDto> UploadWebModuleCollectionsImage(string fileName, string ResponsiveFileName, int id)
        {
            try
            {
                var data = await _context.WebModuleCollections.FindAsync(id);
                var oldData = await _context.WebModuleCollections.Select(x => new WebModuleCollectionsAddDto()
                {
                    CollectionId = x.CollectionId,
                    ImageUrl = JsonExtensions.JsonValue(x.ImageUrl, header.Language),
                    FkIModuleId = x.FkIModuleId,
                    ResponsiveImageUrl = JsonExtensions.JsonValue(x.ResponsiveImageUrl, header.Language)
                }).FirstOrDefaultAsync(x => x.CollectionId == id);
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    data.ImageUrl = JsonExtensions.JsonEdit(fileName, data.ImageUrl, header);
                }
                if (!string.IsNullOrWhiteSpace(ResponsiveFileName))
                {
                    data.ResponsiveImageUrl = JsonExtensions.JsonEdit(ResponsiveFileName, data.ResponsiveImageUrl, header);
                }
                await _context.SaveChangesAsync();
                return oldData;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<WebModuleCollectionsGetDto> WebModuleCollectionsGetById(int id)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var data = await _context.WebModuleCollections
                .Include(x => x.FkCollectionType)
                .Select(x => new WebModuleCollectionsGetDto()
                {
                    CollectionId = x.CollectionId,
                    FkIModuleId = x.FkIModuleId,
                    FkCollectionTypeId = x.FkCollectionTypeId,
                    SequenceNumber = x.SequenceNumber,
                    CollectionTitle = JsonExtensions.JsonValue(x.CollectionTitle, header.Language),
                    HaveLink = x.HaveLink,
                    LinkUrl = JsonExtensions.JsonValue(x.LinkUrl, header.Language),
                    ImageUrl = JsonExtensions.JsonValue(x.ImageUrl, header.Language),
                    CriteriaType = x.CriteriaType,
                    CriteriaFrom = x.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (x.CriteriaFrom  / rate) : x.CriteriaFrom,
                    CriteriaTo = x.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (x.CriteriaTo  / rate) : x.CriteriaTo,
                    FkCollectionType = new WebCollectionTypeDto(x.FkCollectionType),
                    ResponsiveImageUrl = JsonExtensions.JsonValue(x.ResponsiveImageUrl, header.Language),
                    XitemIds = x.XitemIds
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CollectionId == id);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<WebHomeModuleCollectionsDto>> WebModuleCollectionsByModuleId(int moduleId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var data = await _context.WebModuleCollections
                .Include(x => x.FkCollectionType)
                .Where(x => x.FkIModuleId == moduleId)
                .OrderBy(x => x.SequenceNumber)
                .Select(x => new WebHomeModuleCollectionsDto()
                {
                    CollectionId = x.CollectionId,
                    CollectionTitle = JsonExtensions.JsonValue(x.CollectionTitle, header.Language),
                    CollectionTypeTitle = JsonExtensions.JsonValue(x.FkCollectionType.CollectionTypeTitle, header.Language) ,
                    FkCollectionTypeId = x.FkCollectionTypeId,
                    HaveLink = x.HaveLink,
                    LinkUrl = JsonExtensions.JsonValue(x.LinkUrl, header.Language),
                    ImageUrl = JsonExtensions.JsonValue(x.ImageUrl, header.Language),
                    SequenceNumber = x.SequenceNumber,
                    CriteriaFrom = x.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (x.CriteriaFrom  / rate) : x.CriteriaFrom,
                    CriteriaTo = x.CriteriaType == (int)DiscountTypeId.FixedDiscount ? (x.CriteriaTo  / rate) : x.CriteriaTo,
                    CriteriaType = x.CriteriaType,
                    FkIModuleId = x.FkIModuleId,
                    ResponsiveImageUrl = JsonExtensions.JsonValue(x.ResponsiveImageUrl, header.Language),
                    XitemIds = x.XitemIds
                })
                .AsNoTracking()
                .ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

    }
}