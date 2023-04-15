using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Dashboard;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace MarketPlace.API.Data.Repositories.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public DashboardRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
        }

        public async Task<List<RecentOrderDto>> GetDashboardOrder(int statusId, int itemCount, decimal rate)
        {
            try
            {
                return await _context.TOrderItem.Where(x => x.FkStatusId == statusId && (token.Rule==UserGroupEnum.Seller?x.FkShopId == token.Id : true))
                .OrderByDescending(x => x.ItemId)
                .Take(itemCount)
                .Include(t=>t.FkVariety)
                .Include(t=>t.FkShop)
                .Include(t=>t.FkGoods)
                .Include(t=>t.FkOrder).ThenInclude(i=>i.FkCustomer)
                .Select(x => new RecentOrderDto()
                {
                    ItemId = x.ItemId,
                    CustomerId = x.FkOrder.FkCustomerId,
                    CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                    FinalPrice =  decimal.Round( (decimal) (statusId == (int)OrderStatusEnum.Cart? (x.FkVariety.FinalPrice * (decimal) x.ItemCount):(decimal)x.FinalPrice)/ rate, 2, MidpointRounding.AwayFromZero) ,
                    ShippingCost =  decimal.Round( (decimal) (statusId == (int)OrderStatusEnum.Cart ? 0 : x.FkOrder.ShippingCost)/ rate, 2, MidpointRounding.AwayFromZero) ,
                    VatAmount =  decimal.Round( (decimal) (statusId == (int)OrderStatusEnum.Cart ? (x.FkVariety.Vatamount * (decimal) x.ItemCount) : (x.FkOrder.Vatamount))/ rate, 2, MidpointRounding.AwayFromZero) ,
                    Discount =  decimal.Round( (decimal) ( statusId == (int)OrderStatusEnum.Cart ? (x.FkVariety.DiscountAmount * (decimal) x.ItemCount) : (x.FkOrder.DiscountAmount))/ rate, 2, MidpointRounding.AwayFromZero) ,
                    GoodsCode = x.FkGoods.GoodsCode,
                    GoodsId = x.FkGoodsId,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    ItemCount = (double)x.ItemCount,
                    SerialNumber = x.FkGoods.SerialNumber,
                    ShopId = x.FkShopId,
                    ShopTitle = x.FkShop.StoreName,
                    Date = statusId == (int)OrderStatusEnum.Cart?  Extentions.PersianDateString(x.FkOrder.InitialDateTime):  Extentions.PersianDateString( (DateTime) x.FkOrder.PlacedDateTime),
                    UnitPrice = 
                    decimal.Round( (decimal) (statusId == (int)OrderStatusEnum.Cart? x.FkVariety.Price:x.UnitPrice) / rate, 2, MidpointRounding.AwayFromZero)
                      ,
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<RecentReturnOrderDto>> GetDashboardReturnOrder(int statusId, int itemCount)
        {
            try
            {
                return await _context.TOrderReturning
                .Include(x=>x.FkOrderItem)
                .Where(x => x.FkStatusId == statusId && (token.Rule==UserGroupEnum.Seller?x.FkOrderItem.FkShopId == token.Id : true))
                .OrderByDescending(x => x.ReturningId)
                .Take(itemCount)
                .Include(x => x.FkReturningAction)
                .Include(x => x.FkReturningReason)
                .Include(x => x.FkOrderItem).ThenInclude(t=>t.FkShop)
                .Include(x => x.FkOrderItem).ThenInclude(t=>t.FkGoods)
                .Include(x => x.FkOrderItem).ThenInclude(t=>t.FkOrder).ThenInclude(i=>i.FkCustomer)
                .Select(x => new RecentReturnOrderDto()
                {
                    CustomerId = x.FkOrder.FkCustomerId,
                    CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                    FkReturningActionId = x.FkReturningActionId,
                    FkReturningReasonId = x.FkReturningReasonId,
                    GoodsCode = x.FkOrderItem.FkGoods.GoodsCode,
                    GoodsId = x.FkOrderItem.FkGoods.GoodsId,
                    GoodsImage = x.FkOrderItem.FkGoods.ImageUrl,
                    GoodsSerialNumber = x.FkOrderItem.FkGoods.SerialNumber,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkOrderItem.FkGoods.Title, header.Language),
                    RegisterDateTime = Extentions.PersianDateString(x.RegisterDateTime),
                    ReturningActionTitle = JsonExtensions.JsonValue(x.FkReturningAction.ReturningTypeTitle, header.Language),
                    ReturningId = x.ReturningId,
                    ReturningReasonTitle = JsonExtensions.JsonValue(x.FkReturningReason.ReasonTitle, header.Language),
                    ShopId = x.FkOrderItem.FkShopId,
                    ShopTitle = x.FkOrderItem.FkShop.StoreName,
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ShopRequestDto>> GetDashboardShopRequest(int itemCount)
        {
            try
            {
                return await _context.TShop.Where(x => x.FkStatusId == (int)ShopStatusEnum.Waiting)
                .OrderByDescending(x => x.ShopId)
                .Take(itemCount)
                .Include(x => x.TShopCategory).ThenInclude(t => t.FkCategory)
                .Include(x=>x.FkCountry)
                .Include(x=>x.FkPlan)
                .Select(x => new ShopRequestDto()
                {
                    CountryId = x.FkCountryId,
                    CountryTitle = JsonExtensions.JsonValue(x.FkCountry.CountryTitle,header.Language),
                    Date = Extentions.PersianDateString(x.RegisteryDateTime),
                    Email = x.Email,
                    PlanId = x.FkPlanId,
                    PlanTitle = x.FkPlanId == null ? null:JsonExtensions.JsonValue(x.FkPlan.PlanTitle,header.Language),
                    ShopId = x.ShopId,
                    ShopTitle = x.StoreName,
                    Category = x.TShopCategory.Select(t => new CategoryFormGetDto
                    {
                        CategoryId = t.FkCategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language)
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GoodsRequestDto>> GetDashboardGoodsRequest(int itemCount)
        {
            try
            {
                return await _context.TGoods.Where(x => x.IsAccepted == null && (token.Rule==UserGroupEnum.Seller?x.FkOwnerId == token.Id : true))
                .OrderByDescending(x => x.GoodsId)
                .Take(itemCount)
                .Include(x => x.FkBrand)
                .Include(x => x.FkOwner)
                .Include(x => x.FkBrand)
                .Select(x => new GoodsRequestDto()
                {
                    BrandId = x.FkBrandId,
                    BrandTitle = x.FkBrandId == null ? null : JsonExtensions.JsonValue(x.FkBrand.BrandTitle, header.Language),
                    CategoryId = x.FkCategoryId,
                    CategoryTitle = JsonExtensions.JsonValue(x.FkCategory.CategoryTitle, header.Language),
                    Date = x.RegisterDate,
                    Description = JsonExtensions.JsonValue(x.Description, header.Language),
                    GoodsCode = x.GoodsCode,
                    GoodsId = x.GoodsId,
                    Image = x.ImageUrl,
                    SerialNumber = x.SerialNumber,
                    ShopId = x.FkOwnerId,
                    GoodsTitle = JsonExtensions.JsonValue(x.Title, header.Language),
                    ShopTitle = x.FkOwnerId == null ? null : x.FkOwner.StoreName,
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<BrandRequestDto>> GetDashboardBrandRequest(int itemCount)
        {
            try
            {
                return await _context.TBrand.Where(x => x.IsAccepted == null)
                .OrderByDescending(x => x.BrandId)
                .Take(itemCount)
                .Include(x => x.TCategoryBrand).ThenInclude(t => t.FkCategory)
                .Select(x => new BrandRequestDto()
                {
                    BrandId = x.BrandId,
                    BrandTitle = JsonExtensions.JsonValue(x.BrandTitle, header.Language),
                    BrandLogoImage = x.BrandLogoImage,
                    Description = JsonExtensions.JsonValue(x.Description, header.Language),
                    Category = x.TCategoryBrand.Select(t => new CategoryFormGetDto
                    {
                        CategoryId = t.FkCategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language)
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<GarenteeRequestDto>> GetDashboardGarenteeRequest(int itemCount)
        {
            try
            {
                return await _context.TGuarantee.Where(x => x.IsAccepted == null)
                .OrderByDescending(x => x.GuaranteeId)
                .Take(itemCount)
                .Include(x => x.TCategoryGuarantee).ThenInclude(t => t.FkCategory)
                .Select(x => new GarenteeRequestDto()
                {
                    GuaranteeId = x.GuaranteeId,
                    GuaranteeTitle = JsonExtensions.JsonValue(x.GuaranteeTitle, header.Language),
                    Description = JsonExtensions.JsonValue(x.Description, header.Language),
                    Category = x.TCategoryGuarantee.Select(t => new CategoryFormGetDto
                    {
                        CategoryId = t.FkCategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language)
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<WithDrawalRequestDto>> GetDashboardWithDrawalRequest(int itemCount, decimal rate)
        {
            try
            {
                return await _context.TShopWithdrawalRequest.Where(x => x.Status == null && (token.Rule==UserGroupEnum.Seller?x.FkShopId == token.Id : true))
                .OrderByDescending(x => x.RequestId)
                .Take(itemCount)
                .Include(x => x.FkShop)
                .Select(x => new WithDrawalRequestDto()
                {
                    Amount = decimal.Round(x.Amount  / rate, 2, MidpointRounding.AwayFromZero) ,
                    FkShopId = x.FkShopId,
                    RequestDate = x.RequestDate,
                    RequestId = x.RequestId,
                    RequestText = x.RequestText,
                    ShopTitle = x.FkShop.StoreName
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetSettingItemCount()
        {
            try
            {
                return await _context.TSetting.AsNoTracking().Select(x => x.DashboardTablesRows).FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return 10;
            }
        }

        public async Task<List<ApproveRequestDto>> GetApproveRequest()
        {
            try
            {
                var Requests = new List<ApproveRequestDto>();


                var shop = new ApproveRequestDto();
                shop.Type = (int)ApproveRequestEnum.Shop;
                shop.Count = await _context.TShop.AsNoTracking().CountAsync(x => x.FkStatusId == (int)ShopStatusEnum.Waiting);
                Requests.Add(shop);


                var goods = new ApproveRequestDto();
                goods.Type = (int)ApproveRequestEnum.Goods;
                goods.Count = await _context.TGoods.AsNoTracking().CountAsync(x => x.IsAccepted == null  && (token.Rule==UserGroupEnum.Seller?x.FkOwnerId == token.Id : true));
                Requests.Add(goods);

                var brand = new ApproveRequestDto();
                brand.Type = (int)ApproveRequestEnum.Brand;
                brand.Count = await _context.TBrand.AsNoTracking().CountAsync(x => x.IsAccepted == null);
                Requests.Add(brand);

                var garentee = new ApproveRequestDto();
                garentee.Type = (int)ApproveRequestEnum.Garentee;
                garentee.Count = await _context.TGuarantee.AsNoTracking().CountAsync(x => x.IsAccepted == null);
                Requests.Add(garentee);

                var WithDrawal = new ApproveRequestDto();
                WithDrawal.Type = (int)ApproveRequestEnum.WithDrawal;
                WithDrawal.Count = await _context.TShopWithdrawalRequest.AsNoTracking().CountAsync(x => x.Status == null  && (token.Rule==UserGroupEnum.Seller?x.FkShopId == token.Id : true));
                Requests.Add(WithDrawal);

                return Requests;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<DashboardOrderStatusDto>> GetOrderStatus()
        {
            try
            {
                return await _context.TOrderStatus
                .OrderBy(x=>x.StatusId)
                .Include(x=>x.TOrderItem)
                .Select(x=> new DashboardOrderStatusDto(){
                     StatusId = x.StatusId,
                     StautsTitle = JsonExtensions.JsonValue(x.StatusTitle,header.Language),
                     Count = x.TOrderItem.Where(t=>  (token.Rule==UserGroupEnum.Seller?t.FkShopId == token.Id : true)).Count()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<DashboardOrderReturningStatusDto>> GetOrderReturningStatus()
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TReturningStatus
                    .OrderBy(x => x.StatusId)
                    .Include(x => x.TOrderReturning)
                    .Select(x => new DashboardOrderReturningStatusDto()
                    {
                        StatusId = x.StatusId,
                        StautsTitle = JsonExtensions.JsonValue(x.StatusTitle, header.Language),
                        Count = x.TOrderReturning.Count()
                    })
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    return await _context.TReturningStatus
                    .OrderBy(x => x.StatusId)
                    .Include(x => x.TOrderReturning).ThenInclude(t=>t.FkOrderItem)
                    .Select(x => new DashboardOrderReturningStatusDto()
                    {
                        StatusId = x.StatusId,
                        StautsTitle = JsonExtensions.JsonValue(x.StatusTitle, header.Language),
                        Count = x.TOrderReturning.Where(t => (token.Rule == UserGroupEnum.Seller ? t.FkOrderItem.FkShopId == token.Id : true)).Count()
                    })
                    .AsNoTracking().ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<long> GetCategoryCount()
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TCategory.AsNoTracking().CountAsync();
                }
                else
                {
                    var shopCategory = await _context.TShopCategory.Where(x=>x.FkShopId == token.Id).AsNoTracking().Select(x=>x.FkCategoryId).ToListAsync();
                    var predicates = shopCategory.Select(k => (Expression<Func<TCategory, bool>>)(x => x.CategoryPath.Contains("/"+k+"/")));
                    return await _context.TCategory.WhereAny(predicates.ToArray()).AsNoTracking().CountAsync();
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<long> GetCustomerCount()
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TCustomer.AsNoTracking().CountAsync(x => x.TUser.Any(t => t.Active == true) && x.CustomerId != -1000);
                }
                else
                {
                    return await _context.TCustomer.Include(x=>x.TOrder).ThenInclude(t=>t.TOrderItem).AsNoTracking().CountAsync(x => x.TOrder.Any(t=>t.TOrderItem.Any(i=>i.FkShopId == token.Id &&  i.FkStatusId != (int)OrderStatusEnum.Cart))&& x.TUser.Any(t => t.Active == true) && x.CustomerId != -1000);
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<long> GetOutOfStockCount()
        {
            try
            {
               return await _context.TGoodsProvider.AsNoTracking().CountAsync(x=>x.HasInventory == false && (token.Rule == UserGroupEnum.Seller ? x.FkShopId == token.Id : true)); 
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<long> GetPromotionsCount()
        {
            try
            {
               return await _context.TDiscountPlan.AsNoTracking().CountAsync(x=>x.Status == true && (token.Rule == UserGroupEnum.Seller ? x.FkShopId == token.Id : true)); 
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<long> GetShopCount()
        {
            try
            {
               return await _context.TShop.AsNoTracking().CountAsync(x=>x.FkStatusId == (int)ShopStatusEnum.Active); 
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<long> GetGoodsCount()
        {
            try
            {
               return await _context.TGoods.AsNoTracking().CountAsync(x=>x.IsAccepted == true &&(token.Rule == UserGroupEnum.Seller ?x.FkOwnerId == token.Id : true)); 
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<long> GetOrdersCount()
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TOrder.AsNoTracking().CountAsync(x => x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled && x.FkOrderStatusId != (int)OrderStatusEnum.Cart);
                }
                else
                {
                    return await _context.TOrderItem.AsNoTracking().CountAsync(x => x.FkStatusId != (int)OrderStatusEnum.Cancelled && x.FkStatusId != (int)OrderStatusEnum.Cart && x.FkShopId == token.Id);
                }

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<long> GetTodayOrdersCount()
        {
            try
            {
                if(token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TOrder.AsNoTracking().CountAsync(x=>x.PlacedDateTime.Value.Date == DateTime.Today.Date && x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled && x.FkOrderStatusId != (int)OrderStatusEnum.Cart);
                }
                else
                {
                    return await _context.TOrderItem.Include(x=>x.FkOrder).AsNoTracking().CountAsync(x=> x.FkShopId == token.Id && x.FkOrder.PlacedDateTime.Value.Date == DateTime.Today.Date && x.FkStatusId != (int)OrderStatusEnum.Cancelled && x.FkStatusId != (int)OrderStatusEnum.Cart);
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> GetIncome(decimal rate)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    decimal price = await _context.TOrder
                    .Where(x => x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled && x.FkOrderStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x => x.ComissionPrice == null ? (decimal) 0.00 : (decimal) x.ComissionPrice);

                    return decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decimal price = await _context.TOrderItem
                    .Where(x =>x.FkShopId == token.Id && x.FkStatusId != (int)OrderStatusEnum.Cancelled && x.FkStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x => x.ComissionPrice == null ? (decimal) 0.00 : (decimal)x.ComissionPrice);
                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);
                }
                
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> GetDiscount(decimal rate)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    decimal price = await _context.TOrder
                    .Where(x => x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled && x.FkOrderStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x => x.DiscountAmount == null ? (decimal) 0.00 : (decimal)x.DiscountAmount);
                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    decimal price = await _context.TOrderItem
                    .Where(x =>x.FkShopId == token.Id && x.FkStatusId != (int)OrderStatusEnum.Cancelled && x.FkStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x => x.DiscountAmount == null ? (decimal) 0.0 : (decimal)x.DiscountAmount);

                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);

                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> GetTax(decimal rate)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    decimal price = await _context.TOrder
                    .Where(x => x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled && x.FkOrderStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x => x.Vatamount == null ? (decimal) 0.00 : (decimal)x.Vatamount);
                    return decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);

                }
                else
                {
                    decimal price =  await _context.TOrderItem
                    .Where(x =>x.FkShopId == token.Id && x.FkStatusId != (int)OrderStatusEnum.Cancelled && x.FkStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x => x.Vatamount == null ? (decimal) 0.00 : (decimal)x.Vatamount);
                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);

                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> GetTotal(decimal rate)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    decimal price = await _context.TOrder
                    .Where(x => x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled && x.FkOrderStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x => x.FinalPrice == null ? (decimal) 0.00 : (decimal)x.FinalPrice);
                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);

                }
                else
                {
                    decimal price  = await _context.TOrderItem
                    .Where(x =>x.FkShopId == token.Id && x.FkStatusId != (int)OrderStatusEnum.Cancelled && x.FkStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x => x.FinalPrice == null ? (decimal) 0.00 : (decimal)x.FinalPrice);
                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);

                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> GetTodayIncome(decimal rate)
        {
            try
            {
                if(token.Rule == UserGroupEnum.Admin)
                {
                    decimal price = await _context.TOrder
                    .Where(x=>x.PlacedDateTime.Value.Date == DateTime.Today.Date &&x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled && x.FkOrderStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x=>x.ComissionPrice == null ? (decimal) 0.00 : (decimal)x.ComissionPrice);
                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);

                }
                else
                {
                    decimal price = await _context.TOrderItem.Include(x=>x.FkOrder)
                    .Where(x=>x.FkShopId == token.Id &&x.FkOrder.PlacedDateTime.Value.Date == DateTime.Today.Date &&x.FkStatusId != (int)OrderStatusEnum.Cancelled && x.FkStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x=>x.ComissionPrice== null ? (decimal) 0.00 : (decimal)x.ComissionPrice);

                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);

                }
                
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> GetTodayTotal(decimal rate)
        {
            try
            {
                if(token.Rule == UserGroupEnum.Admin)
                {
                    decimal price = await _context.TOrder
                    .Where(x=>x.PlacedDateTime.Value.Date == DateTime.Today.Date &&x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled && x.FkOrderStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x=>x.FinalPrice== null ? (decimal) 0.00 : (decimal)x.FinalPrice);
                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);

                }
                else
                {
                    decimal price = await _context.TOrderItem.Include(x=>x.FkOrder)
                    .Where(x=>x.FkShopId == token.Id &&x.FkOrder.PlacedDateTime.Value.Date == DateTime.Today.Date &&x.FkStatusId != (int)OrderStatusEnum.Cancelled && x.FkStatusId != (int)OrderStatusEnum.Cart)
                    .AsNoTracking()
                    .SumAsync(x=>x.FinalPrice== null ? (decimal) 0.00 : (decimal)x.FinalPrice);
                    return  decimal.Round(price  / rate, 2, MidpointRounding.AwayFromZero);

                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<OrderChartDto>> GetDashboardChart()
        {
            try
            {
                var Dates = new List<DateTime>();
                for (int i = -9; i <= 0; i++)
                {
                    Dates.Add(DateTime.Today.Date.AddDays(i));
                }
                var order = new List<ChartDto>();
                if (token.Rule == UserGroupEnum.Admin)
                {
                    order = await _context.TOrder.Where(x => x.FkOrderStatusId != (int)OrderStatusEnum.Cart && x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled && Dates.Contains((DateTime)x.PlacedDateTime.Value.Date))
                    .GroupBy(x => new { x.PlacedDateTime.Value.Date })
                    .OrderBy(x => x.Key.Date)
                    .Select(x => new ChartDto
                    {
                        Date = x.Key.Date,
                        Count = x.Count()
                    }).ToListAsync();
                }
                else
                {
                    order = await _context.TOrderItem.Include(x=>x.FkOrder).Where(x =>x.FkShopId == token.Id && x.FkStatusId != (int)OrderStatusEnum.Cart && x.FkStatusId != (int)OrderStatusEnum.Cancelled && Dates.Contains((DateTime)x.FkOrder.PlacedDateTime.Value.Date))
                    .GroupBy(x => new { x.FkOrder.PlacedDateTime.Value.Date })
                    .OrderBy(x => x.Key.Date)
                    .Select(x => new ChartDto
                    {
                        Date = x.Key.Date,
                        Count = x.Count()
                    }).ToListAsync();
                }
                

                var cancel = new List<ChartDto>();
                if(token.Rule == UserGroupEnum.Admin)
                {
                    cancel = await _context.TOrder.Where(x=>x.FkOrderStatusId == (int)OrderStatusEnum.Cancelled&&  Dates.Contains((DateTime)x.PlacedDateTime.Value.Date))
                    .GroupBy(x=>new {x.PlacedDateTime.Value.Date})
                    .OrderBy(x=>x.Key.Date)
                    .Select(x=>new ChartDto{
                        Date = x.Key.Date,
                        Count = x.Count()
                    }).ToListAsync();
                }
                else
                {
                    cancel = await _context.TOrderItem.Include(x=>x.FkOrder).Where(x=>x.FkShopId == token.Id && x.FkStatusId == (int)OrderStatusEnum.Cancelled&&  Dates.Contains((DateTime)x.FkOrder.PlacedDateTime.Value.Date))
                    .GroupBy(x=>new {x.FkOrder.PlacedDateTime.Value.Date})
                    .OrderBy(x=>x.Key.Date)
                    .Select(x=>new ChartDto{
                        Date = x.Key.Date,
                        Count = x.Count()
                    }).ToListAsync();
                }
                

                var charts = new List<OrderChartDto>();
                foreach (var item in Dates)
                {
                    var data = new OrderChartDto();
                    data.Date = item.ToString("MM/dd/yyyy");
                    data.order = order.FirstOrDefault(x=>x.Date == item.Date).Count;
                    data.Canceled = cancel.FirstOrDefault(x=>x.Date == item.Date).Count;
                }
                return charts;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}