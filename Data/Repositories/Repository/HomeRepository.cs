using MarketPlace.API.Data.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.WebSlider;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Brand;
using System.Linq.Expressions;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Setting;
using System.Globalization;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class HomeRepository : IHomeRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMapper _mapper { get; set; }
        public ICategoryRepository _categoryRepository { get; }
        public IWareHouseRepository _wareHouseRepository { get; }

        public HomeRepository(MarketPlaceDbContext context, IWareHouseRepository wareHouseRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICategoryRepository categoryRepository)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
            this._categoryRepository = categoryRepository;
            _mapper = mapper;
            _wareHouseRepository = wareHouseRepository;
        }

        public async Task<List<GoodsHomeDto>> GetNewGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                return await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (ids == null ? true : (x.FkCategory.CategoryPath.Contains("/" + ids + "/") && x.FkCategory.IsActive == true)) &&
                x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                )
                .OrderByDescending(x => x.RegisterDate)
                .Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,

                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),

                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,
                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetAllGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                return await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (ids == null ? true : (x.FkCategory.CategoryPath.Contains("/" + ids + "/") && x.FkCategory.IsActive == true)) &&
                x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                )
                .Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,

                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
              (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,

                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                 (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),

                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),

                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetMostLikesGoodsHome(int? ids, int number, DateTime fromDay, DateTime toDay, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                return await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TGoodsLike)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (ids == null ? true : (x.FkCategory.CategoryPath.Contains("/" + ids + "/") && x.FkCategory.IsActive == true)) &&
                x.TGoodsLike.Any(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay)))
                && x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                )
                .OrderByDescending(x => x.TGoodsLike.Count(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay))))
                .Take(number)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetMostExpensiveGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                var goods = await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (ids == null ? true : (x.FkCategory.CategoryPath.Contains("/" + ids + "/") && x.FkCategory.IsActive == true)) &&
                x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                )
                .Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
              (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderByDescending(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
                goods = goods.OrderByDescending(c => c.FinalPrice).ToList();
                return goods;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public async Task<List<GoodsHomeDto>> GetCheapestGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                var goods = await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (ids == null ? true : (x.FkCategory.CategoryPath.Contains("/" + ids + "/") && x.FkCategory.IsActive == true)) &&
                x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                ).Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
                goods = goods.OrderBy(c => c.FinalPrice).ToList();
                return goods;


            }
            catch (System.Exception)
            {
                return null;
            }

        }
        public async Task<List<GoodsHomeDto>> GetMostDiscountGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                var goods = await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (ids == null ? true : (x.FkCategory.CategoryPath.Contains("/" + ids + "/") && x.FkCategory.IsActive == true)) &&
                x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                ).Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
                goods = goods.OrderByDescending(c => c.DiscountPercentage).ToList();
                return goods;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetMostViewGoodsHome(int? ids, int number, DateTime fromDay, DateTime toDay, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                return await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TGoodsView)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (ids == null ? true : (x.FkCategory.CategoryPath.Contains("/" + ids + "/") && x.FkCategory.IsActive == true)) &&
                x.TGoodsView.Any(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay)))
                && x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                )
                .OrderByDescending(x => x.TGoodsView.Where(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay))).Count())
                .Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetSpecialOfferGoodsHome(int customerId, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                return await _context.TGoods
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (x.FkCategoryId == _context.TGoodsView.Include(t => t.FkGoods).OrderByDescending(t => t.ViewId).FirstOrDefault(t => t.FkCustomerId == customerId).FkGoods.FkCategoryId) &&
                x.IsAccepted == true && x.ToBeDisplayed == true &&
                x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active))
                .OrderByDescending(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true).Max(t => t.DiscountPercentage))
                .Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).ProviderId,
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetLastViewGoodsHome(int customerId, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo, string ipAddress)
        {
            try
            {
                return await _context.TGoods
                .Include(x => x.TGoodsView)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x => x.TGoodsView.Any(t => t.FkCustomerId == customerId || t.IpAddress == ipAddress) && x.IsAccepted == true && x.ToBeDisplayed == true &&
                x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active))
                .OrderByDescending(x => x.TGoodsView.OrderByDescending(t => t.ViewDate).FirstOrDefault(t => t.FkCustomerId == customerId).ViewDate)
                .Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).ProviderId,
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public async Task<List<GoodsHomeDto>> GetSpecialGoodsHome(List<int> ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                return await _context.TGoods
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                ids.Contains(x.GoodsId) &&
                x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                )
                .OrderByDescending(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Max(t => t.DiscountPercentage))
                .Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
              (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
              (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
            (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetSpecialSaleGoodsHome(int couponId, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                var goodsId = new List<int>();
                var categryIds = new List<int>();
                var allow = true;
                var shopId = 0;

                var plan = await _context.TDiscountPlan
                .Include(x => x.TDiscountCategory)
                .Include(x => x.TDiscountGoods)
                .FirstOrDefaultAsync(x => x.PlanId == couponId);
                if (plan != null)
                {
                    if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders)
                    {

                    }
                    else if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialCategory)
                    {
                        allow = plan.TDiscountCategory.Any(x => x.Allowed == true);
                        categryIds = plan.TDiscountCategory.Select(x => x.FkCategoryId).ToList();
                    }
                    else if (plan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialGoods)
                    {
                        allow = plan.TDiscountGoods.Any(x => x.Allowed == true);
                        goodsId = plan.TDiscountGoods.Select(x => x.FkGoodsId).ToList();
                    }
                    shopId = plan.FkShopId == null ? 0 : (int)plan.FkShopId;
                }
                var predicates = categryIds.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/") == allow));
                return await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .WhereAny(predicates.ToArray())
                .Where(x =>
                (goodsId.Count > 0 ? true : (goodsId.Contains(x.GoodsId) == allow)) &&
                x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (shopId == 0 ? true : t.FkShopId == shopId)
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                )
                .OrderByDescending(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (shopId == 0 ? true : t.FkShopId == shopId)
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Max(t => t.DiscountPercentage))
                .Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (shopId == 0 ? true : t.FkShopId == shopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (shopId == 0 ? true : t.FkShopId == shopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (shopId == 0 ? true : t.FkShopId == shopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (shopId == 0 ? true : t.FkShopId == shopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (shopId == 0 ? true : t.FkShopId == shopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (shopId == 0 ? true : t.FkShopId == shopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
              (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (shopId == 0 ? true : t.FkShopId == shopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (shopId == 0 ? true : t.FkShopId == shopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetCustomerLikeGoodsHome(int customerId, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                return await _context.TGoods
                .Include(x => x.TGoodsLike)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x => x.TGoodsLike.Any(t => t.FkCustomerId == customerId) && x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active))
                .OrderByDescending(x => x.TGoodsLike.OrderByDescending(t => t.LikeDate).FirstOrDefault(t => t.FkCustomerId == customerId).LikeDate)
                .Take(number)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Price / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Vatamount / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FinalPrice / rate),
                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,
                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).ProviderId,
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetMostSellGoodsHome(int? ids, int number, DateTime fromDay, DateTime toDay, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo)
        {
            try
            {
                return await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkOrder)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (ids == null ? true : (x.FkCategory.CategoryPath.Contains("/" + ids + "/") && x.FkCategory.IsActive == true)) &&
                x.TOrderItem.Any(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime <= toDay)))
                && x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true))
                )
                .OrderByDescending(x => x.TOrderItem.Where(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : ((t.FkOrder.PlacedDateTime) <= toDay))).Sum(t => t.ItemCount))
                .Take(number)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
               (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).Vatamount / rate),

                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true && (criteriaType == (int)DiscountTypeId.FixedDiscount ? ((criteriaFrom == null ? true : t.FinalPrice >= criteriaFrom) && (criteriaTo == null ? true : t.FinalPrice <= criteriaTo)) : true) &&
                (criteriaType == (int)DiscountTypeId.PercentDiscount ? ((criteriaFrom == null ? true : t.DiscountPercentage >= criteriaFrom) && (criteriaTo == null ? true : t.DiscountPercentage <= criteriaTo)) : true)).ProviderId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> FilterGoodsGetType1(WebsiteFilterDto filterDto, WebSliderGetDto slider, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, decimal rate, bool allow)
        {
            // slider
            try
            {
                var goods = new List<GoodsHomeDto>();
                if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostView)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsView)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsView.Any(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                      (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.TGoodsView.Count(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay))))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostLike)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsLike.Any(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                      (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.TGoodsLike.Count(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay))))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.Expensive)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                            (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.FinalPrice).ToList();
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.Cheapest)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)(decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                            (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderBy(c => c.FinalPrice).ToList();

                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostSeller)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TOrderItem).ThenInclude(t => t.FkOrder)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TOrderItem.Any(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                      (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.TOrderItem.Where(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : ((t.FkOrder.PlacedDateTime) <= toDay))).Sum(t => t.ItemCount))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                            (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.New)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                      (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.RegisterDate)
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                            (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.AllProduct)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                      (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostDiscount)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.DiscountPercentage).ToList();
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialSale)
                {
                    var predicates = catIds.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/") == allow));
                    goods = await _context.TGoods
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .WhereAny((catIds.Count > 0 ? predicates.ToArray() : null))
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (goodsIds.Count > 0 ? (goodsIds.Contains(x.GoodsId) == allow) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                      (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.TGoodsProvider.Max(t => t.DiscountPercentage))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                             (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                              (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }

                return goods;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> FilterGoodsGetType2(WebsiteFilterDto filterDto, List<int> catIds, decimal rate)
        {
            // category
            try
            {
                var goods = new List<GoodsHomeDto>();

                if ((int)OrderingEnum.Cheapest == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();

                    goods = goods.OrderBy(c => c.FinalPrice).ToList();
                }

                else if ((int)OrderingEnum.Expensive == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.FinalPrice).ToList();
                }
                else if ((int)OrderingEnum.MostDiscount == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.DiscountPercentage).ToList();

                }
                else if ((int)OrderingEnum.MostLike == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.LikedCount).ToList();
                }
                else if ((int)OrderingEnum.MostSeller == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)))
                    .ThenBy(x => x.SaleCount)
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if ((int)OrderingEnum.MostView == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)))
                    .ThenBy(x => x.ViewCount)
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        ViewCount = x.ViewCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,
                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.ViewCount).ToList();
                }
                else
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)))
                    .ThenBy(x => x.RegisterDate)
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                return goods;
            }
            catch (System.Exception)
            {
                return null;
            }
        }



        public async Task<List<GoodsHomeDto>> FilterGoodsGetType3(WebsiteFilterDto filterDto, WebModuleCollectionsGetDto module, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, decimal rate, bool allow)
        {
            try
            {
                var goods = new List<GoodsHomeDto>();
                if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostView)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsView)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsView.Any(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                      (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.TGoodsView.Count(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay))))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                           && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                           (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostLike)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsLike.Any(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                      (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.TGoodsLike.Count(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay))))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                           && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                           (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.Expensive)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.FinalPrice).ToList();

                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.Cheapest)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderBy(c => c.FinalPrice).ToList();

                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostSeller)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TOrderItem).ThenInclude(t => t.FkOrder)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TOrderItem.Any(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                      (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.TOrderItem.Where(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : ((t.FkOrder.PlacedDateTime) <= toDay))).Sum(t => t.ItemCount))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.New)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                      (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.RegisterDate)
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.AllProduct)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                      (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostDiscount)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.DiscountPercentage).ToList();

                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialSale)
                {
                    var predicates = catIds.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/") == allow));
                    goods = await _context.TGoods
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .WhereAny(predicates.ToArray())
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (goodsIds.Count > 0 ? (goodsIds.Contains(x.GoodsId) == allow) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                      (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)))
                    .ThenByDescending(x => x.TGoodsProvider.Max(t => t.DiscountPercentage))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                            && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                            (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                           && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                           (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }

                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialGoods)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (goodsIds.Contains(x.GoodsId)) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                      && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                      (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)))
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                             && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                             (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).Vatamount / rate),

                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                              && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                              (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }



                return goods;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> FilterGoodsGetType4(WebsiteFilterDto filterDto, decimal rate, List<int> catIds)
        {
            try
            {
                var goods = new List<GoodsHomeDto>();

                if ((int)OrderingEnum.Cheapest == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderBy(c => c.FinalPrice).ToList();

                }
                else if ((int)OrderingEnum.Expensive == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.FinalPrice).ToList();

                }
                else if ((int)OrderingEnum.MostDiscount == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.DiscountPercentage).ToList();

                }
                else if ((int)OrderingEnum.MostLike == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.LikedCount).ToList();

                }
                else if ((int)OrderingEnum.MostSeller == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)))
                    .ThenBy(x => x.SaleCount)
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else if ((int)OrderingEnum.MostView == filterDto.OrderByType)
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        ViewCount = x.ViewCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                    goods = goods.OrderByDescending(c => c.ViewCount).ToList();

                }
                else
                {
                    goods = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                    (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .OrderByDescending(x => x.TGoodsProvider.Any(t => t.HasInventory == true && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)))
                    .ThenBy(x => x.RegisterDate)
                    .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                    .Include(x => x.TGoodsLike)
                    .Select(x => new GoodsHomeDto()
                    {
                        GoodsId = x.GoodsId,
                        Title = JsonExtensions.JsonValue(x.Title, header.Language),
                        GoodsImage = x.ImageUrl,
                        GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                        ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                        SurveyCount = x.SurveyCount,
                        Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                        DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                        DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                        FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                        Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                        LikedCount = x.LikedCount,
                        SurveyScore = x.SurveyScore,
                        HaveVariation = x.HaveVariation,
                        SaleWithCall = x.SaleWithCall,
                        IsDownloadable = x.IsDownloadable,

                        InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                        ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenBy(t => t.FinalPrice).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                return goods;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<List<GoodsHomeDto>> FilterGoodsGetType5(WebsiteFilterDto filterDto, decimal rate)
        {
            try
            {
                var goods = new List<GoodsHomeDto>();

                goods = await _context.TGoods
                .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                 x.IsAccepted == true && x.ToBeDisplayed == true &&
                (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.DiscountPercentage > 0 && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                )
                .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).FinalPrice / rate),
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).Vatamount / rate),
                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    InventoryCount = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderByDescending(t => t.HasInventory == true).ThenByDescending(t => t.DiscountPercentage).FirstOrDefault(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)).ProviderId
                })
                .AsNoTracking()
                .ToListAsync();
                goods = goods.OrderByDescending(c => c.DiscountPercentage).ToList();

                return goods;
            }
            catch (System.Exception)
            {
                return null;
            }
        }




        public async Task<int> FilterGoodsGetType1Count(WebsiteFilterDto filterDto, WebSliderGetDto slider, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, bool allow)
        {
            try
            {
                var count = 0;
                if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostView)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsView)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsView.Any(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    );
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostLike)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsLike.Any(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    );
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.Expensive)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    );
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.Cheapest)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    );
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostSeller)
                {
                    count = await _context.TGoods
                    .Include(x => x.TOrderItem).ThenInclude(t => t.FkOrder)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TOrderItem.Any(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    );
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.New)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    );
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.AllProduct)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    );
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostDiscount)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    );
                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialSale)
                {
                    var predicates = catIds.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/") == allow));
                    count = await _context.TGoods
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .WhereAny(predicates.ToArray())
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (goodsIds.Count > 0 ? (goodsIds.Contains(x.GoodsId) == allow) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    );
                }

                return count;

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> MaxPriceGetType1(WebsiteFilterDto filterDto, WebSliderGetDto slider, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, decimal rate, bool allow)
        {
            try
            {
                var price = (decimal)0.0;
                if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostView)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsView)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsView.Any(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostLike)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsLike.Any(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Max(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.Expensive)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Max(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.Cheapest)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Max(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostSeller)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TOrderItem).ThenInclude(t => t.FkOrder)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TOrderItem.Any(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Max(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.New)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Max(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.AllProduct)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Max(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.MostDiscount)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Max(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (slider.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialSale)
                {//copon
                    var predicates = catIds.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/") == allow));
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .WhereAny(predicates.ToArray())
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (goodsIds.Count > 0 ? (goodsIds.Contains(x.GoodsId) == allow) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (slider.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((slider.CriteriaFrom == null ? true : t.FinalPrice >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.FinalPrice <= slider.CriteriaTo)) : true) &&
                    (slider.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((slider.CriteriaFrom == null ? true : t.DiscountPercentage >= slider.CriteriaFrom) && (slider.CriteriaTo == null ? true : t.DiscountPercentage <= slider.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Max(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }

                return price;

            }
            catch (System.Exception)
            {
                return (decimal)0.0;
            }
        }

        public async Task<int> FilterGoodsGetType2Count(WebsiteFilterDto filterDto, List<int> catIds)
        {
            try
            {
                return await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> MaxPriceGetType2(WebsiteFilterDto filterDto, List<int> catIds, decimal rate)
        {
            try
            {
                var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                return Extentions.DecimalRound(priceList.Max());
            }
            catch (System.Exception)
            {
                return (decimal)0.0;
            }
        }

        public async Task<int> FilterGoodsGetType3Count(WebsiteFilterDto filterDto, WebModuleCollectionsGetDto module, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, bool allow)
        {
            try
            {
                var count = 0;
                if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostView)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsView)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsView.Any(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostLike)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsLike.Any(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.Expensive)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.Cheapest)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostSeller)
                {
                    count = await _context.TGoods
                    .Include(x => x.TOrderItem).ThenInclude(t => t.FkOrder)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TOrderItem.Any(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.New)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.AllProduct)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialGoods)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (goodsIds.Contains(x.GoodsId)) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostDiscount)
                {
                    count = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialSale)
                {//copon
                    var predicates = catIds.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/") == allow));
                    count = await _context.TGoods
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .WhereAny(predicates.ToArray())
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (goodsIds.Count > 0 ? (goodsIds.Contains(x.GoodsId) == allow) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    );
                }
                return count;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> MaxPriceGetType3(WebsiteFilterDto filterDto, WebModuleCollectionsGetDto module, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, decimal rate, bool allow)
        {
            try
            {
                var price = (decimal)0.0;
                if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostView)
                {
                    var priceList = await _context.TGoods
                     .Include(x => x.TGoodsView)
                     .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                     .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                     .Where(x =>
                     (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                     (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                     (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                      x.IsAccepted == true && x.ToBeDisplayed == true &&
                     x.TGoodsView.Any(t => (fromDay == (DateTime?)null ? true : (t.ViewDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.ViewDate <= toDay))) &&
                     (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                     x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                     && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                     (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                     )
                     .AsNoTracking()
                     .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                     .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostLike)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsLike)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TGoodsLike.Any(t => (fromDay == (DateTime?)null ? true : (t.LikeDate >= fromDay)) && (toDay == (DateTime?)null ? true : (t.LikeDate <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.Expensive)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.Cheapest)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostSeller)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TOrderItem).ThenInclude(t => t.FkOrder)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    x.TOrderItem.Any(t => t.FkOrder.PaymentStatus == true && (fromDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime >= fromDay)) && (toDay == (DateTime?)null ? true : (t.FkOrder.PlacedDateTime <= toDay))) &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.New)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.AllProduct)
                {
                    var priceList = await _context.TGoods
                     .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                     .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                     .Where(x =>
                     (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                     (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                     (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                      x.IsAccepted == true && x.ToBeDisplayed == true &&
                     (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                     x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                     && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                     (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                     )
                     .AsNoTracking()
                     .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                     .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialGoods)
                {
                    var priceList = await _context.TGoods
                     .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                     .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                     .Where(x =>
                     (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                     (goodsIds.Contains(x.GoodsId)) &&
                     (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                      x.IsAccepted == true && x.ToBeDisplayed == true &&
                     (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                     x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                     && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                     (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                     )
                     .AsNoTracking()
                     .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                     .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());
                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.MostDiscount)
                {
                    var priceList = await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                else if (module.FkCollectionTypeId == (int)CollectionTypeEnum.SpecialSale)
                {
                    var predicates = catIds.Select(k => (Expression<Func<TGoods, bool>>)(x => x.FkCategory.CategoryPath.Contains("/" + k + "/") == allow));
                    var priceList = await _context.TGoods
                    .Include(x => x.FkCategory)
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .WhereAny(predicates.ToArray())
                    .Where(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (goodsIds.Count > 0 ? goodsIds.Contains(x.GoodsId) : true) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true)
                    && (module.CriteriaType == (int)DiscountTypeId.FixedDiscount ? ((module.CriteriaFrom == null ? true : t.FinalPrice >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.FinalPrice <= module.CriteriaTo)) : true) &&
                    (module.CriteriaType == (int)DiscountTypeId.PercentDiscount ? ((module.CriteriaFrom == null ? true : t.DiscountPercentage >= module.CriteriaFrom) && (module.CriteriaTo == null ? true : t.DiscountPercentage <= module.CriteriaTo)) : true))
                    )
                    .AsNoTracking()
                    .Select(x => x.TGoodsProvider.Where(t => (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                    .ToListAsync();
                    price = Extentions.DecimalRound(priceList.Max());

                }
                return price;

            }
            catch (System.Exception)
            {
                return (decimal)0.0;
            }
        }

        public async Task<int> FilterGoodsGetType4Count(WebsiteFilterDto filterDto, List<int> catIds)
        {
            try
            {
                return await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                    (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> MaxPriceGetType4(WebsiteFilterDto filterDto, decimal rate, List<int> catIds)
        {
            try
            {
                var priceList = await _context.TGoods
                     .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                     .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                     .Where(x =>
                     (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                     (catIds.Count > 0 ? catIds.Contains(x.FkCategoryId) : true) &&
                     (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                     (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                      x.IsAccepted == true && x.ToBeDisplayed == true &&
                     (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                     x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                     )
                     .AsNoTracking()
                     .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                     .ToListAsync();
                return Extentions.DecimalRound(priceList.Max());
            }
            catch (System.Exception)
            {
                return (decimal)0;
            }
        }


        public async Task<int> FilterGoodsGetType5Count(WebsiteFilterDto filterDto)
        {
            try
            {
                return await _context.TGoods
                    .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                    .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                    .AsNoTracking()
                    .CountAsync(x =>
                    (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                    (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                    (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                    (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                    x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.DiscountPercentage > 0 && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                    );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> MaxPriceGetType5(WebsiteFilterDto filterDto, decimal rate)
        {
            try
            {
                var priceList = await _context.TGoods
                     .Include(x => x.TGoodsSpecification).ThenInclude(i => i.TGoodsSpecificationOptions)
                     .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                     .Where(x =>
                     (filterDto.GoodsCreatedDay == 0 ? true : (x.RegisterDate > DateTime.Now.AddDays(-filterDto.GoodsCreatedDay))) &&
                     (string.IsNullOrWhiteSpace(filterDto.Search) ? true : (JsonExtensions.JsonValue(x.Title, header.Language).Contains(filterDto.Search))) &&
                     (filterDto.BrandId.Count > 0 ? filterDto.BrandId.Contains((int)x.FkBrandId) : true) &&
                      x.IsAccepted == true && x.ToBeDisplayed == true &&
                     (filterDto.OptionIds.Count > 0 ? x.TGoodsSpecification.Any(i => i.TGoodsSpecificationOptions.Any(t => filterDto.OptionIds.Contains(t.FkSpecOptionId))) : true) &&
                     x.TGoodsProvider.Any(t => (filterDto.JustExist ? t.HasInventory == true : true) && t.ToBeDisplayed == true && t.IsAccepted == true && t.DiscountPercentage > 0 && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && (filterDto.FromPrice != 0 ? t.FinalPrice >= filterDto.FromPrice : true) && (filterDto.ToPrice != 0 ? t.FinalPrice <= filterDto.ToPrice : true))
                     )
                     .AsNoTracking()
                     .Select(x => x.TGoodsProvider.Where(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.DiscountPercentage > 0 && (filterDto.ShopId == 0 ? true : t.FkShopId == filterDto.ShopId) && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Min(t => (decimal)t.FinalPrice / rate))
                     .ToListAsync();
                return Extentions.DecimalRound(priceList.Max());
            }
            catch (System.Exception)
            {
                return (decimal)0;
            }
        }


        public async Task<int> GetAllGoodsCountForSelling(int shopId)
        {
            try
            {
                return await _context.TGoods
                     .Include(x => x.TGoodsProvider)
                     .AsNoTracking()
                     .CountAsync(x =>
                     x.IsAccepted == true && x.ToBeDisplayed == true &&
                     x.TGoodsProvider.Any(i => i.ToBeDisplayed == true && i.IsAccepted == true && (shopId == 0 ? true : i.FkShopId == shopId))
                     );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<string> GetMobileDescriptionPageData(MobileDescriptionPageParams model)
        {
            try
            {
                if (model.Type == "goods")
                {
                    var goods = await _context.TGoods.FindAsync(model.GoodsId);
                    return JsonExtensions.JsonGet(goods.Description, header);
                }
                else if (model.Type == "article")
                {
                    var article = await _context.THelpArticle.FindAsync(model.ArticleId);
                    return JsonExtensions.JsonGet(article.Description, header);
                }
                else if (model.Type == "storeTerms")
                {
                    var shop = await _context.TShop.FirstOrDefaultAsync(x => x.StoreName == model.StoreName);
                    return JsonExtensions.JsonGet(shop.TermCondition, header);
                }
                else if (model.Type == "afterCreateStore")
                {
                    var setting = await _context.TSetting.FirstOrDefaultAsync();
                    return JsonExtensions.JsonGet(setting.RegistrationFinalMessage, header);
                }
                return null;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<GoodsDetailesDto> GoodsDetailsGet(int goodsId, int providerId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var setting = await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.ShopCalculateComment, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();

                CultureInfo ci = new CultureInfo("en-US");
                if (header.Language == "$." + LanguageEnum.Ar.ToString())
                {
                    ci = new CultureInfo("ar-BH");
                }
                if (header.Language == "$." + LanguageEnum.Fa.ToString())
                {
                    ci = new CultureInfo("fa-IR");
                }
                var goodsProvider = await _context.TGoodsProvider.Include(t => t.FkShop).Include(t => t.FkGoods).FirstOrDefaultAsync(g => g.ToBeDisplayed == true && g.IsAccepted == true && g.ProviderId == providerId && g.FkShop.FkStatusId == (int)ShopStatusEnum.Active);
                var goodsIsDownloadAble = goodsProvider.FkGoods.IsDownloadable;
                var timeCountry = await _context.TShippingOnCity.FirstOrDefaultAsync(c => c.FkCityId == goodsProvider.FkShop.FkCityId || c.FkProviceId == goodsProvider.FkShop.FkProvinceId);
                var postTimeoutDay = timeCountry == null ? 1 : timeCountry.PostTimeoutDay;
                var postId = goodsProvider.PostId ?? default(int);
                var postDay = DateTime.Now.AddDays((double)postTimeoutDay).ToString("dddd,MMMM dd", ci);
                var goods = await _context.TGoods
                .Include(s => s.FkBrand)
                .Include(s => s.FkCategory)
                .Include(s => s.TGoodsDocument)
                .Include(s => s.FkUnit)
                .Include(x => x.TGoodsLike)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkGuarantee)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop).ThenInclude(i => i.TShopActivityCity).ThenInclude(q => q.FkShippingMethod)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop).ThenInclude(i => i.FkCity)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop).ThenInclude(i => i.TShopComment)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameter)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameterValue)
                .Where(x => x.GoodsId == goodsId && x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(g => g.ToBeDisplayed == true && g.IsAccepted == true && g.ProviderId == providerId && g.FkShop.FkStatusId == (int)ShopStatusEnum.Active))
                .Select(x => new GoodsDetailesDto
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    Image = x.ImageUrl,
                    Category = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                    FkCategoryId = x.FkCategoryId,
                    ModelNumber = x.SerialNumber,
                    ShopDetailAccess = x.TCallRequest.Any(n => n.FkGoodsId == x.GoodsId && n.FkCustomerId == token.Id && n.FkGoodsProviderId == providerId),
                    Brand = x.FkBrand == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                    BrandId = x.FkBrand == null ? 0 : x.FkBrand.BrandId,
                    Description = x.Description,
                    UnitTitle = JsonExtensions.JsonValue(x.FkUnit.UnitTitle, header.Language),
                    Like = token.Id != 0 ? x.TGoodsLike.Any(t => t.FkCustomerId == token.Id) : false,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,

                    GoodsDocument = x.TGoodsDocument.Select(image => new GoodsDocumentDto
                    {
                        FkGoodsId = image.FkGoodsId,
                        DocumentUrl = image.DocumentUrl,
                        ImageId = image.ImageId
                    }).ToList(),
                    ShopCityId = (int)x.TGoodsProvider.FirstOrDefault(t => t.FkShopId == x.TGoodsProvider.FirstOrDefault(i => i.ProviderId == providerId).FkShopId).FkShop.FkCityId,
                    ShopCityTitle = JsonExtensions.JsonValue(x.TGoodsProvider.FirstOrDefault(t => t.FkShopId == x.TGoodsProvider.FirstOrDefault(i => i.ProviderId == providerId).FkShopId).FkShop.FkCity.CityTitle, header.Language),
                    DescriptionCalculateShopRate = setting.Description,
                    ShopCountryId = x.TGoodsProvider.FirstOrDefault(t => t.FkShopId == x.TGoodsProvider.FirstOrDefault(i => i.ProviderId == providerId).FkShopId).FkShop.FkCountryId,
                    ShopProvinceId = (int)x.TGoodsProvider.FirstOrDefault(t => t.FkShopId == x.TGoodsProvider.FirstOrDefault(i => i.ProviderId == providerId).FkShopId).FkShop.FkProvinceId,
                    GoodsProviderVarity = x.TGoodsProvider.Where(t => t.FkShopId == x.TGoodsProvider.FirstOrDefault(i => i.ProviderId == providerId).FkShopId && t.ToBeDisplayed == true && t.IsAccepted == true)
                    .Select(t => new HomeGoodsProviderDto()
                    {
                        ProviderId = t.ProviderId,
                        FkShopId = t.FkShopId,
                        FkGoodsId = t.FkGoodsId,
                        FkGuaranteeId = t.FkGuaranteeId,
                        GuaranteeTitle = t.FkGuarantee != null ? JsonExtensions.JsonValue(t.FkGuarantee.GuaranteeTitle, header.Language) : "",
                        HaveGuarantee = t.HaveGuarantee,
                        GuaranteeMonthDuration = t.GuaranteeMonthDuration,
                        Vatfree = t.Vatfree,
                        Price = Extentions.DecimalRound((decimal)t.Price / rate),
                        Vatamount = t.Vatamount == null ? (decimal)0.00 : Extentions.DecimalRound((decimal)t.Vatamount / rate),
                        PostTimeoutDayByShop = t.PostTimeoutDayByShop,
                        ReturningAllowed = t.ReturningAllowed,
                        MaxDeadlineDayToReturning = t.MaxDeadlineDayToReturning,
                        HasInventory = t.HasInventory,
                        InventoryCount = t.InventoryCount,
                        ShopTitle = t.FkShop.StoreName,
                        VendorUrl = t.FkShop.VendorUrlid,
                        Microstore = t.FkShop.Microstore,
                        ShopSurvey = t.FkShop.SurveyScore == null ? 4 : t.FkShop.SurveyScore,
                        ShopSurveyCount = t.FkShop.TShopComment.Count(s => s.IsAccepted == true) == 0 ? 1 : t.FkShop.TShopComment.Count(s => s.IsAccepted == true),
                        ShippingPossibilities = goodsIsDownloadAble ? 8 : (postId != 0 ?  postId : t.FkShop.TShopActivityCity.FirstOrDefault(i => (i.FkCityId == t.FkShop.FkCityId || i.FkProviceId == t.FkShop.FkProvinceId)) == null ? (int)PostMethodEnum.NotPossible :
                                  t.FkShop.TShopActivityCity.FirstOrDefault(i => (i.FkCityId == t.FkShop.FkCityId || i.FkProviceId == t.FkShop.FkProvinceId)).FkShippingMethodId),
                        ShippingDesc = t.FkShop.TShopActivityCity.FirstOrDefault(i => (i.FkCityId == t.FkShop.FkCityId || i.FkProviceId == t.FkShop.FkProvinceId)) == null ? null :
                                 JsonExtensions.JsonValue(t.FkShop.TShopActivityCity.FirstOrDefault(i => (i.FkCityId == t.FkShop.FkCityId || i.FkProviceId == t.FkShop.FkProvinceId)).FkShippingMethod.Description, header.Language),
                        ShippingImage = t.FkShop.TShopActivityCity.FirstOrDefault(i => (i.FkCityId == t.FkShop.FkCityId || i.FkProviceId == t.FkShop.FkProvinceId)) == null ? null :
                                 JsonExtensions.JsonValue(t.FkShop.TShopActivityCity.FirstOrDefault(i => (i.FkCityId == t.FkShop.FkCityId || i.FkProviceId == t.FkShop.FkProvinceId)).FkShippingMethod.Image, header.Language),
                        PostTimeoutDay = t.FkShop.TShopActivityCity.FirstOrDefault(i => (i.FkCityId == t.FkShop.FkCityId || i.FkProviceId == t.FkShop.FkProvinceId)) == null ? null :
                                ((postId != 0 ?  postId : t.FkShop.TShopActivityCity.FirstOrDefault(i => (i.FkCityId == t.FkShop.FkCityId || i.FkProviceId == t.FkShop.FkProvinceId)).FkShippingMethodId) == (int)ShippingMethodEnum.Podinis ?
                                postDay :
                                (DateTime.Now.AddDays((double)t.FkShop.TShopActivityCity.FirstOrDefault(i => (i.FkCityId == t.FkShop.FkCityId || i.FkProviceId == t.FkShop.FkProvinceId)).PostTimeoutDayByShop).ToString("dddd,dd MMMM", ci))),
                        DiscountAmount = Extentions.DecimalRound((decimal)t.DiscountAmount / rate),
                        DiscountPercentage = t.DiscountPercentage,
                        FinalPrice = Extentions.DecimalRound((decimal)t.FinalPrice / rate),
                        TGoodsVariety = t.TGoodsVariety.Select(i => new GoodsVarietyGetDto()
                        {
                            VarietyId = i.VarietyId,
                            FkProviderId = i.FkProviderId,
                            FkGoodsId = i.FkGoodsId,
                            FkVariationParameterId = i.FkVariationParameterId,
                            ParameterTitle = JsonExtensions.JsonValue(i.FkVariationParameter.ParameterTitle, header.Language),
                            FkVariationParameterValueId = i.FkVariationParameterValueId,
                            ValueTitle = JsonExtensions.JsonValue(i.FkVariationParameterValue.Value, header.Language),
                            ImageUrl = i.ImageUrl,
                            ValuesHaveImage = i.FkVariationParameter.ValuesHaveImage
                        }).ToList()
                    })
                    .ToList(),
                    LikedCount = x.LikedCount,
                    MetaDescription = JsonExtensions.JsonValue(x.MetaDescription, header.Language),
                    MetaKeywords = JsonExtensions.JsonValue(x.MetaKeywords, header.Language),
                    MetaTitle = JsonExtensions.JsonValue(x.MetaTitle, header.Language),
                    SurveyCount = x.SurveyCount,
                    SurveyScore = x.SurveyScore,
                    PageTitle = x.PageTitle,
                    ViewCount = x.ViewCount,
                    OtherProvider = x.TGoodsProvider.Where(g => g.ToBeDisplayed == true && g.IsAccepted == true && g.FkShop.FkStatusId == (int)ShopStatusEnum.Active).OrderBy(f => f.HasInventory).ThenBy(h => h.FinalPrice)
                      .Select(m => new HomeGoodsProviderDto()
                      {
                          ProviderId = m.ProviderId,
                          FkShopId = m.FkShopId,
                          FkGoodsId = m.FkGoodsId,
                          FkGuaranteeId = m.FkGuaranteeId,
                          GuaranteeTitle = m.FkGuarantee != null ? JsonExtensions.JsonValue(m.FkGuarantee.GuaranteeTitle, header.Language) : "",
                          HaveGuarantee = m.HaveGuarantee,
                          GuaranteeMonthDuration = m.GuaranteeMonthDuration,
                          Vatfree = m.Vatfree,
                          VendorUrl = m.FkShop.VendorUrlid,
                          Price = Extentions.DecimalRound((decimal)m.Price / rate),
                          Vatamount = m.Vatamount == null ? (decimal)0.00 : Extentions.DecimalRound((decimal)m.Vatamount / rate),
                          PostTimeoutDayByShop = m.PostTimeoutDayByShop,
                          ReturningAllowed = m.ReturningAllowed,
                          MaxDeadlineDayToReturning = m.MaxDeadlineDayToReturning,
                          HasInventory = m.HasInventory,
                          InventoryCount = m.InventoryCount,
                          ShopTitle = m.FkShop.StoreName,
                          Microstore = m.FkShop.Microstore,
                          DiscountAmount = Extentions.DecimalRound((decimal)m.DiscountAmount / rate),
                          DiscountPercentage = m.DiscountPercentage,
                          FinalPrice = Extentions.DecimalRound((decimal)m.FinalPrice / rate),
                          ShippingPossibilities = m.FkShop.ShippingPossibilities == true ? (int)PostMethodEnum.Market : (int)PostMethodEnum.Express
                      }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

                if (goods != null && goods.OtherProvider != null)
                {
                    var ShopIds = goods.OtherProvider.Where(x => x.FkShopId != goods.GoodsProviderVarity[0].FkShopId).Select(x => x.FkShopId).Distinct().ToList();
                    var otherProvider = new List<HomeGoodsProviderDto>();
                    foreach (var item in ShopIds)
                    {
                        otherProvider.Add(goods.OtherProvider.FirstOrDefault(x => x.FkShopId == item));
                    }
                    goods.OtherProvider = otherProvider;
                }
                goods.Description = JsonExtensions.JsonGet(goods.Description, header);
                return goods;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsHomeDto>> GetCustomerLike(int customerId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TGoods
                .Include(x => x.TGoodsLike)
                .Include(x => x.FkBrand)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x => x.TGoodsLike.Any(t => t.FkCustomerId == customerId) && x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active))
                .OrderByDescending(x => x.TGoodsLike.OrderByDescending(t => t.LikeDate).FirstOrDefault(t => t.FkCustomerId == customerId).LikeDate)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsBrand = JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    Price = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Price / rate),
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).DiscountPercentage,
                    DiscountAmount = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).DiscountAmount / rate),
                    FinalPrice = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FinalPrice / rate),
                    HaveGuarantee = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).HaveGuarantee,
                    ReturningAllowed = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).ReturningAllowed,
                    GuaranteeMonthDuration = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).GuaranteeMonthDuration,
                    StoreName = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active) == null ? "" :
                                x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FkShop.StoreName,
                    Vat = Extentions.DecimalRound((decimal)x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).Vatamount / rate),
                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,
                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).InventoryCount,
                    ShopHaveMicroStore = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FkShop.Microstore,
                    ShopUrl = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FkShop.VendorUrlid,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).ProviderId,
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<HomeSearchAutoComplete> GetSearchAutoComplete(string search)
        {
            try
            {
                var data = new HomeSearchAutoComplete();

                data.Category = await _context.TCategory.Where(x => x.IsActive == true && JsonExtensions.JsonValue(x.CategoryTitle, header.Language).Contains(search))
                .OrderByDescending(x => x.CategoryId)
                .Take(5)
                .Select(x => new CategoryFormGetDto()
                {
                    CategoryId = x.CategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language)
                })
                .AsNoTracking().ToListAsync();

                data.Brand = await _context.TBrand.Where(x => x.IsAccepted == true && JsonExtensions.JsonValue(x.BrandTitle, header.Language).Contains(search))
                .OrderByDescending(x => x.BrandId)
                .Take(5)
                .Select(x => new BrandFormDto()
                {
                    BrandId = x.BrandId,
                    BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language)
                })
                .AsNoTracking().ToListAsync();

                data.Goods = await _context.TGoods.Include(x => x.TGoodsProvider).Where(x =>
                  x.IsAccepted == true && x.ToBeDisplayed == true &&
                  x.TGoodsProvider.Any(i => i.ToBeDisplayed == true && i.IsAccepted == true)
                  && JsonExtensions.JsonValue(x.Title, header.Language).Contains(search))
                .OrderByDescending(x => x.GoodsId)
                .Take(10)
                .Include(x => x.FkCategory)
                .Select(x => new GoodsSearchDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                    CategoryId = x.FkCategoryId
                })
                .AsNoTracking().ToListAsync();

                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<SpecificationGroupGetForGoodsDto>> GoodsSpecifictionGet(int goodsId)
        {


            var specification = await _context.TSpecificationGroup
                   .Include(m => m.TCategorySpecificationGroup)
                   .Include(m => m.TSpecification).ThenInclude(n => n.TGoodsSpecification)
                   .ThenInclude(t => t.TGoodsSpecificationOptions).ThenInclude(n => n.FkSpecOption)
                   .Where(b => b.TSpecification.Any(s => s.TGoodsSpecification.Any(h => h.FkGoodsId == goodsId) && s.Status == true))
                   .OrderBy(b => b.TCategorySpecificationGroup.Min(n => n.PriorityNumber))
                   .Select(g => new SpecificationGroupGetForGoodsDto
                   {
                       SpecGroupTitle = JsonExtensions.JsonValue(g.SpecGroupTitle, header.Language),
                       SpecGroupId = g.SpecGroupId,
                       Specification = g.TSpecification
                       .Where(e => e.TGoodsSpecification.Any(h => h.FkGoodsId == goodsId && (!string.IsNullOrWhiteSpace(h.SpecValueText) || h.TGoodsSpecificationOptions.Count() > 0)) && e.Status == true)
                    .OrderBy(y => y.PriorityNumber).Select(spec => new SpecificationGetDto
                    {
                        FkSpecGroupId = spec.FkSpecGroupId,
                        IsMultiLineText = spec.IsMultiLineText,
                        IsMultiSelect = spec.IsMultiSelect,
                        IsSelectable = spec.IsSelectable,
                        IsMultiSelectInFilter = spec.IsMultiSelectInFilter,
                        SpecId = spec.SpecId,
                        SpecTitle = JsonExtensions.JsonValue(spec.SpecTitle, header.Language),
                        TGoodsSpecification = spec.TGoodsSpecification
                          .Where(h => h.FkGoodsId == goodsId)
                          .Select(goodsSpec => new GoodsSpecificationDto
                          {
                              FkGoodsId = goodsSpec.FkGoodsId,
                              FkSpecId = goodsSpec.FkSpecId,
                              SpecValueText = JsonExtensions.JsonValue(goodsSpec.SpecValueText, header.Language),
                              Gsid = goodsSpec.Gsid,
                              TGoodsSpecificationOptions = goodsSpec.TGoodsSpecificationOptions.Select(goodsOption => new GoodsSpecificationOptionsDto
                              {
                                  SpecOptionId = goodsOption.SpecOptionId,
                                  FkGsid = goodsOption.FkGsid,
                                  FkSpecOptionId = goodsOption.FkSpecOptionId,
                                  OptionTitle = JsonExtensions.JsonValue(goodsOption.FkSpecOption.OptionTitle, header.Language)
                              }).ToList()
                          }).ToList()
                    }).ToList(),
                   }).ToListAsync();

            return specification;

        }




        // for mobile

        public async Task<MobileSplashDataDto> GetMobileSplashData()
        {
            try
            {
                var data = new MobileSplashDataDto();
                data.CartCount = await _context.TOrderItem.Include(c => c.FkOrder)
                .CountAsync(x => (token.Id == 0 ? (x.FkOrder.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.FkOrder.CookieId == token.CookieId) : (token.Id == x.FkOrder.FkCustomerId)) && x.FkOrder.FkOrderStatusId == (int)OrderStatusEnum.Cart);
                var currency = await _context.TCurrency.FirstOrDefaultAsync(x => x.DefaultCurrency == true);
                if (currency != null)
                {
                    data.DefualtCurrency = currency.CurrencyCode;
                }
                var lang = await _context.TLanguage.FirstOrDefaultAsync(x => x.DefaultLanguage == true);
                if (lang != null)
                {
                    data.DefualtLanguage = lang.LanguageCode;
                    data.IsRtl = lang.IsRtl;
                }

                var setting = await _context.TSetting.FirstOrDefaultAsync();
                if (setting != null)
                {
                    data.HeaderLogo = setting.LogoUrlShopHeader;
                }

                if (token.UserId != null)
                {
                    var user = await _context.TUser.FirstOrDefaultAsync(x => x.UserId == token.UserId);

                    if (user != null)
                    {
                        data.NotificationKey = user.ClientMobileFirebasePushNotificationKey;
                    }
                }

                // data.Domain = "http://192.168.1.4:4000/";

                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<DefualtLanguageCurrencyDto> GetDefualtLanguageAndCurrency()
        {
            try
            {
                var data = new DefualtLanguageCurrencyDto();
                var currency = await _context.TCurrency.FirstOrDefaultAsync(x => x.DefaultCurrency == true);
                if (currency != null)
                {
                    data.DefualtCurrency = currency.CurrencyCode;
                }
                var lang = await _context.TLanguage.FirstOrDefaultAsync(x => x.DefaultLanguage == true);
                if (lang != null)
                {
                    data.DefualtLanguage = lang.LanguageCode;
                }
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<string> GetFooterContent(string content)
        {
            try
            {
                var setting = await _context.TSetting.FirstAsync();
                var result = "";
                switch (content)
                {
                    case "PrivacyPolicy":
                        result = JsonExtensions.JsonGet(setting.FooterPrivacyPolicy, header);
                        break;
                    case "TermsOfSale":
                        result = JsonExtensions.JsonGet(setting.FooterTermOfSale, header);
                        break;
                    case "TermsOfUse":
                        result = JsonExtensions.JsonGet(setting.FooterTermOfUser, header);
                        break;
                    case "WarrantyPolicy":
                        result = JsonExtensions.JsonGet(setting.FooterWarrantyPolicy, header);
                        break;
                    case "CustomerRights":
                        result = JsonExtensions.JsonGet(setting.FooterCustomerRights, header);
                        break;
                    case "AboutUs":
                        result = JsonExtensions.JsonGet(setting.AboutUs, header);
                        break;
                    case "Overview":
                        result = JsonExtensions.JsonGet(setting.ShortDescription, header);
                        break;
                    default:
                        // code block
                        break;
                }
                return result;
            }
            catch (System.Exception)
            {
                return null;
            }
        }





    }

}