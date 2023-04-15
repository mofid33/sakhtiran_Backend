using MarketPlace.API.Data.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.ProductsStatistics;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Pagination;
using System.Linq.Expressions;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class ProductStatisticsRepository : IProductStatisticsRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMapper _mapper { get; set; }
        public ICategoryRepository _categoryRepository { get; }
        public IWareHouseRepository _wareHouseRepository { get; }

        public ProductStatisticsRepository(MarketPlaceDbContext context, IWareHouseRepository wareHouseRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICategoryRepository categoryRepository)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
            this._categoryRepository = categoryRepository;
            _mapper = mapper;
            _wareHouseRepository = wareHouseRepository;
        }

        public async Task<List<ProductStatisticsDto>> GetMostPopularGoods(ProductStatisticsPaginationDto pagination)
        {
            try
            {
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                var predicates = shopCategory.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/")));

                if (token.Rule == UserGroupEnum.Admin)
                {

                    return await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Where(x =>
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    )
                    .OrderByDescending(x => x.LikedCount)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkBrand)
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Include(x => x.FkOwner)
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsView)
                    .Include(x => x.TOrderItem)
                    .Select(x => new ProductStatisticsDto()
                    {
                        GoodsId = x.GoodsId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        Image = x.ImageUrl,
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        LikeCount = x.TGoodsLike.Count,
                        VisitCount = x.TGoodsView.Count,
                        SellCount = x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed),
                        RegisterdView = x.TGoodsView.Count(o => o.FkCustomerId != (int)CustomerTypeEnum.Unknown),
                        NotRegisterdView = x.TGoodsView.Count(o => o.FkCustomerId == (int)CustomerTypeEnum.Unknown),
                        BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                        Shops = x.FkOwnerId != null ? null : (x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => new ShopFormDto()
                        {
                            ShopId = v.ShopId,
                            ShopTitle = v.ShopTitle

                        }).ToList()),
                        ShopList = string.Join(",", x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => v.ShopTitle)),
                        Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else
                {
                    return await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Where(x =>
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    )
                    .OrderByDescending(x => x.LikedCount)
                    .WhereAny(predicates.ToArray())
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkBrand)
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Include(x => x.FkOwner)
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsView)
                    .Include(x => x.TOrderItem)
                    .Select(x => new ProductStatisticsDto()
                    {
                        GoodsId = x.GoodsId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        Image = x.ImageUrl,
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        LikeCount = x.TGoodsLike.Count,
                        VisitCount = x.TGoodsView.Count,
                        SellCount = x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed),
                        RegisterdView = x.TGoodsView.Count(o => o.FkCustomerId != (int)CustomerTypeEnum.Unknown),
                        NotRegisterdView = x.TGoodsView.Count(o => o.FkCustomerId == (int)CustomerTypeEnum.Unknown),
                        BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                        Shops = x.FkOwnerId != null ? null : (x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => new ShopFormDto()
                        {
                            ShopId = v.ShopId,
                            ShopTitle = v.ShopTitle

                        }).ToList()),
                        ShopList = string.Join(",", x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => v.ShopTitle)),
                        Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ProductStatisticsDto>> GetMostVisitedGoods(ProductStatisticsPaginationDto pagination)
        {
            try
            {
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                var predicates = shopCategory.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/")));
                if (token.Rule == UserGroupEnum.Admin)
                {

                    return await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Where(x =>
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    )
                    .OrderByDescending(x => x.ViewCount)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkBrand)
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Include(x => x.FkOwner)
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsView)
                    .Include(x => x.TOrderItem)
                    .Select(x => new ProductStatisticsDto()
                    {
                        GoodsId = x.GoodsId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        Image = x.ImageUrl,
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        LikeCount = x.TGoodsLike.Count,
                        VisitCount = x.TGoodsView.Count,
                        SellCount = x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed),
                        RegisterdView = x.TGoodsView.Count(o => o.FkCustomerId != (int)CustomerTypeEnum.Unknown),
                        NotRegisterdView = x.TGoodsView.Count(o => o.FkCustomerId == (int)CustomerTypeEnum.Unknown),
                        BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                        Shops = x.FkOwnerId != null ? null : (x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => new ShopFormDto()
                        {
                            ShopId = v.ShopId,
                            ShopTitle = v.ShopTitle

                        }).ToList()),
                        ShopList = string.Join(",", x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => v.ShopTitle)),
                        Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else
                {
                    return await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Where(x =>
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    )
                    .OrderByDescending(x => x.ViewCount)
                    .WhereAny(predicates.ToArray())
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkBrand)
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Include(x => x.FkOwner)
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsView)
                    .Include(x => x.TOrderItem)
                    .Select(x => new ProductStatisticsDto()
                    {
                        GoodsId = x.GoodsId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        Image = x.ImageUrl,
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        LikeCount = x.TGoodsLike.Count,
                        VisitCount = x.TGoodsView.Count,
                        SellCount = x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed),
                        RegisterdView = x.TGoodsView.Count(o => o.FkCustomerId != (int)CustomerTypeEnum.Unknown),
                        NotRegisterdView = x.TGoodsView.Count(o => o.FkCustomerId == (int)CustomerTypeEnum.Unknown),
                        BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                        Shops = x.FkOwnerId != null ? null : (x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => new ShopFormDto()
                        {
                            ShopId = v.ShopId,
                            ShopTitle = v.ShopTitle

                        }).ToList()),
                        ShopList = string.Join(",", x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => v.ShopTitle)),
                        Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public async Task<List<ProductStatisticsDto>> GetBestSellerGoods(ProductStatisticsPaginationDto pagination)
        {
            try
            {
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                var predicates = shopCategory.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/")));
                if (token.Rule == UserGroupEnum.Admin)
                {

                    return await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Include(x => x.TOrderItem)
                    .Where(x => x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed) != 0 &&
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    )
                    .OrderByDescending(x => x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed))
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkBrand)
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Include(x => x.FkOwner)
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsView)
                    .Select(x => new ProductStatisticsDto()
                    {
                        GoodsId = x.GoodsId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        Image = x.ImageUrl,
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        LikeCount = x.TGoodsLike.Count,
                        VisitCount = x.TGoodsView.Count,
                        SellCount = x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed),
                        RegisterdView = x.TGoodsView.Count(o => o.FkCustomerId != (int)CustomerTypeEnum.Unknown),
                        NotRegisterdView = x.TGoodsView.Count(o => o.FkCustomerId == (int)CustomerTypeEnum.Unknown),
                        BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                        Shops = x.FkOwnerId != null ? null : (x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => new ShopFormDto()
                        {
                            ShopId = v.ShopId,
                            ShopTitle = v.ShopTitle

                        }).ToList()),
                        ShopList = string.Join(",", x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => v.ShopTitle)),
                        Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else
                {

                    return await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Include(x => x.TOrderItem)
                    .Where(x => x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed) != 0 &&
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    )
                    .WhereAny(predicates.ToArray())
                    .OrderByDescending(x => x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed))
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkBrand)
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Include(x => x.FkOwner)
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsView)
                    .Select(x => new ProductStatisticsDto()
                    {
                        GoodsId = x.GoodsId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        Image = x.ImageUrl,
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        LikeCount = x.TGoodsLike.Count,
                        VisitCount = x.TGoodsView.Count,
                        SellCount = x.TOrderItem.Count(o => o.FkStatusId == (int)OrderStatusEnum.Completed),
                        RegisterdView = x.TGoodsView.Count(o => o.FkCustomerId != (int)CustomerTypeEnum.Unknown),
                        NotRegisterdView = x.TGoodsView.Count(o => o.FkCustomerId == (int)CustomerTypeEnum.Unknown),
                        BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                        Shops = x.FkOwnerId != null ? null : (x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => new ShopFormDto()
                        {
                            ShopId = v.ShopId,
                            ShopTitle = v.ShopTitle

                        }).ToList()),
                        ShopList = string.Join(",", x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => v.ShopTitle)),
                        Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public async Task<int> CountProductStatisticsGoods(ProductStatisticsPaginationDto pagination)
        {
            try
            {
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                var predicates = shopCategory.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/")));
                if (token.Rule == UserGroupEnum.Admin)
                {

                    return await _context.TGoods
                    .CountAsync(x =>
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    );
                }
                else
                {
                    return await _context.TGoods
                   .WhereAny(predicates.ToArray())
                   .CountAsync(x =>
                   (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                    && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                   && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                   && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                   && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                   );
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<int> CountBestSellerGoods(ProductStatisticsPaginationDto pagination)
        {
            try
            {
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                var predicates = shopCategory.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/")));
                if (token.Rule == UserGroupEnum.Admin)
                {

                    return await _context.TGoods
                    .CountAsync(x => x.SaleCount != 0 &&
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    );
                }
                else
                {

                    return await _context.TGoods
                    .WhereAny(predicates.ToArray())
                    .CountAsync(x => x.SaleCount != 0 &&
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    );
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<ProductStatisticsDetailsDto>> GetGoodsCustomerLikeAndView(PaginationFormDto pagination, string type)
        {
            try
            {
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                if (type == "like")
                {
                    var predicates = shopCategory.Select(k => (Expression<Func<TGoodsLike, bool>>)(x => x.FkGoods.FkCategory.CategoryPath.Contains("/" + k + "/")));

                    if (token.Rule == UserGroupEnum.Admin)
                    {
                        return await _context.TGoodsLike
                           .Include(t => t.FkGoods)
                           .Include(t => t.FkCustomer)
                           .Where(c => c.FkGoodsId == pagination.Id
                           && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                           )
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                          .Select(x => new ProductStatisticsDetailsDto()
                          {
                              CustomerName = x.FkCustomer.Name + " " + x.FkCustomer.Family,
                              CustomerId = x.FkCustomer.CustomerId,
                              Date = Extentions.PersianDateString(x.LikeDate)
                          })
                          .AsNoTracking()
                          .ToListAsync();
                    }
                    else
                    {
                        return await _context.TGoodsLike
                            .Include(t => t.FkGoods)
                            .ThenInclude(b => b.FkCategory)
                            .Include(t => t.FkCustomer)
                            .Where(c => c.FkGoodsId == pagination.Id
                            && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                            )
                            .WhereAny(predicates.ToArray())
                          .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                            .Select(x => new ProductStatisticsDetailsDto()
                            {
                                CustomerName = x.FkCustomer.Name + " " + x.FkCustomer.Family,
                                CustomerId = x.FkCustomer.CustomerId,
                                Date = Extentions.PersianDateString(x.LikeDate)
                            })
                            .AsNoTracking()
                            .ToListAsync();
                    }
                }
                else
                {
                    var predicatesView = shopCategory.Select(k => (Expression<Func<TGoodsView, bool>>)(x => x.FkGoods.FkCategory.CategoryPath.Contains("/" + k + "/")));

                    if (token.Rule == UserGroupEnum.Admin)
                    {
                        return await _context.TGoodsView
                         .Include(t => t.FkGoods)
                        .ThenInclude(b => b.FkCategory)
                         .Include(t => t.FkCustomer)
                         .Where(c => c.FkGoodsId == pagination.Id
                         && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                         )
                        .Where(x => pagination.Type == (int)GoodCustomerViewTypeEnum.All ? true : pagination.Type == (int)GoodCustomerViewTypeEnum.Registered ? x.FkCustomerId != (int)CustomerTypeEnum.Unknown : x.FkCustomerId == (int)CustomerTypeEnum.Unknown)
                       .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                        .Select(x => new ProductStatisticsDetailsDto()
                        {
                            CustomerName = x.FkCustomer.CustomerId == (int)CustomerTypeEnum.Unknown ? x.IpAddress : (x.FkCustomer.Name + " " + x.FkCustomer.Family),
                            CustomerId = x.FkCustomer.CustomerId,
                            Date = Extentions.PersianDateString(x.ViewDate)
                        })
                        .AsNoTracking()
                        .ToListAsync();
                    }
                    else
                    {
                        return await _context.TGoodsView
                         .Include(t => t.FkGoods)
                        .ThenInclude(b => b.FkCategory)
                         .Include(t => t.FkCustomer)
                         .Where(c => c.FkGoodsId == pagination.Id
                         && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                         )
                        .Where(x => pagination.Type == (int)GoodCustomerViewTypeEnum.All ? true : pagination.Type == (int)GoodCustomerViewTypeEnum.Registered ? x.FkCustomerId != (int)CustomerTypeEnum.Unknown : x.FkCustomerId == (int)CustomerTypeEnum.Unknown)
                        .WhereAny(predicatesView.ToArray())
                       .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                        .Select(x => new ProductStatisticsDetailsDto()
                        {
                            CustomerName = x.FkCustomer.CustomerId == (int)CustomerTypeEnum.Unknown ? x.IpAddress : (x.FkCustomer.Name + " " + x.FkCustomer.Family),
                            CustomerId = x.FkCustomer.CustomerId,
                            Date = Extentions.PersianDateString(x.ViewDate)
                        })
                        .AsNoTracking()
                        .ToListAsync();
                    }
                }

            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<List<ProductStatisticsDetailsDto>> GetGoodsSellDetails(PaginationFormDto pagination)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var customerList = await _context.TOrderItem
                   .Include(t => t.FkOrder)
                   .ThenInclude(c => c.FkCustomer)
                   .Where(c => c.FkGoodsId == pagination.Id
                   && c.FkStatusId == (int)OrderStatusEnum.Completed
                   && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                   )
                   .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                  .Select(x => new ProductStatisticsDetailsDto()
                  {
                      CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                      CustomerId = x.FkOrder.FkCustomer.CustomerId,
                      Date = Extentions.PersianDateString((DateTime)x.ShippmentDate),
                      Price = decimal.Round((decimal)x.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                      Quantity = (int)x.ItemCount
                  })
                  .AsNoTracking()
                  .ToListAsync();
                return customerList;



            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetCountGoodsSellDetails(PaginationFormDto pagination)
        {
            try
            {
                return await _context.TOrderItem
                .Include(t => t.FkOrder)
                .CountAsync(c => c.FkGoodsId == pagination.Id
                       && c.FkStatusId == (int)OrderStatusEnum.Completed
                       && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                       );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }



        public async Task<List<ProductStatisticsDto>> GetNoSelleGoods(ProductStatisticsPaginationDto pagination)
        {
            try
            {
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                var predicates = shopCategory.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/")));
                if (token.Rule == UserGroupEnum.Admin)
                {

                    return await _context.TGoods
                    .Include(x => x.TGoodsProvider)
                    .Where(x => x.SaleCount == 0 &&
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    )
                    .OrderByDescending(x => x.GoodsId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.FkBrand)
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Include(x => x.FkOwner)
                    .Select(x => new ProductStatisticsDto()
                    {
                        GoodsId = x.GoodsId,
                        CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        Image = x.ImageUrl,
                        GoodsCode = x.GoodsCode,
                        SerialNumber = x.SerialNumber,
                        RegisterDate = Extentions.PersianDateString(x.RegisterDate),
                        LastUpdate = Extentions.PersianDateString((DateTime)x.LastUpdateDateTime),
                        BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                        Shops = x.FkOwnerId != null ? null : (x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => new ShopFormDto()
                        {
                            ShopId = v.ShopId,
                            ShopTitle = v.ShopTitle

                        }).ToList()),
                        ShopList = string.Join(",", x.TGoodsProvider.Select(t => new ShopFormDto()
                        {
                            ShopId = t.FkShop.ShopId,
                            ShopTitle = t.FkShop.StoreName
                        }).Distinct().Select(v => v.ShopTitle)),
                        Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else
                {
                    return await _context.TGoods
                   .Include(x => x.TGoodsProvider)
                   .Where(x => x.SaleCount == 0 &&
                   (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                    && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                   && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                   && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                   && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                   )
                   .OrderByDescending(x => x.GoodsId)
                   .WhereAny(predicates.ToArray())
                   .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                   .Include(x => x.FkBrand)
                   .Include(x => x.FkCategory)
                   .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                   .Include(x => x.FkOwner)
                   .Select(x => new ProductStatisticsDto()
                   {
                       GoodsId = x.GoodsId,
                       CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                       Title = JsonExtensions.JsonValue(x.Title, header.Language),
                       Image = x.ImageUrl,
                       GoodsCode = x.GoodsCode,
                       SerialNumber = x.SerialNumber,
                       RegisterDate = Extentions.PersianDateString(x.RegisterDate),
                       LastUpdate = Extentions.PersianDateString((DateTime)x.LastUpdateDateTime),
                       BrandTitle = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                       Shops = x.FkOwnerId != null ? null : (x.TGoodsProvider.Select(t => new ShopFormDto()
                       {
                           ShopId = t.FkShop.ShopId,
                           ShopTitle = t.FkShop.StoreName
                       }).Distinct().Select(v => new ShopFormDto()
                       {
                           ShopId = v.ShopId,
                           ShopTitle = v.ShopTitle

                       }).ToList()),
                       ShopList = string.Join(",", x.TGoodsProvider.Select(t => new ShopFormDto()
                       {
                           ShopId = t.FkShop.ShopId,
                           ShopTitle = t.FkShop.StoreName
                       }).Distinct().Select(v => v.ShopTitle)),
                       Shop = new ShopFormDto(x.FkOwner.ShopId, x.FkOwner.StoreName)
                   })
                   .AsNoTracking()
                   .ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public async Task<int> CountProductStatisticsNoSaleGoods(ProductStatisticsPaginationDto pagination)
        {
            try
            {
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                var predicates = shopCategory.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/")));
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TGoods
                    .CountAsync(x => x.SaleCount == 0 &&
                    (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                     && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                    && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                    && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                    && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                    );
                }
                else
                {
                    return await _context.TGoods
                   .WhereAny(predicates.ToArray())
                   .CountAsync(x => x.SaleCount == 0 &&
                   (pagination.CategoryId != 0 ? (x.FkCategoryId == pagination.CategoryId) : true)
                    && (token.Id != 0 ? (x.FkOwnerId == token.Id || x.IsCommonGoods == true) : true)
                   && (pagination.ProductId != 0 ? (x.GoodsId == pagination.ProductId) : true)
                   && (pagination.FromDate == (DateTime?)null ? true : (x.RegisterDate >= pagination.FromDate))
                   && (pagination.ToDate == (DateTime?)null ? true : (x.RegisterDate <= pagination.ToDate))
                   );
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        public async Task<int> GetCountGoodsCustomerLikeAndView(PaginationFormDto pagination, string type)
        {
            try
            {
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == token.Id).AsNoTracking().Select(x => x.FkCategoryId).ToListAsync();
                if (type == "like")
                {
                    var predicates = shopCategory.Select(k => (Expression<Func<TGoodsLike, bool>>)(x => x.FkGoods.FkCategory.CategoryPath.Contains("/" + k + "/")));

                    if (token.Rule == UserGroupEnum.Admin)
                    {
                        return await _context.TGoodsLike
                           .Include(t => t.FkGoods)
                           .CountAsync(c => c.FkGoodsId == pagination.Id
                           && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                           );
                    }
                    else
                    {
                        return await _context.TGoodsLike
                          .Include(t => t.FkGoods)
                          .ThenInclude(b => b.FkCategory)
                          .WhereAny(predicates.ToArray())
                          .CountAsync(c => c.FkGoodsId == pagination.Id
                          && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                          );
                    }
                }
                else
                {
                    var predicatesView = shopCategory.Select(k => (Expression<Func<TGoodsView, bool>>)(x => x.FkGoods.FkCategory.CategoryPath.Contains("/" + k + "/")));
                    if (token.Rule == UserGroupEnum.Admin)
                    {
                        return await _context.TGoodsView
                         .Include(t => t.FkGoods)
                         .Include(t => t.FkCustomer)
                         .Where(x => pagination.Type == (int)GoodCustomerViewTypeEnum.All ? true : pagination.Type == (int)GoodCustomerViewTypeEnum.Registered ? x.FkCustomerId != (int)CustomerTypeEnum.Unknown : x.FkCustomerId == (int)CustomerTypeEnum.Unknown)
                         .CountAsync(c => c.FkGoodsId == pagination.Id
                         && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                         );
                    }
                    else
                    {
                        return await _context.TGoodsView
                        .Include(t => t.FkGoods)
                        .ThenInclude(b => b.FkCategory)
                        .Include(t => t.FkCustomer)
                        .WhereAny(predicatesView.ToArray())
                         .Where(x => pagination.Type == (int)GoodCustomerViewTypeEnum.All ? true : pagination.Type == (int)GoodCustomerViewTypeEnum.Registered ? x.FkCustomerId != (int)CustomerTypeEnum.Unknown : x.FkCustomerId == (int)CustomerTypeEnum.Unknown)
                         .CountAsync(c => c.FkGoodsId == pagination.Id
                         && (token.Id != 0 ? (c.FkGoods.FkOwnerId == token.Id || c.FkGoods.IsCommonGoods == true) : true)
                         );
                    }
                }

            }
            catch (System.Exception)
            {
                return 0;
            }
        }









    }


}