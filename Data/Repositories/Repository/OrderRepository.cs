using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public IAccountingRepository _accountingRepository { get; }
        public IWareHouseRepository _wareHouseRepository { get; }
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public OrderRepository(MarketPlaceDbContext context, IWareHouseRepository wareHouseRepository, IHttpContextAccessor httpContextAccessor, IAccountingRepository accountingRepository)
        {
            _accountingRepository = accountingRepository;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            _wareHouseRepository = wareHouseRepository;
            this._context = context;
        }
        public async Task<List<OrderListDto>> GetOrderList(OrderListPaginationDto pagination)
        {
            try
            {
                return await _context.TOrder.Include(x => x.TOrderItem).Where(x =>
                  x.FkOrderStatusId != (int)OrderStatusEnum.Cart &&
                  (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                  (pagination.OrderId == 0 ? true : x.OrderId == pagination.OrderId) &&
                  (pagination.PaymentMethodId == 0 ? true : x.FkPaymentMethodId == pagination.PaymentMethodId) &&
                  (pagination.ShippingMethodId == 0 ? true : x.TOrderItem.Any(t => t.FkShippingMethodId == pagination.ShippingMethodId)) &&
                  (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.TrackingCode.Contains(pagination.TrackingCode)) &&
                  (pagination.StatusId == 0 ? true : x.FkOrderStatusId == pagination.StatusId) &&
                  (pagination.PlaceFrom == (DateTime?)null ? true : x.PlacedDateTime >= pagination.PlaceFrom) &&
                  (pagination.PlaceTo == (DateTime?)null ? true : x.PlacedDateTime <= pagination.PlaceTo) &&
                  (pagination.ShopId == 0 ? true : x.TOrderItem.Any(t => t.FkShopId == pagination.ShopId))
                )
                .OrderByDescending(x => x.OrderId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkCustomer)
                .Include(x => x.FkOrderStatus)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop)
                .Select(x => new OrderListDto()
                {
                    OrderId = x.OrderId,
                    FkCustomerId = x.FkCustomerId,
                    CustomerName = x.FkCustomer.Name + " " + x.FkCustomer.Family,
                    FkOrderStatusId = x.FkOrderStatusId,
                    StatusColor = "#" + x.FkOrderStatus.Color,
                    OrderStatusTitle = JsonExtensions.JsonValue(x.FkOrderStatus.StatusTitle, header.Language),
                    PaymentStatus = x.PaymentStatus,
                    PlacedDateTime = Extentions.PersianDateString( (DateTime) x.PlacedDateTime),
                    FinalPrice = decimal.Round((decimal)x.FinalPrice * pagination.Rate, 2, MidpointRounding.AwayFromZero),
                    Shops = x.TOrderItem.Select(t => new ShopFormDto()
                    {
                        ShopId = t.FkShopId,
                        ShopTitle = t.FkShop.StoreName
                    }).ToList(),
                   ShopString =  string.Join(",", x.TOrderItem.Select(t => t.FkShop.StoreName)),
                    
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<decimal?> GetOrderListComissionPrice(OrderListPaginationDto pagination)
        {
            try
            {

                var comissionPrice = await _context.TOrder.Include(x => x.TOrderItem).Where(x =>
                 x.FkOrderStatusId != (int)OrderStatusEnum.Cart &&
                 (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                 (pagination.OrderId == 0 ? true : x.OrderId == pagination.OrderId) &&
                 (pagination.PaymentMethodId == 0 ? true : x.FkPaymentMethodId == pagination.PaymentMethodId) &&
                 (pagination.PlaceFrom == (DateTime?)null ? true : x.PlacedDateTime >= pagination.PlaceFrom) &&
                 (pagination.PlaceTo == (DateTime?)null ? true : x.PlacedDateTime <= pagination.PlaceTo) &&
                 (pagination.ShippingMethodId == 0 ? true : x.TOrderItem.Any(t => t.FkShippingMethodId == pagination.ShippingMethodId)) &&
                 (pagination.ShopId == 0 ? true : x.TOrderItem.Any(t => t.FkShopId == pagination.ShopId)) &&
                 (pagination.StatusId == 0 ? true : x.FkOrderStatusId == pagination.StatusId) &&
                 (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.TrackingCode.Contains(pagination.TrackingCode))
                ).AsNoTracking().SumAsync(x => x.ComissionPrice);

                return decimal.Round((decimal)comissionPrice * pagination.Rate, 2, MidpointRounding.AwayFromZero);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<int> GetOrderListCount(OrderListPaginationDto pagination)
        {
            return await _context.TOrder.Include(x => x.TOrderItem).AsNoTracking().CountAsync(x =>
              x.FkOrderStatusId != (int)OrderStatusEnum.Cart &&
              (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
              (pagination.OrderId == 0 ? true : x.OrderId == pagination.OrderId) &&
              (pagination.PaymentMethodId == 0 ? true : x.FkPaymentMethodId == pagination.PaymentMethodId) &&
              (pagination.PlaceFrom == (DateTime?)null ? true : x.PlacedDateTime >= pagination.PlaceFrom) &&
              (pagination.PlaceTo == (DateTime?)null ? true : x.PlacedDateTime <= pagination.PlaceTo) &&
              (pagination.ShippingMethodId == 0 ? true : x.TOrderItem.Any(t => t.FkShippingMethodId == pagination.ShippingMethodId)) &&
              (pagination.ShopId == 0 ? true : x.TOrderItem.Any(t => t.FkShopId == pagination.ShopId)) &&
              (pagination.StatusId == 0 ? true : x.FkOrderStatusId == pagination.StatusId) &&
              (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.TrackingCode.Contains(pagination.TrackingCode))
            );
        }

        public async Task<decimal?> GetOrderListDiscount(OrderListPaginationDto pagination)
        {
            try
            {
                var discountAmount = await _context.TOrder.Include(x => x.TOrderItem).Where(x =>
                 x.FkOrderStatusId != (int)OrderStatusEnum.Cart &&
                 (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                 (pagination.OrderId == 0 ? true : x.OrderId == pagination.OrderId) &&
                 (pagination.PaymentMethodId == 0 ? true : x.FkPaymentMethodId == pagination.PaymentMethodId) &&
                 (pagination.PlaceFrom == (DateTime?)null ? true : x.PlacedDateTime >= pagination.PlaceFrom) &&
                 (pagination.PlaceTo == (DateTime?)null ? true : x.PlacedDateTime <= pagination.PlaceTo) &&
                 (pagination.ShippingMethodId == 0 ? true : x.TOrderItem.Any(t => t.FkShippingMethodId == pagination.ShippingMethodId)) &&
                 (pagination.ShopId == 0 ? true : x.TOrderItem.Any(t => t.FkShopId == pagination.ShopId)) &&
                 (pagination.StatusId == 0 ? true : x.FkOrderStatusId == pagination.StatusId) &&
                 (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.TrackingCode.Contains(pagination.TrackingCode))
                ).AsNoTracking().SumAsync(x => x.DiscountAmount);
                return decimal.Round((decimal)discountAmount * pagination.Rate, 2, MidpointRounding.AwayFromZero);

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal?> GetOrderListFinalPrice(OrderListPaginationDto pagination)
        {
            try
            {
                var finalPrice = await _context.TOrder.Include(x => x.TOrderItem).Where(x =>
                  x.FkOrderStatusId != (int)OrderStatusEnum.Cart &&
                  (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                  (pagination.OrderId == 0 ? true : x.OrderId == pagination.OrderId) &&
                  (pagination.PaymentMethodId == 0 ? true : x.FkPaymentMethodId == pagination.PaymentMethodId) &&
                  (pagination.PlaceFrom == (DateTime?)null ? true : x.PlacedDateTime >= pagination.PlaceFrom) &&
                  (pagination.PlaceTo == (DateTime?)null ? true : x.PlacedDateTime <= pagination.PlaceTo) &&
                  (pagination.ShippingMethodId == 0 ? true : x.TOrderItem.Any(t => t.FkShippingMethodId == pagination.ShippingMethodId)) &&
                  (pagination.ShopId == 0 ? true : x.TOrderItem.Any(t => t.FkShopId == pagination.ShopId)) &&
                  (pagination.StatusId == 0 ? true : x.FkOrderStatusId == pagination.StatusId) &&
                  (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.TrackingCode.Contains(pagination.TrackingCode))
                ).AsNoTracking().SumAsync(x => x.FinalPrice);

                return decimal.Round((decimal)finalPrice * pagination.Rate, 2, MidpointRounding.AwayFromZero);

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal?> GetOrderListShipingCost(OrderListPaginationDto pagination)
        {
            try
            {
                var shippingCost = await _context.TOrder.Include(x => x.TOrderItem).Where(x =>
                  x.FkOrderStatusId != (int)OrderStatusEnum.Cart &&
                  (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                  (pagination.OrderId == 0 ? true : x.OrderId == pagination.OrderId) &&
                  (pagination.PaymentMethodId == 0 ? true : x.FkPaymentMethodId == pagination.PaymentMethodId) &&
                  (pagination.PlaceFrom == (DateTime?)null ? true : x.PlacedDateTime >= pagination.PlaceFrom) &&
                  (pagination.PlaceTo == (DateTime?)null ? true : x.PlacedDateTime <= pagination.PlaceTo) &&
                  (pagination.ShippingMethodId == 0 ? true : x.TOrderItem.Any(t => t.FkShippingMethodId == pagination.ShippingMethodId)) &&
                  (pagination.ShopId == 0 ? true : x.TOrderItem.Any(t => t.FkShopId == pagination.ShopId)) &&
                  (pagination.StatusId == 0 ? true : x.FkOrderStatusId == pagination.StatusId) &&
                  (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.TrackingCode.Contains(pagination.TrackingCode))
                ).AsNoTracking().SumAsync(x => x.ShippingCost);

                return decimal.Round((decimal)shippingCost * pagination.Rate, 2, MidpointRounding.AwayFromZero);

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal?> GetOrderListVatAmount(OrderListPaginationDto pagination)
        {
            try
            {
                var vatamount = await _context.TOrder.Include(x => x.TOrderItem).Where(x =>
                  x.FkOrderStatusId != (int)OrderStatusEnum.Cart &&
                  (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                  (pagination.OrderId == 0 ? true : x.OrderId == pagination.OrderId) &&
                  (pagination.PaymentMethodId == 0 ? true : x.FkPaymentMethodId == pagination.PaymentMethodId) &&
                  (pagination.PlaceFrom == (DateTime?)null ? true : x.PlacedDateTime >= pagination.PlaceFrom) &&
                  (pagination.PlaceTo == (DateTime?)null ? true : x.PlacedDateTime <= pagination.PlaceTo) &&
                  (pagination.ShippingMethodId == 0 ? true : x.TOrderItem.Any(t => t.FkShippingMethodId == pagination.ShippingMethodId)) &&
                  (pagination.ShopId == 0 ? true : x.TOrderItem.Any(t => t.FkShopId == pagination.ShopId)) &&
                  (pagination.StatusId == 0 ? true : x.FkOrderStatusId == pagination.StatusId) &&
                  (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.TrackingCode.Contains(pagination.TrackingCode))
                ).AsNoTracking().SumAsync(x => x.Vatamount);

                return decimal.Round((decimal)vatamount * pagination.Rate, 2, MidpointRounding.AwayFromZero);


            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<double?> GetOrderListItemCount(OrderListPaginationDto pagination)
        {
            try
            {
                return await _context.TOrder.Include(x => x.TOrderItem).Where(x =>
                 x.FkOrderStatusId != (int)OrderStatusEnum.Cart &&
                 (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                 (pagination.OrderId == 0 ? true : x.OrderId == pagination.OrderId) &&
                 (pagination.PaymentMethodId == 0 ? true : x.FkPaymentMethodId == pagination.PaymentMethodId) &&
                 (pagination.PlaceFrom == (DateTime?)null ? true : x.PlacedDateTime >= pagination.PlaceFrom) &&
                 (pagination.PlaceTo == (DateTime?)null ? true : x.PlacedDateTime <= pagination.PlaceTo) &&
                 (pagination.ShippingMethodId == 0 ? true : x.TOrderItem.Any(t => t.FkShippingMethodId == pagination.ShippingMethodId)) &&
                 (pagination.ShopId == 0 ? true : x.TOrderItem.Any(t => t.FkShopId == pagination.ShopId)) &&
                 (pagination.StatusId == 0 ? true : x.FkOrderStatusId == pagination.StatusId) &&
                 (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.TrackingCode.Contains(pagination.TrackingCode))
               ).AsNoTracking().SumAsync(x => x.TOrderItem.Sum(t => t.ItemCount));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<OrderDetailGetDto> GetOrderDetail(long orderId, decimal rate , int shopId)
        {
            try
            {
                if(shopId == 0 && token.Rule == UserGroupEnum.Admin) {
                return await _context.TOrder
                .Include(x => x.AdFkCity)
                .Include(x => x.FkCustomer)
                .Include(x => x.FkDiscountCode)
                .Include(x => x.AdFkCountry)
                .Include(x => x.FkPaymentMethod)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkShippingMethod)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkStatus)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                .Select(x => new OrderDetailGetDto()
                {
                    OrderId = x.OrderId,
                    PlacedDateTime = x.PlacedDateTime != null ? Extentions.PersianDateString( (DateTime)x.PlacedDateTime) : null,
                    AdFkCountryId = x.AdFkCountryId,
                    AdLocationX = x.AdLocationX,
                    AdLocationY = x.AdLocationY,
                    AdPostalCode = x.AdPostalCode,
                    AdTransfereeFamily = x.AdTransfereeFamily,
                    AdTransfereeMobile = "+" + x.AdFkCountry.PhoneCode + x.AdTransfereeMobile,
                    AdTransfereeName = x.AdTransfereeName,
                    AdTransfereeTel = x.AdTransfereeTel,
                    AdAddress = x.AdAddress,
                    CityName = JsonExtensions.JsonValue(x.AdFkCity.CityTitle, header.Language),
                    ComissionPrice = decimal.Round((decimal)x.ComissionPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    CustomerName = x.FkCustomer.Name + " " + x.FkCustomer.Family,
                    CustomerId = x.FkCustomerId,
                    MobileNumber = x.FkCustomer.MobileNumber,
                    CountryName = JsonExtensions.JsonValue(x.AdFkCountry.CountryTitle, header.Language),
                    DiscountAmount = decimal.Round((decimal)x.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    DiscountCode = x.FkDiscountCode.DiscountCode,
                    Email = x.FkCustomer.Email,
                    FinalPrice = decimal.Round((decimal)x.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    FkDiscountCodeId = x.FkDiscountCodeId,
                    FkPaymentMethodId = x.FkPaymentMethodId,
                    MethodTitle = JsonExtensions.JsonValue(x.FkPaymentMethod.MethodTitle, header.Language),
                    PaymentStatus = x.PaymentStatus,
                    Price = decimal.Round((decimal)x.Price  / rate, 2, MidpointRounding.AwayFromZero),
                    ShippingCost = decimal.Round((decimal)x.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                    TrackingCode = x.TrackingCode,
                    Vatamount = decimal.Round((decimal)x.Vatamount  / rate, 2, MidpointRounding.AwayFromZero),
                    Items = x.TOrderItem.Select(t => new OrderDetailListDto()
                    {
                        ComissionPrice = decimal.Round((decimal)t.ComissionPrice  / rate, 2, MidpointRounding.AwayFromZero),
                        DeliveredDate = t.DeliveredDate != null ? Extentions.PersianDateString( (DateTime)t.DeliveredDate) : null ,
                        DiscountAmount = decimal.Round((decimal)t.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                        FinalPrice = decimal.Round((decimal)t.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                        FkGoodsId = t.FkGoodsId,
                        FkShippingMethodId = t.FkShippingMethodId,
                        FkShopId = t.FkShopId,
                        FkStatusId = t.FkStatusId,
                        GoodsCode = t.FkGoods.GoodsCode,
                        GoodsTitle = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                        GuaranteeMonthDuration = t.GuaranteeMonthDuration,
                        HaveGuarantee = t.HaveGuarantee,
                        ImageUrl = t.FkGoods.ImageUrl,
                        ItemCount = t.ItemCount,
                        ItemId = t.ItemId,
                        ReturningAllowed = t.ReturningAllowed,
                        SerialNumber = t.FkGoods.SerialNumber,
                        ShippingCost = t.ShippingCost,
                        ShippingMethodTitle = JsonExtensions.JsonValue(t.FkShippingMethod.ShippingMethodTitle, header.Language),
                        ShippmentDate = t.ShippmentDate != null ? Extentions.PersianDateString( (DateTime)t.ShippmentDate) : null,
                        ShopTitle = t.FkShop.StoreName,
                        StatusTitle = JsonExtensions.JsonValue(t.FkStatus.StatusTitle, header.Language),
                        UnitPrice = t.UnitPrice  / rate,
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
                        }).ToList()
                    }).ToList()
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == orderId);

                }

              if(shopId != 0 && token.Rule == UserGroupEnum.Seller) {
                return await _context.TOrder
                .Include(x => x.AdFkCity)
                .Include(x => x.FkCustomer)
                .Include(x => x.FkDiscountCode)
                .Include(x => x.AdFkCountry)
                .Include(x => x.FkPaymentMethod)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkShippingMethod)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkStatus)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                .Where(v => v.TOrderItem.Any(c=>c.ItemId ==  orderId && c.FkShopId == shopId))
                .Select(x => new OrderDetailGetDto()
                {
                    OrderId = x.OrderId,
                    PlacedDateTime = x.PlacedDateTime != null ? Extentions.PersianDateString((DateTime)x.PlacedDateTime) : null,
                    AdFkCountryId = x.AdFkCountryId,
                    AdLocationX = x.AdLocationX,
                    AdLocationY = x.AdLocationY,
                    AdPostalCode = x.AdPostalCode,
                    AdTransfereeFamily = x.AdTransfereeFamily,
                    AdTransfereeMobile = "+" + x.AdFkCountry.PhoneCode + x.AdTransfereeMobile,
                    AdTransfereeName = x.AdTransfereeName,
                    AdTransfereeTel = x.AdTransfereeTel,
                    AdAddress = x.AdAddress,
                    CityName = JsonExtensions.JsonValue(x.AdFkCity.CityTitle, header.Language),
                    CustomerName = x.FkCustomer.Name + " " + x.FkCustomer.Family,
                    CustomerId = x.FkCustomerId,
                    MobileNumber = x.FkCustomer.MobileNumber,
                    CountryName = JsonExtensions.JsonValue(x.AdFkCountry.CountryTitle, header.Language),
                    Email = x.FkCustomer.Email,
                    FkPaymentMethodId = x.FkPaymentMethodId,
                    TrackingCode = x.TrackingCode,
                    MethodTitle = JsonExtensions.JsonValue(x.FkPaymentMethod.MethodTitle, header.Language),
                    PaymentStatus = x.PaymentStatus,
                    Items = x.TOrderItem.Select(t => new OrderDetailListDto()
                    {
                        ComissionPrice = decimal.Round((decimal)t.ComissionPrice  / rate, 2, MidpointRounding.AwayFromZero),
                        DiscountAmount = decimal.Round((decimal)t.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                        FinalPrice = decimal.Round((decimal)t.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                        FkGoodsId = t.FkGoodsId,
                        FkShippingMethodId = t.FkShippingMethodId,
                        FkShopId = t.FkShopId,
                        FkStatusId = t.FkStatusId,
                        GoodsCode = t.FkGoods.GoodsCode,
                        GoodsTitle = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                        GuaranteeMonthDuration = t.GuaranteeMonthDuration,
                        HaveGuarantee = t.HaveGuarantee,
                        ImageUrl = t.FkGoods.ImageUrl,
                        ItemCount = t.ItemCount,
                        ItemId = t.ItemId,
                        DeliveredDate =  t.DeliveredDate != null ? Extentions.PersianDateString((DateTime) t.DeliveredDate ) : null,
                        Vatamount = t.Vatamount,
                        ReturningAllowed = t.ReturningAllowed,
                        SerialNumber = t.FkGoods.SerialNumber,
                        ShippingCost = t.ShippingCost,
                        ShippingMethodTitle = JsonExtensions.JsonValue(t.FkShippingMethod.ShippingMethodTitle, header.Language),
                        ShippmentDate =   t.ShippmentDate != null ?  Extentions.PersianDateString((DateTime) t.ShippmentDate ) : null,
                        ShopTitle = t.FkShop.StoreName,
                        StatusTitle = JsonExtensions.JsonValue(t.FkStatus.StatusTitle, header.Language),
                        UnitPrice = t.UnitPrice  / rate,
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
                        }).ToList()
                    }).ToList()
                })
                .AsNoTracking().FirstOrDefaultAsync();

              }

              else {
                return null;
              }

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<OrderLogDto>> GetOrderLog(long orderId)
        {
            try
            {
                return await _context.TOrderLog.Include(x => x.FkOrder).ThenInclude(t => t.TOrderItem).Where(x => (token.Rule == UserGroupEnum.Seller ? (x.FkOrderItemId == orderId && x.FkOrderItem.FkShopId == token.Id) : x.FkOrderId == orderId))
                .Include(x => x.FkStatus)
                .Include(x => x.FkUser)
                .Select(x => new OrderLogDto()
                {
                    FkOrderId = x.FkOrderId,
                    FkStatusId = x.FkStatusId,
                    FkUserId = x.FkUserId,
                    FkOrderItemId = x.FkOrderItemId,
                    LogComment = x.LogComment,
                    LogDateTime = Extentions.PersianDateString( (DateTime)x.LogDateTime),
                    LogId = x.LogId,
                    StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                    UserName = x.FkUser.UserName
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<LiveCartListDto>> GetLiveCartList(LiveCartListPaginationDto pagination)
        {
            try
            {
                return await _context.TOrder.Where(x =>
                x.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                (pagination.FromDate == (DateTime?)null ? true : x.InitialDateTime >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.InitialDateTime <= pagination.ToDate)
                )
                .OrderByDescending(x => x.OrderId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkCustomer)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety)
                .Select(x => new LiveCartListDto()
                {
                    CustomerName = x.FkCustomer.Name + " " + x.FkCustomer.Family,
                    FinalPrice = decimal.Round((decimal)x.TOrderItem.Sum(t => t.FkVariety.FinalPrice * (decimal)t.ItemCount) * pagination.Rate, 2, MidpointRounding.AwayFromZero),
                    FkCustomerId = x.FkCustomerId,
                    InitialDateTime = Extentions.PersianDateString( (DateTime) x.InitialDateTime),
                    ItemCount = x.TOrderItem.Count,
                    GoodsCount = x.TOrderItem.Sum(t => t.ItemCount),
                    OrderId = x.OrderId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetLiveCartListCount(LiveCartListPaginationDto pagination)
        {
            try
            {
                return await _context.TOrder.AsNoTracking().CountAsync(x =>
                x.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                (pagination.FromDate == (DateTime?)null ? true : x.InitialDateTime >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.InitialDateTime <= pagination.ToDate)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<LiveCartDetailGetDto> GetLiveCartDetail(long orderId, decimal rate)
        {
            try
            {
                return await _context.TOrder
                .Include(x => x.FkCustomer)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                .Select(x => new LiveCartDetailGetDto()
                {
                    CustomerId = x.FkCustomerId,
                    CustomerName = x.FkCustomer.Name + " " + x.FkCustomer.Family,
                    Email = x.FkCustomer.Email,
                    OrderId = x.OrderId,
                    Phone = x.FkCustomer.MobileNumber,
                    Items = x.TOrderItem.Select(t => new LiveCartDetailDto()
                    {
                        FinalPrice = decimal.Round((decimal)t.FkVariety.FinalPrice  / rate * (decimal)t.ItemCount, 2, MidpointRounding.AwayFromZero),
                        FkGoodsId = t.FkGoodsId,
                        GoodsCode = t.FkGoods.GoodsCode,
                        GoodsTitle = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                        ImageUrl = t.FkGoods.ImageUrl,
                        ItemCount = t.ItemCount,
                        SerialNumber = t.FkGoods.SerialNumber,
                        UnitPrice = decimal.Round((decimal)t.FkVariety.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
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
                        }).ToList()
                    }).ToList()
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == orderId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteLiveCart(long orderId)
        {
            try
            {
                var data = await _context.TOrder.Include(x => x.TOrderItem)
                .FirstOrDefaultAsync(x => x.OrderId == orderId && x.FkOrderStatusId == (int)OrderStatusEnum.Cart);

                if (data == null)
                {
                    return false;
                }

                _context.TOrderItem.RemoveRange(data.TOrderItem);
                _context.TOrder.Remove(data);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<ShippmentDto> GetShippmentList(ShippmentPaginationDto pagination)
        {
            try
            {
                var ordersItem =  await _context.TOrderItem
                .Include(x => x.FkOrder)
                .ThenInclude(c => c.AdFkCountry)
                .Include(x => x.FkOrder)
                .ThenInclude(c => c.AdFkCity)
                .Include(x => x.FkOrder)
                .ThenInclude(c => c.AdFkProvince)
                .Where(x =>
                x.FkStatusId != (int) OrderStatusEnum.Cart &&
                x.FkStatusId != (int) OrderStatusEnum.Cancelled &&
                x.FkStatusId != (int) OrderStatusEnum.ReturnComplete &&
                x.FkStatusId != (int) OrderStatusEnum.ReturnProcessing &&
                x.FkShippingMethodId != null &&
                (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
                (pagination.MethodId == 0 ? true : x.FkShippingMethodId == pagination.MethodId) &&
                (pagination.OrderId == 0 ? true : x.ItemId == pagination.OrderId) &&
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                (pagination.DeliveredFrom == (DateTime?)null ? true : x.DeliveredDate >= pagination.DeliveredFrom) &&
                (pagination.PlacedFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.PlacedFrom) &&
                (pagination.ShippmentFrom == (DateTime?)null ? true : x.ShippmentDate >= pagination.ShippmentFrom) &&
                (pagination.DeliveredFrom == (DateTime?)null ? true : x.DeliveredDate <= pagination.DeliveredTo) &&
                (pagination.PlacedFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.PlacedTo) &&
                (pagination.ShippmentFrom == (DateTime?)null ? true : x.ShippmentDate <= pagination.ShippmentTo)
                )
                .OrderByDescending(x => x.ItemId)
                .Include(x => x.FkShop)
                .Include(x => x.FkShippingMethod)
                .Include(x => x.FkOrder).ThenInclude(t => t.FkCustomer)
                .AsNoTracking().ToListAsync();

            var result1 = ordersItem
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .GroupBy(e => e.FkShopId)
                .ToDictionary(e => e.Key, e => e.Select(x => new ShippmentListDto()
                {
                    CustomerId = x.FkOrder.FkCustomerId,
                    CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                    CustomerEmail = x.FkOrder.FkCustomer.Email,
                    CustomerMobileNumber = x.FkOrder.FkCustomer.MobileNumber,
                    DeliveredDate =  x.DeliveredDate != null ? Extentions.PersianDateString( (DateTime)  x.DeliveredDate) : null,
                    FkOrderId = x.FkOrderId,
                    ShippingCost = x.ShippingCost,
                    FkShippingMethodId = x.FkShippingMethodId,
                    FkShopId = x.FkShopId,
                    FkVarietyId = x.FkVarietyId,
                    ItemId = x.ItemId,
                    PlacedDateTime = x.FkOrder.PlacedDateTime != null ?  Extentions.PersianDateString( (DateTime) x.FkOrder.PlacedDateTime) : null,
                    ShippingMethodTitle = JsonExtensions.JsonGet(x.FkShippingMethod.ShippingMethodTitle, header),
                    ShippmentDate =  x.ShippmentDate != null ? Extentions.PersianDateString( (DateTime)  x.ShippmentDate) : null,
                    shopId = x.FkShopId,
                    ShopTitle = x.FkShop.StoreName,
                    Country = JsonExtensions.JsonGet(x.FkOrder.AdFkCountry.CountryTitle, header),
                    Province = JsonExtensions.JsonGet(x.FkOrder.AdFkProvince.ProvinceName, header),
                    City = JsonExtensions.JsonGet(x.FkOrder.AdFkCity.CityTitle, header),
                    Address = x.FkOrder.AdAddress,
                }).ToArray());   
             
             var shippmment = new ShippmentDto();
             shippmment.ShippmentList = new List<ShippmentListDto>();
             shippmment.Count = result1.Count;
            foreach(var item in result1)
            {      
                if(item.Value.Length > 0) {
                   shippmment.ShippmentList.Add(item.Value[0]);
                }
                // foreach(var exercise in student.Value)
                //     Console.Write($"{ exercise } ");
                // Console.WriteLine();
            }             

            return shippmment ;


            }
            catch (System.Exception)
            {
                return null;
            }
        }

        // public async Task<int> GetShippmentListCount(ShippmentPaginationDto pagination)
        // {
        //     try
        //     {
        //         return await _context.TOrderItem
        //         .Include(x => x.FkOrder)
        //         .AsNoTracking()
        //         .CountAsync(x =>
        //         (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
        //         (pagination.MethodId == 0 ? true : x.FkShippingMethodId == pagination.MethodId) &&
        //         (String.IsNullOrWhiteSpace(pagination.OrderId)  ? true : x.FkOrder.TrackingCode == pagination.OrderId) &&
        //         (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
        //         (pagination.DeliveredFrom == (DateTime?)null ? true : x.DeliveredDate >= pagination.DeliveredFrom) &&
        //         (pagination.PlacedFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.PlacedFrom) &&
        //         (pagination.ShippmentFrom == (DateTime?)null ? true : x.ShippmentDate >= pagination.ShippmentFrom) &&
        //         (pagination.DeliveredFrom == (DateTime?)null ? true : x.DeliveredDate <= pagination.DeliveredTo) &&
        //         (pagination.PlacedFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.PlacedTo) &&
        //         (pagination.ShippmentFrom == (DateTime?)null ? true : x.ShippmentDate <= pagination.ShippmentTo)
        //         );
        //     }
        //     catch (System.Exception)
        //     {

        //         throw;
        //     }
        // }

        public async Task<List<ShippmentDetailDto>> GetShippmentDetail(int shopId , int customerId)
        {
            try
            {
                var rate = (decimal)1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TOrderItem
                .Include(x => x.FkOrder).ThenInclude(t => t.FkCustomer)
                .Include(x => x.FkOrder).ThenInclude(t => t.AdFkCity)
                .Include(x => x.FkOrder).ThenInclude(t => t.AdFkCountry)
                .Include(x => x.FkGoods)
                .Include(x => x.FkVariety).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameter)
                .Include(x => x.FkVariety).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameterValue)
                .Where(x => x.FkShopId == shopId && x.FkShippingMethodId != null &&  x.FkOrder.FkCustomerId == customerId && (token.Rule == UserGroupEnum.Seller ? x.FkShopId == token.Id : true))
                .Select(x => new ShippmentDetailDto()
                {
                    FkGoodsId = x.FkGoodsId,
                    FkOrderId = x.FkOrderId,
                    FkShippingMethodId = x.FkShippingMethodId,
                    FkShopId = x.FkShopId,
                    FkVarietyId = x.FkVarietyId,
                    GoodsCode = x.FkGoods.GoodsCode,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    ImageUrl = x.FkGoods.ImageUrl,
                    ItemId = x.ItemId,
                    PlacedDateTime =  Extentions.PersianDateString((DateTime) x.FkOrder.PlacedDateTime),
                    SerialNumber = x.FkGoods.SerialNumber,
                    ShippingMethodTitle = JsonExtensions.JsonValue(x.FkShippingMethod.ShippingMethodTitle, header.Language),
                    ShippmentDate =  Extentions.PersianDateString((DateTime) x.ShippmentDate),
                    shopId = x.FkShopId,
                    CustomerId = x.FkOrder.FkCustomerId,
                    ShopTitle = x.FkShop.StoreName,
                    AdFkCityId = x.FkOrder.AdFkCityId,
                    AdFkCountryId = x.FkOrder.AdFkCountryId,
                    AdLocationX = x.FkOrder.AdLocationX,
                    AdLocationY = x.FkOrder.AdLocationY,
                    AdPostalCode = x.FkOrder.AdPostalCode,
                    AdTransfereeFamily = x.FkOrder.AdTransfereeFamily,
                    AdTransfereeMobile = x.FkOrder.AdTransfereeMobile,
                    AdTransfereeName = x.FkOrder.AdTransfereeName,
                    AdTransfereeTel = x.FkOrder.AdTransfereeTel,
                    AdAddress = x.FkOrder.AdAddress,
                    CityTitle = JsonExtensions.JsonValue(x.FkOrder.AdFkCity.CityTitle, header.Language),
                    ComissionPrice = decimal.Round((decimal)x.ComissionPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    CountryTitle = JsonExtensions.JsonValue(x.FkOrder.AdFkCountry.CountryTitle, header.Language),
                    DiscountAmount = decimal.Round((decimal)x.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    FinalPrice = decimal.Round((decimal)x.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    Heigth = x.FkGoods.Heigth,
                    ItemCount = x.ItemCount,
                    Length = x.FkGoods.Length,
                    ShippingCost = decimal.Round((decimal)x.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                    UnitPrice = decimal.Round((decimal)x.UnitPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    Vatamount = decimal.Round((decimal)x.Vatamount  / rate, 2, MidpointRounding.AwayFromZero),
                    Weight = x.FkGoods.Weight,
                    Width = x.FkGoods.Width,
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SalesDetailDto> GetSalesDetail(long itemId)
        {
            try
            {
                var rate = (decimal)1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TOrderItem
                .Include(x => x.FkOrder).ThenInclude(t => t.FkCustomer)
                .Include(x => x.FkOrder).ThenInclude(t => t.FkDiscountCode)
                .Include(x => x.FkOrder).ThenInclude(t => t.AdFkCity)
                .Include(x => x.FkOrder).ThenInclude(t => t.AdFkCountry)
                .Include(x => x.FkGoods)
                .Include(x => x.FkVariety).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameter)
                .Include(x => x.FkVariety).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameterValue)
                .Select(x => new SalesDetailDto()
                {
                    CustomerId = x.FkOrder.FkCustomerId,
                    CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                    DeliveredDate =  Extentions.PersianDateString((DateTime) x.DeliveredDate),
                    FkGoodsId = x.FkGoodsId,
                    FkOrderId = x.FkOrderId,
                    FkShippingMethodId = x.FkShippingMethodId,
                    FkShopId = x.FkShopId,
                    FkVarietyId = x.FkVarietyId,
                    GoodsCode = x.FkGoods.GoodsCode,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    ImageUrl = x.FkGoods.ImageUrl,
                    ItemId = x.ItemId,
                    PlacedDateTime =  Extentions.PersianDateString((DateTime) x.FkOrder.PlacedDateTime),
                    SerialNumber = x.FkGoods.SerialNumber,
                    ShippingMethodTitle = JsonExtensions.JsonValue(x.FkShippingMethod.ShippingMethodTitle, header.Language),
                    ShippmentDate =  Extentions.PersianDateString((DateTime) x.ShippmentDate),
                    shopId = x.FkShopId,
                    CoponCode = x.FkDiscountCodeId != null ? x.FkDiscountCode.DiscountCode : null,
                    ShopTitle = x.FkShop.StoreName,
                    AdFkCityId = x.FkOrder.AdFkCityId,
                    AdFkCountryId = x.FkOrder.AdFkCountryId,
                    AdLocationX = x.FkOrder.AdLocationX,
                    AdLocationY = x.FkOrder.AdLocationY,
                    AdTransfereeFamily = x.FkOrder.AdTransfereeFamily,
                    AdTransfereeMobile = "+" + x.FkOrder.AdFkCountry.PhoneCode + x.FkOrder.AdTransfereeMobile,
                    AdTransfereeName = x.FkOrder.AdTransfereeName,
                    AdAddress = x.FkOrder.AdAddress,
                    CityTitle = JsonExtensions.JsonValue(x.FkOrder.AdFkCity.CityTitle, header.Language),
                    ComissionPrice = decimal.Round((decimal)x.ComissionPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    CountryTitle = JsonExtensions.JsonValue(x.FkOrder.AdFkCountry.CountryTitle, header.Language),
                    DiscountAmount = decimal.Round((decimal)x.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    Email = x.FkOrder.FkCustomer.Email,
                    FinalPrice = decimal.Round((decimal)x.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    ItemCount = x.ItemCount,
                    Phone = "+" + x.FkOrder.AdFkCountry.PhoneCode + x.FkOrder.FkCustomer.MobileNumber,
                    ShippingCost = decimal.Round((decimal)x.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                    UnitPrice = x.UnitPrice  / rate,
                    Vatamount = x.Vatamount  / rate,
                    FkPaymentMethodId = x.FkOrder.FkPaymentMethodId,
                    FkStatusId = x.FkStatusId,
                    GuaranteeMonthDuration = x.GuaranteeMonthDuration,
                    HaveGuarantee = x.HaveGuarantee,
                    PaymentMethodTitle = JsonExtensions.JsonValue(x.FkOrder.FkPaymentMethod.MethodTitle, header.Language),
                    ReturningAllowed = x.ReturningAllowed,
                    StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                    TrackingCode = x.FkOrder.TrackingCode,
                    Varity = x.FkVariety.TGoodsVariety.Select(t => new GoodsVarietyGetDto()
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
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ItemId == itemId && (token.Rule == UserGroupEnum.Seller ? token.Id == x.FkShopId : true));
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<int>> ChangeStauts(long orderItemId, int statusId)
        {
            var orderItem = await _context.TOrderItem
                    .Include(x => x.FkGoods)
                    .Include(x => x.FkVariety)
                    .Include(x => x.TStockOperation)
                    .Include(x => x.FkShop).ThenInclude(t => t.TUser)
                    .Include(x => x.FkOrder).ThenInclude(t => t.TOrderItem)
                    .FirstOrDefaultAsync(x => x.ItemId == orderItemId && (token.Rule == UserGroupEnum.Seller ? token.Id == x.FkShopId : true));

            try
            {
                if (orderItem == null)
                {
                    return new RepRes<int>(Message.OrderItemNotFound, false, orderItem.FkStatusId);
                }
                if (!OrderStatusEnumMethods.CheckCanChangeToNewStatus((OrderStatusEnum)orderItem.FkStatusId, (OrderStatusEnum)statusId, token.Rule))
                {
                    return new RepRes<int>(Message.OrderStautsCantChangeToThisStatus, false, orderItem.FkStatusId);
                }

                if (statusId == (int)OrderStatusEnum.Shipped)
                {
                    var exportedNumber = orderItem.TStockOperation.Where(x => x.FkOperationTypeId == (int)StockOperationTypeEnum.Export).Sum(x => x.OperationStockCount);
                    if (exportedNumber < orderItem.ItemCount)
                    {
                        var itemShouldExportNow = (double)orderItem.ItemCount - exportedNumber;
                        if (orderItem.FkVariety.InventoryCount >= itemShouldExportNow)
                        {
                            // add stock opration
                            await _wareHouseRepository.AddStockOpration((int)StockOperationTypeEnum.Export, orderItem.FkVarietyId, orderItem.ItemId, itemShouldExportNow, orderItem.UnitPrice, "خروج از انبار برای خرید کاربر");
                        }
                        else
                        {
                            //inventory count less than item should exported. you cant change Status
                            return new RepRes<int>(Message.InventoryCountLessThanItemShouldExportedYouCantChangeStatus, false, orderItem.FkStatusId);
                        }
                    }
                    orderItem.ShippmentDate = DateTime.Now;
                }
                if (statusId == (int)OrderStatusEnum.Delivered)
                {
                    orderItem.DeliveredDate = DateTime.Now;
                }

                orderItem.FkStatusId = statusId;
                orderItem.FkOrder.TOrderItem.First(x => x.ItemId == orderItemId).FkStatusId = statusId;
                if (orderItem.FkOrder.PaymentStatus == true)
                {
                    if (statusId == (int)OrderStatusEnum.Completed)
                    {
                        var transactions = await _context.TUserTransaction.Where(x => x.FkOrderItemId == orderItemId &&
                         x.FkApprovalStatusId == (int)TransactionStatusEnum.Pending &&
                         (x.FkTransactionTypeId == (int)TransactionTypeEnum.Sales || x.FkTransactionTypeId == (int)TransactionTypeEnum.Commission)).ToListAsync();

                        foreach (var item in transactions)
                        {
                            item.FkApprovalStatusId = (int)TransactionStatusEnum.Completed;
                        }
                    }
                }

                if (statusId == (int)OrderStatusEnum.Completed)
                {
                    orderItem.FkGoods.SaleCount = orderItem.FkGoods.SaleCount + (long)orderItem.ItemCount;
                }

                orderItem.FkOrder.FkOrderStatusId = orderItem.FkOrder.TOrderItem.Min(x => x.FkStatusId);

                await _context.SaveChangesAsync();
                await this.AddOrderLog(orderItem.FkOrderId, orderItem.ItemId, orderItem.FkStatusId, token.UserId, "وضعیت سفارش تغییر کرد");

                if ((orderItem.FkOrder.PaymentStatus == true && statusId == (int)OrderStatusEnum.Completed))
                {
                    await _accountingRepository.UpdateCredit(orderItem.FkShop.TUser.FirstOrDefault().UserId);
                }

                return new RepRes<int>(Message.Successfull, true, orderItem.FkStatusId);
            }
            catch (System.Exception)
            {
                return new RepRes<int>(Message.ChangeOrderStauts, false, orderItem.FkStatusId);
            }
        }

        public async Task<List<SalesListDto>> GeSalesList(SalesListPaginationDto pagination)
        {
            try
            {

                var rate = (decimal)1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TOrderItem
                .Include(x => x.FkOrder)
                .Include(x => x.FkGoods)
                .Where(x =>
                  x.FkStatusId != (int)OrderStatusEnum.Cart &&
                  (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
                  (pagination.FromDate == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.FromDate) &&
                  (pagination.ToDate == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.ToDate) &&
                  (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                  (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
                  (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                  (pagination.CategoryId == 0 ? true : x.FkGoods.FkCategoryId == pagination.CategoryId)
                )
                .OrderByDescending(x => x.ItemId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkOrder).ThenInclude(x => x.FkCustomer)
                .Include(x => x.FkStatus)
                .Include(t => t.FkShop)
                .Include(x => x.FkVariety).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameter)
                .Include(x => x.FkVariety).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameterValue)
                .Select(x => new SalesListDto()
                {
                    ItemId = x.ItemId,
                    CustomerId = x.FkOrder.FkCustomerId,
                    CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                    StatusId = x.FkStatusId,
                    StatusColor = "#" + x.FkStatus.Color,
                    StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                    FinalPrice = decimal.Round((decimal)x.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    GoodsCode = x.FkGoods.GoodsCode,
                    GoodsImage = x.FkGoods.ImageUrl,
                    GoodsId = x.FkGoodsId,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    ItemCount = (double)x.ItemCount,
                    SerialNumber = x.FkGoods.SerialNumber,
                    ShopId = x.FkShopId,
                    ShopTitle = x.FkShop.StoreName,
                    Varity = x.FkVariety.TGoodsVariety.Select(t => new GoodsVarietyGetDto()
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
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GeSalesListCount(SalesListPaginationDto pagination)
        {
            return await _context.TOrderItem
            .Include(x => x.FkOrder)
            .Include(x => x.FkGoods)
            .AsNoTracking()
            .CountAsync(x =>
            x.FkStatusId != (int)OrderStatusEnum.Cart &&
            (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
            (pagination.FromDate == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.FromDate) &&
            (pagination.ToDate == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.ToDate) &&
            (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
            (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
            (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
            (pagination.CategoryId == 0 ? true : x.FkGoods.FkCategoryId == pagination.CategoryId)
            );
        }

        public async Task<List<ShopOrderListDto>> GetShopOrderList(OrderListPaginationDto pagination)
        {
            try
            {
                return await _context.TOrderItem
                .Include(x => x.FkOrder)
                .Where(x =>
                x.FkStatusId != (int)OrderStatusEnum.Cart &&
                (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
                (pagination.PlaceFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.PlaceFrom) &&
                (pagination.PlaceTo == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.PlaceTo) &&
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                (pagination.PaymentMethodId == 0 ? true : x.FkOrder.FkPaymentMethodId == pagination.PaymentMethodId) &&
                (pagination.ShippingMethodId == 0 ? true : pagination.ShippingMethodId == x.FkShippingMethodId) &&
                (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.FkOrder.TrackingCode.Contains(pagination.TrackingCode)) &&
                (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.OrderId == 0 ? true : x.ItemId == pagination.OrderId)
                )
                .OrderByDescending(x => x.ItemId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkOrder).ThenInclude(x => x.FkCustomer)
                .Include(x => x.FkStatus)
                .Include(t => t.FkGoods)
                .Include(x => x.FkVariety).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameter)
                .Include(x => x.FkVariety).ThenInclude(t => t.TGoodsVariety).ThenInclude(i => i.FkVariationParameterValue)
                .Select(x => new ShopOrderListDto()
                {
                    ItemId = x.ItemId,
                    CustomerId = x.FkOrder.FkCustomerId,
                    CustomerName = x.FkOrder.FkCustomer.Name + " " + x.FkOrder.FkCustomer.Family,
                    StatusId = x.FkStatusId,
                    StatusColor = "#" + x.FkStatus.Color,
                    StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                    Total = decimal.Round((decimal)x.FinalPrice * pagination.Rate, 2, MidpointRounding.AwayFromZero),
                    Date =  Extentions.PersianDateString( (DateTime)  x.FkOrder.PlacedDateTime),
                    Price = decimal.Round((decimal)x.UnitPrice * pagination.Rate, 2, MidpointRounding.AwayFromZero),
                    GoodsCode = x.FkGoods.GoodsCode,
                    GoodsImage = x.FkGoods.ImageUrl,
                    GoodsId = x.FkGoodsId,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    ItemCount = x.ItemCount,
                    SerialNumber = x.FkGoods.SerialNumber,
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetShopOrderListCount(OrderListPaginationDto pagination)
        {
            try
            {
                return await _context.TOrderItem
                .Include(x => x.FkOrder)
                .AsNoTracking()
                .CountAsync(x =>
                x.FkStatusId != (int)OrderStatusEnum.Cart &&
                (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
                (pagination.PlaceFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.PlaceFrom) &&
                (pagination.PlaceTo == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.PlaceTo) &&
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                (pagination.PaymentMethodId == 0 ? true : x.FkOrder.FkPaymentMethodId == pagination.PaymentMethodId) &&
                (pagination.ShippingMethodId == 0 ? true : pagination.ShippingMethodId == x.FkShippingMethodId) &&
                (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.FkOrder.TrackingCode.Contains(pagination.TrackingCode)) &&
                (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.OrderId == 0 ? true : x.ItemId == pagination.OrderId)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal?> GetShopOrderListDiscount(OrderListPaginationDto pagination)
        {
            try
            {
                var discountAmount = await _context.TOrderItem.Include(x => x.FkOrder).Where(x =>
               x.FkStatusId != (int)OrderStatusEnum.Cart &&
               (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
               (pagination.PlaceFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.PlaceFrom) &&
               (pagination.PlaceTo == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.PlaceTo) &&
               (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
               (pagination.PaymentMethodId == 0 ? true : x.FkOrder.FkPaymentMethodId == pagination.PaymentMethodId) &&
               (pagination.ShippingMethodId == 0 ? true : pagination.ShippingMethodId == x.FkShippingMethodId) &&
               (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.FkOrder.TrackingCode.Contains(pagination.TrackingCode)) &&
               (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
               (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
               (pagination.OrderId == 0 ? true : x.ItemId == pagination.OrderId)
              ).AsNoTracking().SumAsync(x => x.DiscountAmount);

                return decimal.Round((decimal)discountAmount * pagination.Rate, 2, MidpointRounding.AwayFromZero);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal?> GetShopOrderListShipingCost(OrderListPaginationDto pagination)
        {
            try
            {
                var shippingCost = await _context.TOrderItem.Include(x => x.FkOrder).Where(x =>
                x.FkStatusId != (int)OrderStatusEnum.Cart &&
                (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
                (pagination.PlaceFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.PlaceFrom) &&
                (pagination.PlaceTo == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.PlaceTo) &&
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                (pagination.PaymentMethodId == 0 ? true : x.FkOrder.FkPaymentMethodId == pagination.PaymentMethodId) &&
                (pagination.ShippingMethodId == 0 ? true : pagination.ShippingMethodId == x.FkShippingMethodId) &&
                (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.FkOrder.TrackingCode.Contains(pagination.TrackingCode)) &&
                (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.OrderId == 0 ? true : x.ItemId == pagination.OrderId)
              ).AsNoTracking().SumAsync(x => x.ShippingCost);
                return decimal.Round((decimal)shippingCost * pagination.Rate, 2, MidpointRounding.AwayFromZero);

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal?> GetShopOrderListVatAmount(OrderListPaginationDto pagination)
        {
            try
            {
                var vatAmount = await _context.TOrderItem.Include(x => x.FkOrder).Where(x =>
                x.FkStatusId != (int)OrderStatusEnum.Cart &&
                (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
                (pagination.PlaceFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.PlaceFrom) &&
                (pagination.PlaceTo == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.PlaceTo) &&
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                (pagination.PaymentMethodId == 0 ? true : x.FkOrder.FkPaymentMethodId == pagination.PaymentMethodId) &&
                (pagination.ShippingMethodId == 0 ? true : pagination.ShippingMethodId == x.FkShippingMethodId) &&
                (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.FkOrder.TrackingCode.Contains(pagination.TrackingCode)) &&
                (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.OrderId == 0 ? true : x.ItemId == pagination.OrderId)
              ).AsNoTracking().SumAsync(x => x.Vatamount);
                return decimal.Round((decimal)vatAmount * pagination.Rate, 2, MidpointRounding.AwayFromZero);

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal?> GetShopOrderListFinalPrice(OrderListPaginationDto pagination)
        {
            try
            {
                var finalPrice = await _context.TOrderItem.Include(x => x.FkOrder).Where(x =>
                x.FkStatusId != (int)OrderStatusEnum.Cart &&
                (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
                (pagination.PlaceFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.PlaceFrom) &&
                (pagination.PlaceTo == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.PlaceTo) &&
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                (pagination.PaymentMethodId == 0 ? true : x.FkOrder.FkPaymentMethodId == pagination.PaymentMethodId) &&
                (pagination.ShippingMethodId == 0 ? true : pagination.ShippingMethodId == x.FkShippingMethodId) &&
                (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.FkOrder.TrackingCode.Contains(pagination.TrackingCode)) &&
                (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.OrderId == 0 ? true : x.ItemId == pagination.OrderId)
              ).AsNoTracking().SumAsync(x => x.FinalPrice);
                return decimal.Round((decimal)finalPrice * pagination.Rate, 2, MidpointRounding.AwayFromZero);

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<double?> GetShopOrderListItemCount(OrderListPaginationDto pagination)
        {
            try
            {
                return await _context.TOrderItem.Include(x => x.FkOrder).Where(x =>
                x.FkStatusId != (int)OrderStatusEnum.Cart &&
                (pagination.CustomerId == 0 ? true : x.FkOrder.FkCustomerId == pagination.CustomerId) &&
                (pagination.PlaceFrom == (DateTime?)null ? true : x.FkOrder.PlacedDateTime >= pagination.PlaceFrom) &&
                (pagination.PlaceTo == (DateTime?)null ? true : x.FkOrder.PlacedDateTime <= pagination.PlaceTo) &&
                (pagination.ShopId == 0 ? true : x.FkShopId == pagination.ShopId) &&
                (pagination.PaymentMethodId == 0 ? true : x.FkOrder.FkPaymentMethodId == pagination.PaymentMethodId) &&
                (pagination.ShippingMethodId == 0 ? true : pagination.ShippingMethodId == x.FkShippingMethodId) &&
                (string.IsNullOrWhiteSpace(pagination.TrackingCode) ? true : x.FkOrder.TrackingCode.Contains(pagination.TrackingCode)) &&
                (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.OrderId == 0 ? true : x.ItemId == pagination.OrderId)
              ).AsNoTracking().SumAsync(x => x.ItemCount);

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> AddOrderLog(long fkOrderId, long? fkOrderItemId, int? fkStatusId, Guid fkUserId, string logComment)
        {
            try
            {
                var log = new TOrderLog();
                log.FkOrderId = fkOrderId;
                log.FkOrderItemId = fkOrderItemId;
                log.FkStatusId = fkStatusId;
                log.FkUserId = fkUserId;
                log.LogComment = logComment;
                log.LogDateTime = DateTime.Now;

                await _context.TOrderLog.AddAsync(log);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }




        public async Task<List<OrderCallRequestDto>> GetOrderCallRequest(OrderCallRequestPaginationDto pagination)
        {
            try
            {
                return await _context.TCallRequest.Include(x => x.FkGoodsProvider).Include(f => f.FkCustomer).Where(x =>
                  (string.IsNullOrWhiteSpace(pagination.Customer) ? true : (x.FkCustomer.Name + " " + x.FkCustomer.Family).Contains(pagination.Customer)) &&
                  (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                  (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
                  (pagination.DateFrom == (DateTime?)null ? true : x.RequestDateTime >= pagination.DateFrom) &&
                  (pagination.DateTo == (DateTime?)null ? true : x.RequestDateTime <= pagination.DateTo) &&
                  (pagination.ShopId == 0 ? true : x.FkGoodsProvider.FkShopId == pagination.ShopId)
                )
                .OrderByDescending(x => x.RequestId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkCustomer)
                .Include(x => x.FkGoods)
                .Include(x => x.FkGoodsProvider)
                .ThenInclude(y => y.FkShop)
                .Select(x => new OrderCallRequestDto()
                {
                    RequestId = x.RequestId,
                    Date =  Extentions.PersianDateString((DateTime) x.RequestDateTime),
                    Customer = x.FkCustomer.Name + " " + x.FkCustomer.Family,
                    Phone = x.FkCustomer.MobileNumber,
                    Email = x.FkCustomer.Email,
                    Vendor = x.FkGoodsProvider.FkShop.StoreName,
                    ProductName = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    Status = x.FkStatusId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetOrderCallRequestCount(OrderCallRequestPaginationDto pagination)
        {
            try
            {
                return await _context.TCallRequest.Include(x => x.FkGoodsProvider).Include(f => f.FkCustomer).CountAsync(x =>
                  (string.IsNullOrWhiteSpace(pagination.Customer) ? true : (x.FkCustomer.Name + " " + x.FkCustomer.Family).Contains(pagination.Customer)) &&
                  (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                  (pagination.StatusId == 0 ? true : x.FkStatusId == pagination.StatusId) &&
                  (pagination.DateFrom == (DateTime?)null ? true : x.RequestDateTime >= pagination.DateFrom) &&
                  (pagination.DateTo == (DateTime?)null ? true : x.RequestDateTime <= pagination.DateTo) &&
                  (pagination.ShopId == 0 ? true : x.FkGoodsProvider.FkShopId == pagination.ShopId)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }



        public async Task<bool> ChangeCallRequestStatus(long callRequestId, int status)
        {
            try
            {
                var callRequest = await _context.TCallRequest.FirstOrDefaultAsync(x => x.RequestId == callRequestId);
                if (callRequest == null)
                {
                    return false;
                }
                callRequest.FkStatusId = status;
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