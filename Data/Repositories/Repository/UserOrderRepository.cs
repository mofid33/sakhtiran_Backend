using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.PostServices.Aramex;
using MarketPlace.API.PostServices.Dhl;
using MarketPlace.API.PostServices.IranPost;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class UserOrderRepository : IUserOrderRepository
    {
        public IAccountingRepository _accountingRepository { get; }
        public IWareHouseRepository _wareHouseRepository { get; }
        public ICategoryRepository _categoryRepository { get; }
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public INotificationService _notificationService { get; }

        public UserOrderRepository(MarketPlaceDbContext context,
         ICategoryRepository categoryRepository,
         INotificationService notificationService,
         IWareHouseRepository wareHouseRepository, IHttpContextAccessor httpContextAccessor, IAccountingRepository accountingRepository)
        {
            _accountingRepository = accountingRepository;
            _notificationService = notificationService;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._context = context;
            _categoryRepository = categoryRepository;
            this._wareHouseRepository = wareHouseRepository;
        }

        public async Task<List<int>> GetGoodsIdsInOrder(int customerId, Guid cookieId)
        {
            try
            {
                return await _context.TOrderItem.Where(x =>
                (cookieId == Guid.Empty ? true : x.FkOrder.CookieId == cookieId) &&
                (customerId == 0 ? true : x.FkOrder.FkCustomerId == customerId) &&
                x.FkStatusId == (int)OrderStatusEnum.Cart
                )
                .AsNoTracking()
                .Select(x => x.FkGoodsId)
                .Distinct()
                .ToListAsync();
            }
            catch (System.Exception)
            {
                return new List<int>();
            }
        }

        public async Task<RepRes<OrderAddReturnDto>> AddOrder(OrderAddDto addOrder, int customerId, Guid? cookieId)
        {
            try
            {
                var resultReturndata = new OrderAddReturnDto();
                if (addOrder.CityId != null && addOrder.ProvinceId == null)
                {
                    var city = await _context.TCity.FirstAsync(b => b.CityId == addOrder.CityId);
                    addOrder.ProvinceId = (int)city.FkProvinceId;
                }
                var data = await _context.TOrder
                .Include(x => x.TOrderItem)
                .FirstOrDefaultAsync(x => x.FkCustomerId == customerId && x.FkOrderStatusId == (int)OrderStatusEnum.Cart && x.CookieId == cookieId && x.TOrderItem.All(t => t.FkStatusId == (int)OrderStatusEnum.Cart));

                if (data == null)
                {
                    var goods = await _context.TGoodsProvider
                    .Include(x => x.FkGoods)
                    .Include(x => x.FkShop)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ProviderId == addOrder.ProviderId && x.ToBeDisplayed == true && x.IsAccepted == true && x.FkGoods.IsAccepted == true && x.FkGoods.ToBeDisplayed == true && x.FkShop.FkStatusId == (int)ShopStatusEnum.Active && x.FkGoodsId == addOrder.GoodsId);

                    if ((addOrder.CityId != null && addOrder.ProvinceId == null && addOrder.CountryId == null)
                        || (addOrder.CityId != null && addOrder.ProvinceId != null && addOrder.CountryId == null)
                        || (addOrder.CityId == null && addOrder.ProvinceId != null && addOrder.CountryId == null)
                        || (addOrder.CityId == null && addOrder.ProvinceId != null && addOrder.CountryId != null)
                        || (addOrder.CityId == null && addOrder.ProvinceId == null && addOrder.CountryId != null)
                        || (addOrder.CityId != null && addOrder.ProvinceId == null && addOrder.CountryId != null)
                        )
                    {
                        return new RepRes<OrderAddReturnDto>(Message.YouShouldChooseACityToDeliver, false, null);
                    }

                    if (goods == null)
                    {
                        return new RepRes<OrderAddReturnDto>(Message.GoodsNotFoundById, false, null);
                    }

                    if ((goods.InventoryCount < addOrder.Number) && goods.FkGoods.IsDownloadable == false)
                    {
                        return new RepRes<OrderAddReturnDto>(Message.NotExistInWareHouse, false, null);
                    }

                    var order = new TOrder();
                    if (customerId == (int)CustomerTypeEnum.Unknown)
                    {
                        order.CookieId = cookieId;
                    }
                    order.InitialDateTime = DateTime.Now;
                    order.FkCustomerId = customerId;
                    order.FkOrderStatusId = (int)OrderStatusEnum.Cart;
                    order.PaymentStatus = false;
                    if (addOrder.CityId == null && addOrder.CountryId == null && addOrder.ProvinceId == null)
                    {
                        order.AdFkCityId = goods.FkShop.FkCityId;
                        order.AdFkCountryId = goods.FkShop.FkCountryId;
                        order.AdFkProvinceId = goods.FkShop.FkProvinceId;
                    }
                    else
                    {
                        order.AdFkCityId = addOrder.CityId;
                        order.AdFkCountryId = addOrder.CountryId;
                        order.AdFkProvinceId = addOrder.ProvinceId;
                    }

                    if (addOrder.OneClick == true)
                    {
                        var customerAddress = await _context.TCustomerAddress.FirstOrDefaultAsync(b => b.IsDefualt == true && b.FkCustomerId == customerId);
                        if (customerAddress != null)
                        {
                            if (order.AdFkCityId != customerAddress.FkCityId && order.AdFkCountryId != customerAddress.FkCountryId && order.AdFkProvinceId != customerAddress.FkProvinceId)
                            {
                                resultReturndata.SetOneClick = false;
                            }
                            else
                            {
                                resultReturndata.SetOneClick = true;
                                order.AdAddress = customerAddress.Address;
                                order.AdLocationX = customerAddress.LocationX;
                                order.AdLocationY = customerAddress.LocationY;
                                order.AdPostalCode = customerAddress.PostalCode;
                                order.AdTransfereeFamily = customerAddress.TransfereeFamily;
                                order.AdTransfereeMobile = customerAddress.TransfereeMobile;
                                order.AdTransfereeName = customerAddress.TransfereeName;
                                order.AdTransfereeTel = customerAddress.TransfereeMobile;
                            }

                        }
                        else
                        {
                            resultReturndata.SetOneClick = false;
                        }
                    }


                    var orderItems = new List<TOrderItem>();
                    var orderItem = new TOrderItem();
                    orderItem.FkGoodsId = addOrder.GoodsId;
                    orderItem.FkVarietyId = addOrder.ProviderId;
                    orderItem.FkShopId = goods.FkShopId;
                    orderItem.ItemCount = addOrder.Number;
                    orderItem.FkStatusId = (int)OrderStatusEnum.Cart;
                    orderItems.Add(orderItem);
                    order.TOrderItem = (orderItems);

                    await _context.TOrder.AddAsync(order);
                    await _context.SaveChangesAsync();
                    resultReturndata.Result = true;
                    return new RepRes<OrderAddReturnDto>(Message.Successfull, true, resultReturndata);
                }
                else
                {
                    if (data.TOrderItem.Any(x => x.FkVarietyId == addOrder.ProviderId && x.FkGoodsId == addOrder.GoodsId))
                    {
                        var orderItem = data.TOrderItem.FirstOrDefault(x => x.FkGoodsId == addOrder.GoodsId && x.FkVarietyId == addOrder.ProviderId);

                        var goods = await _context.TGoodsProvider
                        .Include(x => x.FkGoods)
                        .Include(x => x.FkShop)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ProviderId == addOrder.ProviderId && x.ToBeDisplayed == true && x.IsAccepted == true && x.FkGoods.IsAccepted == true && x.FkGoods.ToBeDisplayed == true && x.FkShop.FkStatusId == (int)ShopStatusEnum.Active && x.FkGoodsId == addOrder.GoodsId);
                        if (goods == null)
                        {
                            // tu anbar momkene mojud bashe vali khob dige hatman ejaze kharid az in kala ro nadari
                            return new RepRes<OrderAddReturnDto>(Message.NotExistInWareHouse, false, null);
                        }
                        if ((goods.InventoryCount < addOrder.Number + orderItem.ItemCount) && goods.FkGoods.IsDownloadable == false)
                        {
                            return new RepRes<OrderAddReturnDto>(Message.NotExistInWareHouse, false, null);
                        }
                        orderItem.ItemCount = goods.FkGoods.IsDownloadable == true ? 1 : orderItem.ItemCount + addOrder.Number;
                    }
                    else
                    {
                        var goods = await _context.TGoodsProvider
                        .Include(x => x.FkGoods)
                        .Include(x => x.FkShop)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ProviderId == addOrder.ProviderId && x.ToBeDisplayed == true && x.IsAccepted == true && x.FkGoods.IsAccepted == true && x.FkGoods.ToBeDisplayed == true && x.FkShop.FkStatusId == (int)ShopStatusEnum.Active && x.FkGoodsId == addOrder.GoodsId);

                        if (goods == null)
                        {
                            return new RepRes<OrderAddReturnDto>(Message.GoodsNotFoundById, false, null);
                        }

                        if ((goods.InventoryCount < addOrder.Number) && goods.FkGoods.IsDownloadable == false)
                        {
                            return new RepRes<OrderAddReturnDto>(Message.NotExistInWareHouse, false, null);
                        }

                        var orderItem = new TOrderItem();
                        orderItem.FkGoodsId = addOrder.GoodsId;
                        orderItem.FkVarietyId = addOrder.ProviderId;
                        orderItem.FkShopId = goods.FkShopId;
                        orderItem.ItemCount = addOrder.Number;
                        orderItem.FkStatusId = (int)OrderStatusEnum.Cart;
                        orderItem.FkOrderId = data.OrderId;

                        await _context.TOrderItem.AddAsync(orderItem);
                    }


                    if (addOrder.OneClick == true)
                    {
                        var customerAddress = await _context.TCustomerAddress.FirstOrDefaultAsync(b => b.IsDefualt == true && b.FkCustomerId == customerId);
                        if (customerAddress != null)
                        {
                            if (data.AdFkCityId != customerAddress.FkCityId && data.AdFkCountryId != customerAddress.FkCountryId)
                            {
                                resultReturndata.SetOneClick = false;
                            }
                            else
                            {
                                resultReturndata.SetOneClick = true;
                                data.AdAddress = customerAddress.Address;
                                data.AdLocationX = customerAddress.LocationX;
                                data.AdLocationY = customerAddress.LocationY;
                                data.AdPostalCode = customerAddress.PostalCode;
                                data.AdTransfereeFamily = customerAddress.TransfereeFamily;
                                data.AdTransfereeMobile = customerAddress.TransfereeMobile;
                                data.AdTransfereeName = customerAddress.TransfereeName;
                                data.AdTransfereeTel = customerAddress.TransfereeMobile;
                            }

                        }
                        else
                        {
                            resultReturndata.SetOneClick = false;
                        }
                    }


                    await _context.SaveChangesAsync();
                    resultReturndata.Result = true;
                }
                return new RepRes<OrderAddReturnDto>(Message.Successfull, true, resultReturndata);
            }
            catch (System.Exception)
            {
                return new RepRes<OrderAddReturnDto>(Message.OrderAdding, false, null);
            }
        }

        public async Task<RepRes<bool>> IncreaseOrderItem(OrderAddDto orderDto)
        {
            try
            {
                var orderItem = await _context.TOrderItem
                .Include(x => x.FkOrder)
                .FirstOrDefaultAsync(x => (token.Id == 0 ? (x.FkOrder.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.FkOrder.CookieId == token.CookieId) : (token.Id == x.FkOrder.FkCustomerId && x.FkOrder.CookieId == null)) && x.FkOrder.FkOrderStatusId == (int)OrderStatusEnum.Cart && x.FkStatusId == (int)OrderStatusEnum.Cart && x.FkGoodsId == orderDto.GoodsId && x.FkVarietyId == orderDto.ProviderId);

                if (orderItem == null)
                {
                    return new RepRes<bool>(Message.OrderItemNotFound, false, false);
                }

                var goods = await _context.TGoodsProvider
                .Include(x => x.FkGoods)
                .Include(x => x.FkShop)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProviderId == orderDto.ProviderId && x.ToBeDisplayed == true && x.IsAccepted == true && x.FkGoods.IsAccepted == true && x.FkGoods.ToBeDisplayed == true && x.FkShop.FkStatusId == (int)ShopStatusEnum.Active && x.FkGoodsId == orderDto.GoodsId);

                if (goods == null)
                {
                    // tu anbar momkene mojud bashe vali khob dige hatman ejaze kharid az in kala ro nadari
                    return new RepRes<bool>(Message.NotExistInWareHouse, false, false);
                }

                if (goods.InventoryCount < orderDto.Number)
                {
                    return new RepRes<bool>(Message.NotExistInWareHouse, false, false);
                }
                orderItem.ItemCount = orderDto.Number;

                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.OrderAdding, false, false);
            }
        }


        public async Task<RepRes<bool>> DeleteOrderItem(long orderItem)
        {
            try
            {
                var data = await _context.TOrderItem
                .Include(x => x.FkOrder).ThenInclude(t => t.TOrderItem)
                .Include(x => x.FkOrder).ThenInclude(b => b.TPaymentTransaction)
                .FirstOrDefaultAsync(x => x.ItemId == orderItem && x.FkOrder.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                (token.Id == 0 ? (x.FkOrder.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.FkOrder.CookieId == token.CookieId) : (token.Id == x.FkOrder.FkCustomerId && x.FkOrder.CookieId == null)) &&
                 x.FkStatusId == (int)OrderStatusEnum.Cart);
                if (data == null)
                {
                    return new RepRes<bool>(Message.OrderItemNotFound, false, false);
                }

                if (data.FkOrder.TOrderItem.Count > 1)
                {
                    data.FkOrder = null;
                    _context.TOrderItem.Remove(data);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.TOrderItem.RemoveRange(data.FkOrder.TOrderItem);
                    _context.TPaymentTransaction.RemoveRange(data.FkOrder.TPaymentTransaction);
                    _context.TOrder.Remove(data.FkOrder);
                    await _context.SaveChangesAsync();
                }

                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.OrderItemDelete, false, false);
            }
        }

        public async Task<RepRes<WebsiteOrderGetDto>> GetOrderDetail(string code, bool forPay, int? setCurrency = null, string paymentId = null)
        {
            try
            {
                var rate = (decimal)1.00;
                if (setCurrency == null)
                {
                    if (  header.CurrencyNum != CurrencyEnum.TMN)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                        rate = currency == null ? (decimal)1.00 : (decimal)currency.RatesAgainstOneDollar;
                    }
                }
                else if (setCurrency != null)
                {

                    if (setCurrency == (int)CurrencyEnum.BHD)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == CurrencyEnum.BHD.ToString());
                        rate = currency == null ? (decimal)1.00 : (decimal)currency.RatesAgainstOneDollar;
                    }

                }

                var order = await _context.TOrder
                .Include(x => x.TOrderItem)
                .Include(x => x.TPaymentTransaction)
                .Where(x => x.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                (string.IsNullOrWhiteSpace(paymentId) ?
                 (token.Id == 0 ? (x.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.CookieId == token.CookieId) : (token.Id == x.FkCustomerId && x.CookieId == null))
                : (x.TPaymentTransaction.Any(t => t.PaymentId == paymentId)))
                &&
                 x.TOrderItem.All(t => t.FkStatusId == (int)OrderStatusEnum.Cart))
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop)
                 .Include(x => x.AdFkCity)
                 .Include(x => x.FkCustomer)
                 .Include(x => x.AdFkCountry)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods).ThenInclude(t => t.FkBrand)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop).ThenInclude(c => c.FkCountry)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop).ThenInclude(c => c.FkCity)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                 .Select(x => new WebsiteOrderGetDto()
                 {
                     OrderId = x.OrderId,
                     ItemsCount = 0.0,
                     Discount = (decimal)0.00,
                     Shipping = (decimal)0.00,
                     Total = (decimal)0.00,
                     Vat = (decimal)0.00,
                     TotalWithOutDiscountCode = (decimal)0.00,
                     CityId = (int)x.AdFkCityId,
                     CountryId = (int)x.AdFkCountryId,
                     ProvinceId = (int)x.AdFkProvinceId,
                     Address = forPay == false ? null : x.AdAddress,
                     CityTitle = JsonExtensions.JsonValue(x.AdFkCity.CityTitle, header.Language),
                     ProvinceTitle = JsonExtensions.JsonValue(x.AdFkProvince.ProvinceName, header.Language),
                     CountryTitle = JsonExtensions.JsonValue(x.AdFkCountry.CountryTitle, header.Language),
                     Iso = x.AdFkCountry.Iso2,
                     TransfereeFamily = forPay == false ? null : (string.IsNullOrWhiteSpace(x.AdTransfereeFamily) ? x.FkCustomer.Family : x.AdTransfereeFamily),
                     TransfereeEmail = x.FkCustomer.Email,
                     PayerId = x.FkCustomer.CustomerId,
                     TransfereeMobile = forPay == false ? null : x.AdTransfereeMobile,
                     TransfereeName = forPay == false ? null : (string.IsNullOrWhiteSpace(x.AdTransfereeName) ? x.FkCustomer.Name : x.AdTransfereeName),
                     Items = x.TOrderItem.Select(t => new WebsiteOrderItemGetDto()
                     {
                         Exist = t.FkVariety.InventoryCount < t.ItemCount ? false : true,
                         InventoryCount = t.FkVariety.InventoryCount == null ? 0 : (double)t.FkVariety.InventoryCount,
                         GoodsCode = t.FkGoods.GoodsCode,
                         GoodsBrand = JsonExtensions.JsonValue(t.FkGoods.FkBrand.BrandTitle, header.Language),
                         GoodsId = t.FkGoodsId,
                         GoodsImage = t.FkGoods.ImageUrl,
                         ItemId = t.ItemId,
                         ModelNumber = t.FkGoods.SerialNumber,
                         SaleWithCall = t.FkGoods.SaleWithCall,
                         IsDownloadable = t.FkGoods.IsDownloadable,
                         ProviderId = t.FkVarietyId,
                         Quantity = (double)t.ItemCount,
                         Shipping = (decimal)0.00,
                         ShippingDay = 0,
                         CategoryId = t.FkGoods.FkCategoryId,
                         CityId = (int)t.FkShop.FkCityId,
                         CountryId = t.FkShop.FkCountryId,
                         CountryIso = t.FkShop.FkCountry.Iso2,
                         CountryTitle = JsonExtensions.JsonValue(t.FkShop.FkCountry.CountryTitle, header.Language),
                         CityTitle = JsonExtensions.JsonValue(t.FkShop.FkCity.CityTitle, header.Language),
                         ProvinceId = (int)t.FkShop.FkProvinceId,
                         DiscountCouponAmount = 0,
                         ShopId = t.FkShopId,
                         ShopShippingCode = t.FkShop.ShopShippingCode,
                         ShopHaveMicroStore = (bool)t.FkShop.Microstore,
                         ShopUrl = t.FkShop.VendorUrlid,
                         StoreName = t.FkShop.StoreName,
                         ShippingAvailable = false,
                         HaveGuarantee = t.FkVariety.HaveGuarantee,
                         ReturningAllowed = t.FkVariety.ReturningAllowed,
                         PostId = t.FkVariety.PostId ?? default(int),
                         GuaranteeMonthDuration = t.FkVariety.GuaranteeMonthDuration == null ? 0 : (int)t.FkVariety.GuaranteeMonthDuration,
                         Method = 0,
                         Weight = t.FkGoods.Weight == null ? 1.0 : (double)t.FkGoods.Weight,
                         Length = t.FkGoods.Length == null ? 1.0 : (double)t.FkGoods.Length,
                         Heigth = t.FkGoods.Heigth == null ? 1.0 : (double)t.FkGoods.Heigth,
                         Width = t.FkGoods.Width == null ? 1.0 : (double)t.FkGoods.Width,
                         Title = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                         DiscountAmount = decimal.Round((decimal)(t.FkVariety.DiscountAmount == null ? 0 : (t.FkVariety.DiscountAmount * (decimal)t.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero), // order item discount
                         DiscountPercent = (decimal)(t.FkVariety.DiscountPercentage == null ? 0 : t.FkVariety.DiscountPercentage),
                         TotalPrice = decimal.Round((decimal)(t.FkVariety.Price * (decimal)t.ItemCount)  / rate, 2, MidpointRounding.AwayFromZero),
                         UnitPrice = decimal.Round((decimal)t.FkVariety.Price  / rate, 2, MidpointRounding.AwayFromZero),
                         PriceWithDiscount = decimal.Round((decimal)((t.FkVariety.FinalPrice) * (decimal)t.ItemCount)  / rate, 2, MidpointRounding.AwayFromZero),
                         Vat = decimal.Round((decimal)(t.FkVariety.Vatamount == null ? 0 : (t.FkVariety.Vatamount * (decimal)t.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero),
                         GoodsVariety = t.FkVariety.TGoodsVariety.Select(i => new Dtos.Home.HomeGoodsVarietyGetDto()
                         {
                             ImageUrl = i.ImageUrl,
                             ParameterTitle = JsonExtensions.JsonValue(i.FkVariationParameter.ParameterTitle, header.Language),
                             ValueTitle = JsonExtensions.JsonValue(i.FkVariationParameterValue.Value, header.Language),
                             ValuesHaveImage = i.FkVariationParameter.ValuesHaveImage
                         }).ToList()
                     }).ToList()
                 })
                 .AsNoTracking()
                 .FirstOrDefaultAsync();
                if (order != null)
                {
                    var digitalFile = false;
                    var downloadAbleItem = order.Items.Where(c => c.IsDownloadable).ToList();
                    if (downloadAbleItem.Count() == order.Items.Count())
                    {
                        digitalFile = true;
                    }
                    if (forPay)
                    {

                        if (order.Address == null && order.TransfereeMobile == null && !digitalFile)
                        {
                            return new RepRes<WebsiteOrderGetDto>(Message.OrderGetting, false, null);
                        }
                    }

                    // ذخیره مشخصات پست برای محاسبه ی قیمت

                    var ShopPostService = new List<ShopPostServiceDto>();

                    foreach (var item in order.Items)
                    {

                        // agar file digital bashad
                        if (item.IsDownloadable)
                        {
                            item.ShippingAvailable = true;
                            item.Shipping = (decimal)0.00;
                            item.ShippingDay = 0;
                        }
                        // agar file digital nabashd
                        else
                        {
                            if (order.CountryId == item.CountryId)
                            {
                                // tuye city haye shop search kon
                                var method = await _context.TShopActivityCity
                                .Include(x => x.FkShippingMethod)
                                .Include(x => x.FkShop)
                                .Include(x => x.FkProvice)
                                .ThenInclude(x => x.FkCountry)
                                .AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == item.ShopId && order.CityId == x.FkCityId);
                                if (method == null)
                                {
                                    // tuye ostan haye shop search kon

                                    method = await _context.TShopActivityCity
                                   .Include(x => x.FkShippingMethod)
                                   .Include(x => x.FkShop)
                                   .Include(x => x.FkProvice)
                                   .ThenInclude(x => x.FkCountry)
                                   .AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == item.ShopId && order.ProvinceId == x.FkProviceId);
                                }
                                if (method != null)
                                {
                                    method.FkShippingMethodId = item.PostId != 0 ? item.PostId : method.FkShippingMethodId ;
                                    item.ShippingAvailable = true;
                                    item.Method =  method.FkShippingMethodId;
                                    item.ShippingDay = method.PostTimeoutDayByShop == null ? 0 : (int)method.PostTimeoutDayByShop;
                                    var shopPost = new ShopPostServiceDto();
                                    shopPost.shopId = item.ShopId;
                                    shopPost.shopShippingCode = item.ShopShippingCode;
                                    shopPost.originCountryCode = item.CountryIso;
                                    shopPost.destinationCountryCode = order.Iso;
                                    shopPost.destinationCityName = order.CityTitle;
                                    shopPost.originCityName = item.CityTitle;
                                    shopPost.destinationCountryCode = order.Iso;
                                    shopPost.weight = item.Weight;
                                    shopPost.height = item.Heigth;
                                    shopPost.length = item.Length;
                                    shopPost.width = item.Width;
                                    shopPost.price = item.PriceWithDiscount;
                                    shopPost.qty = item.Quantity;
                                    shopPost.ShopMethodCity = method;

                                    if (method.FkShippingMethodId != (int)ShippingMethodEnum.Market && !method.FkShippingMethod.HaveOnlineService)
                                    {
                                        var ajyalmethodCountry = new TShippingOnCountry();
                                        var ajyalmethod = await _context.TShippingOnCity
                                        .Include(x => x.FkShippingMethod)
                                        .AsNoTracking().FirstOrDefaultAsync(x => order.CityId == x.FkCityId);
                                        if (ajyalmethod == null)
                                        {
                                            ajyalmethod = await _context.TShippingOnCity
                                          .Include(x => x.FkShippingMethod)
                                          .AsNoTracking().FirstOrDefaultAsync(x => order.ProvinceId == x.FkProviceId);
                                        }
                                        if (ajyalmethod == null)
                                        {
                                            ajyalmethodCountry = await _context.TShippingOnCountry
                                        .Include(x => x.FkShippingMethod)
                                        .AsNoTracking().FirstOrDefaultAsync(x => order.CountryId == x.FkCountryId);
                                        }
                                        if (ajyalmethod == null && ajyalmethodCountry == null)
                                        {
                                            item.ShippingAvailable = false;
                                            item.Shipping = (decimal)0.00;
                                            item.ShippingDay = 0;
                                        }
                                        else
                                        {
                                            item.ShippingDay = ajyalmethod != null ? (int)ajyalmethod.PostTimeoutDay : (int)ajyalmethodCountry.PostTimeoutDay;
                                        }
                                        shopPost.AjyalmethodCountry = ajyalmethodCountry;
                                        shopPost.AjyalmethodCity = ajyalmethod;

                                    }

                                    ShopPostService.Add(shopPost);

                                }
                                else
                                {
                                    item.ShippingAvailable = false;
                                    item.Shipping = (decimal)0.00;
                                    item.ShippingDay = 0;
                                }
                            }
                            else
                            {
                                // tuye country haye shop search kon
                                var methodShopCountry = await _context.TShopActivityCountry.Include(c => c.FkShippingMethod).Include(t => t.FkCountry).FirstOrDefaultAsync(x => x.FkShopId == item.ShopId && order.CountryId == x.FkCountryId);
                                if (methodShopCountry != null)
                                {
                                    methodShopCountry.FkShippingMethodId = item.PostId != 0 ? item.PostId : methodShopCountry.FkShippingMethodId ;
                                    item.ShippingAvailable = true;
                                    item.Method = methodShopCountry.FkShippingMethodId;
                                    var shopPost = new ShopPostServiceDto();
                                    shopPost.shopId = item.ShopId;
                                    shopPost.shopShippingCode = item.ShopShippingCode;
                                    shopPost.originCountryCode = item.CountryIso;
                                    shopPost.destinationCountryCode = order.Iso;
                                    shopPost.destinationCityName = order.CityTitle;
                                    shopPost.originCityName = item.CityTitle;
                                    shopPost.destinationCountryCode = order.Iso;
                                    shopPost.weight = item.Weight;
                                    shopPost.height = item.Heigth;
                                    shopPost.length = item.Length;
                                    shopPost.width = item.Width;
                                    shopPost.price = item.PriceWithDiscount;
                                    shopPost.qty = item.Quantity;
                                    shopPost.currency = header.CurrencyNum.ToString();
                                    shopPost.MethodShopCountry = methodShopCountry;

                                    if (!methodShopCountry.FkShippingMethod.HaveOnlineService)
                                    {
                                        var methodCity = await _context.TShippingOnCity.Include(c => c.FkShippingMethod).AsNoTracking().FirstOrDefaultAsync(x => order.CityId == x.FkCityId || order.ProvinceId == (int)x.FkProviceId);
                                        var methodCountry = await _context.TShippingOnCountry.Include(c => c.FkShippingMethod).AsNoTracking().FirstOrDefaultAsync(x => order.CountryId == x.FkCountryId);
                                        if (methodCountry == null && methodCity == null)
                                        {
                                            item.ShippingAvailable = false;
                                            item.Shipping = (decimal)0.00;
                                        }
                                        else
                                        {
                                            item.ShippingDay = methodCity != null ? (int)methodCity.PostTimeoutDay : (int)methodCountry.PostTimeoutDay;

                                            shopPost.AjyalmethodCountry = methodCountry;
                                            shopPost.AjyalmethodCity = methodCity;

                                        }
                                    }

                                    ShopPostService.Add(shopPost);

                                }
                                else
                                {
                                    item.ShippingAvailable = false;
                                    item.Shipping = (decimal)0.00;
                                }
                            }
                        }

                        item.TotalPrice = item.TotalPrice + item.Shipping + item.Vat;
                    }



                    var itemCantBuy = new List<WebsiteOrderItemGetDto>();
                    if (forPay)
                    {
                        itemCantBuy = order.Items.Where(x => x.ShippingAvailable == false || x.Exist == false).ToList();
                        order.Items = order.Items.Where(x => x.ShippingAvailable == true && x.Exist == true).ToList();
                    }


                    // محاسبه قیمت پست برای تامین کنندگان 

                    var ShopPostServiceGroup = ShopPostService.GroupBy(c => c.shopId)
                    .ToDictionary(e => e.Key, e => e.ToList());
                    foreach (var shops in ShopPostServiceGroup)
                    {
                        decimal shopShipping = 0;
                        double allProductWeight = shops.Value.Sum(c => (c.weight * c.qty)) / 1000;
                        // شهر های اکتیو
                        if (shops.Value[0].ShopMethodCity != null)
                        {
                            if (shops.Value[0].ShopMethodCity.FkShippingMethodId == (int)ShippingMethodEnum.Market)
                            {

                                var baseWeight = shops.Value[0].ShopMethodCity.FkShop.ShippingBaseWeight == null ? 1 : (double)shops.Value[0].ShopMethodCity.FkShop.ShippingBaseWeight;
                                var ShippingPriceFewerBaseWeight = decimal.Round((shops.Value[0].ShopMethodCity.ShippingPriceFewerBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].ShopMethodCity.ShippingPriceFewerBaseWeight)  / rate, 2, MidpointRounding.AwayFromZero);
                                var ShippingPriceMoreBaseWeight = decimal.Round((shops.Value[0].ShopMethodCity.ShippingPriceMoreBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].ShopMethodCity.ShippingPriceMoreBaseWeight)  / rate, 2, MidpointRounding.AwayFromZero);
                                if (allProductWeight <= baseWeight)
                                {
                                    order.Shipping += ShippingPriceFewerBaseWeight;
                                    shopShipping = ShippingPriceFewerBaseWeight;
                                }
                                else
                                {
                                    order.Shipping += ShippingPriceMoreBaseWeight;
                                    shopShipping = ShippingPriceMoreBaseWeight;
                                }

                            }

                            else
                            {
                                // سرویس های آنلاین پست
                                if (shops.Value[0].ShopMethodCity.FkShippingMethod.HaveOnlineService)
                                {
                                    if (shops.Value[0].ShopMethodCity.FkShippingMethodId == (int)ShippingMethodEnum.IranPost || shops.Value[0].ShopMethodCity.FkShippingMethodId == (int)ShippingMethodEnum.IranPostPshtaz)
                                    {
                                        var areaCode = await _context.TShippingMethodAreaCode.FirstOrDefaultAsync(b => b.FkCityId == order.CityId);
                                        if (areaCode == null)
                                        {
                                            order.Shipping += (decimal)0.00  / rate; //call online method
                                        }
                                        else
                                        {
                                            double shippingPrice =
                                            await IranPost.executedIranPost(allProductWeight, areaCode.StateCode.ToString(), areaCode.Code.ToString(), shops.Value[0].ShopMethodCity.FkShippingMethodId == (int)ShippingMethodEnum.IranPost ? "0" : "1", "1", shops.Value[0].price, shops.Value[0].shopShippingCode);
                                            order.Shipping += (decimal)shippingPrice;
                                            shopShipping = (decimal)shippingPrice;
                                        }
                                    }

                                    else
                                    {
                                        order.Shipping += (decimal)0.00  / rate; //call online method
                                    }
                                }
                                else
                                {

                                    // در استان جستجو کن برای روش اجیال

                                    if (shops.Value[0].AjyalmethodCity != null || shops.Value[0].AjyalmethodCountry != null)
                                    {
                                        var baseWeight = shops.Value[0].AjyalmethodCity != null ? (shops.Value[0].AjyalmethodCity.FkShippingMethod.BaseWeight == null ? 1.0 : (double)shops.Value[0].AjyalmethodCity.FkShippingMethod.BaseWeight) :
                                                         (shops.Value[0].AjyalmethodCountry.FkShippingMethod.BaseWeight == null ? 1.0 : (double)shops.Value[0].AjyalmethodCountry.FkShippingMethod.BaseWeight);

                                        var ShippingPriceFewerBaseWeight =
                                        decimal.Round((shops.Value[0].AjyalmethodCity != null ? (shops.Value[0].AjyalmethodCity.ShippingPriceFewerBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].AjyalmethodCity.ShippingPriceFewerBaseWeight) :
                                                         (shops.Value[0].AjyalmethodCountry.ShippingPriceFewerBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].AjyalmethodCountry.ShippingPriceFewerBaseWeight))  / rate, 2, MidpointRounding.AwayFromZero);
                                        var ShippingPriceMoreBaseWeight =
                                        decimal.Round((shops.Value[0].AjyalmethodCity != null ? (shops.Value[0].AjyalmethodCity.ShippingPriceMoreBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].AjyalmethodCity.ShippingPriceMoreBaseWeight) :
                                                         (shops.Value[0].AjyalmethodCountry.ShippingPriceMoreBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].AjyalmethodCountry.ShippingPriceMoreBaseWeight))  / rate, 2, MidpointRounding.AwayFromZero);
                                        if (allProductWeight <= baseWeight)
                                        {
                                            order.Shipping += ShippingPriceFewerBaseWeight;
                                            shopShipping = ShippingPriceFewerBaseWeight;
                                        }
                                        else
                                        {
                                            order.Shipping += ShippingPriceMoreBaseWeight;
                                            shopShipping = ShippingPriceMoreBaseWeight;
                                        }
                                    }


                                }
                            }


                        }

                        // کشورهای اکتیو
                        if (shops.Value[0].MethodShopCountry != null)
                        {

                            if (shops.Value[0].MethodShopCountry.FkShippingMethod.HaveOnlineService)
                            {
                                if (shops.Value[0].MethodShopCountry.FkShippingMethodId == (int)ShippingMethodEnum.IranPost || shops.Value[0].MethodShopCountry.FkShippingMethodId == (int)ShippingMethodEnum.IranPostPshtaz)
                                {

                                    var areaCode = await _context.TShippingMethodAreaCode.FirstOrDefaultAsync(b => b.FkCityId == order.CityId);
                                    if (areaCode == null)
                                    {
                                        order.Shipping += (decimal)0.00  / rate; //call online method
                                    }
                                    else
                                    {
                                        double shippingPrice =
                                        await IranPost.executedIranPost(allProductWeight, areaCode.StateCode.ToString(), areaCode.Code.ToString(), shops.Value[0].MethodShopCountry.FkShippingMethodId == (int)ShippingMethodEnum.IranPost ? "0" : "1", "1", shops.Value[0].price, shops.Value[0].shopShippingCode);
                                        order.Shipping += (decimal)shippingPrice;
                                        shopShipping = (decimal)shippingPrice;
                                    }
                                }
                                else
                                {
                                    order.Shipping += (decimal)0.00  / rate; //call online method
                                }
                            }
                            else
                            {
                                // در استان جستجو کن برای روش اجیال

                                if (shops.Value[0].AjyalmethodCity != null || shops.Value[0].AjyalmethodCountry != null)
                                {
                                    var baseWeight = shops.Value[0].AjyalmethodCity != null ? (shops.Value[0].AjyalmethodCity.FkShippingMethod.BaseWeight == null ? 1.0 : (double)shops.Value[0].AjyalmethodCity.FkShippingMethod.BaseWeight) :
                                                     (shops.Value[0].AjyalmethodCountry.FkShippingMethod.BaseWeight == null ? 1.0 : (double)shops.Value[0].AjyalmethodCountry.FkShippingMethod.BaseWeight);

                                    var ShippingPriceFewerBaseWeight =
                                    decimal.Round((shops.Value[0].AjyalmethodCity != null ? (shops.Value[0].AjyalmethodCity.ShippingPriceFewerBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].AjyalmethodCity.ShippingPriceFewerBaseWeight) :
                                                     (shops.Value[0].AjyalmethodCountry.ShippingPriceFewerBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].AjyalmethodCountry.ShippingPriceFewerBaseWeight))  / rate, 2, MidpointRounding.AwayFromZero);
                                    var ShippingPriceMoreBaseWeight =
                                    decimal.Round((shops.Value[0].AjyalmethodCity != null ? (shops.Value[0].AjyalmethodCity.ShippingPriceMoreBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].AjyalmethodCity.ShippingPriceMoreBaseWeight) :
                                                     (shops.Value[0].AjyalmethodCountry.ShippingPriceMoreBaseWeight == null ? (decimal)0.00 : (decimal)shops.Value[0].AjyalmethodCountry.ShippingPriceMoreBaseWeight))  / rate, 2, MidpointRounding.AwayFromZero);
                                    if (allProductWeight <= baseWeight)
                                    {
                                        order.Shipping += ShippingPriceFewerBaseWeight;
                                        shopShipping = ShippingPriceFewerBaseWeight;
                                    }
                                    else
                                    {
                                        order.Shipping += ShippingPriceMoreBaseWeight;
                                        shopShipping = ShippingPriceMoreBaseWeight;
                                    }
                                }
                            }
                        }


                        var items = order.Items.Where(c => c.ShopId == shops.Value[0].shopId).ToArray();
                        for (int i = 0; i < items.Length; i++)
                        {

                            items[i].Shipping = shopShipping / items.Length;
                            items[i].TotalPrice = items[i].TotalPrice + items[i].Shipping;

                        }

                    }

                    order.ItemsCount = order.Items.Sum(x => x.Quantity);
                    order.Discount = order.Items.Sum(x => x.DiscountAmount);//inja bayad mablaghe discount code ham rush biad vali chon inja code nadarim nemiarim
                                                                            // order.Shipping = order.Items.Sum(x => x.Shipping);
                    order.Vat = order.Items.Sum(x => x.Vat);
                    order.TotalWithOutDiscountCode = order.Items.Sum(x => x.TotalPrice);
                    order.Total = order.TotalWithOutDiscountCode - order.Discount;//inja chon hanuz kode takhfif mohasebe nemishe hamun order.TotalWithOutDiscountCode mishe

                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        var discount = await this.SetCopon(code, order, rate);
                        if (forPay)
                        {
                            order.Items.AddRange(itemCantBuy);
                        }

                        if (discount.Result == true)
                        {
                            return new RepRes<WebsiteOrderGetDto>(discount.Message, true, discount.Data);
                        }
                        else
                        {
                            return new RepRes<WebsiteOrderGetDto>(discount.Message, false, order);
                        }
                    }
                    if (forPay)
                    {
                        order.Items.AddRange(itemCantBuy);
                    }

                }

                return new RepRes<WebsiteOrderGetDto>(Message.Successfull, true, order);
            }
            catch (System.Exception)
            {
                return new RepRes<WebsiteOrderGetDto>(Message.OrderGetting, false, null);
            }
        }





        public async Task<RepRes<WebsiteOrderGetDto>> OrderCityCountryDetail()
        {
            try
            {

                var order = await _context.TOrder
                .Include(x => x.TOrderItem)
                .Include(x => x.TPaymentTransaction)
                .Where(x => x.FkOrderStatusId == (int)OrderStatusEnum.Cart
                && (token.Id == x.FkCustomerId && x.CookieId == null)
                && x.TOrderItem.All(t => t.FkStatusId == (int)OrderStatusEnum.Cart))
                 .Include(x => x.AdFkCity)
                 .Include(x => x.FkCustomer)
                 .Include(x => x.AdFkCountry)
                 .Include(x => x.AdFkProvince)
                 .Select(x => new WebsiteOrderGetDto()
                 {
                     OrderId = x.OrderId,
                     CityId = (int)x.AdFkCityId,
                     CountryId = (int)x.AdFkCountryId,
                     ProvinceId = (int)x.AdFkProvinceId,
                     CityTitle = JsonExtensions.JsonValue(x.AdFkCity.CityTitle, header.Language),
                     CountryTitle = JsonExtensions.JsonValue(x.AdFkCountry.CountryTitle, header.Language),
                     ProvinceTitle = JsonExtensions.JsonValue(x.AdFkProvince.ProvinceName, header.Language),
                     Iso = x.AdFkCountry.Iso2,
                     PhoneCode = x.AdFkCountry.PhoneCode,
                 })
                 .AsNoTracking()
                 .FirstOrDefaultAsync();


                return new RepRes<WebsiteOrderGetDto>(Message.Successfull, true, order);
            }
            catch (System.Exception)
            {
                return new RepRes<WebsiteOrderGetDto>(Message.OrderGetting, false, null);
            }
        }









        public async Task<bool> ChangeOrderItemsCustomer(Guid? cookieId, int customerId)
        {
            try
            {
                if (cookieId != (Guid?)null)
                {
                    var order = await _context.TOrder.FirstOrDefaultAsync(x => x.FkOrderStatusId == (int)OrderStatusEnum.Cart && x.CookieId == cookieId && x.FkCustomerId == (int)CustomerTypeEnum.Unknown);
                    if (order == null)
                    {
                        return true;
                    }
                    var OrderExist = await _context.TOrder.FirstOrDefaultAsync(x => x.FkOrderStatusId == (int)OrderStatusEnum.Cart && x.FkCustomerId == customerId);
                    if (OrderExist == null)
                    {
                        order.FkCustomerId = customerId;
                        order.CookieId = null;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var ordetItems = await _context.TOrderItem.Where(x => x.FkOrderId == order.OrderId).ToListAsync();
                        var ordetItemsAsli = await _context.TOrderItem.Where(x => x.FkOrderId == OrderExist.OrderId).ToListAsync();
                        foreach (var item in ordetItems)
                        {
                            var existThisItem = ordetItemsAsli.FirstOrDefault(x => x.FkGoodsId == item.FkGoodsId && x.FkVarietyId == item.FkVarietyId);
                            if (existThisItem == null)
                            {
                                item.FkOrderId = OrderExist.OrderId;
                            }
                            else
                            {
                                existThisItem.ItemCount = existThisItem.ItemCount + item.ItemCount;
                                _context.TOrderItem.Remove(item);
                                await _context.SaveChangesAsync();
                            }
                        }
                        await _context.SaveChangesAsync();
                        _context.TOrder.Remove(order);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                }
                return true;
            }
            catch (System.Exception)
            {
                return true;
            }
        }

        public async Task<int> GetUserOrderCount(int customerId, Guid? cookieId)
        {
            try
            {
                return await _context.TOrderItem
                .Include(x => x.FkOrder)
                .CountAsync(x => x.FkOrder.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                (customerId == 0 ? (x.FkOrder.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.FkOrder.CookieId == cookieId) : (customerId == x.FkOrder.FkCustomerId && x.FkOrder.CookieId == null)) &&
                 x.FkStatusId == (int)OrderStatusEnum.Cart);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> ChangeAreaOrder(int cityId, int countryId, int provinceId)
        {
            try
            {
                var order = await _context.TOrder.Include(x => x.TOrderItem).FirstOrDefaultAsync(x => x.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                  (token.Id == 0 ? (x.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.CookieId == token.CookieId) : (token.Id == x.FkCustomerId && x.CookieId == null)) &&
                   x.TOrderItem.All(t => t.FkStatusId == (int)OrderStatusEnum.Cart));

                order.AdFkCityId = cityId;
                order.AdFkCountryId = countryId;
                order.AdFkProvinceId = provinceId;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<RepRes<WebsiteOrderGetDto>> SetCopon(string Code, WebsiteOrderGetDto order, decimal rate)
        {
            var date = DateTime.Now.Date;

            var copon = await _context.TDiscountCouponCode
            .AsNoTracking()
            .Include(x => x.FkDiscountPlan).ThenInclude(t => t.TDiscountCategory)
            .Include(x => x.FkDiscountPlan).ThenInclude(t => t.TDiscountCustomers)
            .Include(x => x.FkDiscountPlan).ThenInclude(t => t.TDiscountGoods)
            .Include(x => x.FkDiscountPlan).ThenInclude(t => t.TDiscountShops)
            .FirstOrDefaultAsync(x => x.DiscountCode == Code);
            //check code exist
            if (copon == null)
            {
                return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeNotExist, false, order);
            }

            //check code is valid
            if (copon.IsValid == false)
            {
                return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeNotValid, false, order);
            }

            copon.FkDiscountPlan.MinimumOrderAmount = decimal.Round((decimal)copon.FkDiscountPlan.MinimumOrderAmount  / rate, 2, MidpointRounding.AwayFromZero);
            copon.FkDiscountPlan.MaximumDiscountAmount = decimal.Round((decimal)copon.FkDiscountPlan.MaximumDiscountAmount  / rate, 2, MidpointRounding.AwayFromZero);
            if ((int)DiscountTypeId.FixedDiscount == copon.FkDiscountPlan.FkDiscountTypeId)
            {
                copon.FkDiscountPlan.DiscountAmount = copon.FkDiscountPlan.DiscountAmount  / rate;
            }


            // check the activation and timing
            if (copon.FkDiscountPlan.Status == false)
            {
                return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeActive, false, order);
            }
            else
            {
                if (copon.FkDiscountPlan.TimingType == true)
                {
                    if (copon.FkDiscountPlan.StartDateTime.Value.Date > date || copon.FkDiscountPlan.EndDateTime.Value.Date < date)
                    {
                        return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeOutOfDate, false, order);

                    }
                }
            }

            //check the maximum use of thin code
            if (copon.MaxUse <= copon.UseCount)
            {
                return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeMaximumUse, false, order);
            }

            //this customer can use the code
            if (copon.FkDiscountPlan.TDiscountCustomers.Count > 0)
            {
                var allow = copon.FkDiscountPlan.TDiscountCustomers.Any(x => x.Allowed == true);

                if (allow)
                {
                    if (!copon.FkDiscountPlan.TDiscountCustomers.Any(x => x.FkCustomerId == token.Id))
                    {
                        return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeYouCantUseIt, false, order);
                    }
                }
                else
                {
                    if (copon.FkDiscountPlan.TDiscountCustomers.Any(x => x.FkCustomerId != token.Id))
                    {
                        return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeYouCantUseIt, false, order);
                    }
                }
            }

            if (copon.FkDiscountPlan.ActiveForFirstBuy == true)
            {
                var CustomerHaveAnyOtherOrder = await _context.TOrder.Include(x => x.TOrderItem).AsNoTracking().AnyAsync(x => x.FkCustomerId == token.Id && x.FkOrderStatusId != (int)OrderStatusEnum.Cart && (copon.FkDiscountPlan.FkShopId != null ? (!x.TOrderItem.Any(t => t.FkShopId == (int)copon.FkDiscountPlan.FkShopId)) : true));
                if (CustomerHaveAnyOtherOrder == true)
                {
                    return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeIsForFirstBuy, false, order);
                }
            }

            //how many time you can use plan
            var countUseCustomerThisPlan = await _context.TOrder
            .Include(x => x.FkDiscountCode)
            .CountAsync(x => (token.Id == 0 ? (x.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.CookieId == token.CookieId) : (token.Id == x.FkCustomerId && x.CookieId == null)) && x.FkDiscountCode.FkDiscountPlanId == copon.FkDiscountPlanId);

            if (copon.FkDiscountPlan.PermittedUseNumberPerCustomer <= countUseCustomerThisPlan)
            {
                return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeMaximumUse, false, order);
            }




            //check can use with other plan
            //check order items that can use this copon with them
            if (copon.FkDiscountPlan.UseWithOtherDiscountPlan == true)
            {
                if (copon.FkDiscountPlan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders)
                {
                    var totalIncludedPrice = (decimal)0;
                    foreach (var item in order.Items.Where(x => (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                    {
                        totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                    }
                    if (totalIncludedPrice != 0)
                    {
                        if (totalIncludedPrice >= copon.FkDiscountPlan.MinimumOrderAmount)
                        {
                            foreach (var item in order.Items.Where(x => (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                            {
                                if (copon.FkDiscountPlan.FreeShippingCost == true)
                                {
                                    order.Shipping = order.Shipping - item.Shipping;
                                    order.Total = order.Total - item.Shipping;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.Shipping;
                                    item.Shipping = 0;
                                }
                                if (copon.FkDiscountPlan.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                                {
                                    //orderitem dor dorost kon
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * copon.FkDiscountPlan.DiscountAmount;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));


                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                                else
                                {
                                    //orderitem dor dorost kon
                                    var DiscountPrice = (copon.FkDiscountPlan.DiscountAmount * totalIncludedPrice) / 100;
                                    if (DiscountPrice > copon.FkDiscountPlan.MaximumDiscountAmount)
                                    {
                                        DiscountPrice = (decimal)copon.FkDiscountPlan.MaximumDiscountAmount;
                                    }
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * DiscountPrice;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                            }
                        }
                        else
                        {
                            return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeOrderPriceIsLow, false, order);
                        }
                    }
                    else
                    {
                        return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeNotIncludeAnyGoods, false, order);
                    }
                }
                else if (copon.FkDiscountPlan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialCategory)
                {
                    var totalIncludedPrice = (decimal)0;
                    foreach (var item in order.Items.Where(x => (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                    {
                        var categoryIds = await _categoryRepository.GetParentCatIds(item.CategoryId);
                        var allow = copon.FkDiscountPlan.TDiscountCategory.Any(x => x.Allowed == true);
                        if (allow == true)
                        {
                            if (copon.FkDiscountPlan.TDiscountCategory.Any(x => categoryIds.Contains(x.FkCategoryId)))
                            {
                                totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                            }
                        }
                        else
                        {
                            if (copon.FkDiscountPlan.TDiscountCategory.Any(x => !categoryIds.Contains(x.FkCategoryId)))
                            {
                                totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                            }
                        }

                    }
                    if (totalIncludedPrice != 0)
                    {
                        if ((totalIncludedPrice) >= copon.FkDiscountPlan.MinimumOrderAmount)
                        {
                            foreach (var item in order.Items.Where(x => (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                            {
                                if (copon.FkDiscountPlan.FreeShippingCost == true)
                                {
                                    order.Shipping = order.Shipping - item.Shipping;
                                    order.Total = order.Total - item.Shipping;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.Shipping;
                                    item.Shipping = 0;
                                }
                                if (copon.FkDiscountPlan.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                                {
                                    //orderitem dor dorost kon
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * copon.FkDiscountPlan.DiscountAmount;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                                else
                                {
                                    //orderitem dor dorost kon
                                    var DiscountPrice = (copon.FkDiscountPlan.DiscountAmount * totalIncludedPrice) / 100;
                                    if (DiscountPrice > copon.FkDiscountPlan.MaximumDiscountAmount)
                                    {
                                        DiscountPrice = (decimal)copon.FkDiscountPlan.MaximumDiscountAmount;
                                    }
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * DiscountPrice;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                            }
                        }
                        else
                        {
                            return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeOrderPriceIsLow, false, order);
                        }
                    }
                    else
                    {
                        return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeNotIncludeAnyGoods, false, order);
                    }

                }
                else if (copon.FkDiscountPlan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialGoods)
                {
                    var totalIncludedPrice = (decimal)0.0;
                    foreach (var item in order.Items.Where(x => (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                    {
                        var allow = copon.FkDiscountPlan.TDiscountGoods.Any(x => x.Allowed == true);
                        if (allow == true)
                        {
                            if (copon.FkDiscountPlan.TDiscountGoods.Any(x => (item.GoodsId == x.FkGoodsId && x.FkVarietyId == null) || item.GoodsId == x.FkGoodsId && x.FkVarietyId == item.ProviderId))
                            {
                                totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                            }
                        }
                        else
                        {
                            if (copon.FkDiscountPlan.TDiscountGoods.Any(x => (item.GoodsId != x.FkGoodsId && x.FkVarietyId == null) || (x.FkVarietyId != item.ProviderId)))
                            {
                                totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                            }
                        }

                    }
                    if (totalIncludedPrice != 0)
                    {
                        if (totalIncludedPrice >= copon.FkDiscountPlan.MinimumOrderAmount)
                        {
                            foreach (var item in order.Items.Where(x => (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                            {
                                if (copon.FkDiscountPlan.FreeShippingCost == true)
                                {
                                    order.Shipping = order.Shipping - item.Shipping;
                                    order.Total = order.Total - item.Shipping;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.Shipping;
                                    item.Shipping = 0;
                                }
                                if (copon.FkDiscountPlan.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                                {
                                    //orderitem dor dorost kon
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * copon.FkDiscountPlan.DiscountAmount;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                                else
                                {
                                    //orderitem dor dorost kon
                                    var DiscountPrice = (copon.FkDiscountPlan.DiscountAmount * totalIncludedPrice) / 100;
                                    if (DiscountPrice > copon.FkDiscountPlan.MaximumDiscountAmount)
                                    {
                                        DiscountPrice = (decimal)copon.FkDiscountPlan.MaximumDiscountAmount;
                                    }
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * DiscountPrice;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                            }
                        }
                        else
                        {
                            return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeOrderPriceIsLow, false, order);
                        }
                    }
                    else
                    {
                        return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeNotIncludeAnyGoods, false, order);
                    }
                }
            }
            else
            {
                if (copon.FkDiscountPlan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.AllGoods_AllOrders)
                {
                    var totalIncludedPrice = (decimal)0.0;
                    foreach (var item in order.Items.Where(x => x.DiscountAmount == 0 && (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                    {
                        if (item.DiscountAmount == 0)
                        {
                            totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                        }

                    }
                    if ((totalIncludedPrice) >= copon.FkDiscountPlan.MinimumOrderAmount)
                    {
                        foreach (var item in order.Items.Where(x => x.DiscountAmount == 0 && (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                        {
                            if (copon.FkDiscountPlan.FreeShippingCost == true)
                            {
                                order.Shipping = order.Shipping - item.Shipping;
                                order.Total = order.Total - item.Shipping;
                                item.PriceWithDiscount = item.PriceWithDiscount - item.Shipping;
                                item.Shipping = 0;
                            }
                            if (copon.FkDiscountPlan.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                            {
                                //orderitem dor dorost kon
                                item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * copon.FkDiscountPlan.DiscountAmount;
                                item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                order.Total = order.Total - item.DiscountCouponAmount;
                                order.Discount = item.DiscountAmount;
                            }
                            else
                            {
                                //orderitem dor dorost kon
                                var DiscountPrice = (copon.FkDiscountPlan.DiscountAmount * totalIncludedPrice) / 100;
                                if (DiscountPrice > copon.FkDiscountPlan.MaximumDiscountAmount)
                                {
                                    DiscountPrice = (decimal)copon.FkDiscountPlan.MaximumDiscountAmount;
                                }
                                item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * DiscountPrice;
                                item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                item.DiscountPercent = (item.DiscountAmount / (item.UnitPrice * (decimal)item.Quantity) * 100);

                                order.Total = order.Total - item.DiscountCouponAmount;
                                order.Discount = item.DiscountCouponAmount;
                            }
                        }
                    }
                    else
                    {
                        return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeOrderPriceIsLow, false, order);
                    }
                }
                else if (copon.FkDiscountPlan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialCategory)
                {
                    var totalIncludedPrice = (decimal)0.0;
                    foreach (var item in order.Items.Where(x => x.DiscountAmount == 0 && (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                    {

                        var categoryIds = await _categoryRepository.GetParentCatIds(item.CategoryId);
                        var allow = copon.FkDiscountPlan.TDiscountCategory.Any(x => x.Allowed == true);
                        if (allow == true)
                        {
                            if (copon.FkDiscountPlan.TDiscountCategory.Any(x => categoryIds.Contains(x.FkCategoryId)))
                            {
                                totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                            }
                        }
                        else
                        {
                            if (copon.FkDiscountPlan.TDiscountCategory.Any(x => !categoryIds.Contains(x.FkCategoryId)))
                            {
                                totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                            }
                        }
                    }
                    if (totalIncludedPrice != 0)
                    {
                        if (totalIncludedPrice >= copon.FkDiscountPlan.MinimumOrderAmount)
                        {
                            foreach (var item in order.Items.Where(x => x.DiscountAmount == 0 && (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                            {
                                if (copon.FkDiscountPlan.FreeShippingCost == true)
                                {
                                    order.Shipping = order.Shipping - item.Shipping;
                                    order.Total = order.Total - item.Shipping;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.Shipping;
                                    item.Shipping = 0;
                                }
                                if (copon.FkDiscountPlan.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                                {
                                    //orderitem dor dorost kon
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * copon.FkDiscountPlan.DiscountAmount;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                                else
                                {
                                    //orderitem dor dorost kon
                                    var DiscountPrice = (copon.FkDiscountPlan.DiscountAmount * totalIncludedPrice) / 100;
                                    if (DiscountPrice > copon.FkDiscountPlan.MaximumDiscountAmount)
                                    {
                                        DiscountPrice = (decimal)copon.FkDiscountPlan.MaximumDiscountAmount;
                                    }
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * DiscountPrice;
                                    item.PriceWithDiscount = item.PriceWithDiscount - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                            }
                        }
                        else
                        {
                            return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeOrderPriceIsLow, false, order);
                        }
                    }
                    else
                    {
                        return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeNotIncludeAnyGoods, false, order);
                    }
                }
                else if (copon.FkDiscountPlan.FkDiscountRangeTypeId == (int)DiscountRangeTypeEnum.SpecialGoods)
                {
                    var totalIncludedPrice = (decimal)0.0;
                    foreach (var item in order.Items.Where(x => x.DiscountAmount == 0 && (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                    {
                        if (item.DiscountAmount == 0)
                        {
                            var allow = copon.FkDiscountPlan.TDiscountGoods.Any(x => x.Allowed == true);
                            if (allow == true)
                            {
                                if (copon.FkDiscountPlan.TDiscountGoods.Any(x => (item.GoodsId == x.FkGoodsId && x.FkVarietyId == null) || item.GoodsId == x.FkGoodsId && x.FkVarietyId == item.ProviderId))
                                {
                                    totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                                }
                            }
                            else
                            {
                                if (copon.FkDiscountPlan.TDiscountGoods.Any(x => (item.GoodsId != x.FkGoodsId && x.FkVarietyId == null) || (x.FkVarietyId != item.ProviderId)))
                                {
                                    totalIncludedPrice = totalIncludedPrice + item.PriceWithDiscount;
                                }
                            }
                        }
                    }
                    if (totalIncludedPrice != 0)
                    {
                        if (totalIncludedPrice >= copon.FkDiscountPlan.MinimumOrderAmount)
                        {
                            foreach (var item in order.Items.Where(x => x.DiscountAmount == 0 && (copon.FkDiscountPlan.FkShopId == null ? true : x.ShopId == (int)copon.FkDiscountPlan.FkShopId)).ToList())
                            {
                                if (copon.FkDiscountPlan.FreeShippingCost == true)
                                {
                                    order.Shipping = order.Shipping - item.Shipping;
                                    order.Total = order.Total - item.Shipping;
                                    item.TotalPrice = item.TotalPrice - item.Shipping;
                                    item.Shipping = 0;
                                }
                                if (copon.FkDiscountPlan.FkDiscountTypeId == (int)DiscountTypeId.FixedDiscount)
                                {
                                    //orderitem dor dorost kon
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * copon.FkDiscountPlan.DiscountAmount;
                                    item.TotalPrice = item.TotalPrice - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                                else
                                {
                                    //orderitem dor dorost kon
                                    var DiscountPrice = (copon.FkDiscountPlan.DiscountAmount * totalIncludedPrice) / 100;
                                    if (DiscountPrice > copon.FkDiscountPlan.MaximumDiscountAmount)
                                    {
                                        DiscountPrice = (decimal)copon.FkDiscountPlan.MaximumDiscountAmount;
                                    }
                                    item.DiscountCouponAmount = (item.PriceWithDiscount / totalIncludedPrice) * DiscountPrice;
                                    item.TotalPrice = item.TotalPrice - item.DiscountCouponAmount;
                                    item.DiscountAmount = item.DiscountAmount + item.DiscountCouponAmount;
                                    item.DiscountPercent = Extentions.DecimalRound((item.DiscountAmount / ((item.UnitPrice + item.Vat) * (decimal)item.Quantity) * 100));

                                    order.Total = order.Total - item.DiscountCouponAmount;
                                    order.Discount = item.DiscountAmount;
                                }
                            }
                        }
                        else
                        {
                            return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeOrderPriceIsLow, false, order);
                        }
                    }
                    else
                    {
                        return new RepRes<WebsiteOrderGetDto>(Message.CopounCodeNotIncludeAnyGoods, false, order);
                    }
                }
            }
            return new RepRes<WebsiteOrderGetDto>(Message.Successfull, true, order);
        }



        public async Task<RepRes<bool>> ChangeDestination(long orderId, bool forCart, int addressId)
        {
            try
            {
                var order = await _context.TOrder.Include(x => x.TOrderItem).FirstOrDefaultAsync(x =>
                  (forCart == false ? (x.OrderId == orderId && x.TOrderItem.All(t => t.FkStatusId < (int)OrderStatusEnum.Shipping)) : (x.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                  (token.Id == 0 ? (x.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.CookieId == token.CookieId) : (token.Id == x.FkCustomerId && x.CookieId == null)) &&
                   x.TOrderItem.All(t => t.FkStatusId == (int)OrderStatusEnum.Cart)))
                );
                if (order == null)
                {
                    return new RepRes<bool>(Message.YouCantChangeOrderAddress, false, false);
                }
                var address = await _context.TCustomerAddress.FirstOrDefaultAsync(x => x.AddressId == addressId);
                if (address == null)
                {
                    return new RepRes<bool>(Message.OrderAddressEditing, false, false);
                }
                if (address.FkCountryId != order.AdFkCountryId || address.FkCityId != order.AdFkCityId)
                {
                    return new RepRes<bool>(Message.OrderCantBeSentToThisAddress, false, false);
                }
                order.AdAddress = address.Address;
                order.AdFkCityId = address.FkCityId;
                order.AdFkCountryId = address.FkCountryId;
                order.AdLocationX = address.LocationX;
                order.AdLocationY = address.LocationY;
                order.AdPostalCode = address.PostalCode;
                order.AdTransfereeFamily = address.TransfereeFamily;
                order.AdTransfereeMobile = address.TransfereeMobile;
                order.AdTransfereeName = address.TransfereeName;
                order.AdTransfereeTel = address.TransfereeMobile;
                await _context.SaveChangesAsync();

                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.OrderAddressEditing, false, false);
            }
        }




        public async Task<bool> InitOrderPayment(PayOrderDto PayDto)
        {
            try
            {
                var initOrder = new TPaymentTransaction();
                initOrder.FkPaymentMethodId = PayDto.PaymentType;
                initOrder.PaymentToken = PayDto.Token;
                initOrder.PayerId = PayDto.PayerID;
                initOrder.FkOrderId = PayDto.OrderId;
                initOrder.PaymentId = PayDto.PaymentId;
                initOrder.FkCurrencyId = PayDto.CurrencyId;
                if (PayDto.PaymentType == (int)PaymentMethodEnum.PayPal)
                {
                    initOrder.FkCurrencyId = (int)CurrencyEnum.USD;
                }
                if (!string.IsNullOrWhiteSpace(PayDto.Code))
                {
                    initOrder.TempDiscountCode = PayDto.Code;
                }
                await _context.TPaymentTransaction.AddAsync(initOrder);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }

        public async Task<bool> UpdatePaymentIdWithTransactionId(string transactionID, string paymentId)
        {
            try
            {
                var payment = await _context.TPaymentTransaction.FirstOrDefaultAsync(x => x.PaymentToken == transactionID);
                if (payment != null)
                {
                    payment.PaymentId = paymentId;
                    await _context.SaveChangesAsync();
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




        public async Task<RepRes<WebsiteOrderGetDto>> PayOrder(PayOrderDto PayDto)
        {
            try
            {
                var paymentTransaction = await _context.TPaymentTransaction.FirstOrDefaultAsync(x => x.PaymentId == PayDto.PaymentId);
                if (paymentTransaction == null || PayDto.PaymentType == 0)
                {
                    return new RepRes<WebsiteOrderGetDto>(Message.OrderGetting, false, null);
                }
                PayDto.Code = paymentTransaction.TempDiscountCode;
                var checkOrder = await this.GetOrderDetail(PayDto.Code, true, null, paymentTransaction.PaymentId);
                if (checkOrder.Result == false)
                {
                    return new RepRes<WebsiteOrderGetDto>(checkOrder.Message, false, checkOrder.Data);
                }
                if (checkOrder.Data.Items.Any(x => x.Exist == false || !x.ShippingAvailable))
                {
                    return new RepRes<WebsiteOrderGetDto>(Message.FirstEditYourCart, false, checkOrder.Data);
                }

                var order = await _context.TOrder
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods).ThenInclude(p => p.FkCategory)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(t => t.FkShop).ThenInclude(t => t.TUser)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop).ThenInclude(t => t.TShopCategory)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop).ThenInclude(t => t.TUser)
                .Include(x => x.FkCustomer)
                .ThenInclude(e => e.TUser)
                .Include(x => x.AdFkCountry)
                .Include(x => x.AdFkCity)
                .Include(x => x.TPaymentTransaction)
                .FirstOrDefaultAsync(x => x.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                 x.TPaymentTransaction.Any(t => t.PaymentId == paymentTransaction.PaymentId) &&
                 x.TOrderItem.All(t => t.FkStatusId == (int)OrderStatusEnum.Cart));

                if (order == null)
                {
                    return new RepRes<WebsiteOrderGetDto>(Message.OrderNotFound, false, checkOrder.Data);
                }
                if (PayDto.PaymentType != 0)
                {
                    order.PaymentStatus = true;
                    paymentTransaction.Status = true;
                }

                var currentUser = await _context.TUser.FirstAsync(x => x.FkCustumerId == order.FkCustomerId);
                token.UserId = currentUser.UserId;
                var discountCode = (TDiscountCouponCode)null;
                if (!string.IsNullOrWhiteSpace(PayDto.Code))
                {
                    discountCode = await _context.TDiscountCouponCode.FirstOrDefaultAsync(x => x.DiscountCode == PayDto.Code);
                }

                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }

                var allItemDownloadAbleCount = 0;

                foreach (var item in order.TOrderItem)
                {
                    //   if (item.FkShop.CommissionFraction == 100)
                    //   {
                    item.ComissionPrice = (decimal)item.FkShop.TShopCategory.FirstOrDefault(x => item.FkGoods.FkCategory.CategoryPath.Contains("/" + x.FkCategoryId + "/")).ContractCommissionFee;
                    // if (item.ComissionPrice == null)
                    // {
                    //     item.ComissionPrice = 0;
                    // }
                    //    }
                    // else
                    // {
                    //     item.ComissionPrice = 0;
                    // }
                    item.FkShippingMethodId = checkOrder.Data.Items.First(x => x.ItemId == item.ItemId).Method;
                    if (item.FkShippingMethodId == 0)
                    {
                        item.FkShippingMethodId = null;
                    }
                    if (item.FkGoods.IsDownloadable)
                    {
                        ++allItemDownloadAbleCount;
                        item.FkGoods.SaleCount = item.FkGoods.SaleCount + 1 ;
                    }
                    item.FkStatusId = item.FkGoods.IsDownloadable ? (int)OrderStatusEnum.Completed : (int)OrderStatusEnum.Ongoing;
                    if (item.FkVariety.HaveGuarantee == true)
                    {
                        item.HaveGuarantee = true;
                        item.GuaranteeMonthDuration = item.FkVariety.GuaranteeMonthDuration == null ? 0 : item.FkVariety.GuaranteeMonthDuration;
                    }
                    else
                    {
                        item.HaveGuarantee = false;
                        item.GuaranteeMonthDuration = 0;
                    }

                    item.ReturningAllowed = item.FkVariety.ReturningAllowed;
                    item.MaxDeadlineDayToReturning = item.FkVariety.MaxDeadlineDayToReturning == null ? 0 : item.FkVariety.MaxDeadlineDayToReturning;
                    // item.ShippmentDate = null;

                    item.ShippingCost = decimal.Round((decimal)checkOrder.Data.Items.First(x => x.ItemId == item.ItemId).Shipping / rate, 2, MidpointRounding.AwayFromZero);
                    item.DiscountAmount = decimal.Round(checkOrder.Data.Items.First(x => x.ItemId == item.ItemId).DiscountAmount / rate, 2, MidpointRounding.AwayFromZero);
                    item.DiscountCouponAmount = decimal.Round(checkOrder.Data.Items.First(x => x.ItemId == item.ItemId).DiscountCouponAmount / rate, 2, MidpointRounding.AwayFromZero);
                    item.UnitPrice = item.FkVariety.Price;
                    item.Vatamount = item.FkVariety.Vatamount == null ? 0 : (decimal)(item.FkVariety.Vatamount * (decimal)item.ItemCount);
                    item.FinalPrice = (double)(((item.UnitPrice * (decimal)item.ItemCount) - item.DiscountAmount) + item.ShippingCost + item.Vatamount);
                    if (discountCode != null)
                    {
                        item.FkDiscountCodeId = discountCode.CodeId;
                    }

                    await this.AddOrderLog(item.FkOrderId, item.ItemId, item.FkStatusId, token.UserId, "این سفارش توسط مشتری خریداری شده است" + " تعداد کالا " + item.FinalPrice + "قیمت نهایی " + item.FinalPrice);
                }

                order.ComissionPrice = order.TOrderItem.Sum(x => x.ComissionPrice == null ? 0 : (decimal)x.ComissionPrice);
                order.DiscountAmount = order.TOrderItem.Sum(x => x.DiscountAmount == null ? 0 : (decimal)x.DiscountAmount);
                if (allItemDownloadAbleCount == order.TOrderItem.Count)
                {
                    order.FkOrderStatusId = (int)OrderStatusEnum.Completed;
                }
                else
                {
                    order.FkOrderStatusId = (int)OrderStatusEnum.Ongoing;
                }
                order.PlacedDateTime = DateTime.Now;
                order.Price = order.TOrderItem.Sum(x => x.UnitPrice == null ? 0 : (x.UnitPrice * (decimal)x.ItemCount));
                order.ShippingCost = order.TOrderItem.Sum(x => x.ShippingCost == null ? 0 : (decimal)x.ShippingCost);
                order.Vatamount = order.TOrderItem.Sum(x => x.Vatamount == null ? 0 : (decimal)x.Vatamount);
                if (discountCode != null)
                {
                    if (discountCode != null)
                    {
                        discountCode.UseCount = discountCode.UseCount + 1;
                        if (discountCode.UseCount >= discountCode.MaxUse)
                        {
                            discountCode.IsValid = false;
                        }
                        order.FkDiscountCodeId = discountCode.CodeId;
                    }
                }
                order.FinalPrice = (order.Price + order.ShippingCost + order.Vatamount) - order.DiscountAmount;

                order.FkPaymentMethodId = PayDto.PaymentType;
                order.PaymentStatus = true;
                order.TrackingCode = await this.GetRandomString();

                await this.AddOrderLog(order.OrderId, null, order.FkOrderStatusId, token.UserId, "این سفارش توسط مشتری خریداری شده است " + "قیمت نهایی " + order.FinalPrice);


                if (order.PaymentStatus == true)
                {
                    await _accountingRepository.AddTransaction((int)TransactionTypeEnum.CashPayment, token.UserId, null, null, null, (int)TransactionStatusEnum.Completed, (decimal)order.FinalPrice, "پرداخت نقدی" + Extentions.DecimalRoundWithZiro( (decimal) order.FinalPrice ) + " تومان");
                    await _accountingRepository.AddTransaction((int)TransactionTypeEnum.Purchased, token.UserId, order.OrderId, null, null, (int)TransactionStatusEnum.Completed, (decimal)order.FinalPrice, "پرداخت نقدی" + Extentions.DecimalRoundWithZiro( (decimal) order.FinalPrice ) + " تومان");
                    foreach (var item in order.TOrderItem)
                    {
                        await _accountingRepository.AddTransaction((int)TransactionTypeEnum.Sales, item.FkShop.TUser.FirstOrDefault().UserId, order.OrderId, item.ItemId, null, (int)TransactionStatusEnum.Pending, (decimal)item.FinalPrice, "فروش" + Extentions.DecimalRoundWithZiro( (decimal) order.FinalPrice ) + " تومان");
                        if (item.ComissionPrice > 0)
                        {
                            await _accountingRepository.AddTransaction((int)TransactionTypeEnum.Commission, item.FkShop.TUser.FirstOrDefault().UserId, order.OrderId, item.ItemId, null, (int)TransactionStatusEnum.Pending,  ((decimal)item.ComissionPrice *  (decimal)order.FinalPrice), "قیمت کمیسیون" + Extentions.DecimalRoundWithZiro( (decimal) order.FinalPrice ) + " تومان ");
                        }
                        // kam kardan kala az anbar aval check kon alan mojud hast // age nist nabas bezani
                        if (await _context.TGoodsProvider.AsNoTracking().AnyAsync(x => x.ProviderId == item.FkVarietyId && x.InventoryCount >= item.ItemCount))
                        {
                            if (!item.FkGoods.IsDownloadable)
                            {
                                await _wareHouseRepository.AddStockOpration((int)StockOperationTypeEnum.Export, item.FkVarietyId, item.ItemId, (double)item.ItemCount, item.UnitPrice, "خروج از انبار برای خرید کاربر");
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in order.TOrderItem)
                    {
                        await _accountingRepository.AddTransaction((int)TransactionTypeEnum.Sales, item.FkShop.TUser.FirstOrDefault().UserId, order.OrderId, item.ItemId, null, (int)TransactionStatusEnum.Pending, (decimal)item.FinalPrice, "فروش" + Extentions.DecimalRoundWithZiro( (decimal) order.FinalPrice ) + " تومان");
                        if (item.ComissionPrice > 0)
                        {
                            await _accountingRepository.AddTransaction((int)TransactionTypeEnum.Commission, item.FkShop.TUser.FirstOrDefault().UserId, order.OrderId, item.ItemId, null, (int)TransactionStatusEnum.Pending, ((decimal)item.ComissionPrice *  (decimal)order.FinalPrice) , "قیمت کمیسیون" + Extentions.DecimalRoundWithZiro( (decimal) order.FinalPrice ) + " تومان ");
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // ارسال پیام به کاربر و تامین کننده

                var orderSaved = await _context.TOrder
            .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods).ThenInclude(p => p.FkCategory)
            .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(t => t.FkShop).ThenInclude(t => t.TUser)
            .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop).ThenInclude(t => t.TShopCategory)
            .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop).ThenInclude(t => t.TUser)
            .Include(x => x.FkCustomer)
            .ThenInclude(e => e.TUser)
            .Include(x => x.AdFkCountry)
            .Include(x => x.AdFkCity)
            .Include(x => x.TPaymentTransaction)
            .FirstOrDefaultAsync(x => x.OrderId == order.OrderId);
                orderSaved.ComissionPrice = orderSaved.ComissionPrice * (decimal)orderSaved.FinalPrice;
                try
                {

                    // ارسال پیام به کاربر
                  await  _notificationService.SendNotification((int)NotificationSettingTypeEnum.AfterOrderClient
                    , orderSaved.FkCustomer.TUser.First().ClientWebFirebasePushNotificationKey,
                    orderSaved.FkCustomer.TUser.First().ClientMobileFirebasePushNotificationKey
                     , orderSaved.FkCustomer.Email, "+" + orderSaved.AdFkCountry.PhoneCode + orderSaved.AdTransfereeMobile);
                }
                catch (System.Exception)
                {

                }
                // ارسال پیام به فروشنده ها

                foreach (var item in orderSaved.TOrderItem)
                {
                    item.ComissionPrice = item.ComissionPrice * (decimal)item.FinalPrice;

                    try
                    {

                      await  _notificationService.SendNotification((int)NotificationSettingTypeEnum.NewOrderProvider
                        , item.FkShop.TUser.First().ClientWebFirebasePushNotificationKey,
                          item.FkShop.TUser.First().ClientMobileFirebasePushNotificationKey
                         , item.FkShop.Email, item.FkShop.Phone, item.FkShop.TUser.First().UserId,
                         JsonExtensions.JsonGet(item.FkGoods.Title , LanguageEnum.Fa.ToString()));

                        // Minimum inventory alert - vendors 

                        if (item.FkVariety.MinimumInventory >= item.FkVariety.InventoryCount)
                        {
                         await   _notificationService.SendNotification((int)NotificationSettingTypeEnum.InventoryReachToMinimumProvider
                            , item.FkVariety.FkShop.TUser.First().ClientWebFirebasePushNotificationKey,
                              item.FkVariety.FkShop.TUser.First().ClientMobileFirebasePushNotificationKey
                             , item.FkVariety.FkShop.Email, item.FkVariety.FkShop.Phone, item.FkVariety.FkShop.TUser.First().UserId , 
                              JsonExtensions.JsonGet(item.FkGoods.Title , LanguageEnum.Fa.ToString()));
                        }

                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                }
                await _context.SaveChangesAsync();



                var webSiteOrder = new WebsiteOrderGetDto();
                webSiteOrder.OrderId = order.OrderId;
                webSiteOrder.ItemsCount = order.TOrderItem.Count();
                webSiteOrder.Discount = decimal.Round((decimal)order.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero);
                webSiteOrder.Shipping = decimal.Round((decimal)order.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero);
                webSiteOrder.Total = decimal.Round((decimal)order.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero);
                webSiteOrder.Vat = decimal.Round((decimal)order.Vatamount  / rate, 2, MidpointRounding.AwayFromZero);
                webSiteOrder.TotalWithOutDiscountCode = decimal.Round(((decimal)order.Price + (decimal)order.Vatamount + (decimal)order.ShippingCost)  / rate, 2, MidpointRounding.AwayFromZero);
                webSiteOrder.CityId = (int)order.AdFkCityId;
                webSiteOrder.CountryId = (int)order.AdFkCountryId;
                webSiteOrder.Address = order.AdAddress;
                webSiteOrder.TrackingCode = order.TrackingCode;
                webSiteOrder.CityTitle = JsonExtensions.JsonGet(order.AdFkCity.CityTitle, header);
                webSiteOrder.CountryTitle = JsonExtensions.JsonGet(order.AdFkCountry.CountryTitle, header);
                webSiteOrder.TransfereeFamily = (string.IsNullOrWhiteSpace(order.AdTransfereeFamily) ? order.FkCustomer.Family : order.AdTransfereeFamily);
                webSiteOrder.TransfereeEmail = order.FkCustomer.Email;
                webSiteOrder.TransfereeMobile = "+" + order.AdFkCountry.PhoneCode + order.AdTransfereeMobile;
                webSiteOrder.Iso = order.AdFkCountry.Iso2;
                webSiteOrder.TransfereeName = (string.IsNullOrWhiteSpace(order.AdTransfereeName) ? order.FkCustomer.Name : order.AdTransfereeName);
                webSiteOrder.Items = order.TOrderItem.Select(t => new WebsiteOrderItemGetDto()
                {
                    Exist = true,
                    InventoryCount = t.FkVariety.InventoryCount == null ? 0 : (double)t.FkVariety.InventoryCount,
                    GoodsCode = t.FkGoods.GoodsCode,
                    GoodsId = t.FkGoodsId,
                    GoodsImage = t.FkGoods.ImageUrl,
                    ItemId = t.ItemId,
                    StoreName = t.FkShop.StoreName,
                    ModelNumber = t.FkGoods.SerialNumber,
                    ProviderId = t.FkVarietyId,
                    Quantity = (double)t.ItemCount,
                    Shipping = decimal.Round((decimal)t.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                    CategoryId = t.FkGoods.FkCategoryId,
                    CityId = (int)t.FkShop.FkCityId,
                    CountryId = t.FkShop.FkCountryId,
                    DiscountCouponAmount = decimal.Round((decimal)t.DiscountCouponAmount  / rate, 2, MidpointRounding.AwayFromZero),
                    ShopId = t.FkShopId,
                    ShippingAvailable = true,
                    Method = t.FkShippingMethodId == null ? 0 : (int)t.FkShippingMethodId,
                    Weight = t.FkGoods.Weight == null ? 1.0 : (double)t.FkGoods.Weight,
                    Title = JsonExtensions.JsonGet(t.FkGoods.Title, header),
                    DiscountAmount = decimal.Round((decimal)(t.DiscountAmount == null ? (decimal)0 : (t.DiscountAmount))  / rate, 2, MidpointRounding.AwayFromZero), // order item discount
                    DiscountPercent = decimal.Round((decimal)(((decimal)t.DiscountAmount / (decimal)t.ItemCount) / ((decimal)t.UnitPrice == (decimal)0.00 ? 1 : (decimal)t.UnitPrice)) * 100, 2, MidpointRounding.AwayFromZero),
                    TotalPrice = decimal.Round((decimal)((t.UnitPrice * (decimal)t.ItemCount) - t.DiscountAmount)  / rate, 2, MidpointRounding.AwayFromZero),
                    UnitPrice = decimal.Round((decimal)(t.UnitPrice)  / rate, 2, MidpointRounding.AwayFromZero),
                    PriceWithDiscount = decimal.Round((decimal)((t.UnitPrice) - ((decimal)t.DiscountAmount / (decimal)t.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero),
                    Vat = decimal.Round((decimal)(t.Vatamount == null ? (decimal)0 : (t.Vatamount))  / rate, 2, MidpointRounding.AwayFromZero),
                    GoodsVariety = t.FkVariety.TGoodsVariety.Select(i => new Dtos.Home.HomeGoodsVarietyGetDto()
                    {
                        ImageUrl = i.ImageUrl,
                        ParameterTitle = JsonExtensions.JsonGet(i.FkVariationParameter.ParameterTitle, header),
                        ValueTitle = JsonExtensions.JsonGet(i.FkVariationParameterValue.Value, header),
                        ValuesHaveImage = i.FkVariationParameter.ValuesHaveImage
                    }).ToList()
                }).ToList();

                return new RepRes<WebsiteOrderGetDto>(Message.Successfull, true, webSiteOrder);
            }
            catch (System.Exception)
            {
                return new RepRes<WebsiteOrderGetDto>(Message.OrderAdding, false, null);
            }

        }



        public async Task<WebsiteOrderGetDto> GetOrderWithPaymentID(string paymentId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }

                var order = await _context.TOrder
                 .Include(x => x.TOrderItem)
                 .Include(x => x.TPaymentTransaction)
                 .Where(x => x.TPaymentTransaction.Any(t => t.PaymentId == paymentId))
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop)
                 .Include(x => x.AdFkCity)
                 .Include(x => x.FkCustomer)
                 .Include(x => x.AdFkCountry)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                 .Select(x => new WebsiteOrderGetDto()
                 {
                     OrderId = x.OrderId,
                     ItemsCount = x.TOrderItem.Count(),
                     Discount = decimal.Round((decimal)x.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                     Shipping = decimal.Round((decimal)x.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                     Total = decimal.Round((decimal)x.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                     Vat = decimal.Round((decimal)x.Vatamount  / rate, 2, MidpointRounding.AwayFromZero),
                     TotalWithOutDiscountCode = decimal.Round(((decimal)x.Price + (decimal)x.Vatamount + (decimal)x.ShippingCost)  / rate, 2, MidpointRounding.AwayFromZero),
                     CityId = (int)x.AdFkCityId,
                     CountryId = (int)x.AdFkCountryId,
                     Address = x.AdAddress,
                     IsDownloadable = false,
                     TrackingCode = x.TrackingCode,
                     CityTitle = JsonExtensions.JsonValue(x.AdFkCity.CityTitle, header.Language),
                     CountryTitle = JsonExtensions.JsonValue(x.AdFkCountry.CountryTitle, header.Language),
                     ProvinceTitle = JsonExtensions.JsonValue(x.AdFkProvince.ProvinceName, header.Language),
                     TransfereeFamily = (string.IsNullOrWhiteSpace(x.AdTransfereeFamily) ? x.FkCustomer.Family : x.AdTransfereeFamily),
                     TransfereeEmail = x.FkCustomer.Email,
                     TransfereeMobile = "+" + x.AdFkCountry.PhoneCode + x.AdTransfereeMobile,
                     Iso = x.AdFkCountry.Iso2,
                     TransfereeName = (string.IsNullOrWhiteSpace(x.AdTransfereeName) ? x.FkCustomer.Name : x.AdTransfereeName),
                     Items = x.TOrderItem.Select(t => new WebsiteOrderItemGetDto()
                     {
                         Exist = true,
                         InventoryCount = t.FkVariety.InventoryCount == null ? 0 : (double)t.FkVariety.InventoryCount,
                         GoodsCode = t.FkGoods.GoodsCode,
                         GoodsId = t.FkGoodsId,
                         GoodsImage = t.FkGoods.ImageUrl,
                         ItemId = t.ItemId,
                         ModelNumber = t.FkGoods.SerialNumber,
                         ProviderId = t.FkVarietyId,
                         IsDownloadable = t.FkGoods.IsDownloadable,
                         Quantity = (double)t.ItemCount,
                         Shipping = decimal.Round((decimal)t.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                         CategoryId = t.FkGoods.FkCategoryId,
                         CityId = (int)t.FkShop.FkCityId,
                         CountryId = t.FkShop.FkCountryId,
                         StoreName = t.FkShop.StoreName,
                         DiscountCouponAmount = decimal.Round((decimal)t.DiscountCouponAmount  / rate, 2, MidpointRounding.AwayFromZero),
                         ShopId = t.FkShopId,
                         ShippingAvailable = true,
                         Method = (int)t.FkShippingMethodId,
                         Weight = t.FkGoods.Weight == null ? 1.0 : (double)t.FkGoods.Weight,
                         Title = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                         DiscountAmount = decimal.Round((decimal)(t.FkVariety.DiscountAmount == null ? 0 : (t.FkVariety.DiscountAmount * (decimal)t.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero), // order item discount
                         DiscountPercent = (decimal)(t.FkVariety.DiscountPercentage == null ? 0 : t.FkVariety.DiscountPercentage),
                         TotalPrice = decimal.Round((decimal)t.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                         UnitPrice = decimal.Round((decimal)t.FkVariety.Price  / rate, 2, MidpointRounding.AwayFromZero),
                         PriceWithDiscount = decimal.Round((decimal)((t.FkVariety.Price - (t.FkVariety.DiscountAmount == null ? (decimal)0 : (decimal)t.FkVariety.DiscountAmount)) * (decimal)t.ItemCount)  / rate, 2, MidpointRounding.AwayFromZero),
                         Vat = decimal.Round((decimal)(t.FkVariety.Vatamount == null ? (decimal)0 : (t.FkVariety.Vatamount * (decimal)t.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero),
                         GoodsVariety = t.FkVariety.TGoodsVariety.Select(i => new Dtos.Home.HomeGoodsVarietyGetDto()
                         {
                             ImageUrl = i.ImageUrl,
                             ParameterTitle = JsonExtensions.JsonValue(i.FkVariationParameter.ParameterTitle, header.Language),
                             ValueTitle = JsonExtensions.JsonValue(i.FkVariationParameterValue.Value, header.Language),
                             ValuesHaveImage = i.FkVariationParameter.ValuesHaveImage
                         }).ToList()
                     }).ToList()
                 })
                 .AsNoTracking()
                 .FirstOrDefaultAsync();

                 
                 var downloadAbleCount = 0 ;

                  foreach (var item in order.Items)
                  {

                      if(item.IsDownloadable) {
                          downloadAbleCount = downloadAbleCount + 1 ;
                          item.Exist = true;
                      }
                      
                  }

                  if(downloadAbleCount == 1) {
                      order.IsDownloadable = true ;
                  }

                return order;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<string> GetRandomString()
        {
            try
            {
                var randomCode = await _context.TCodeRepository.Where(x => x.CodeLength == 8).FirstOrDefaultAsync();
                var code = randomCode.DiscountCode;
                _context.Remove(randomCode);
                await _context.SaveChangesAsync();
                return code;
            }
            catch (System.Exception)
            {
                return Extentions.GetRandomString();
            }
        }

        public async Task<List<ProfileOrderGetDto>> GetProfileOrderDetail(PaginationFormDto pagination)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var data = await _context.TOrder
                .Where(x => x.FkCustomerId == pagination.Id
                && (x.FkOrderStatusId != (int)OrderStatusEnum.Cart)
                )
                .OrderByDescending(x => x.OrderId)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods)
                 .Include(x => x.TOrderItem).ThenInclude(t => t.FkStatus)

                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Select(x => new ProfileOrderGetDto()
                {
                    FinalPrice = decimal.Round((decimal)x.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                    OrderId = x.OrderId,
                    PlacedDateTime = x.PlacedDateTime != null ? Extentions.PersianDateString((DateTime)x.PlacedDateTime) : "",
                    TrackingCode = x.TrackingCode,
                    OrderItemCount = x.TOrderItem.Sum(v => v.ItemCount),
                    Items = x.TOrderItem.Select(t => new ProfileOrderItemGetDto()
                    {
                        GoodsImage = t.FkGoods.ImageUrl,
                        ItemId = t.ItemId,
                        Title = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                        StatusTitle = JsonExtensions.JsonValue(t.FkStatus.StatusTitle, header.Language),
                        StatusId = t.FkStatusId,
                        GoodsId = t.FkGoodsId,
                        IsDownloadable = t.FkGoods.IsDownloadable,
                        DownloadUrl = t.FkGoods.DownloadableFileUrl,
                    }).ToList()


                }).AsNoTracking().ToListAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetProfileOrderDetailCount(PaginationFormDto pagination)
        {
            try
            {
                var data = await _context.TOrder
                .CountAsync(x => x.FkCustomerId == pagination.Id
                && (x.FkOrderStatusId != (int)OrderStatusEnum.Cart)
                && (x.FkOrderStatusId != (int)OrderStatusEnum.ReturnComplete)
                && (x.FkOrderStatusId != (int)OrderStatusEnum.ReturnProcessing)
                );
                return data;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        // گرفتن کشور و شهر سفارش
        public async Task<string> GetOrderCountryAndCity()
        {
            try
            {
                var order = await _context.TOrder
                .Include(c => c.AdFkCity)
                .Include(c => c.AdFkCountry)
                .FirstOrDefaultAsync(x => x.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                token.Id == x.FkCustomerId &&
                 x.TOrderItem.All(t => t.FkStatusId == (int)OrderStatusEnum.Cart));

                if (order == null)
                {
                    return null;
                }

                var countryAndCity = JsonExtensions.JsonGet(order.AdFkCountry.CountryTitle, header) + "," +
                JsonExtensions.JsonGet(order.AdFkCity.CityTitle, header) + "," + order.AdFkCountry.PhoneCode + "," + order.AdFkCountry.Iso2;
                return countryAndCity;
            }
            catch (System.Exception)
            {
                return null;
            }
        }



        public async Task<ProfileOrderGetDto> GetProfileOrderItem(int orderId, string trackingCode = null)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.00 : (decimal)currency.RatesAgainstOneDollar;
                }
                var order = await _context.TOrder
                .Include(b => b.TOrderItem)
                .Where(x => orderId != 0 ? (x.OrderId == orderId &&
                (x.FkCustomerId == token.Id)) : (!string.IsNullOrWhiteSpace(trackingCode) ? x.TrackingCode == trackingCode : false))
                 .Include(y => y.TOrderItem).ThenInclude(t => t.FkShop)
                 .Include(d => d.AdFkCity)
                 .Include(n => n.AdFkCountry)
                 .Include(n => n.AdFkProvince)
                 .Include(u => u.TPaymentTransaction)
                 .ThenInclude(d => d.FkPaymentMethod)
                 .Include(m => m.TOrderItem).ThenInclude(t => t.FkGoods)
                 .Include(r => r.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                 .Include(y => y.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                 .Select(x => new ProfileOrderGetDto()
                 {
                     OrderId = x.OrderId,
                     Discount = decimal.Round((decimal)x.DiscountAmount  / rate, 2, MidpointRounding.AwayFromZero),
                     Shipping = decimal.Round((decimal)x.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                     Total = decimal.Round((decimal)((x.FinalPrice - x.ShippingCost) + x.DiscountAmount)  / rate, 2, MidpointRounding.AwayFromZero),
                     TrackingCode = x.TrackingCode,
                     FinalPrice = decimal.Round((decimal)x.FinalPrice  / rate, 2, MidpointRounding.AwayFromZero),
                     PlacedDateTime = x.PlacedDateTime != null ? Extentions.PersianDateString((DateTime)x.PlacedDateTime) : "",
                     Vat = decimal.Round((decimal)x.Vatamount  / rate, 2, MidpointRounding.AwayFromZero),
                     TotalWithOutDiscountCode = decimal.Round((decimal)(x.FinalPrice + x.DiscountAmount)  / rate, 2, MidpointRounding.AwayFromZero),
                     Address = x.AdAddress,
                     ProvinceTitle = JsonExtensions.JsonValue(x.AdFkProvince.ProvinceName, header.Language),
                     CityTitle =JsonExtensions.JsonValue(x.AdFkCity.CityTitle, header.Language),
                     TransfereeFamily = x.AdTransfereeFamily,
                     TransfereeName = x.AdTransfereeName,
                     Iso = x.AdFkCountry.Iso2,
                     ItemQuantity = x.TOrderItem.Sum(v => v.ItemCount),
                     StatusTitle = JsonExtensions.JsonValue(x.FkOrderStatus.StatusTitle, header.Language),
                     StatusId = x.FkOrderStatus.StatusId,
                     CancelingAllowed = x.TOrderItem.Count(v => v.FkGoods.IsDownloadable == false) > 0 ? x.FkOrderStatus.AllowCancelOrder : false,
                     Payment = x.TPaymentTransaction.FirstOrDefault(m => m.Status == true) == null ? "" :
                     JsonExtensions.JsonValue(x.TPaymentTransaction.FirstOrDefault(m => m.Status == true).FkPaymentMethod.MethodTitle, header.Language),
                     Items = x.TOrderItem.Select(t => new ProfileOrderItemGetDto()
                     {
                         GoodsCode = t.FkGoods.GoodsCode,
                         GoodsImage = t.FkGoods.ImageUrl,
                         IsDownloadable = t.FkGoods.IsDownloadable,
                         DownloadUrl = t.FkGoods.DownloadableFileUrl,
                         GoodsId = t.FkGoods.GoodsId,
                         ItemId = t.ItemId,
                         ReturningAllowed = t.ReturningAllowed,
                         CancelingAllowed = t.FkStatus.AllowCancelOrder,
                         ShippingMethod = t.FkShippingMethodId,
                         StatusTitle = JsonExtensions.JsonValue(t.FkStatus.StatusTitle, header.Language),
                         StatusId = t.FkStatus.StatusId,
                         ModelNumber = t.FkGoods.SerialNumber,
                         Quantity = (double)t.ItemCount,
                         Shipping = decimal.Round((decimal)t.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                         ShopName = t.FkShop.StoreName,
                         ShopSpacialPage = (bool)t.FkShop.Microstore,
                         ShopUrl = t.FkShop.VendorUrlid,
                         Title = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                         OrderStatusPlacedDateTime = t.TOrderLog.FirstOrDefault(b => b.FkStatusId == t.FkStatus.StatusId) != null ?
                         Extentions.PersianDateString((DateTime)t.TOrderLog.FirstOrDefault(b => b.FkStatusId == t.FkStatus.StatusId).LogDateTime)
                          : null,
                         DiscountAmount = decimal.Round((decimal)(t.DiscountAmount == null ? (decimal)0 : (t.DiscountAmount))  / rate, 2, MidpointRounding.AwayFromZero), // order item discount
                         DiscountPercent = decimal.Round((decimal)(((decimal)t.DiscountAmount / (decimal)t.ItemCount) / ((decimal)t.UnitPrice == (decimal)0.00 ? 1 : (decimal)t.UnitPrice)) * 100, 2, MidpointRounding.AwayFromZero),
                         TotalPrice = decimal.Round((decimal)((t.UnitPrice * (decimal)t.ItemCount) - t.DiscountAmount)  / rate, 2, MidpointRounding.AwayFromZero),
                         UnitPrice = decimal.Round((decimal)(t.UnitPrice)  / rate, 2, MidpointRounding.AwayFromZero),
                         PriceWithDiscount = decimal.Round((decimal)((t.UnitPrice) - ((decimal)t.DiscountAmount / (decimal)t.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero),
                         Vat = decimal.Round((decimal)(t.Vatamount == null ? (decimal)0 : (t.Vatamount))  / rate, 2, MidpointRounding.AwayFromZero)

                     }).ToList()
                 })
                 .AsNoTracking()
                 .FirstOrDefaultAsync();
                return order;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        // کالاهایی که کاربر میتواند مرجوعش کند

        public async Task<List<ProfileOrderItemGetDto>> ProfileOrdersItemReturned()
        {
            try
            {
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var currentDate = DateTime.Now;
                var order = await _context.TOrderItem
                .Include(x => x.FkOrder)
                .Where(x =>
                (x.FkOrder.FkCustomerId == token.Id) && x.ReturningAllowed == true
                && x.FkStatusId == (int)OrderStatusEnum.Completed
                && x.DeliveredDate != null)
                 .Include(t => t.FkShop)
                 .Include(t => t.FkGoods)
                 .Include(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                 .Include(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                 .Select(t => new ProfileOrderItemGetDto()
                 {
                     GoodsCode = t.FkGoods.GoodsCode,
                     GoodsImage = t.FkGoods.ImageUrl,
                     GoodsId = t.FkGoods.GoodsId,
                     ItemId = t.ItemId,
                     CancelingAllowed = t.FkStatus.AllowCancelOrder,
                     StatusTitle = JsonExtensions.JsonValue(t.FkStatus.StatusTitle, header.Language),
                     StatusId = t.FkStatus.StatusId,
                     ModelNumber = t.FkGoods.SerialNumber,
                     DeliveredDate = t.DeliveredDate,
                     MaxDeadlineDayToReturning = t.MaxDeadlineDayToReturning,
                     Quantity = (double)t.ItemCount,
                     Shipping = decimal.Round((decimal)t.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                     ShopName = t.FkShop.StoreName,
                     ShopSpacialPage = (bool) t.FkShop.Microstore,
                     ShopUrl= t.FkShop.VendorUrlid,
                     Title = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                     OrderStatusPlacedDateTime = t.DeliveredDate != null ? Extentions.PersianDateString((DateTime)t.DeliveredDate) : "",
                     DiscountAmount = decimal.Round((decimal)(t.DiscountAmount == null ? (decimal)0 : (t.DiscountAmount))  / rate, 2, MidpointRounding.AwayFromZero), // order item discount
                     DiscountPercent = decimal.Round((decimal)(((decimal)t.DiscountAmount / (decimal)t.ItemCount) / ((decimal)t.UnitPrice == (decimal)0.00 ? 1 : (decimal)t.UnitPrice)) * 100, 2, MidpointRounding.AwayFromZero),
                     TotalPrice = decimal.Round((decimal)((t.UnitPrice * (decimal)t.ItemCount) - t.DiscountAmount)  / rate, 2, MidpointRounding.AwayFromZero),
                     UnitPrice = decimal.Round((decimal)(t.UnitPrice)  / rate, 2, MidpointRounding.AwayFromZero),
                     PriceWithDiscount = decimal.Round((decimal)((t.UnitPrice) - ((decimal)t.DiscountAmount / (decimal)t.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero),
                     Vat = decimal.Round((decimal)(t.Vatamount == null ? (decimal)0 : (t.Vatamount))  / rate, 2, MidpointRounding.AwayFromZero)
                 })
                 .AsNoTracking()
                 .ToListAsync();
                if (order.Count > 0)
                {
                    order = order.Where(x => (currentDate - x.DeliveredDate).Value.Days <= x.MaxDeadlineDayToReturning).ToList();
                }
                return order;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        // کالایی که کاربر میتواند مرجوعش کند

        public async Task<ProfileOrderItemGetDto> ProfileProductReturned(int ItemId)
        {
            try
            {
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var currentDate = DateTime.Now;
                var order = await _context.TOrderItem
                .Include(x => x.FkOrder)
                .Where(x => (x.ItemId == ItemId) &&
                (x.FkOrder.FkCustomerId == token.Id) && x.ReturningAllowed == true
                && x.FkStatusId == (int)OrderStatusEnum.Completed
                && x.DeliveredDate != null)
                 .Include(t => t.FkShop)
                 .Include(t => t.FkGoods)
                 .Include(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                 .Include(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                 .Select(t => new ProfileOrderItemGetDto()
                 {
                     GoodsCode = t.FkGoods.GoodsCode,
                     GoodsImage = t.FkGoods.ImageUrl,
                     GoodsId = t.FkGoods.GoodsId,
                     ItemId = t.ItemId,
                     CancelingAllowed = t.FkStatus.AllowCancelOrder,
                     StatusTitle = JsonExtensions.JsonValue(t.FkStatus.StatusTitle, header.Language),
                     StatusId = t.FkStatus.StatusId,
                     ModelNumber = t.FkGoods.SerialNumber,
                     DeliveredDate = t.DeliveredDate,
                     MaxDeadlineDayToReturning = t.MaxDeadlineDayToReturning,
                     Quantity = (double)t.ItemCount,
                     Shipping = decimal.Round((decimal)t.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                     ShopName = t.FkShop.StoreName,
                    ShopSpacialPage = (bool) t.FkShop.Microstore,
                     ShopUrl= t.FkShop.VendorUrlid,
                     Title = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                     OrderStatusPlacedDateTime = t.DeliveredDate != null ? Extentions.PersianDateString((DateTime) t.DeliveredDate) : "",
                     DiscountAmount = decimal.Round((decimal)(t.DiscountAmount == null ? (decimal)0 : (t.DiscountAmount))  / rate, 2, MidpointRounding.AwayFromZero), // order item discount
                     DiscountPercent = decimal.Round((decimal)(((decimal)t.DiscountAmount / (decimal)t.ItemCount) / ((decimal)t.UnitPrice == (decimal)0.00 ? 1 : (decimal)t.UnitPrice)) * 100, 2, MidpointRounding.AwayFromZero),
                     TotalPrice = decimal.Round((decimal)((t.UnitPrice * (decimal)t.ItemCount) - t.DiscountAmount)  / rate, 2, MidpointRounding.AwayFromZero),
                     UnitPrice = decimal.Round((decimal)(t.UnitPrice)  / rate, 2, MidpointRounding.AwayFromZero),
                     PriceWithDiscount = decimal.Round((decimal)((t.UnitPrice) - ((decimal)t.DiscountAmount / (decimal)t.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero),
                     Vat = decimal.Round((decimal)(t.Vatamount == null ? (decimal)0 : (t.Vatamount))  / rate, 2, MidpointRounding.AwayFromZero)
                 })
                 .AsNoTracking()
                 .ToListAsync();
                if (order.Count > 0)
                {
                    order = order.Where(x => (currentDate - x.DeliveredDate).Value.Days <= x.MaxDeadlineDayToReturning).ToList();
                }
                return order.FirstOrDefault();
            }
            catch (System.Exception)
            {
                return null;
            }
        }



        // سفارش های کاربر  که برای مرجوعی درخواست داده شده

        public async Task<List<ProfileOrderItemReturnGetDto>> GetProfileReturnRequested(PaginationFormDto pagination, int type)
        {
            try
            {
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var order = await _context.TOrderItem
                .Include(x => x.FkStatus)
                .Where(
                x => (type == 0 ? (x.FkStatusId == (int)OrderStatusEnum.ReturnProcessing) : (x.FkStatusId == (int)OrderStatusEnum.ReturnComplete))
                 &&
                (x.FkOrder.FkCustomerId == pagination.Id))
                 .Include(x => x.FkShop)
                 .Include(x => x.FkGoods)
                 .Include(x => x.FkOrder)
                 .ThenInclude(x => x.AdFkCountry)
                 .Include(x => x.FkOrder)
                 .ThenInclude(x => x.AdFkCity)                 
                 .Include(x => x.FkOrder)
                 .ThenInclude(x => x.AdFkProvince)
                 .Include(x => x.TOrderReturning)
                 .ThenInclude(t => t.FkReturningReason)
                 .Include(x => x.TOrderReturning)
                 .ThenInclude(t => t.FkReturningAction)
                 .Select(x => new ProfileOrderItemReturnGetDto()
                 {
                     GoodsCode = x.FkGoods.GoodsCode,
                     GoodsImage = x.FkGoods.ImageUrl,
                     GoodsId = x.FkGoods.GoodsId,
                     ItemId = x.ItemId,
                     StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                     StatusId = x.FkStatus.StatusId,
                     ModelNumber = x.FkGoods.SerialNumber,
                     ShopName = x.FkShop.StoreName,
                     ShopSpacialPage = (bool) x.FkShop.Microstore,
                     ShopUrl = x.FkShop.VendorUrlid,
                     CityTitle = JsonExtensions.JsonValue(x.FkOrder.AdFkCity.CityTitle, header.Language),
                     ProvinceTitle = JsonExtensions.JsonValue(x.FkOrder.AdFkProvince.ProvinceName, header.Language),
                     Quantity = (double)x.ItemCount,
                     ReturnReason = JsonExtensions.JsonValue(x.TOrderReturning.OrderByDescending(x => x.ReturningId).First().FkReturningReason.ReasonTitle, header.Language),
                     ReturnAction = JsonExtensions.JsonValue(x.TOrderReturning.OrderByDescending(x => x.ReturningId).First().FkReturningAction.ReturningTypeTitle, header.Language),
                     ReturnActionId = x.TOrderReturning.OrderByDescending(x => x.ReturningId).First().FkReturningActionId,
                     Title = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                     OrderStatusPlacedDateTime = x.FkOrder.PlacedDateTime != null ? Extentions.PersianDateString((DateTime) x.FkOrder.PlacedDateTime) : "",
                     TrackingCode = x.FkOrder.TrackingCode,
                     AdAddress = x.FkOrder.AdAddress,
                     AdTransfereeMobile = x.FkOrder.AdTransfereeMobile,
                     AdTransfereeFamily = x.FkOrder.AdTransfereeFamily,
                     AdTransfereeName = x.FkOrder.AdTransfereeName,
                     Iso = x.FkOrder.AdFkCountry.Iso2,
                     PhoneCode = x.FkOrder.AdFkCountry.PhoneCode,
                     DiscountAmount = decimal.Round((decimal)(x.DiscountAmount == null ? (decimal)0 : (x.DiscountAmount))  / rate, 2, MidpointRounding.AwayFromZero), // order item discount
                     DiscountPercent = decimal.Round((decimal)(((decimal)x.DiscountAmount / (decimal)x.ItemCount) / ((decimal)x.UnitPrice)) * 100, 2, MidpointRounding.AwayFromZero),
                     TotalPrice = decimal.Round((decimal)((x.UnitPrice * (decimal)x.ItemCount) - x.DiscountAmount)  / rate, 2, MidpointRounding.AwayFromZero),
                     UnitPrice = decimal.Round((decimal)(x.UnitPrice)  / rate, 2, MidpointRounding.AwayFromZero),
                     PriceWithDiscount = decimal.Round((decimal)((x.UnitPrice) - ((decimal)x.DiscountAmount / (decimal)x.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero),
                     Vat = decimal.Round((decimal)(x.Vatamount == null ? (decimal)0 : (x.Vatamount))  / rate, 2, MidpointRounding.AwayFromZero)
                 })
                 .OrderByDescending(c=>c.ItemId)
                 .AsNoTracking()
                 .ToListAsync();
                return order;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<int> GetProfileReturnRequestedCount(PaginationFormDto pagination, int type)
        {
            try
            {
                return await _context.TOrderItem
                .CountAsync(
                x => x.FkStatusId == (type == 0 ? (int)OrderStatusEnum.ReturnProcessing : (int)OrderStatusEnum.ReturnComplete)
                 &&
                (x.FkOrder.FkCustomerId == pagination.Id));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }



        public async Task<RepRes<List<ProfileOrderItemGetDto>>> CancelOrder(List<TOrderCanceling> orderCanceling)
        {
            try
            {
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var order = await _context.TOrder
                .Include(x => x.TOrderItem).ThenInclude(t => t.TStockOperation)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkStatus)
                .Include(x => x.TOrderItem).ThenInclude(t => t.TOrderLog)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkGoods)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkStatus)
                .Include(x => x.FkCustomer)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                .Include(x => x.TOrderItem).ThenInclude(t => t.FkShop).ThenInclude(t => t.TUser)
                .FirstOrDefaultAsync(x => x.FkCustomerId == token.Id && x.OrderId == orderCanceling[0].FkOrderId);
                if (order == null)
                {
                    return new RepRes<List<ProfileOrderItemGetDto>>(Message.OrderNotFound, false, null);
                }


                var orderItemList = new List<ProfileOrderItemGetDto>();
                foreach (var item in orderCanceling)
                {
                    var orderItem = order.TOrderItem.FirstOrDefault(x => x.ItemId == item.FkOrderItemId);
                    if (orderItem == null)
                    {
                        return new RepRes<List<ProfileOrderItemGetDto>>(Message.OrderNotFound, false, null);
                    }
                    if (orderItem.FkStatus.AllowCancelOrder != true)
                    {
                        return new RepRes<List<ProfileOrderItemGetDto>>(Message.OrderCanNotCanceled, false, null);
                    }

                    var itemList = new ProfileOrderItemGetDto();
                    itemList.GoodsCode = orderItem.FkGoods.GoodsCode;
                    itemList.GoodsImage = orderItem.FkGoods.ImageUrl;
                    itemList.GoodsId = orderItem.FkGoods.GoodsId;
                    itemList.CustomerRefound = orderItem.FkOrder.FkCustomer.RefundPreference;
                    itemList.ItemId = orderItem.ItemId;
                    itemList.CancelingAllowed = orderItem.FkStatus.AllowCancelOrder;
                    itemList.StatusTitle = JsonExtensions.JsonGet(orderItem.FkStatus.StatusTitle, header);
                    itemList.StatusId = orderItem.FkStatus.StatusId;
                    itemList.ModelNumber = orderItem.FkGoods.SerialNumber;
                    itemList.DeliveredDate = orderItem.DeliveredDate;
                    itemList.MaxDeadlineDayToReturning = orderItem.MaxDeadlineDayToReturning;
                    itemList.Quantity = (double)orderItem.ItemCount;
                    itemList.Shipping = decimal.Round((decimal)orderItem.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero);
                    itemList.ShopName = orderItem.FkShop.StoreName;
                    itemList.Title = JsonExtensions.JsonGet(orderItem.FkGoods.Title, header);
                    itemList.OrderStatusPlacedDateTime = orderItem.DeliveredDate != null ? Extentions.PersianDateString((DateTime) orderItem.DeliveredDate) : "";
                    itemList.DiscountAmount = decimal.Round((decimal)(orderItem.DiscountAmount == null ? (decimal)0 : (orderItem.DiscountAmount))  / rate, 2, MidpointRounding.AwayFromZero); // order item discount
                    itemList.DiscountPercent = decimal.Round((decimal)(((decimal)orderItem.DiscountAmount / (decimal)orderItem.ItemCount) / ((decimal)orderItem.UnitPrice)) * 100, 2, MidpointRounding.AwayFromZero);
                    itemList.TotalPrice = decimal.Round((decimal)((orderItem.UnitPrice * (decimal)orderItem.ItemCount) - orderItem.DiscountAmount)  / rate, 2, MidpointRounding.AwayFromZero);
                    itemList.UnitPrice = decimal.Round((decimal)(orderItem.UnitPrice)  / rate, 2, MidpointRounding.AwayFromZero);
                    itemList.PriceWithDiscount = decimal.Round((decimal)((orderItem.UnitPrice) - ((decimal)orderItem.DiscountAmount / (decimal)orderItem.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero);
                    itemList.Vat = decimal.Round((decimal)(orderItem.Vatamount == null ? (decimal)0 : (orderItem.Vatamount))  / rate, 2, MidpointRounding.AwayFromZero);
                    orderItemList.Add(itemList);
                }



                var orderDeleted = false;
                var orderItemCanceledIds = orderCanceling.Select(x => x.FkOrderItemId).ToList();

                if (order.PaymentStatus == true)
                {
                    foreach (var item in order.TOrderItem.Where(t => orderItemCanceledIds.Contains(t.ItemId)).ToList())
                    {
                        await _wareHouseRepository.AddStockOpration((int)StockOperationTypeEnum.SaleReturn, item.FkVarietyId, item.ItemId, (double)item.ItemCount, item.UnitPrice, "بازگشت به انبار برای لفو سفارش کاربر");
                        //delete shop transaction
                        _context.TUserTransaction.RemoveRange(item.TUserTransaction.Where(x => x.FkUserId == item.FkShop.TUser.FirstOrDefault().UserId).ToList());
                        // transaction for customer
                        await _accountingRepository.AddTransaction((int)TransactionTypeEnum.Refund, token.UserId, item.FkOrderId, item.ItemId, null, (int)TransactionStatusEnum.Completed, (decimal)item.FinalPrice, "بازپرداخت  " + Extentions.DecimalRoundWithZiro( (decimal) order.FinalPrice ) + "تومان برای لغو سفارش");
                        item.FkStatusId = (int)OrderStatusEnum.Cancelled;
                        await this.AddOrderLog(item.FkOrderId, item.ItemId, item.FkStatusId, token.UserId, "این آیتم توسط کاربر لغو شده است");
                    }
                    await _context.TOrderCanceling.AddRangeAsync(orderCanceling);
                }
                else
                {
                    _context.TUserTransaction.RemoveRange(order.TOrderItem.Where(t => orderItemCanceledIds.Contains(t.ItemId)).SelectMany(x => x.TUserTransaction).ToList());
                    _context.TOrderLog.RemoveRange(order.TOrderItem.Where(t => orderItemCanceledIds.Contains(t.ItemId)).SelectMany(x => x.TOrderLog).ToList());
                    _context.TOrderItem.RemoveRange(order.TOrderItem.Where(t => orderItemCanceledIds.Contains(t.ItemId)).ToList());

                    order.ComissionPrice = order.TOrderItem.Where(x => !orderItemCanceledIds.Contains(x.ItemId)).Sum(x => x.ComissionPrice == null ? 0 : (decimal)x.ComissionPrice);
                    order.DiscountAmount = order.TOrderItem.Where(x => !orderItemCanceledIds.Contains(x.ItemId)).Sum(x => x.DiscountAmount == null ? 0 : (decimal)x.DiscountAmount);
                    order.Price = order.TOrderItem.Where(x => !orderItemCanceledIds.Contains(x.ItemId)).Sum(x => x.UnitPrice == null ? 0 : (x.UnitPrice * (decimal)x.ItemCount));
                    order.ShippingCost = order.TOrderItem.Where(x => !orderItemCanceledIds.Contains(x.ItemId)).Sum(x => x.ShippingCost == null ? 0 : (decimal)x.ShippingCost);
                    order.Vatamount = order.TOrderItem.Where(x => !orderItemCanceledIds.Contains(x.ItemId)).Sum(x => x.Vatamount == null ? 0 : (decimal)x.Vatamount);
                    order.FinalPrice = (order.Price + order.ShippingCost + order.Vatamount) - order.DiscountAmount;

                    if (order.TOrderItem.Where(t => orderItemCanceledIds.Contains(t.ItemId)).Count() == order.TOrderItem.Count)
                    {
                        _context.TOrderLog.RemoveRange(order.TOrderLog.Where(x => x.FkOrderItemId == null).ToList());
                        _context.TOrder.Remove(order);
                        orderDeleted = true;
                    }
                }
                if (orderDeleted == false)
                {
                    order.FkOrderStatusId = order.TOrderItem.Min(x => x.FkStatusId);
                }

                await _context.SaveChangesAsync();

                return new RepRes<List<ProfileOrderItemGetDto>>(Message.Successfull, true, orderItemList);
            }
            catch (System.Exception)
            {
                return new RepRes<List<ProfileOrderItemGetDto>>(Message.OrderItemCanceling, false, null);
            }
        }

        public async Task<RepRes<bool>> ReturnOrder(TOrderReturning orderReturning)
        {
            try
            {
                var orderItem = await _context.TOrderItem
                .Include(x => x.FkOrder)
                .Include(x => x.TOrderReturning)
                .FirstOrDefaultAsync(x => x.ItemId == orderReturning.FkOrderItemId && x.FkOrder.FkCustomerId == token.Id);

                if (orderItem == null)
                {
                    return new RepRes<bool>(Message.OrderItemNotFound, false, false);
                }

                if (orderItem.FkStatusId != (int)OrderStatusEnum.Completed && orderItem.FkStatusId != (int)OrderStatusEnum.ReturnProcessing && orderItem.FkStatusId != (int)OrderStatusEnum.ReturnComplete)
                {
                    return new RepRes<bool>(Message.YouCantRequestForReturningForInThisStatusOfOrder, false, false);
                }
                if (orderItem.ReturningAllowed == false)
                {
                    return new RepRes<bool>(Message.ThisItemNotAllowdtoReturning, false, false);
                }
                var MaxDayToReturnDate = orderItem.DeliveredDate.Value.AddDays(orderItem.MaxDeadlineDayToReturning == null ? 0 : (int)orderItem.MaxDeadlineDayToReturning);
                if (DateTime.Now > MaxDayToReturnDate)
                {
                    return new RepRes<bool>(Message.AllowdReturningDatePassed, false, false);
                }

                if (orderItem.TOrderReturning != null)
                {
                    var oldItemCountRequested = orderItem.TOrderReturning.Sum(x => x.Quantity);
                }


                orderItem.FkStatusId = (int)OrderStatusEnum.ReturnProcessing;
                await this.AddOrderLog(orderItem.FkOrderId, orderItem.ItemId, orderItem.FkStatusId, token.UserId, " در خواست برای مرجوعی ");

                orderReturning.FkOrderId = orderItem.FkOrderId;
                orderReturning.FkStatusId = (int)ReturningStatusEnum.Processing;
                orderReturning.RegisterDateTime = DateTime.Now;

                var statusOrder = await _context.TReturningStatus.FirstOrDefaultAsync(c=>c.StatusId == orderReturning.FkStatusId);
                var statusDesc = "";
                if(statusOrder != null) {
                  statusDesc =  JsonExtensions.JsonGet(statusOrder.Description, header)  ;
                }

                var returnLog = new TOrderReturningLog();
                returnLog.FkStatusId = orderReturning.FkStatusId;
                returnLog.FkUserId = token.UserId;
                returnLog.LogDateTime = DateTime.Now;
                returnLog.LogId = 0;
                returnLog.LogComment = statusDesc;
                orderReturning.TOrderReturningLog = new List<TOrderReturningLog>();
                orderReturning.TOrderReturningLog.Add(returnLog);

                await _context.TOrderReturning.AddAsync(orderReturning);
                await _context.SaveChangesAsync();

                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.OrderItemReturning, false, false);
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



        public async Task<TPaymentTransaction> GetPaymentTransaction(string paymentId)
        {
            try
            {
                var result = await _context.TPaymentTransaction.FirstAsync(x => x.PaymentId == paymentId);

                return result;
            }
            catch (System.Exception)
            {
                return null;
            }
        }



        // for mobile

        public async Task<CustomerOrderCountDto> GetCustomerOrderCount()
        {
            try
            {
                var result = new CustomerOrderCountDto();
                var currentCustomer = await _context.TCustomer.FirstAsync(x => x.CustomerId == token.Id);

                var orders = await _context.TOrder
                 .Where(x => x.FkCustomerId == token.Id).ToListAsync();
                result.FullName = currentCustomer.Name + " " + currentCustomer.Family;
                result.Email = currentCustomer.Email;
                result.EmailValid = currentCustomer.EmailVerifed;
                result.OrderCount = orders
                .Count(x => x.FkCustomerId == token.Id && x.FkOrderStatusId != (int)OrderStatusEnum.Cart);
                result.PaymentCount = orders
                .Count(x => x.FkCustomerId == token.Id
                && x.FkOrderStatusId != (int)OrderStatusEnum.Cart
                && x.FkOrderStatusId != (int)OrderStatusEnum.Cancelled
                && x.FkOrderStatusId != (int)OrderStatusEnum.ReturnComplete
                && x.FkOrderStatusId != (int)OrderStatusEnum.ReturnProcessing);
                result.ReturnCount = orders
                .Count(x => x.FkCustomerId == token.Id
                && x.FkOrderStatusId == (int)OrderStatusEnum.ReturnComplete
                && x.FkOrderStatusId == (int)OrderStatusEnum.ReturnProcessing);

                return result;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<TPaymentTransaction> GetPaymentTransactionWithOrderId(long orderId)
        {
            try
            {
                var result = await _context.TPaymentTransaction.FirstAsync(x => x.FkOrderId == orderId);

                return result;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> SendMessageForCustomerForUnUseCartOrder()
        {
            try
            {
                var result = await _context.TOrder.Include(c => c.FkCustomer).ThenInclude(z => z.FkCountry).Where(x => x.FkOrderStatusId == (int)OrderStatusEnum.Cart && x.FkCustomerId != (int)CustomerTypeEnum.Unknown).ToListAsync();
                var notif = await _context.TNotificationSetting.FirstAsync(c => c.NotificationSettingId == (int)NotificationSettingTypeEnum.AbandonedShoppingCartsDaysLater);

                foreach (var order in result)
                {
                    if (order.InitialDateTime.AddDays((int)notif.Days).Date == DateTime.Now.Date)
                    {
                        // ارسال پیام به کاربر
                      await  _notificationService.SendNotification((int)NotificationSettingTypeEnum.AbandonedShoppingCartsDaysLater
                        , null,
                        null
                         , order.FkCustomer.Email, order.FkCustomer.FkCountry != null ? ("+" + order.FkCustomer.FkCountry.PhoneCode + order.FkCustomer.MobileNumber) : null);
                    }
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }




        // کالاهایی که کاربر میتواند کنسل کند

        public async Task<List<ProfileOrderItemGetDto>> ProfileOrdersItemCanceled(long orderId)
        {
            try
            {
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var currentDate = DateTime.Now;
                var order = await _context.TOrderItem
                .Include(x => x.FkOrder)
                .ThenInclude(x => x.FkCustomer)
                .Include(x => x.FkStatus)
                .Where(x =>
                (x.FkOrder.FkCustomerId == token.Id) && x.FkOrderId == orderId && x.FkStatus.AllowCancelOrder == true)
                 .Include(t => t.FkShop)
                 .Include(t => t.FkGoods)
                 .Include(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameter)
                 .Include(t => t.FkVariety).ThenInclude(i => i.TGoodsVariety).ThenInclude(p => p.FkVariationParameterValue)
                 .Select(t => new ProfileOrderItemGetDto()
                 {
                     GoodsCode = t.FkGoods.GoodsCode,
                     GoodsImage = t.FkGoods.ImageUrl,
                     GoodsId = t.FkGoods.GoodsId,
                     CustomerRefound = t.FkOrder.FkCustomer.RefundPreference,
                     ItemId = t.ItemId,
                     CancelingAllowed = t.FkStatus.AllowCancelOrder,
                     StatusTitle = JsonExtensions.JsonValue(t.FkStatus.StatusTitle, header.Language),
                     StatusId = t.FkStatus.StatusId,
                     ModelNumber = t.FkGoods.SerialNumber,
                     DeliveredDate = t.DeliveredDate,
                     MaxDeadlineDayToReturning = t.MaxDeadlineDayToReturning,
                     Quantity = (double)t.ItemCount,
                     Shipping = decimal.Round((decimal)t.ShippingCost  / rate, 2, MidpointRounding.AwayFromZero),
                     ShopName = t.FkShop.StoreName,
                     Title = JsonExtensions.JsonValue(t.FkGoods.Title, header.Language),
                     OrderStatusPlacedDateTime = t.DeliveredDate != null ? Extentions.PersianDateString((DateTime) t.DeliveredDate) : "",
                     DiscountAmount = decimal.Round((decimal)(t.DiscountAmount == null ? (decimal)0 : (t.DiscountAmount))  / rate, 2, MidpointRounding.AwayFromZero), // order item discount
                     DiscountPercent = decimal.Round((decimal)(((decimal)t.DiscountAmount / (decimal)t.ItemCount) / ((decimal)t.UnitPrice == (decimal)0.00 ? 1 : (decimal)t.UnitPrice)) * 100, 2, MidpointRounding.AwayFromZero),
                     TotalPrice = decimal.Round((decimal)((t.UnitPrice * (decimal)t.ItemCount) - t.DiscountAmount)  / rate, 2, MidpointRounding.AwayFromZero),
                     UnitPrice = decimal.Round((decimal)(t.UnitPrice)  / rate, 2, MidpointRounding.AwayFromZero),
                     PriceWithDiscount = decimal.Round((decimal)((t.UnitPrice) - ((decimal)t.DiscountAmount / (decimal)t.ItemCount))  / rate, 2, MidpointRounding.AwayFromZero),
                     Vat = decimal.Round((decimal)(t.Vatamount == null ? (decimal)0 : (t.Vatamount))  / rate, 2, MidpointRounding.AwayFromZero)
                 })
                 .AsNoTracking()
                 .ToListAsync();
                return order;
            }
            catch (System.Exception)
            {
                return null;
            }
        }












    }
}