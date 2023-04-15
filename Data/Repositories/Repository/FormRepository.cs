using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.City;
using MarketPlace.API.Data.Dtos.Country;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Discount;
using MarketPlace.API.Data.Dtos.DocumentType;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.MeasurementUnit;
using MarketPlace.API.Data.Dtos.Message;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Data.Dtos.OrderCancelingReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.PaymentMethod;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.ReturningAction;
using MarketPlace.API.Data.Dtos.ReturningReason;
using MarketPlace.API.Data.Dtos.ReturningStatus;
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.ShopStatus;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class FormRepository : IFormRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }

        public FormRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
        }

        public async Task<List<ShopFormDto>> GetShopList(FormPagination pagination)
        {
            try
            {
                // pagination.id is for goods id
                var data = new List<ShopFormDto>();
                if (pagination.Id == 0)
                {
                    data = await _context.TShop
                    .Where(x => string.IsNullOrWhiteSpace(pagination.Filter) ? true : x.StoreName.Contains(pagination.Filter))
                    .OrderByDescending(x => x.ShopId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new ShopFormDto()
                    {
                        ShopId = x.ShopId,
                        ShopTitle = x.StoreName
                    })
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    data = await _context.TShop
                    .Include(x => x.TGoodsProvider)
                    .Where(x => (string.IsNullOrWhiteSpace(pagination.Filter) ? true : x.StoreName.Contains(pagination.Filter)) && x.TGoodsProvider.Any(t => t.FkGoodsId == pagination.Id))
                    .OrderByDescending(x => x.ShopId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new ShopFormDto()
                    {
                        ShopId = x.ShopId,
                        ShopTitle = x.StoreName
                    })
                    .AsNoTracking().ToListAsync();
                }


                /// اصافه کردن دیتا بایند شده در سمت کلاینت به دراپ داون به لیست
                if (!String.IsNullOrWhiteSpace(pagination.valueId) && String.IsNullOrWhiteSpace(pagination.Filter) && pagination.PageNumber == 1)
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    var notExistDataId = idList.Where(p => data.All(p2 => p2.ShopId.ToString() != p)).ToList();
                    if (notExistDataId.Count > 1 || (notExistDataId.Count == 1 && !string.IsNullOrWhiteSpace(notExistDataId[0])))
                    {
                        var oldData = await _context.TShop.Where(x => idList.Contains(x.ShopId.ToString()))
                        .Select(x => new ShopFormDto()
                        {
                            ShopId = x.ShopId,
                            ShopTitle = x.StoreName
                        })
                        .AsNoTracking().ToListAsync();
                        if (oldData.Count > 0)
                        {
                            data.AddRange(oldData);
                        }
                    }
                }
                if (pagination.PageNumber > 1 && !string.IsNullOrWhiteSpace(pagination.valueId))
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    data = data.Where(b => !idList.Contains(b.ShopId.ToString())).ToList();
                }
                ///////////////////////

                return data;

            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<int> GetShopListCount(FormPagination pagination)
        {
            try
            {
                return await _context.TShop.AsNoTracking()
                .CountAsync(x => string.IsNullOrWhiteSpace(pagination.Filter) ? true :
                 x.StoreName.Contains(pagination.Filter));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<CustomerFormDto>> GetCustomerList(FormPagination pagination)
        {
            try
            {
                var data = new List<CustomerFormDto>();
                if (token.Rule == UserGroupEnum.Admin)
                {
                    data = await _context.TCustomer
                    .Where(x => x.CustomerId != (int)CustomerTypeEnum.Unknown && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (x.Name.Contains(pagination.Filter) || x.Family.Contains(pagination.Filter) || x.NationalCode.Contains(pagination.Filter))))
                    .OrderByDescending(x => x.CustomerId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new CustomerFormDto()
                    {
                        CustomerId = x.CustomerId,
                        CustomerName = x.Name + " " + x.Family
                    })
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    data = await _context.TCustomer.Include(x => x.TOrder).ThenInclude(t => t.TOrderItem)
                    .Where(x => (x.TOrder.Any(t => t.TOrderItem.Any(i => i.FkShopId == token.Id && i.FkStatusId != (int)OrderStatusEnum.Cart))) && x.CustomerId != (int)CustomerTypeEnum.Unknown &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (x.Name.Contains(pagination.Filter) || x.Family.Contains(pagination.Filter) || x.NationalCode.Contains(pagination.Filter))))
                    .OrderByDescending(x => x.CustomerId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new CustomerFormDto()
                    {
                        CustomerId = x.CustomerId,
                        CustomerName = x.Name + " " + x.Family
                    })
                    .AsNoTracking().ToListAsync();
                }

                /// اصافه کردن دیتا بایند شده در سمت کلاینت به دراپ داون به لیست
                if (!String.IsNullOrWhiteSpace(pagination.valueId) && String.IsNullOrWhiteSpace(pagination.Filter) && pagination.PageNumber == 1)
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    var notExistDataId = idList.Where(p => data.All(p2 => p2.CustomerId.ToString() != p)).ToList();
                    if (notExistDataId.Count > 1 || (notExistDataId.Count == 1 && !string.IsNullOrWhiteSpace(notExistDataId[0])))
                    {
                        var oldData = await _context.TCustomer.Where(x => idList.Contains(x.CustomerId.ToString()))
                        .Select(x => new CustomerFormDto()
                        {
                            CustomerId = x.CustomerId,
                            CustomerName = x.Name + " " + x.Family
                        })
                        .AsNoTracking().ToListAsync();
                        if (oldData.Count > 0)
                        {
                            data.AddRange(oldData);
                        }
                    }
                }
                if (pagination.PageNumber > 1 && !string.IsNullOrWhiteSpace(pagination.valueId))
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    data = data.Where(b => !idList.Contains(b.CustomerId.ToString())).ToList();
                }

                return data;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetCustomerCount(FormPagination pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TCustomer.AsNoTracking()
                    .CountAsync(x => x.CustomerId != (int)CustomerTypeEnum.Unknown && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (x.Name.Contains(pagination.Filter) || x.Family.Contains(pagination.Filter) || x.NationalCode.Contains(pagination.Filter))));
                }
                else
                {
                    return await _context.TCustomer.Include(x => x.TOrder).ThenInclude(t => t.TOrderItem).AsNoTracking()
                    .CountAsync(x => (x.TOrder.Any(t => t.TOrderItem.Any(i => i.FkShopId == token.Id && i.FkStatusId != (int)OrderStatusEnum.Cart))) && x.CustomerId != (int)CustomerTypeEnum.Unknown &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (x.Name.Contains(pagination.Filter) || x.Family.Contains(pagination.Filter) || x.NationalCode.Contains(pagination.Filter))));
                }

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<GoodsFormGetDto>> GetGoodsList(FormPagination pagination)
        {
            try
            {
                var data = new List<GoodsFormGetDto>();

                if (token.Rule == UserGroupEnum.Admin)
                {
                    data = await _context.TGoods
                    .Where(x => (pagination.ProductCallRequest == true ? x.SaleWithCall == true : true) && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Filter) || x.GoodsCode.Contains(pagination.Filter) || x.SerialNumber.Contains(pagination.Filter))))
                    .OrderByDescending(x => x.GoodsId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new GoodsFormGetDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        HaveVariation = x.HaveVariation,
                        IsCommonGoods = x.IsCommonGoods
                    })
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    data = await _context.TGoods.Include(x => x.TGoodsProvider)
                    .Where(x => x.TGoodsProvider.Any(t => t.FkShopId == token.Id) && (pagination.ProductCallRequest == true ? x.SaleWithCall == true : true) &&
                      (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Filter) || x.GoodsCode.Contains(pagination.Filter) || x.SerialNumber.Contains(pagination.Filter))))
                    .OrderByDescending(x => x.GoodsId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new GoodsFormGetDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        HaveVariation = x.HaveVariation,
                        IsCommonGoods = x.IsCommonGoods
                    })
                    .AsNoTracking().ToListAsync();
                }


                /// اصافه کردن دیتا بایند شده در سمت کلاینت به دراپ داون به لیست
                if (!String.IsNullOrWhiteSpace(pagination.valueId) && String.IsNullOrWhiteSpace(pagination.Filter) && pagination.PageNumber == 1)
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    var notExistDataId = idList.Where(p => data.All(p2 => p2.GoodsId.ToString() != p)).ToList();
                    if (notExistDataId.Count > 1 || (notExistDataId.Count == 1 && !string.IsNullOrWhiteSpace(notExistDataId[0])))
                    {
                        var oldData = await _context.TGoods.Where(x => idList.Contains(x.GoodsId.ToString()))
                        .Select(x => new GoodsFormGetDto()
                        {
                            GoodsId = x.GoodsId,
                            Title = JsonExtensions.JsonValue(x.Title, header.Language),
                            GoodsCode = x.GoodsCode,
                            SerialNumber = x.SerialNumber,
                            HaveVariation = x.HaveVariation,
                            IsCommonGoods = x.IsCommonGoods
                        })
                        .AsNoTracking().ToListAsync();
                        if (oldData.Count > 0)
                        {
                            data.AddRange(oldData);
                        }
                    }
                }
                if (pagination.PageNumber > 1 && !string.IsNullOrWhiteSpace(pagination.valueId))
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    data = data.Where(b => !idList.Contains(b.GoodsId.ToString())).ToList();
                }
                ///////////////////////

                return data;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetGoodsCount(FormPagination pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TGoods.AsNoTracking()
                    .CountAsync(x => (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Filter) || x.GoodsCode.Contains(pagination.Filter) || x.SerialNumber.Contains(pagination.Filter))));
                }
                else
                {
                    return await _context.TGoods.Include(x => x.TGoodsProvider).AsNoTracking().CountAsync(x => x.TGoodsProvider.Any(t => t.FkShopId == token.Id) &&
                      (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Filter) || x.GoodsCode.Contains(pagination.Filter) || x.SerialNumber.Contains(pagination.Filter))));
                }

            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        public async Task<List<CategoryFormListDto>> GetCategoryList(FormPagination pagination)
        {
            try
            {
                var data = new List<CategoryFormListDto>();
                if (token.Rule == UserGroupEnum.Admin)
                {
                    data = await _context.TCategory
                    .Where(x =>
                    (pagination.Id == 0 ? true : x.CategoryPath.Contains("/" + pagination.Id + "/")) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CategoryTitle, header.Language).Contains(pagination.Filter))))
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new CategoryFormListDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        CategoryPath = x.CategoryPath,
                        Parents = null
                    }).OrderBy(x => x.CategoryTitle)
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TCategory, bool>>)(x => x.CategoryPath.Contains("/" + k + "/")));
                    data = await _context.TCategory
                    .WhereAny(predicates.ToArray())
                    .Where(x => (pagination.Id == 0 ? true : x.CategoryPath.Contains("/" + pagination.Id + "/")) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CategoryTitle, header.Language).Contains(pagination.Filter))))
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new CategoryFormListDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        IconUrl = x.IconUrl,
                        CategoryPath = x.CategoryPath,
                        Parents = null
                    }).OrderBy(x => x.CategoryTitle)
                    .AsNoTracking().ToListAsync();
                }

                /// اصافه کردن دیتا بایند شده در سمت کلاینت به دراپ داون به لیست
                if (!String.IsNullOrWhiteSpace(pagination.valueId) && String.IsNullOrWhiteSpace(pagination.Filter) && pagination.PageNumber == 1)
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    var notExistDataId = idList.Where(p => data.All(p2 => p2.CategoryId.ToString() != p)).ToList();
                    if (notExistDataId.Count > 1 || (notExistDataId.Count == 1 && !string.IsNullOrWhiteSpace(notExistDataId[0])))
                    {
                        var oldData = await _context.TCategory.Where(x => idList.Contains(x.CategoryId.ToString()))
                        .Select(x => new CategoryFormListDto()
                        {
                            CategoryId = x.CategoryId,
                            CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                            IconUrl = x.IconUrl,
                            CategoryPath = x.CategoryPath,
                            Parents = null
                        })
                        .AsNoTracking().ToListAsync();
                        if (oldData.Count > 0)
                        {
                            data.AddRange(oldData);
                        }
                    }
                }
                if (pagination.PageNumber > 1 && !string.IsNullOrWhiteSpace(pagination.valueId))
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    data = data.Where(b => !idList.Contains(b.CategoryId.ToString())).ToList();
                }
                ///////////////////////
                var catids = new List<int>();
                foreach (var item in data)
                {
                    var ids = Extentions.GetParentIds(item.CategoryPath);
                    ids.Remove(item.CategoryId);
                    catids.AddRange(ids);
                }
                catids = catids.Distinct().ToList();
                var categories = await _context.TCategory.Where(x => catids.Contains(x.CategoryId)).Select(x => new CategoryFormGetDto()
                {
                    CategoryId = x.CategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language)
                }).AsNoTracking().ToListAsync();
                foreach (var item in data)
                {
                    item.Parents = string.Join(" / ", categories.Where(x => Extentions.GetParentIds(item.CategoryPath).Contains(x.CategoryId)).OrderBy(x => x.CategoryId).Select(x => x.CategoryTitle).ToList());
                }

                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetCategoryCount(FormPagination pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TCategory.AsNoTracking()
                    .CountAsync(x =>
                    (pagination.Id == 0 ? true : x.CategoryPath.Contains("/" + pagination.Id + "/")) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CategoryTitle, header.Language).Contains(pagination.Filter))));
                }
                else
                {
                    var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TCategory, bool>>)(x => x.CategoryPath.Contains("/" + k + "/")));
                    return await _context.TCategory
                    .AsNoTracking()
                    .WhereAny(predicates.ToArray())
                    .CountAsync(x =>
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CategoryTitle, header.Language).Contains(pagination.Filter))));
                }

            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        public async Task<List<DocumentTypeFormDto>> GetShopDocumentsType(int groupId, bool active)
        {
            try
            {
                return await _context.TDocumentType.Where(x => (active == true ? x.Status == true : true) &&
                (groupId == 0 ? true : x.FkGroupd == groupId))
                .Include(x => x.FkGroupdNavigation)
                .Include(x => x.FkPerson)
                .Select(x => new DocumentTypeFormDto()
                {
                    DocumentTitle = JsonExtensions.JsonValue(x.DocumentTitle, header.Language),
                    DocumentTypeId = x.DocumentTypeId,
                    FkGroupd = x.FkGroupd,
                    FkPersonId = x.FkPersonId,
                    GroupTitle = JsonExtensions.JsonValue(x.FkGroupdNavigation.DocumentTypeTitle, header.Language),
                    PersonTitle = JsonExtensions.JsonValue(x.FkPerson.PersonTypeTitle, header.Language)
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ShippingMethodFormDto>> GetShippingMethod(bool active)
        {
            try
            {
                return await _context.TShippingMethod.Where(x => (active == true ? x.Active == true : true))
                .Select(x => new ShippingMethodFormDto()
                {
                    BaseWeight = x.BaseWeight,
                    CashOnDelivery = x.CashOnDelivery,
                    HaveOnlineService = x.HaveOnlineService,
                    ShippingMethodTitle = JsonExtensions.JsonValue(x.ShippingMethodTitle, header.Language),
                    Id = x.Id
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ShopPlanFormDto>> GetShopPlan(bool active)
        {
            try
            {
                return await _context.TShopPlan.Where(x => (active == true ? x.Status == true : true))
                .Select(x => new ShopPlanFormDto()
                {
                    Desription = JsonExtensions.JsonValue(x.Desription, header.Language),
                    PlanCode = JsonExtensions.JsonValue(x.PlanTitle, header.Language),
                    PlanId = x.PlanId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ShopStatusDto>> GetShopStatus()
        {
            try
            {
                return await _context.TShopStatus
                .Select(x => new ShopStatusDto()
                {
                    Comment = JsonExtensions.JsonValue(x.Comment, header.Language),
                    StatusId = x.StatusId,
                    StatusTitle = JsonExtensions.JsonValue(x.StatusTitle, header.Language)
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<CountryFormDto>> GetCountry(FormPagination pagination, bool active)
        {
            try
            {
                var data = await _context.TCountry
                .Where(x => (active == true ? x.Status == true : true) &&
                (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CountryTitle, header.Language).Contains(pagination.Filter))))
                .OrderBy(x => JsonExtensions.JsonValue(x.CountryTitle, header.Language))
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new CountryFormDto()
                {
                    CountryTitle = JsonExtensions.JsonValue(x.CountryTitle, header.Language),
                    CountryId = x.CountryId,
                    FlagUrl = x.FlagUrl

                }).AsNoTracking().ToListAsync();

                /// اصافه کردن دیتا بایند شده در سمت کلاینت به دراپ داون به لیست
                if (!String.IsNullOrWhiteSpace(pagination.valueId) && String.IsNullOrWhiteSpace(pagination.Filter) && pagination.PageNumber == 1)
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    var notExistDataId = idList.Where(p => data.All(p2 => p2.CountryId.ToString() != p)).ToList();
                    if (notExistDataId.Count > 1 || (notExistDataId.Count == 1 && !string.IsNullOrWhiteSpace(notExistDataId[0])))
                    {
                        var oldData = await _context.TCountry.Where(x => idList.Contains(x.CountryId.ToString()))
                        .Select(x => new CountryFormDto()
                        {
                            CountryTitle = JsonExtensions.JsonValue(x.CountryTitle, header.Language),
                            CountryId = x.CountryId,
                            FlagUrl = x.FlagUrl

                        })
                        .AsNoTracking().ToListAsync();
                        if (oldData.Count > 0)
                        {
                            data.AddRange(oldData);
                        }
                    }
                }
                if (pagination.PageNumber > 1 && !string.IsNullOrWhiteSpace(pagination.valueId))
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    data = data.Where(b => !idList.Contains(b.CountryId.ToString())).ToList();
                }

                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<CountryFormDto>> GetActiveCountry()
        {
            try
            {
                var data = await _context.TCountry.Where(x => x.Status == true)
                .OrderBy(x => JsonExtensions.JsonValue(x.CountryTitle, header.Language))
                .Select(x => new CountryFormDto()
                {
                    CountryTitle = JsonExtensions.JsonValue(x.CountryTitle, header.Language),
                    CountryId = x.CountryId,
                    FlagUrl = x.FlagUrl,
                    Iso = x.Iso2,
                    PhoneCode = x.PhoneCode,
                    DefualtPreCode = x.DefualtPreCode
                }).AsNoTracking().ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetCountryCount(FormPagination pagination, bool active)
        {
            try
            {
                return await _context.TCountry.AsNoTracking()
                .CountAsync(x => (active == true ? x.Status == true : true) &&
                (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CountryTitle, header.Language).Contains(pagination.Filter))));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<CityFormDto>> GetCity(int provinceId, bool active)
        {
            try
            {
                return await _context.TCity.Where(x => (active == true ? x.Status == true : true) &&
                 (provinceId != 0 ? x.FkProvinceId == provinceId : true))
                .OrderBy(x => JsonExtensions.JsonValue(x.CityTitle, header.Language))
                .Select(x => new CityFormDto()
                {
                    CityTitle = JsonExtensions.JsonValue(x.CityTitle, header.Language),
                    CityId = x.CityId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ProvinceFormDto>> GetProvince(int countryId, bool active)
        {
            try
            {
                return await _context.TProvince.Where(x => (active == true ? x.Status == true : true) &&
                 (countryId != 0 ? x.FkCountryId == countryId : true))
                .OrderBy(x => JsonExtensions.JsonValue(x.ProvinceName, header.Language))
                .Select(x => new ProvinceFormDto()
                {
                    ProvinceName = JsonExtensions.JsonValue(x.ProvinceName, header.Language),
                    ProvinceId = x.ProvinceId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ReturningStatusFormDto>> GetReturningStatus(bool active)
        {
            try
            {
                return await _context.TReturningStatus.Where(x =>
                (active == true ? (token.Rule == Enums.UserGroupEnum.Admin ? x.AdminAvailable == true : x.ShopAvailable == true) : true)
                )
                .Select(x => new ReturningStatusFormDto()
                {
                    StatusTitle = JsonExtensions.JsonValue(x.StatusTitle, header.Language),
                    Description = JsonExtensions.JsonValue(x.Description, header.Language),
                    StatusId = x.StatusId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<DocumentGroupDto>> GetDocumentGroup()
        {
            try
            {
                return await _context.TDocumentGroup
                .Select(x => new DocumentGroupDto()
                {
                    DocumentTypeTitle = JsonExtensions.JsonValue(x.DocumentTypeTitle, header.Language),
                    DocumentTypeId = x.DocumentTypeId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<AllDocumentDto>> GetAllShopActiveDocument()
        {
            try
            {
                return await _context.TDocumentGroup.Include(v=>v.TDocumentType)
                .Select(x => new AllDocumentDto()
                {
                    DocumentTypeTitle = JsonExtensions.JsonValue(x.DocumentTypeTitle, header.Language),
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentType = x.TDocumentType.Where(n=>n.Status).Select(x => new DocumentTypeFormDto()
                    {
                        DocumentTitle = JsonExtensions.JsonValue(x.DocumentTitle, header.Language),
                        DocumentTypeId = x.DocumentTypeId,
                        FkGroupd = x.FkGroupd,
                        FkPersonId = x.FkPersonId,
                        GroupTitle = JsonExtensions.JsonValue(x.FkGroupdNavigation.DocumentTypeTitle, header.Language),
                        PersonTitle = JsonExtensions.JsonValue(x.FkPerson.PersonTypeTitle, header.Language)
                    }).ToList()
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<PersonDto>> GetPerson()
        {
            try
            {
                return await _context.TPerson
                .Select(x => new PersonDto()
                {
                    PersonTypeTitle = JsonExtensions.JsonValue(x.PersonTypeTitle, header.Language),
                    PersonTypeId = x.PersonTypeId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<OrderStatusDto>> GetOrderStatus(bool active)
        {
            try
            {
                return await _context.TOrderStatus
                .Where(x => active == false ? true : (token.Rule == Enums.UserGroupEnum.Admin ? x.AdminAvailable == true : x.ShopAvailable == true))
                .Select(x => new OrderStatusDto()
                {
                    StatusTitle = JsonExtensions.JsonValue(x.StatusTitle, header.Language),
                    StatusId = x.StatusId,
                    Color = x.Color
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<TransactionTypeFormDto>> GetTransactionType()
        {
            try
            {
                return await _context.TTransactionType
                .Select(x => new TransactionTypeFormDto()
                {
                    TransactionTypeTitle = JsonExtensions.JsonValue(x.TransactionTypeTitle, header.Language),
                    TransactionTypeId = x.TransactionTypeId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<BrandFormDto>> GetBrand(PaginationDto pagination)
        {
            try
            {

                var data = new List<BrandFormDto>();


                if (token.Rule == UserGroupEnum.Admin)
                {
                    data = await _context.TBrand
                    .Where(x => (pagination.Active == true ? x.IsAccepted == true : true) &&
                   (pagination.ChildIds.Count > 0 ? x.TCategoryBrand.Any(t => pagination.ChildIds.Contains(t.FkCategoryId)) : true) &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(pagination.Filter))))
                    .OrderBy(c=>JsonExtensions.JsonValue(c.BrandTitle, header.Language))
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new BrandFormDto()
                    {
                        BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                        BrandId = x.BrandId
                    }).AsNoTracking().ToListAsync();
                }
                else if (token.Rule == UserGroupEnum.Seller)
                {
                    var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TBrand, bool>>)(x => x.TCategoryBrand.Any(p => p.FkCategory.CategoryPath.Contains("/" + k + "/"))));

                    data = await _context.TBrand
                    .Where(x => (pagination.Active == true ? x.IsAccepted == true : true) &&
                    (pagination.ChildIds.Count > 0 ? x.TCategoryBrand.Any(t => pagination.ChildIds.Contains(t.FkCategoryId)) : true)
                    &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(pagination.Filter))))
                    .WhereAny(predicates.ToArray())
                    .OrderBy(c=>JsonExtensions.JsonValue(c.BrandTitle, header.Language))
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Select(x => new BrandFormDto()
                    {
                        BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                        BrandId = x.BrandId
                    }).AsNoTracking().ToListAsync();
                }


                /// اصافه کردن دیتا بایند شده در سمت کلاینت به دراپ داون به لیست
                if (!String.IsNullOrWhiteSpace(pagination.valueId) && String.IsNullOrWhiteSpace(pagination.Filter) && (pagination.PageNumber == 1))
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    var notExistDataId = idList.Where(p => data.All(p2 => p2.BrandId.ToString() != p)).ToList();
                    if (notExistDataId.Count > 1 || (notExistDataId.Count == 1 && !string.IsNullOrWhiteSpace(notExistDataId[0])))
                    {
                        var oldData = await _context.TBrand.Where(x => idList.Contains(x.BrandId.ToString()))
                        .Select(x => new BrandFormDto()
                        {
                            BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                            BrandId = x.BrandId
                        })
                        .AsNoTracking().ToListAsync();
                        if (oldData.Count > 0)
                        {
                            data.AddRange(oldData);
                        }
                    }
                }
                if (pagination.PageNumber > 1 && !string.IsNullOrWhiteSpace(pagination.valueId))
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    data = data.Where(b => !idList.Contains(b.BrandId.ToString())).ToList();
                }
                ///////////////////////
                return data;

            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<int> GetBrandListCount(PaginationDto pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TBrand.CountAsync(x => (pagination.Active == true ? x.IsAccepted == true : true) &&
                    (pagination.ChildIds.Count > 0 ? x.TCategoryBrand.Any(t => pagination.ChildIds.Contains(t.FkCategoryId)) : true)
                    &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(pagination.Filter))))

                   ;
                }
                else
                {
                    var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TBrand, bool>>)(x => x.TCategoryBrand.Any(p => p.FkCategory.CategoryPath.Contains("/" + k + "/"))));

                    return await _context.TBrand
                    .WhereAny(predicates.ToArray())
                    .CountAsync(x => (pagination.Active == true ? x.IsAccepted == true : true) &&
                    (pagination.ChildIds.Count > 0 ? x.TCategoryBrand.Any(t => pagination.ChildIds.Contains(t.FkCategoryId)) : true)
                    &&
                    (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(pagination.Filter))))

                    ;
                }

            }
            catch (System.Exception)
            {
                return 0;
            }
        }
        public async Task<List<GuaranteeFormDto>> GetGuarantee(List<int> categoryId, bool active)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TGuarantee.Where(x => (active == true ? x.IsAccepted == true : true) &&
                    (categoryId.Count > 0 ? x.TCategoryGuarantee.Any(t => categoryId.Contains(t.FkCategoryId)) : true))
                    .OrderBy(c=>JsonExtensions.JsonValue(c.GuaranteeTitle, header.Language))
                    .Select(x => new GuaranteeFormDto()
                    {
                        GuaranteeTitle = JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language),
                        GuaranteeId = x.GuaranteeId
                    }).AsNoTracking().ToListAsync();
                }
                else
                {
                    var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TGuarantee, bool>>)(x => x.TCategoryGuarantee.Any(p => p.FkCategory.CategoryPath.Contains("/" + k + "/"))));
                    return await _context.TGuarantee.Where(x => (active == true ? x.IsAccepted == true : true) &&
                    (categoryId.Count > 0 ? x.TCategoryGuarantee.Any(t => categoryId.Contains(t.FkCategoryId)) : true))
                    .WhereAny(predicates.ToArray())
                    .OrderBy(c=>JsonExtensions.JsonValue(c.GuaranteeTitle, header.Language))
                    .Select(x => new GuaranteeFormDto()
                    {
                        GuaranteeTitle = JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language),
                        GuaranteeId = x.GuaranteeId
                    }).AsNoTracking().ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<TransactionStatusDto>> GetTransactionStatus()
        {
            try
            {
                return await _context.TTransactionStatus
                .Select(x => new TransactionStatusDto()
                {
                    StatusTitle = JsonExtensions.JsonValue(x.StatusTitle, header.Language),
                    StatusId = x.StatusId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<MeasurementUnitDto>> GetMeasurementUnit()
        {
            try
            {
                return await _context.TMeasurementUnit
                .Select(x => new MeasurementUnitDto()
                {
                    UnitTitle = JsonExtensions.JsonValue(x.UnitTitle, header.Language),
                    UnitId = x.UnitId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<PaymentMethodFormDto>> GetPaymentMethod(bool active)
        {
            try
            {

                var payments = await _context.TPaymentMethod
                .Where(x => active == true ? x.Active == true : true)
                .Select(x => new PaymentMethodFormDto()
                {
                    MethodTitle = JsonExtensions.JsonValue(x.MethodTitle, header.Language),
                    MethodId = x.MethodId,
                    MethodImageUrl = x.MethodImageUrl
                }).AsNoTracking().ToListAsync();

                return payments ;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<DiscountCouponCodeTypeDto>> GetDiscountCouponCodeType()
        {
            try
            {
                return await _context.TDiscountCouponCodeType
                .Select(x => new DiscountCouponCodeTypeDto()
                {
                    CodeTypeTitle = JsonExtensions.JsonValue(x.CodeTypeTitle, header.Language),
                    CodeTypeId = x.CodeTypeId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<DiscountPlanTypeDto>> GetDiscountPlanType()
        {
            try
            {
                return await _context.TDiscountPlanType
                .Select(x => new DiscountPlanTypeDto()
                {
                    TypeTitle = JsonExtensions.JsonValue(x.TypeTitle, header.Language),
                    TypeId = x.TypeId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<DiscountRangeTypeDto>> GetDiscountRangeType()
        {
            try
            {
                return await _context.TDiscountRangeType
                .Select(x => new DiscountRangeTypeDto()
                {
                    RangeTypeTitle = JsonExtensions.JsonValue(x.RangeTypeTitle, header.Language),
                    RangeTypeId = x.RangeTypeId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<DiscountTypeDto>> GetDiscountType()
        {
            try
            {
                return await _context.TDiscountType
                .Select(x => new DiscountTypeDto()
                {
                    DiscountTypeTitle = JsonExtensions.JsonValue(x.DiscountTypeTitle, header.Language),
                    DiscountTypeId = x.DiscountTypeId
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<VarietyFormGetDto>> GetVarietyList(int goodsId)
        {
            try
            {
                return await _context.TGoodsProvider
                .Where(x => x.FkGoodsId == goodsId && (token.Rule == UserGroupEnum.Seller ? x.FkShopId == token.Id : true))
                .Include(x => x.TGoodsVariety).ThenInclude(t => t.FkVariationParameter)
                .Include(x => x.TGoodsVariety).ThenInclude(t => t.FkVariationParameterValue)
                .Select(x => new VarietyFormGetDto()
                {
                    GoodsId = x.FkGoodsId,
                    ProviderId = x.ProviderId,
                    Variety = x.TGoodsVariety.Select(t => new GoodsVarietyGetDto()
                    {
                        FkGoodsId = t.FkGoodsId,
                        FkProviderId = t.FkProviderId,
                        FkVariationParameterId = t.FkVariationParameterId,
                        FkVariationParameterValueId = t.FkVariationParameterValueId,
                        ImageUrl = t.ImageUrl,
                        ParameterTitle = JsonExtensions.JsonValue(t.FkVariationParameter.ParameterTitle, header.Language),
                        ValueTitle = JsonExtensions.JsonValue(t.FkVariationParameterValue.Value, header.Language),
                        VarietyId = t.VarietyId,
                    }).ToList()
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<List<SpecialSellPlanDto>> GetSpecialSellPlan(FormPagination pagination, bool active)
        {
            try
            {
                var data = await _context.TDiscountPlan
                .Where(x => (token.Rule == UserGroupEnum.Seller ? x.FkShopId == token.Id : true) &&
                (string.IsNullOrWhiteSpace(pagination.Filter) ? true : JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Filter)) &&
                 x.FkPlanTypeId == (int)PlanTypeEnum.SpecialSell && (active == true ? x.Status == true : true))
                .OrderByDescending(x => x.PlanId)
                .AsNoTracking().Select(coupon => new SpecialSellPlanDto
                {
                    PlanId = coupon.PlanId,
                    PlanTitle = JsonExtensions.JsonValue(coupon.Title, header.Language),
                }).ToListAsync();

                /// اصافه کردن دیتا بایند شده در سمت کلاینت به دراپ داون به لیست
                // if (!String.IsNullOrWhiteSpace(pagination.valueId) && String.IsNullOrWhiteSpace(pagination.Filter) && pagination.PageNumber == 1)
                // {
                //     var idList = pagination.valueId.Split(',').ToList();
                //     var notExistDataId = idList.Where(p => data.All(p2 => p2.PlanId.ToString() != p)).ToList();
                //     if (notExistDataId.Count > 1 || (notExistDataId.Count == 1 && !string.IsNullOrWhiteSpace(notExistDataId[0])))
                //     {
                //         var oldData = await _context.TDiscountPlan.Where(x => idList.Contains(x.PlanId.ToString()))
                //         .Select(coupon => new SpecialSellPlanDto
                //         {
                //             PlanId = coupon.PlanId,
                //             PlanTitle = JsonExtensions.JsonValue(coupon.Title, header.Language),
                //         })
                //         .AsNoTracking().ToListAsync();
                //         if (oldData.Count > 0)
                //         {
                //             data.AddRange(oldData);
                //         }
                //     }
                // }
                // if (pagination.PageNumber > 1 && !string.IsNullOrWhiteSpace(pagination.valueId))
                // {
                //     var idList = pagination.valueId.Split(',').ToList();
                //     data = data.Where(b => !idList.Contains(b.PlanId.ToString())).ToList();
                // }
                ///////////////////////

                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<SpecialSellPlanDto>> GetDiscountCodePlan(FormPagination pagination, bool active)
        {
            try
            {
                var data = await _context.TDiscountPlan
                .Where(x => (token.Rule == UserGroupEnum.Seller ? x.FkShopId == token.Id : true) &&
                (string.IsNullOrWhiteSpace(pagination.Filter) ? true : JsonExtensions.JsonValue(x.Title, header.Language).Contains(pagination.Filter)) &&
                 x.FkPlanTypeId == (int)PlanTypeEnum.DiscountCode && (active == true ? x.Status == true : true))
                .OrderByDescending(x => x.PlanId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .AsNoTracking().Select(coupon => new SpecialSellPlanDto
                {
                    PlanId = coupon.PlanId,
                    PlanTitle = JsonExtensions.JsonValue(coupon.Title, header.Language),
                }).ToListAsync();

                if (!String.IsNullOrWhiteSpace(pagination.valueId) && String.IsNullOrWhiteSpace(pagination.Filter) && pagination.PageNumber == 1)
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    var notExistDataId = idList.Where(p => data.All(p2 => p2.PlanId.ToString() != p)).ToList();
                    if (notExistDataId.Count > 1 || (notExistDataId.Count == 1 && !string.IsNullOrWhiteSpace(notExistDataId[0])))
                    {
                        var oldData = await _context.TDiscountPlan.Where(x => idList.Contains(x.PlanId.ToString()))
                        .Select(coupon => new SpecialSellPlanDto
                        {
                            PlanId = coupon.PlanId,
                            PlanTitle = JsonExtensions.JsonValue(coupon.Title, header.Language),
                        })
                        .AsNoTracking().ToListAsync();
                        if (oldData.Count > 0)
                        {
                            data.AddRange(oldData);
                        }
                    }
                }
                if (pagination.PageNumber > 1 && !string.IsNullOrWhiteSpace(pagination.valueId))
                {
                    var idList = pagination.valueId.Split(',').ToList();
                    data = data.Where(b => !idList.Contains(b.PlanId.ToString())).ToList();
                }


                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<CategoryFormListDto>> ParentAcitveCategory()
        {
            return await _context.TCategory
                .Where(x => x.FkParentId == null && x.IsActive)
                .Select(x => new CategoryFormListDto()
                {
                    CategoryId = x.CategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                    IconUrl = x.IconUrl,
                    CategoryPath = x.CategoryPath,
                    Parents = null
                }).ToListAsync();
        }

        public async Task<List<ReturningReasonFormDto>> GetReturningReason()
        {
            try
            {
                return await _context.TReturningReason.Where(x => x.Status == true)
                .Select(x => new ReturningReasonFormDto()
                {
                    ReasonId = x.ReasonId,
                    ReasonTitle = JsonExtensions.JsonValue(x.ReasonTitle, header.Language),
                    ReturnCondition = JsonExtensions.JsonValue(x.ReturnCondition, header.Language),
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<OrderCancelingReasonDto>> ActiveCancelingReason()
        {
            try
            {
                return await _context.TOrderCancelingReason.Where(x => x.Status == true)
                .Select(x => new OrderCancelingReasonDto()
                {
                    ReasonId = x.ReasonId,
                    Status = x.Status,
                    ReasonTitle = JsonExtensions.JsonValue(x.ReasonTitle, header.Language)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ReturningActionDto>> GetReturningAction()
        {
            try
            {
                return await _context.TReturningAction
                .Select(x => new ReturningActionDto()
                {
                    ReturningTypeId = x.ReturningTypeId,
                    ReturningTypeTitle = JsonExtensions.JsonValue(x.ReturningTypeTitle, header.Language),
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<MessageUserFilterDto>> GetUsers(PaginationUserDto pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TUser
                    .Include(x => x.FkCustumer)
                    .Include(x => x.FkShop)
                    .Where(x =>
                    x.FkUserGroupId != Guid.Parse(GroupTypes.Admin) &&
                    (string.IsNullOrWhiteSpace(pagination.Email) ? true : (x.FkCustumer.Email.Contains(pagination.Email) || x.FkShop.Email.Contains(pagination.Email))) &&
                    (string.IsNullOrWhiteSpace(pagination.Phone) ? true : (x.FkCustumer.MobileNumber.Contains(pagination.Phone) || x.FkShop.Phone.Contains(pagination.Phone))) &&
                    (string.IsNullOrWhiteSpace(pagination.Name) ? true : (x.FkCustumer.Name.Contains(pagination.Name) || x.FkCustumer.Family.Contains(pagination.Name) || x.FkShop.StoreName.Contains(pagination.Name)))
                    )
                    .OrderByDescending(x => x.UserId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkUserGroup)
                    .Select(x => new MessageUserFilterDto()
                    {
                        UserId = x.UserId,
                        Type = x.FkUserGroup.UserGroupTitle,
                        Name = (x.FkCustumer != null ? (x.FkCustumer.Name + " " + x.FkCustumer.Family) : (x.FkShop != null ? (x.FkShop.StoreName) : ("Admin"))),
                    })
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    return await _context.TUser
                    .Include(x => x.FkCustumer)
                    .Include(x => x.FkCustumer).ThenInclude(x => x.TOrder).ThenInclude(t => t.TOrderItem)
                    .Where(x =>
                    x.FkCustumerId != null &&
                    (x.FkCustumer.TOrder.Any(t => t.TOrderItem.Any(i => i.FkShopId == token.Id && i.FkStatusId != (int)OrderStatusEnum.Cart))) && x.FkCustumerId == (int)CustomerTypeEnum.Unknown &&
                    (string.IsNullOrWhiteSpace(pagination.Email) ? true : (x.FkCustumer.Email.Contains(pagination.Email))) &&
                    (string.IsNullOrWhiteSpace(pagination.Phone) ? true : (x.FkCustumer.MobileNumber.Contains(pagination.Phone))) &&
                    (string.IsNullOrWhiteSpace(pagination.Name) ? true : (x.FkCustumer.Name.Contains(pagination.Name) || x.FkCustumer.Family.Contains(pagination.Name)))
                    )
                    .OrderByDescending(x => x.UserId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkUserGroup)
                    .Select(x => new MessageUserFilterDto()
                    {
                        UserId = x.UserId,
                        Type = x.FkUserGroup.UserGroupTitle,
                        Name = (x.FkCustumer != null ? (x.FkCustumer.Name + " " + x.FkCustumer.Family) : (x.FkShop != null ? (x.FkShop.StoreName) : ("Admin"))),
                    })
                    .AsNoTracking().ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetUsersCount(PaginationUserDto pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TUser
                    .Include(x => x.FkCustumer)
                    .Include(x => x.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    x.FkUserGroupId != Guid.Parse(GroupTypes.Admin) &&
                    (string.IsNullOrWhiteSpace(pagination.Email) ? true : (x.FkCustumer.Email.Contains(pagination.Email) || x.FkShop.Email.Contains(pagination.Email))) &&
                    (string.IsNullOrWhiteSpace(pagination.Phone) ? true : (x.FkCustumer.MobileNumber.Contains(pagination.Phone) || x.FkShop.Phone.Contains(pagination.Phone))) &&
                    (string.IsNullOrWhiteSpace(pagination.Name) ? true : (x.FkCustumer.Name.Contains(pagination.Name) || x.FkCustumer.Family.Contains(pagination.Name) || x.FkShop.StoreName.Contains(pagination.Name)))
                    );
                }
                else
                {
                    return await _context.TUser
                    .Include(x => x.FkCustumer)
                    .Include(x => x.FkCustumer).ThenInclude(x => x.TOrder).ThenInclude(t => t.TOrderItem)
                    .AsNoTracking()
                    .CountAsync(x =>
                    x.FkCustumerId != null &&
                    (x.FkCustumer.TOrder.Any(t => t.TOrderItem.Any(i => i.FkShopId == token.Id && i.FkStatusId != (int)OrderStatusEnum.Cart))) && x.FkCustumerId == (int)CustomerTypeEnum.Unknown &&
                    (string.IsNullOrWhiteSpace(pagination.Email) ? true : (x.FkCustumer.Email.Contains(pagination.Email))) &&
                    (string.IsNullOrWhiteSpace(pagination.Phone) ? true : (x.FkCustumer.MobileNumber.Contains(pagination.Phone))) &&
                    (string.IsNullOrWhiteSpace(pagination.Name) ? true : (x.FkCustumer.Name.Contains(pagination.Name) || x.FkCustumer.Family.Contains(pagination.Name)))
                    );
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<HelpTopicFromDto>> GetHelpTopic(bool active, int? FirstLevel)
        {
            try
            {
                return await _context.THelpTopic
                .Where(x => (active == true ? x.Status == true : true) && (FirstLevel == 0 ? x.FkTopicId != null : (FirstLevel == 1 ? x.FkTopicId == null : true)))
                .Select(x => new HelpTopicFromDto()
                {
                    IconUrl = x.IconUrl,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    TopicId = x.TopicId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<UserMenuDto>> GetFormMenu()
        {
            try
            {
                return await _context.TMenuItem.Include(x => x.InverseParent)
                .Where(x => x.ParentId == null)
                                .Select(x => new UserMenuDto()
                                {
                                    MenuId = x.MenuId,
                                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                                    ParentId = x.ParentId,
                                    Child = x.InverseParent.Select(b => new UserMenuDto()
                                    {
                                        MenuId = b.MenuId,
                                        Title = JsonExtensions.JsonValue(b.Title, header.Language),
                                        ParentId = b.ParentId
                                    }).ToList(),
                                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<OrderCallRequestStatusDto>> GetCallRequestStatus()
        {
            try
            {
                return await _context.TCallRequestStatus
                                .Select(x => new OrderCallRequestStatusDto()
                                {
                                    StatusId = x.StatusId,
                                    StatusTitle = JsonExtensions.JsonValue(x.StatusTitle, header.Language)

                                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<bool>> DeleteUserAccess(Guid UserId)
        {
            try
            {
                if (UserId == Guid.Parse(UserAdminId.ID))
                {
                    return new RepRes<bool>(Message.UserDeleting, false, false);
                }
                var userAccess = await _context.TUserAccessControl.Where(x => x.FkUserId == UserId).ToListAsync();
                var userSenderMessage = await _context.TMessage.Where(x => x.FkSenderId == UserId).ToListAsync();
                var userMessage = await _context.TMessageRecipient.Where(x => x.FkRecieverId == UserId).ToListAsync();
                var user = await _context.TUser.FindAsync(UserId);

                foreach (var item in userSenderMessage)
                {
                    var msg =  await _context.TMessageRecipient.Where(x => x.FkMessageId == item.MessageId).ToListAsync();
                   _context.TMessageRecipient.RemoveRange(msg); 
                }

                _context.TUserAccessControl.RemoveRange(userAccess);
                _context.TMessageRecipient.RemoveRange(userMessage);
                _context.TMessage.RemoveRange(userSenderMessage);
                _context.TUser.Remove(user);
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.UserDeleting, false, false);
            }
        }
    }
}