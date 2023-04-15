using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Data.Dtos.Discount;
using System;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Goods;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Header;

using MarketPlace.API.Data.Dtos.Token;


namespace MarketPlace.API.Data.Repositories.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        public MarketPlaceDbContext _context { get; }
        public ICategoryRepository _categoryRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }

        public DiscountRepository(MarketPlaceDbContext context, ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor)
        {
            this._categoryRepository = categoryRepository;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
        }
        public async Task<List<SpecialSellPlanDto>> GetSpecialSellPlanByIds(List<int> ids)
        {
            var specialSellCoupon = await _context.TDiscountPlan
            .Where(x => x.FkPlanTypeId == (int)PlanTypeEnum.SpecialSell && x.Status && ids.Contains((int)x.PlanId))
            .AsNoTracking().Select(coupon => new SpecialSellPlanDto
            {
                PlanId = coupon.PlanId,
                PlanTitle = JsonExtensions.JsonValue(coupon.Title, header.Language),
            }).ToListAsync();
            return specialSellCoupon;
        }

        public async Task<TDiscountPlan> GetGoodsAndCatIdsByPlanId(int planId)
        {
            try
            {
                return await _context.TDiscountPlan
                .Include(x => x.TDiscountCategory)
                .Include(x => x.TDiscountGoods)
                .FirstOrDefaultAsync(x => x.PlanId == planId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TDiscountPlan> DiscountPlanAdd(TDiscountPlan discount)
        {
            try
            {
                discount.Title = JsonExtensions.JsonAdd(discount.Title, header);
                discount.Comment = JsonExtensions.JsonAdd(discount.Comment, header);
                var rate = (decimal)1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                if (discount.MinimumOrderAmount != null)
                {
                    discount.MinimumOrderAmount = decimal.Round((decimal)(discount.MinimumOrderAmount / rate), 2, MidpointRounding.AwayFromZero);
                }
                if (discount.MaximumDiscountAmount != null)
                {
                    discount.MaximumDiscountAmount = decimal.Round((decimal)(discount.MaximumDiscountAmount / rate), 2, MidpointRounding.AwayFromZero);
                }
                if (discount.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                {
                    discount.DiscountAmount = decimal.Round((decimal)(discount.DiscountAmount / rate), 2, MidpointRounding.AwayFromZero);
                }
                await _context.TDiscountPlan.AddAsync(discount);
                await _context.SaveChangesAsync();
                return discount;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> SetDiscount(long discountId)
        {
            try
            {
                var discount = await _context.TDiscountPlan
                .Include(x => x.TDiscountCategory)
                .Include(x => x.TDiscountGoods)
                .Include(x => x.TDiscountShops)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PlanId == discountId);
                if (discount == null)
                {
                    return false;
                }
                var varieties = new List<TGoodsProvider>();

                // var shopIds = new List<int>();
                // var allowShop = true;
                // if (discount.TDiscountShops != null)
                // {
                //     if (discount.TDiscountShops.Count > 0)
                //     {
                //         allowShop = discount.TDiscountShops.Any(x => x.Allowed == true);
                //         shopIds = discount.TDiscountShops.Select(x => x.FkShopId).ToList();
                //     }
                // }

                if (discount.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders)
                {
                    varieties = await _context.TGoodsProvider.Where(x => (discount.FkShopId != null ? x.FkShopId == discount.FkShopId : true)).ToListAsync();
                }
                else if (discount.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialCategory)
                {
                    var CatIds = new List<int>();

                    foreach (var item in discount.TDiscountCategory)
                    {
                        CatIds.AddRange(await _categoryRepository.GetCategoriesChilds(item.FkCategoryId));
                    }
                    var allow = discount.TDiscountCategory.Any(x => x.Allowed == true);
                    varieties = await _context.TGoodsProvider.Include(x => x.FkGoods).Where(x => (discount.FkShopId != null ? x.FkShopId == discount.FkShopId : true) && CatIds.Contains(x.FkGoods.FkCategoryId) == allow).ToListAsync();
                }
                else if (discount.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialGoods)
                {
                    var goodsIds = discount.TDiscountGoods.Where(x => x.FkVarietyId == null).Select(x => x.FkGoodsId).ToList();
                    var varietyIds = discount.TDiscountGoods.Select(x => x.FkVarietyId).ToList();
                    var allow = discount.TDiscountGoods.Any(x => x.Allowed == true);
                    varieties = await _context.TGoodsProvider.Where(x => (discount.FkShopId != null ? x.FkShopId == discount.FkShopId : true) &&
                    (varietyIds.Contains(x.ProviderId) == allow || goodsIds.Contains(x.FkGoodsId) == allow)).ToListAsync();
                }


                if (discount.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                {
                    foreach (var item in varieties)
                    {
                        item.DiscountPercentage = item.DiscountPercentage == (decimal)0.0 ? (0 + ((discount.DiscountAmount * 100) / item.Price)) : item.DiscountPercentage + ((discount.DiscountAmount * 100) / item.Price);
                        if(item.DiscountPercentage > 100) {
                            item.DiscountPercentage = 100 ;
                        }
                        item.DiscountAmount = item.DiscountAmount == (decimal)0.0 ? (0 + discount.DiscountAmount) : item.DiscountAmount + discount.DiscountAmount;
                        if(item.FinalPrice < item.DiscountAmount) {
                            item.DiscountAmount = item.FinalPrice ;
                        }
                        item.FinalPrice = item.FinalPrice - item.DiscountAmount;
                    }
                }

                if (discount.FkDiscountTypeId == (int)DiscountTypeId.PercentDiscount)
                {
                    foreach (var item in varieties)
                    {
                        item.DiscountPercentage = item.DiscountPercentage == (decimal)0.0 ? (0 + discount.DiscountAmount) : item.DiscountPercentage + discount.DiscountAmount;
                       if(item.DiscountPercentage > 100) {
                            item.DiscountPercentage = 100 ;
                        }
                        item.DiscountAmount = item.DiscountAmount == (decimal)0.0 ? (0 + ((discount.DiscountAmount * item.Price) / 100)) : item.DiscountAmount + ((discount.DiscountAmount * item.Price) / 100);
                        if(item.FinalPrice < item.DiscountAmount) {
                            item.DiscountAmount = item.FinalPrice ;
                        }                        
                        item.FinalPrice = item.FinalPrice - item.DiscountAmount;
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

        public async Task<bool> UnSetDiscount(long discountId)
        {
            try
            {
                var discount = await _context.TDiscountPlan
                .Include(x => x.TDiscountCategory)
                .Include(x => x.TDiscountGoods)
                .FirstOrDefaultAsync(x => x.PlanId == discountId);
                if (discount == null)
                {
                    return false;
                }
                discount.Status = false;
                if (discount.FkPlanTypeId == (int)PlanTypeEnum.DiscountCode)
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    // var shopIds = new List<int>();
                    // var allowShop = true;
                    // if (discount.TDiscountShops != null)
                    // {
                    //     if (discount.TDiscountShops.Count > 0)
                    //     {
                    //         allowShop = discount.TDiscountShops.Any(x => x.Allowed == true);
                    //         shopIds = discount.TDiscountShops.Select(x => x.FkShopId).ToList();
                    //     }
                    // }
                    var varieties = new List<TGoodsProvider>();
                    if (discount.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders)
                    {
                        varieties = await _context.TGoodsProvider.Where(x => (discount.FkShopId != null ? x.FkShopId == discount.FkShopId : true)).ToListAsync();
                    }
                    else if (discount.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialCategory)
                    {

                        var CatIds = new List<int>();

                        foreach (var item in discount.TDiscountCategory)
                        {
                            CatIds.AddRange(await _categoryRepository.GetCategoriesChilds(item.FkCategoryId));
                        }
                        var allowss = discount.TDiscountCategory.Any(x => x.Allowed == true);
                        varieties = await _context.TGoodsProvider.Include(x => x.FkGoods).Where(x => (discount.FkShopId != null ? x.FkShopId == discount.FkShopId : true) && CatIds.Contains(x.FkGoods.FkCategoryId) == allowss).ToListAsync();


                    }
                    else if (discount.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialGoods)
                    {
                        var goodsIds = discount.TDiscountGoods.Where(x => x.FkVarietyId == null).Select(x => x.FkGoodsId).ToList();
                        var varietyIds = discount.TDiscountGoods.Select(x => x.FkVarietyId).ToList();
                        var allows = discount.TDiscountGoods.Any(x => x.Allowed == true);
                        varieties = await _context.TGoodsProvider.Where(x => (discount.FkShopId != null ? x.FkShopId == discount.FkShopId : true) &&
                        (varietyIds.Contains(x.ProviderId) == allows || goodsIds.Contains(x.FkGoodsId) == allows)).ToListAsync();
                    }

                    if (discount.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                    {
                        foreach (var item in varieties)
                        {
                            item.DiscountPercentage = item.DiscountPercentage == (decimal)0.0 ? 0 : item.DiscountPercentage - ((discount.DiscountAmount * 100) / item.Price);
                            item.DiscountAmount = item.DiscountAmount == (decimal)0.0 ? 0 : item.DiscountAmount - (long)discount.DiscountAmount;
                            if(item.DiscountAmount < 0) {
                                item.DiscountAmount = 0 ;
                            }
                            if(item.DiscountPercentage < 0) {
                                item.DiscountPercentage = 0 ;
                            }
                            
                            item.FinalPrice = item.FinalPrice + item.DiscountAmount;
                        }
                    }
                    if (discount.FkDiscountTypeId == (int)DiscountTypeId.PercentDiscount)
                    {
                        foreach (var item in varieties)
                        {
                            item.DiscountPercentage = item.DiscountPercentage == (decimal)0.0 ? 0 : item.DiscountPercentage - discount.DiscountAmount;
                            item.DiscountAmount = item.DiscountAmount == (decimal)0.0 ? 0 : item.DiscountAmount - ((discount.DiscountAmount * item.Price) / 100);
                             if(item.DiscountAmount < 0) {
                                item.DiscountAmount = 0 ;
                            }
                            if(item.DiscountPercentage < 0) {
                                item.DiscountPercentage = 0 ;
                            }                           
                            item.FinalPrice = item.FinalPrice + item.DiscountAmount;
                        }
                    }

                    await _context.SaveChangesAsync();

                    return true;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<string>> GetRandomString(int count)
        {
            try
            {
                var randomCode = await _context.TCodeRepository.OrderBy(x => x.CodeLength).Take(count).ToListAsync();

                _context.RemoveRange(randomCode);
                await _context.SaveChangesAsync();
                return randomCode.Select(x => x.DiscountCode).ToList();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<TDiscountPlan>> GetDiscountByDateAndTime(DateTime date)
        {
            try
            {
                var discount = await _context.TDiscountPlan.Where(x => x.TimingType == true &&
                x.Status == true &&
                (x.StartDateTime.Value.Date == date.Date || x.EndDateTime.Value.Date == date.Date)
                ).AsNoTracking().ToListAsync();

                return discount;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<DiscountPlanGetDto>> DiscountPlanGet(DiscountFilterDto filterDto)
        {
            try
            {

                var data = await _context.TDiscountPlan
                .Include(x => x.TDiscountGoods)
                .Include(x => x.TDiscountCategory)
                .Include(x => x.TDiscountShops)
                .Where(x =>
                (filterDto.Status == null ? true : x.Status == (bool)filterDto.Status) &&
                ((filterDto.CatIds.Count != 0 ? (x.TDiscountCategory.Any(t => t.Allowed == true && filterDto.CatIds.Contains(t.FkCategoryId)) || x.TDiscountCategory.Any(t => t.Allowed == false && !filterDto.CatIds.Contains(t.FkCategoryId))) : true) ||
                (filterDto.GoodsId != 0 ? (x.TDiscountGoods.Any(t => t.Allowed == true && t.FkGoodsId == filterDto.GoodsId) || x.TDiscountGoods.Any(t => t.Allowed == false && t.FkGoodsId != filterDto.GoodsId)) : true) || x.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders) &&
                (filterDto.PlanTypeId != 0 ? (filterDto.PlanTypeId == x.FkPlanTypeId) : true) &&
                // (filterDto.ShopId != 0 ? (x.TDiscountShops.Any(t => t.Allowed == true && t.FkShopId == filterDto.ShopId) || x.TDiscountShops.Any(t => t.Allowed == false && t.FkShopId != filterDto.ShopId)) : true)&&
                (filterDto.ShopId != 0 ? x.FkShopId == filterDto.ShopId : true) &&
                (string.IsNullOrWhiteSpace(filterDto.Title) ? true : filterDto.Title.Contains(JsonExtensions.JsonValue(x.Title, header.Language))))
                .OrderByDescending(x => x.PlanId)
                .Skip(filterDto.PageSize * (filterDto.PageNumber - 1)).Take(filterDto.PageSize)
                .Include(x => x.FkCouponCodeType)
                .Include(x => x.FkDiscountRangeType)
                .Include(x => x.FkDiscountType)
                .Include(x => x.FkPlanType)
                .Include(x => x.FkShop)
                .Select(x => new DiscountPlanGetDto()
                {
                    PlanId = x.PlanId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    FkPlanTypeId = x.FkPlanTypeId,
                    FkShopId = x.FkShopId,
                    PlanTypeTitle = JsonExtensions.JsonValue(x.FkPlanType.TypeTitle, header.Language),
                    ShopTitle = x.FkShopId == null ? null : x.FkShop.StoreName,
                    Status = x.Status,
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

        public async Task<int> DiscountPlanCountGet(DiscountFilterDto filterDto)
        {
            try
            {
                var count = await _context.TDiscountPlan
                .Include(x => x.TDiscountGoods)
                .Include(x => x.TDiscountCategory)
                .Include(x => x.TDiscountShops)
                .AsNoTracking()
                .CountAsync(x =>
                (filterDto.Status == null ? true : x.Status == (bool)filterDto.Status) &&
                ((filterDto.CatIds.Count != 0 ? (x.TDiscountCategory.Any(t => t.Allowed == true && filterDto.CatIds.Contains(t.FkCategoryId)) || x.TDiscountCategory.Any(t => t.Allowed == false && !filterDto.CatIds.Contains(t.FkCategoryId))) : true) ||
                (filterDto.GoodsId != 0 ? (x.TDiscountGoods.Any(t => t.Allowed == true && t.FkGoodsId == filterDto.GoodsId) || x.TDiscountGoods.Any(t => t.Allowed == false && t.FkGoodsId != filterDto.GoodsId)) : true) || x.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders) &&
                (filterDto.PlanTypeId != 0 ? (filterDto.PlanTypeId == x.FkPlanTypeId) : true) &&
                // (filterDto.ShopId != 0 ? (x.TDiscountShops.Any(t => t.Allowed == true && t.FkShopId == filterDto.ShopId) || x.TDiscountShops.Any(t => t.Allowed == false && t.FkShopId != filterDto.ShopId)) : true)&&
                (filterDto.ShopId != 0 ? x.FkShopId == filterDto.ShopId : true) &&
                (string.IsNullOrWhiteSpace(filterDto.Title) ? true : filterDto.Title.Contains(JsonExtensions.JsonValue(x.Title, header.Language))));

                return count;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        public async Task<List<DiscountCodeExelDto>> GetCoponCodeForExel(int planId)
        {
            try
            {
                var data = await _context.TDiscountCouponCode.Include(x => x.FkDiscountPlan).Where(x => x.FkDiscountPlanId == planId && (token.Rule == UserGroupEnum.Seller ? x.FkDiscountPlan.FkShopId == token.Id : true))
                .Select(x => new DiscountCodeExelDto()
                {
                    CodeId = x.CodeId,
                    DiscountCode = x.DiscountCode,
                    MaxUse = x.MaxUse,
                    UseCount = x.UseCount,
                    IsValid = x.IsValid
                })
                .AsNoTracking().ToListAsync();

                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<DiscountCodeDetailDto>> GetDiscountCodeDetail(DiscountCodePaginationDto pagination)
        {
            try
            {
                var rate = (decimal)1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var data = await _context.TDiscountCouponCode
                .Include(x => x.FkDiscountPlan)
                .Where(x =>
                (token.Rule == UserGroupEnum.Seller ? x.FkDiscountPlan.FkShopId == token.Id : true) &&
                (string.IsNullOrWhiteSpace(pagination.Code) ? true : x.DiscountCode.Contains(pagination.Code)) &&
                (string.IsNullOrWhiteSpace(pagination.Plan) ? true : JsonExtensions.JsonValue(x.FkDiscountPlan.Title, header.Language).Contains(pagination.Plan)) &&
                (pagination.Status == null ? true : x.FkDiscountPlan.Status == pagination.Status)
                )
                .OrderByDescending(x => x.CodeId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.TOrder).ThenInclude(t => t.FkCustomer)
                .Include(x => x.TOrder).ThenInclude(t => t.TOrderItem)
                .Select(x => new DiscountCodeDetailDto()
                {
                    PlanId = x.FkDiscountPlan.PlanId,
                    Title = JsonExtensions.JsonValue(x.FkDiscountPlan.Title, header.Language),
                    FkCouponTypeId = x.FkDiscountPlan.FkPlanTypeId,
                    CouponTypeTitle = JsonExtensions.JsonValue(x.FkDiscountPlan.FkPlanType.TypeTitle, header.Language),
                    UseLimitationType = x.FkDiscountPlan.UseLimitationType,
                    CouponCodeCount = x.FkDiscountPlan.CouponCodeCount,
                    PermittedUseNumberPerCode = x.FkDiscountPlan.PermittedUseNumberPerCode,
                    PermittedUseNumberPerCustomer = x.FkDiscountPlan.PermittedUseNumberPerCustomer,
                    PermittedUseNumberAll = x.FkDiscountPlan.PermittedUseNumberAll,
                    CouponCodeType = x.FkDiscountPlan.FkCouponCodeTypeId,
                    CouponCodeTypeTitle = JsonExtensions.JsonValue(x.FkDiscountPlan.FkCouponCodeType.CodeTypeTitle, header.Language),
                    CouponCodePrefix = x.FkDiscountPlan.CouponCodePrefix,
                    MinimumOrderAmount = decimal.Round((decimal)x.FkDiscountPlan.MinimumOrderAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    MaximumDiscountAmount = decimal.Round((decimal)x.FkDiscountPlan.MaximumDiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    TimingType = x.FkDiscountPlan.TimingType,
                    StartDateTime = x.FkDiscountPlan.StartDateTime,
                    EndDateTime = x.FkDiscountPlan.EndDateTime,
                    DiscountTypeTitle = JsonExtensions.JsonValue(x.FkDiscountPlan.FkDiscountType.DiscountTypeTitle, header.Language),
                    DiscountAmount = x.FkDiscountPlan.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount ? decimal.Round(x.FkDiscountPlan.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero) : x.FkDiscountPlan.DiscountAmount,
                    Status = x.FkDiscountPlan.Status,
                    CodeId = x.CodeId,
                    FkDiscountPlanId = x.FkDiscountPlanId,
                    DiscountCode = x.DiscountCode,
                    MaxUse = x.MaxUse,
                    UseCount = x.UseCount,
                    IsValid = x.IsValid,
                    FreeShippingCost = x.FkDiscountPlan.FreeShippingCost,
                    FkDiscountTypeId = x.FkDiscountPlan.FkDiscountTypeId,
                    Orders = x.TOrder.Select(t => new DiscountCodeOrderDto()
                    {
                        Name = t.FkCustomer.Name,
                        Family = t.FkCustomer.Family,
                        PlacedDateTime = t.PlacedDateTime != null ? Extentions.PersianDateString((DateTime)t.PlacedDateTime) : null,
                        OrderId = t.OrderId,
                        DiscountAmount =
                        decimal.Round((decimal)(t.DiscountAmount - (t.TOrderItem.Sum(f => f.DiscountAmount)))  / rate, 2, MidpointRounding.AwayFromZero)
                         ,
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();

                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetDiscountCodeDetailCount(DiscountCodePaginationDto pagination)
        {
            try
            {
                var count = await _context.TDiscountCouponCode
                .Include(x => x.FkDiscountPlan)
                .AsNoTracking()
                .CountAsync(x =>
                (token.Rule == UserGroupEnum.Seller ? x.FkDiscountPlan.FkShopId == token.Id : true) &&
                (string.IsNullOrWhiteSpace(pagination.Code) ? true : x.DiscountCode.Contains(pagination.Code)) &&
                (string.IsNullOrWhiteSpace(pagination.Plan) ? true : JsonExtensions.JsonValue(x.FkDiscountPlan.Title, header.Language).Contains(pagination.Plan)) &&
                (pagination.Status == null ? true : x.FkDiscountPlan.Status == pagination.Status)
                );

                return count;

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<TDiscountPlan> DiscountPlanGetById(long discountId)
        {
            try
            {
                var data = await _context.TDiscountPlan.FindAsync(discountId);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TDiscountPlan> DiscountPlanEdit(TDiscountPlan discountDto)
        {
            try
            {
                var data = await _context.TDiscountPlan.FirstOrDefaultAsync(x => discountDto.PlanId == x.PlanId && (token.Rule == UserGroupEnum.Seller ? x.FkShopId == token.Id : true));
                if (data == null)
                {
                    return null;
                }
                //  var cats = await _context.TDiscountCategory.Where(x => x.FkDiscountPlanId == discountDto.PlanId).ToListAsync();
                //  var customers = await _context.TDiscountCustomers.Where(x => x.FkDiscountPlanId == discountDto.PlanId).ToListAsync();
                  var goods = await _context.TDiscountGoods.Where(x => x.FkDiscountPlanId == discountDto.PlanId).ToListAsync();
                //  var freeGoods = await _context.TDiscountFreeGoods.Where(x => x.FkDiscountPlanId == discountDto.PlanId).ToListAsync();
                // var shops = await _context.TDiscountShops.Where(x => x.FkDiscountPlanId == discountDto.PlanId).ToListAsync();

                // if (cats.Count > 0)
                // {
                //     _context.TDiscountCategory.RemoveRange(cats);
                // }
                // if (customers.Count > 0)
                // {
                //     _context.TDiscountCustomers.RemoveRange(customers);
                // }
                if (goods.Count > 0)
                {
                    _context.TDiscountGoods.RemoveRange(goods);
                }
                // if (freeGoods.Count > 0)
                // {
                //     _context.TDiscountFreeGoods.RemoveRange(freeGoods);
                // }
                // if (shops.Count > 0)
                // {
                //     _context.TDiscountShops.RemoveRange(shops);
                // }

                //   await _context.SaveChangesAsync();

                // if (discountDto.TDiscountCategory.Count > 0)
                // {
                //     foreach (var item in discountDto.TDiscountCategory)
                //     {
                //         item.FkDiscountPlanId = discountDto.PlanId;
                //     }
                //     await _context.TDiscountCategory.AddRangeAsync(discountDto.TDiscountCategory);
                // }
                // if (discountDto.TDiscountCustomers.Count > 0)
                // {
                //     foreach (var item in discountDto.TDiscountCustomers)
                //     {
                //         item.FkDiscountPlanId = discountDto.PlanId;
                //     }
                //     await _context.TDiscountCustomers.AddRangeAsync(discountDto.TDiscountCustomers);
                // }
                if (discountDto.TDiscountGoods.Count > 0)
                {
                    foreach (var item in discountDto.TDiscountGoods)
                    {
                        item.FkDiscountPlanId = discountDto.PlanId;
                    }
                    await _context.TDiscountGoods.AddRangeAsync(discountDto.TDiscountGoods);
                }
                // if (discountDto.TDiscountGoods.Count > 0)
                // {
                //     foreach (var item in discountDto.TDiscountGoods)
                //     {
                //         item.FkDiscountPlanId = discountDto.PlanId;
                //     }
                //     await _context.TDiscountGoods.AddRangeAsync(discountDto.TDiscountGoods);
                // }
                // if (discountDto.TDiscountGoods.Count > 0)
                // {
                //     foreach (var item in discountDto.TDiscountGoods)
                //     {
                //         item.FkDiscountPlanId = discountDto.PlanId;
                //     }
                //     await _context.TDiscountGoods.AddRangeAsync(discountDto.TDiscountGoods);
                // }
                // if (discountDto.TDiscountFreeGoods.Count > 0)
                // {
                //     foreach (var item in discountDto.TDiscountFreeGoods)
                //     {
                //         item.FkDiscountPlanId = discountDto.PlanId;
                //     }
                //     await _context.TDiscountFreeGoods.AddRangeAsync(discountDto.TDiscountFreeGoods);
                // }
                // if (discountDto.TDiscountShops.Count > 0)
                // {
                //     foreach (var item in discountDto.TDiscountShops)
                //     {
                //         item.FkDiscountPlanId = discountDto.PlanId;
                //     }
                //     await _context.TDiscountShops.AddRangeAsync(discountDto.TDiscountShops);
                // }

                 await _context.SaveChangesAsync();

                var rate = (decimal) 1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.0 : (decimal) currency.RatesAgainstOneDollar;
                }
                if(discountDto.MinimumOrderAmount != null)
                {
                    discountDto.MinimumOrderAmount = decimal.Round( (decimal) (discountDto.MinimumOrderAmount / rate), 2, MidpointRounding.AwayFromZero) ;
                }
                if (discountDto.MaximumDiscountAmount != null)
                {
                    discountDto.MaximumDiscountAmount = decimal.Round( (decimal) (discountDto.MaximumDiscountAmount / rate), 2, MidpointRounding.AwayFromZero) ;
                }
                if(discountDto.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                {
                    discountDto.DiscountAmount =  decimal.Round((decimal)(discountDto.DiscountAmount / rate), 2, MidpointRounding.AwayFromZero) ;
                }

                // data.Title = JsonExtensions.JsonEdit(discountDto.Title,data.Title,header);
                // data.PermittedUseNumberPerCustomer = discountDto.PermittedUseNumberPerCustomer;
                // data.FkDiscountRangeTypeId = discountDto.FkDiscountRangeTypeId;
                data.MinimumOrderAmount = discountDto.MinimumOrderAmount;
                data.MaximumDiscountAmount = discountDto.MaximumDiscountAmount;
                data.TimingType = discountDto.TimingType;
                data.StartDateTime = discountDto.StartDateTime;
                data.EndDateTime = discountDto.EndDateTime;
                data.FkDiscountTypeId = discountDto.FkDiscountTypeId;
                data.DiscountAmount = discountDto.DiscountAmount;
                // data.DiscountCustomerDomain = discountDto.DiscountCustomerDomain;
                // data.Comment = JsonExtensions.JsonEdit(discountDto.Comment,data.Comment,header);
                data.Status = discountDto.Status;
                // data.FkShopId = discountDto.FkShopId;
                // data.FreeShippingCost = discountDto.FreeShippingCost;
                // data.FreeProduct = discountDto.FreeProduct;
                await _context.SaveChangesAsync();

                return discountDto;
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
                var data = await _context.TDiscountPlan.FirstOrDefaultAsync(x => x.PlanId == (long)accept.Id && (token.Rule == UserGroupEnum.Seller ? x.FkShopId == token.Id : true));
                if (data == null)
                {
                    return false;
                }

                if (data.FkPlanTypeId == (int)PlanTypeEnum.DiscountCode)
                {
                    data.Status = accept.Accept;
                }
                else
                {
                    if (data.Status == false && accept.Accept == true)
                    {
                        if (data.TimingType == false)
                        {
                            var change = await SetDiscount((long)accept.Id);
                        }

                        else if (data.TimingType == true)
                        {
                            var date = DateTime.Now;
                            if (data.StartDateTime <= date && data.EndDateTime >= date)
                            {
                                var change = await SetDiscount((long)accept.Id);
                            }
                        }
                        data.Status = accept.Accept;
                    }
                    else if (data.Status == true && accept.Accept == false)
                    {
                        var change = await UnSetDiscount((long)accept.Id);
                        data.Status = accept.Accept;
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

        public async Task<RepRes<bool>> deleteDiscount(long planId)
        {
            try
            {
                var data = await _context.TDiscountPlan
                .Include(c => c.TDiscountCouponCode).ThenInclude(o => o.TOrder)
                .Include(c => c.TDiscountCouponCode).ThenInclude(o => o.TOrderItem)
                .FirstOrDefaultAsync(x => x.PlanId == planId && (token.Rule == UserGroupEnum.Seller ? x.FkShopId == token.Id : true));
                if (data == null)
                {
                    return new RepRes<bool>(Message.DiscountPlanGetting, false, false);
                }
                if (data.TDiscountCouponCode.Any(v => v.UseCount > 0 || v.TOrder.Count > 0 || v.TOrderItem.Count > 0))
                {
                    return new RepRes<bool>(Message.DiscountPlanDelete, false, false);
                }
                var unsetDiscount = await UnSetDiscount(planId);
                if (!unsetDiscount)
                {
                    return new RepRes<bool>(Message.DiscountPlanErrorDelete, false, false);
                }

                var dicountCategory = await _context.TDiscountCategory.Where(x => x.FkDiscountPlanId == planId).ToListAsync();
                var dicountCustomers = await _context.TDiscountCustomers.Where(x => x.FkDiscountPlanId == planId).ToListAsync();
                var dicountGoods = await _context.TDiscountGoods.Where(x => x.FkDiscountPlanId == planId).ToListAsync();
                var dicountCouponCode = await _context.TDiscountCouponCode.Where(x => x.FkDiscountPlanId == planId).ToListAsync();
                var dicountFreeGoods = await _context.TDiscountFreeGoods.Where(x => x.FkDiscountPlanId == planId).ToListAsync();
                var dicountShop = await _context.TDiscountShops.Where(x => x.FkDiscountPlanId == planId).ToListAsync();

                _context.TDiscountCategory.RemoveRange(dicountCategory);
                _context.TDiscountCustomers.RemoveRange(dicountCustomers);
                _context.TDiscountGoods.RemoveRange(dicountGoods);
                _context.TDiscountCouponCode.RemoveRange(dicountCouponCode);
                _context.TDiscountFreeGoods.RemoveRange(dicountFreeGoods);
                _context.TDiscountShops.RemoveRange(dicountShop);
                _context.TDiscountPlan.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);

            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.DiscountPlanErrorDelete, false, false);
            }
        }

        public async Task<DiscountPlanGetOneDto> GetOne(int discountId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var data = await _context.TDiscountPlan
                .Include(x => x.FkShop)
                .Include(x => x.FkCouponCodeType)
                .Include(x => x.FkDiscountRangeType)
                .Include(x => x.FkDiscountType)
                .Include(x => x.FkPlanType)
                .Include(x => x.TDiscountCategory).ThenInclude(t => t.FkCategory)
                .Include(x => x.TDiscountCustomers).ThenInclude(t => t.FkCustomer)
                // .Include(x => x.TDiscountShops).ThenInclude(t => t.FkShop)
                .Include(x => x.TDiscountGoods).ThenInclude(t => t.FkGoods).ThenInclude(i => i.FkBrand)
                .Include(x => x.TDiscountGoods).ThenInclude(t => t.FkGoods).ThenInclude(i => i.FkCategory)
                .Include(x => x.TDiscountGoods).ThenInclude(t => t.FkVariety).ThenInclude(h => h.TGoodsVariety).ThenInclude(l => l.FkVariationParameter)
                .Include(x => x.TDiscountGoods).ThenInclude(t => t.FkVariety).ThenInclude(h => h.TGoodsVariety).ThenInclude(l => l.FkVariationParameterValue)
                .Include(x => x.TDiscountFreeGoods).ThenInclude(t => t.FkGoods).ThenInclude(i => i.FkBrand)
                .Include(x => x.TDiscountFreeGoods).ThenInclude(t => t.FkGoods).ThenInclude(i => i.FkCategory)
                .Include(x => x.TDiscountFreeGoods).ThenInclude(t => t.FkVariety).ThenInclude(h => h.TGoodsVariety).ThenInclude(l => l.FkVariationParameter)
                .Include(x => x.TDiscountFreeGoods).ThenInclude(t => t.FkVariety).ThenInclude(h => h.TGoodsVariety).ThenInclude(l => l.FkVariationParameterValue)
                .Select(x => new DiscountPlanGetOneDto()
                {
                    PlanId = x.PlanId,
                    Title = JsonExtensions.JsonValue(x.Title, header.Language),
                    PermittedUseNumberPerCustomer = x.PermittedUseNumberPerCustomer,
                    FkDiscountRangeTypeId = x.FkDiscountRangeTypeId,
                    MinimumOrderAmount = decimal.Round((decimal)x.MinimumOrderAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    MaximumDiscountAmount = decimal.Round((decimal)x.MaximumDiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    TimingType = x.TimingType,
                    StartDateTime = x.StartDateTime,
                    EndDateTime = x.EndDateTime,
                    FkDiscountTypeId = x.FkDiscountTypeId,
                    DiscountAmount = x.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount ? decimal.Round((decimal)x.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero) : x.DiscountAmount,
                    DiscountCustomerDomain = x.DiscountCustomerDomain == null ? false : (bool)x.DiscountCustomerDomain,
                    Comment = JsonExtensions.JsonValue(x.Comment, header.Language),
                    UseLimitationType = x.UseLimitationType,
                    CouponCodeCount = x.CouponCodeCount,
                    PermittedUseNumberPerCode = x.PermittedUseNumberPerCode,
                    PermittedUseNumberAll = x.PermittedUseNumberAll,
                    ActiveForFirstBuy = x.ActiveForFirstBuy,
                    CouponCodePrefix = x.CouponCodePrefix,
                    UseWithOtherDiscountPlan = x.UseWithOtherDiscountPlan,
                    CouponCodeTypeTitle = x.FkCouponCodeTypeId == null ? null : JsonExtensions.JsonValue(x.FkCouponCodeType.CodeTypeTitle, header.Language),
                    DiscountRangeTypeTitle = JsonExtensions.JsonValue(x.FkDiscountRangeType.RangeTypeTitle, header.Language),
                    DiscountTypeTitle = JsonExtensions.JsonValue(x.FkDiscountType.DiscountTypeTitle, header.Language),
                    FkCouponCodeTypeId = x.FkCouponCodeTypeId,
                    FkPlanTypeId = x.FkPlanTypeId,
                    FkShopId = x.FkShopId,
                    FreeProduct = x.FreeProduct,
                    FreeShippingCost = x.FreeShippingCost,
                    PlanTypeTitle = JsonExtensions.JsonValue(x.FkPlanType.TypeTitle, header.Language),
                    ShopTitle = x.FkShopId == null ? null : x.FkShop.StoreName,
                    Status = x.Status,
                    TDiscountFreeGoods = x.TDiscountFreeGoods.Select(t => new DiscountFreeGoodsGetDto()
                    {
                        FkDiscountPlanId = t.FkDiscountPlanId,
                        FkGoodsId = t.FkGoodsId,
                        FkVarietyId = t.FkVarietyId,
                        FreeGoodsId = t.FreeGoodsId,
                        GoodsCode = t.FkGoods.GoodsCode,
                        Quantity = t.Quantity,
                        Varity = t.FkVariety.TGoodsVariety.Select(i => new GoodsVarietyGetDto()
                        {
                            FkGoodsId = i.FkGoodsId,
                            FkProviderId = i.FkProviderId,
                            FkVariationParameterId = i.FkVariationParameterId,
                            FkVariationParameterValueId = i.FkVariationParameterValueId,
                            ImageUrl = i.ImageUrl,
                            ParameterTitle = JsonExtensions.JsonValue(i.FkVariationParameter.ParameterTitle, header.Language),
                            ValueTitle = JsonExtensions.JsonValue(i.FkVariationParameterValue.Value, header.Language),
                            VarietyId = i.VarietyId
                        }).ToList(),
                        SerialNumber = t.FkGoods.SerialNumber,
                        ProductTitle = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                        Image = t.FkGoods.ImageUrl,
                    }).ToList(),
                    TDiscountGoods = x.TDiscountGoods.Select(t => new DiscountGoodsGetDto()
                    {
                        AssingedGoodsId = t.AssingedGoodsId,
                        FkDiscountPlanId = t.FkDiscountPlanId,
                        FkGoodsId = t.FkGoodsId,
                        FkVarietyId = t.FkVarietyId,
                        Allowed = t.Allowed,
                        Brand = t.FkGoods.FkBrandId == null ? null : JsonExtensions.JsonValue(t.FkGoods.FkBrand.BrandTitle, header.Language),
                        Category = JsonExtensions.JsonValue(t.FkGoods.FkCategory.CategoryTitle, header.Language),
                        GoodsCode = t.FkGoods.GoodsCode,
                        Varity = t.FkVarietyId == null ? null : t.FkVariety.TGoodsVariety.Select(i => new GoodsVarietyGetDto()
                        {
                            FkGoodsId = i.FkGoodsId,
                            FkProviderId = i.FkProviderId,
                            FkVariationParameterId = i.FkVariationParameterId,
                            FkVariationParameterValueId = i.FkVariationParameterValueId,
                            ImageUrl = i.ImageUrl,
                            ParameterTitle = JsonExtensions.JsonValue(i.FkVariationParameter.ParameterTitle, header.Language),
                            ValueTitle = JsonExtensions.JsonValue(i.FkVariationParameterValue.Value, header.Language),
                            VarietyId = i.VarietyId
                        }).ToList(),
                        SerialNumber = t.FkGoods.SerialNumber,
                        ProductTitle = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                        Image = t.FkGoods.ImageUrl,
                    }).ToList(),
                    TDiscountCustomers = x.TDiscountCustomers.Select(t => new DiscountCustomersGetDto()
                    {
                        AssignedCustomerId = t.AssignedCustomerId,
                        FkDiscountPlanId = t.FkDiscountPlanId,
                        FkCustomerId = t.FkCustomerId,
                        Allowed = t.Allowed,
                        CustomerTitle = t.FkCustomer.Name + " " + t.FkCustomer.Family
                    }).ToList(),
                    // TDiscountShops = x.TDiscountShops.Select(t => new DiscountShopsGetDto()
                    // {
                    //     FkDiscountPlanId = t.FkDiscountPlanId,
                    //     Allowed = t.Allowed,
                    //     AssignedShopId = t.AssignedShopId,
                    //     FkShopId = t.FkShopId,
                    //     ShopTitle = t.FkShop.StoreName
                    // }).ToList(),
                    TDiscountCategory = x.TDiscountCategory.Select(t => new DiscountCategoryGetDto()
                    {
                        AssingnedCategoryId = t.AssingnedCategoryId,
                        FkDiscountPlanId = t.FkDiscountPlanId,
                        FkCategoryId = t.FkCategoryId,
                        Allowed = t.Allowed,
                        CategoryId = t.FkCategory.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language),
                        CategoryPath = t.FkCategory.CategoryPath
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.PlanId == discountId && (token.Rule == UserGroupEnum.Seller ? k.FkShopId == token.Id : true));

                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ShopHasDiscount(int shopId, long planId)
        {
            try
            {
                return await _context.TDiscountPlan.AsNoTracking().AnyAsync(x => x.FkShopId == shopId && x.PlanId == planId);
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}