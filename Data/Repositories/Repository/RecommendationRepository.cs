using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Recommendation;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class RecommendationRepository : IRecommendationRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public RecommendationRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
            token = new TokenParseDto(httpContextAccessor);

        }

        public async Task<TRecommendation> AddRecommendation(TRecommendation recommendationDto)
        {
            try
            {
                await _context.TRecommendation.AddAsync(recommendationDto);
                await _context.SaveChangesAsync();
                return recommendationDto;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> EditRecommendation(TRecommendation recommendationDto)
        {
            try
            {
                var data = await _context.TRecommendation.FirstOrDefaultAsync(x => x.RecommendationId == recommendationDto.RecommendationId);
                if (data == null)
                {
                    return false;
                }

                _context.Entry(data).CurrentValues.SetValues(recommendationDto);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<RecommendationGetDto>> GetRecommendationList(PaginationRecommendationDto pagination)
        {
            try
            {
                return await _context.TRecommendation
                .Where(c => pagination.CategoryId != 0 ? c.XItemId == pagination.CategoryId : true &&
                          pagination.ProductId != 0 ? c.XItemId == pagination.ProductId : true)
                .Include(x => x.FkCollectionItemType)
                .Include(x => x.FkItemTypeNavigation)
                .OrderByDescending(x => x.RecommendationId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new RecommendationGetDto()
                {
                    RecommendationId = x.RecommendationId,
                    ItemType = JsonExtensions.JsonValue(x.FkItemTypeNavigation.ItemType, header.Language),
                    Title = x.FkItemType == (int)RecommendationItemTypeEnum.Category ? JsonExtensions.JsonValue(_context.TCategory.First(v => v.CategoryId == x.XItemId).CategoryTitle, header.Language) :
                    JsonExtensions.JsonValue(_context.TGoods.First(v => v.GoodsId == x.XItemId).Title, header.Language),
                    Status = x.Status
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<int> GetRecommendationListCount(PaginationRecommendationDto pagination)
        {
            try
            {
                return await _context.TRecommendation
                .CountAsync(c => pagination.CategoryId != 0 ? c.XItemId == pagination.CategoryId : true &&
                          pagination.ProductId != 0 ? c.XItemId == pagination.ProductId : true);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<GoodsHomeDto>> GetRecommendationGoods(List<int> categoryIds, int goodsId)
        {
            try
            {

                var rec = await _context.TRecommendation.OrderByDescending(o => o.RecommendationId).FirstOrDefaultAsync(x => x.XItemId == goodsId && x.Status);
                var recCat = await _context.TRecommendation.OrderByDescending(o => o.RecommendationId).FirstOrDefaultAsync(x => categoryIds.Any(t => t == x.XItemId) && x.Status);
                var targetProductsIds = new List<string>();
                var targetCatId = "";
                var takeProducts = 0;
                var rate = (decimal) 1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.0 : (decimal) currency.RatesAgainstOneDollar;
                }
                if (rec != null)
                {
                    if (rec.FkCollectionItemTypeId == 1)
                    {
                        targetCatId = rec.XCollectionItemIds;
                        takeProducts = (int)rec.RandomCount;
                    }
                    else
                    {
                        targetProductsIds = rec.XCollectionItemIds.Split(",").ToList();
                        takeProducts = targetProductsIds.Count();
                    }
                }
                else if (recCat != null)
                {
                    if (recCat.FkCollectionItemTypeId == 1)
                    {
                        targetCatId = recCat.XCollectionItemIds;
                        takeProducts = (int)recCat.RandomCount;
                    }
                    else
                    {
                        targetProductsIds = recCat.XCollectionItemIds.Split(",").ToList();
                        takeProducts = targetProductsIds.Count();

                    }
                } else {
                    return null ;
                }

                return await _context.TGoods
                .Include(x => x.FkCategory)
                .Include(x => x.TGoodsProvider).ThenInclude(t => t.FkShop)
                .Where(x =>
                x.GoodsId != goodsId &&
                (targetProductsIds.Count == 0 ? (x.FkCategory.CategoryPath.Contains("/" + targetCatId + "/") && x.FkCategory.IsActive == true) : (targetProductsIds.Any(t=>t == x.GoodsId.ToString()))) &&
                x.IsAccepted == true && x.ToBeDisplayed == true && x.TGoodsProvider.Any(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true
                ))
                .Take(takeProducts)
                .Include(x => x.TGoodsLike)
                .Select(x => new GoodsHomeDto()
                {
                    GoodsId = x.GoodsId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    GoodsImage = x.ImageUrl,
                    GoodsLiked = token.Id == 0 ? (bool?)null : x.TGoodsLike.Any(t => t.FkCustomerId == token.Id),
                    ShippingPossibilities = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true).FkShop.ShippingPossibilities,
                    SurveyCount = x.SurveyCount,
                    Price = decimal.Round( (decimal) x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true).Price  / rate, 2, MidpointRounding.AwayFromZero) ,
                    DiscountPercentage = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true ).DiscountPercentage,
                    DiscountAmount =  decimal.Round((decimal) x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true ).DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero) ,
                    FinalPrice = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true ).FinalPrice  / rate,
                    LikedCount = x.LikedCount,
                    SurveyScore = x.SurveyScore,
                    HaveVariation = x.HaveVariation,
                    SaleWithCall = x.SaleWithCall,
                    IsDownloadable = x.IsDownloadable,
                    InventoryCount = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true).InventoryCount,
                    ProviderId = x.TGoodsProvider.OrderBy(t => t.FinalPrice).FirstOrDefault(t => t.ToBeDisplayed == true && t.IsAccepted == true && t.FkShop.FkStatusId == (int)ShopStatusEnum.Active && t.HasInventory == true ).ProviderId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }



        public async Task<List<RecommendationItemDto>> GetRecommendationItemType()
        {
            try
            {
                return await _context.TRecommendationItemType
                .Select(x => new RecommendationItemDto()
                {
                    Id = x.ItemCode,
                    Title = JsonExtensions.JsonValue(x.ItemType, header.Language)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public async Task<List<RecommendationItemDto>> GetRecommendationCollectionType()
        {
            try
            {
                return await _context.TRecommendationCollectionType
                .Select(x => new RecommendationItemDto()
                {
                    Id = x.CollectionTypeId,
                    Title = JsonExtensions.JsonValue(x.CollectionTypeTitle , header.Language)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RecommendationAddDto> GetRecommendationWithId(int recommendationId)
        {
            try
            {
                return await _context.TRecommendation.Where(x=>x.RecommendationId == recommendationId)
                .Select(x => new RecommendationAddDto()
                {
                    RecommendationId = x.RecommendationId,
                    TitleType = x.FkItemType == (int)RecommendationItemTypeEnum.Category ? JsonExtensions.JsonValue(_context.TCategory.First(v => v.CategoryId == x.XItemId).CategoryTitle, header.Language) :
                    "",
                    TitleCollection = x.FkCollectionItemTypeId == (int)RecommendationItemTypeEnum.Category ? JsonExtensions.JsonValue(_context.TCategory.First(v => v.CategoryId.ToString() == x.XCollectionItemIds).CategoryTitle, header.Language) :
                    "",
                    Status = x.Status,
                    FkItemType = x.FkItemType,
                    XItemId = x.XItemId,
                    RandomCount = x.RandomCount,
                    FkCollectionItemTypeId = x.FkCollectionItemTypeId,
                    XCollectionItemIds = x.XCollectionItemIds,
                    CategoryPathType = x.FkItemType == (int)RecommendationItemTypeEnum.Category ?_context.TCategory.First(v => v.CategoryId == x.XItemId).CategoryPath :
                    "",                    
                    CategoryPathCollection = x.FkCollectionItemTypeId == (int)RecommendationItemTypeEnum.Category ?_context.TCategory.First(v => v.CategoryId.ToString() == x.XCollectionItemIds).CategoryPath :
                    ""
                })
                .AsNoTracking().FirstAsync();;
            }
            catch (System.Exception)
            {
                return null;
            }        
        }

       public async Task<bool> ChangeStatus(AcceptDto accept)
        {
            try
            {
                var data = await _context.TRecommendation.FindAsync(accept.Id);
           
                data.Status = (bool) accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<RepRes<TRecommendation>> RecommendationDelete(int recommendationId)
        {
            try
            {
                var data = await _context.TRecommendation.FirstOrDefaultAsync(x => x.RecommendationId == recommendationId);
                if (data == null)
                {
                    return new RepRes<TRecommendation>(Message.RecommendationGetting, false, null);
                }
               
                _context.TRecommendation.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TRecommendation>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return null;
            }         
            
        }
    }
}