using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using System;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Customer;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Survey;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class UserActivityRepository : IUserActivityRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }

        public UserActivityRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
        }
        public async Task<bool> LikeGoodsAdd(int customerId, int goodsId)
        {
            try
            {
                var data = await _context.TGoodsLike.FirstOrDefaultAsync(x => x.FkCustomerId == customerId && goodsId == x.FkGoodsId);
                if (data != null)
                {
                    _context.TGoodsLike.Remove(data);
                    var goods = await _context.TGoods.FindAsync(goodsId);
                    goods.LikedCount = goods.LikedCount - 1;
                    await _context.SaveChangesAsync();

                    return true;
                }
                else
                {
                    var newLike = new TGoodsLike();
                    newLike.FkCustomerId = customerId;
                    newLike.FkGoodsId = goodsId;
                    newLike.LikeDate = DateTime.Now;
                    await _context.TGoodsLike.AddAsync(newLike);
                    var goods = await _context.TGoods.FindAsync(goodsId);
                    goods.LikedCount = goods.LikedCount + 1;
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> ViewGoodsAdd(int customerId, int goodsId, string ipAddress)
        {
            try
            {
                var data = await _context.TGoodsView.FirstOrDefaultAsync(x => (x.FkCustomerId == customerId || x.IpAddress == ipAddress) && goodsId == x.FkGoodsId);
                if (data == null)
                {
                    var newView = new TGoodsView();
                    newView.FkCustomerId = customerId == 0 ? (int)CustomerTypeEnum.Unknown : customerId;
                    newView.FkGoodsId = goodsId;
                    newView.IpAddress = ipAddress;
                    newView.ViewDate = DateTime.Now;
                    await _context.TGoodsView.AddAsync(newView);
                    var goods = await _context.TGoods.FindAsync(goodsId);
                    goods.ViewCount = goods.ViewCount + 1;
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        ///// ثبت نظر کاربر
        public async Task<TGoodsComment> GoodsCommentAdd(GoodsCommentAddDto goodsComment)
        {
            try
            {
                var orderItem = await _context.TOrderItem.FirstAsync(v => v.ItemId == goodsComment.OrderItemId);
                var commentExist = await _context.TGoodsComment.FirstOrDefaultAsync(x => x.FkCustomerId == goodsComment.CustomerId && x.FkGoodsId == orderItem.FkGoodsId && x.FkVarietyId == orderItem.FkVarietyId);
                var goodsCommentPoint = new List<TGoodsCommentPoints>();
                foreach (var item in goodsComment.TGoodsCommentPoints)
                {
                    if(item != null) {
                        goodsCommentPoint.Add(item);
                    }
                }
                if (commentExist != null)
                {
                    
                        var points = await _context.TGoodsCommentPoints.Where(x => x.FkCommentId == commentExist.CommentId).ToListAsync();
                        if (points.Count > 0)
                        {
                            _context.TGoodsCommentPoints.RemoveRange(points);
                        }
                        var survayAns = await _context.TShopSurveyAnswers.Where(x => x.FkCommentId == commentExist.CommentId).ToListAsync();
                        if (survayAns.Count > 0)
                        {
                            _context.TShopSurveyAnswers.RemoveRange(survayAns);
                        }
                        await _context.SaveChangesAsync();

                        if (goodsCommentPoint.Count > 0)
                        {
                            foreach (var item in goodsCommentPoint)
                            {
                                item.FkCommentId = commentExist.CommentId;
                                item.PointId = 0 ;
                            }
                            await _context.TGoodsCommentPoints.AddRangeAsync(goodsCommentPoint);
                        }

                        if (goodsComment.ShopSurveyAnswers.Count > 0)
                        {
                            foreach (var item in goodsComment.ShopSurveyAnswers)
                            {
                                item.FkCommentId = commentExist.CommentId;
                                item.FkCustomerId = commentExist.FkCustomerId;
                                item.FkShopId = orderItem.FkShopId;
                                item.AnsId = 0 ;
                            }
                            await _context.TShopSurveyAnswers.AddRangeAsync(goodsComment.ShopSurveyAnswers);
                        }

                        await _context.SaveChangesAsync();

                        commentExist.CommentDate = DateTime.Now;
                        commentExist.CommentText = goodsComment.CommentText;
                        commentExist.ReviewPoint = goodsComment.ReviewPoint;
                        commentExist.IsAccepted = null;

                        await _context.SaveChangesAsync();


                    return commentExist;
                }
                else
                {
                    var goodsComm = new TGoodsComment();
                    goodsComm.FkGoodsId = orderItem.FkGoodsId;
                    goodsComm.FkVarietyId = orderItem.FkVarietyId;
                    goodsComm.CommentDate = DateTime.Now;
                    goodsComm.CommentText = goodsComment.CommentText;
                    goodsComm.FkCustomerId = goodsComment.CustomerId;
                    goodsComm.IsAccepted = null;
                    goodsComm.ReviewPoint = goodsComment.ReviewPoint;
                    goodsComm.TGoodsCommentPoints = goodsCommentPoint;
                    await _context.TGoodsComment.AddAsync(goodsComm);

                    await _context.SaveChangesAsync();

                    if (goodsComment.ShopSurveyAnswers.Count > 0)
                    {
                        foreach (var item in goodsComment.ShopSurveyAnswers)
                        {
                            item.FkCommentId = goodsComm.CommentId;
                            item.FkCustomerId = goodsComm.FkCustomerId;
                            item.FkShopId = orderItem.FkShopId;
                        }
                        await _context.TShopSurveyAnswers.AddRangeAsync(goodsComment.ShopSurveyAnswers);
                        await _context.SaveChangesAsync();
                    }

                    // var goods = await _context.TGoods
                    // .Include(x => x.TGoodsComment).FirstOrDefaultAsync(x => x.GoodsId == orderItem.FkGoodsId);

                    // goods.SurveyScore = goods.TGoodsComment.Select(x => x.ReviewPoint).Average();
                    // await _context.SaveChangesAsync();
                    return goodsComm;
                }

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<GoodsCommentGetDto> GetCustomerGoodsComment(long orderItemId, int customerId)
        {
            try
            {
                var orderItem = await _context.TOrderItem.Include(b => b.FkShop).FirstAsync(v => v.ItemId == orderItemId);
                var Data = await _context.TGoodsComment
                .Include(x => x.TGoodsCommentPoints)
                .Where(x => x.FkCustomerId == customerId && x.FkGoodsId == orderItem.FkGoodsId && x.FkVarietyId == orderItem.FkVarietyId)
                .AsNoTracking()
                .Select(c => new GoodsCommentGetDto()
                {
                    CommentId = c.CommentId,
                    CommentText = c.CommentText,
                    CommentDate = c.CommentDate,
                    CustomerId = c.FkCustomerId,
                    ReviewPoint = c.ReviewPoint,
                    IsAccepted = c.IsAccepted,
                    ShopName = orderItem.FkShop.StoreName,
                    ShopSpecialPage = (bool) orderItem.FkShop.Microstore,
                    ShopUrl = orderItem.FkShop.VendorUrlid,
                    GoodsCommentPoints = c.TGoodsCommentPoints.Select(b => new GoodsCommentPointsDto()
                    {
                        PointId = b.PointId,
                        FkCommentId = b.FkCommentId,
                        PointText = b.PointText,
                        PointType = b.PointType
                    }).ToList()
                })
                .FirstOrDefaultAsync();
                if (Data != null)
                {
                    Data.ShopSurveyAnswers = await _context.TShopSurveyAnswers
                    .Where(x => x.FkCustomerId == customerId && x.FkOrderItemId == orderItem.ItemId)
                     .Select(b => new ShopSurveyAnswersDto()
                     {
                         AnsId = b.AnsId,
                         FkCommentId = b.FkCommentId,
                         FkQuestionId = b.FkQuestionId,
                         FkCustomerId = b.FkCustomerId,
                         FkShopId = b.FkShopId,
                         AnsValue = b.AnsValue
                     }).ToListAsync();
                }
                else
                {
                    Data = new GoodsCommentGetDto();
                    Data.ShopName = orderItem.FkShop.StoreName;
                }

                return Data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }



        // کالاهای لایک شده کاربر در وب سایت

        public async Task<int> GetCustomerWishListCount(int customerId)
        {
            try
            {
                return await _context.TGoodsLike.AsNoTracking().CountAsync(x => x.FkCustomerId == customerId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        // کالاهای سبد خرید کاربر در وب سایت

        public async Task<int> GetCustomerCartCount(int customerId, Guid? cookieId)
        {
            try
            {
                return await _context.TOrderItem
                .Include(o => o.FkOrder)
                .AsNoTracking().CountAsync
                (x => x.FkOrder.FkCustomerId == customerId
                && ((customerId != (int)CustomerTypeEnum.Unknown && cookieId == Guid.Empty) ? true : x.FkOrder.CookieId == cookieId)
                && x.FkOrder.FkOrderStatusId == (int)OrderStatusEnum.Cart
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<CustomerAddressDto> AddCustomerAddress(TCustomerAddress customerAddress, bool forCart, long? orderId)
        {
            try
            {
                if (forCart == true)
                {
                    var order = await _context.TOrder.Include(x => x.TOrderItem).FirstOrDefaultAsync(x =>
                    (token.Id == 0 ? (x.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.CookieId == token.CookieId) : (token.Id == x.FkCustomerId && x.CookieId == null))
                    && x.TOrderItem.All(t => t.FkStatusId == (int)OrderStatusEnum.Cart));
                    if (order == null)
                    {
                        return null;
                    }
                    customerAddress.FkCityId = (int)order.AdFkCityId;
                    customerAddress.FkCountryId = (int)order.AdFkCountryId;
                    customerAddress.FkProvinceId = (int)order.AdFkProvinceId;
                }
                else
                {
                    var order = await _context.TOrder.FindAsync(orderId);
                    if (order == null)
                    {
                        return null;
                    }
                    customerAddress.FkCityId = (int)order.AdFkCityId;
                    customerAddress.FkCountryId = (int)order.AdFkCountryId;
                    customerAddress.FkProvinceId = (int)order.AdFkProvinceId;
                }

                await _context.TCustomerAddress.AddAsync(customerAddress);
                await _context.SaveChangesAsync();
                return await _context.TCustomerAddress
                .Include(x => x.FkCity)
                .Include(x => x.FkCountry)
                .Include(x => x.FkProvince)
                .Select(x => new CustomerAddressDto
                {
                    Address = x.Address,
                    AddressId = x.AddressId,
                    CityName = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                    ProvinceName = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language),
                    CountryName = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                    FkCityId = x.FkCityId,
                    FkCountryId = x.FkCountryId,
                    FkProvinceId = (int)x.FkProvinceId,
                    LocationX = x.LocationX,
                    LocationY = x.LocationY,
                    PostalCode = x.PostalCode,
                    TransfereeFamily = x.TransfereeFamily,
                    TransfereeMobile = x.TransfereeMobile,
                    TransfereeName = x.TransfereeName
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AddressId == customerAddress.AddressId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<CustomerAddressDto> UpdateCustomerAddress(TCustomerAddress customerAddress, bool forOrder)
        {
            try
            {
                var currentAddress = await _context.TCustomerAddress.FirstOrDefaultAsync(b => b.AddressId == customerAddress.AddressId);

                if (currentAddress == null)
                {
                    return null;
                }

                if (currentAddress.TransfereeMobile != customerAddress.TransfereeMobile)
                {
                    customerAddress.MobileVerifed = false;
                    customerAddress.IsDefualt = false;
                }

                if (forOrder)
                {
                    currentAddress.Address = customerAddress.Address;
                    currentAddress.TransfereeName = customerAddress.TransfereeName;
                    currentAddress.TransfereeFamily = customerAddress.TransfereeFamily;
                    currentAddress.PostalCode = customerAddress.PostalCode;
                    currentAddress.TransfereeMobile = customerAddress.TransfereeMobile;
                }
                else
                {
                    _context.Entry(currentAddress).CurrentValues.SetValues(customerAddress);
                    _context.Entry(currentAddress).Property(x => x.IsDefualt).IsModified = false;
                    _context.Entry(currentAddress).Property(x => x.MobileVerifed).IsModified = false;
                }

                await _context.SaveChangesAsync();
                return await _context.TCustomerAddress
                .Include(x => x.FkCity)
                .Include(x => x.FkCountry)
                .Include(x => x.FkProvince)
                .Select(x => new CustomerAddressDto
                {
                    Address = x.Address,
                    AddressId = x.AddressId,
                    CityName = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                    ProvinceName = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language),
                    CountryName = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                    FkCityId = x.FkCityId,
                    FkCountryId = x.FkCountryId,
                    FkProvinceId = (int)x.FkProvinceId,
                    LocationX = x.LocationX,
                    LocationY = x.LocationY,
                    PostalCode = x.PostalCode,
                    TransfereeFamily = x.TransfereeFamily,
                    TransfereeMobile = x.TransfereeMobile,
                    TransfereeName = x.TransfereeName
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AddressId == customerAddress.AddressId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<CustomerAddressDto>> GetCustomerAddress(string type)
        {
            try
            {
                if (type == "order")
                {
                    var order = await _context.TOrder.FirstOrDefaultAsync(x => x.FkOrderStatusId == (int)OrderStatusEnum.Cart &&
                    (token.Id == 0 ? (x.FkCustomerId == (int)CustomerTypeEnum.Unknown && x.CookieId == token.CookieId) : (token.Id == x.FkCustomerId && x.CookieId == null)) &&
                     x.TOrderItem.All(t => t.FkStatusId == (int)OrderStatusEnum.Cart));
                    return await _context.TCustomerAddress
                    .Where(x => x.FkCountryId == order.AdFkCountryId
                    && x.FkCityId == order.AdFkCityId && x.FkCustomerId == token.Id)
                    .Include(x => x.FkCity)
                    .Include(x => x.FkCountry)
                    .Include(x => x.FkProvince)
                    .Select(x => new CustomerAddressDto
                    {
                        Address = x.Address,
                        AddressId = x.AddressId,
                        CityName = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                        ProvinceName = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language),
                        CountryName = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                        FkCityId = x.FkCityId,
                        FkCountryId = x.FkCountryId,
                        FkProvinceId = (int)x.FkProvinceId,
                        Iso = x.FkCountry.Iso2,
                        MobileVerifed = x.MobileVerifed,
                        IsDefualt = x.IsDefualt,
                        LocationX = x.LocationX,
                        LocationY = x.LocationY,
                        PostalCode = x.PostalCode,
                        PhoneCode = x.FkCountry.PhoneCode,
                        TransfereeFamily = x.TransfereeFamily,
                        TransfereeMobile = x.TransfereeMobile,
                        TransfereeName = x.TransfereeName
                    })
                    .AsNoTracking()
                    .ToListAsync();
                }
                else
                {
                    return await _context.TCustomerAddress
                    .Where(x => x.FkCustomerId == token.Id)
                    .Include(x => x.FkCity)
                    .Include(x => x.FkCountry)
                    .Include(x => x.FkProvince)
                    .Select(x => new CustomerAddressDto
                    {
                        Address = x.Address,
                        AddressId = x.AddressId,
                        CityName = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                        ProvinceName = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language),
                        CountryName = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                        FkCityId = x.FkCityId,
                        FkCountryId = x.FkCountryId,
                        FkProvinceId = (int)x.FkProvinceId,
                        LocationX = x.LocationX,
                        LocationY = x.LocationY,
                        PostalCode = x.PostalCode,
                        Iso = x.FkCountry.Iso2,
                        MobileVerifed = x.MobileVerifed,
                        IsDefualt = x.IsDefualt,
                        PhoneCode = x.FkCountry.PhoneCode,
                        TransfereeFamily = x.TransfereeFamily,
                        TransfereeMobile = x.TransfereeMobile,
                        TransfereeName = x.TransfereeName
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

        public async Task<bool> DeleteCustomerAddress(int addressId, int customerId)
        {
            try
            {
                var address = await _context.TCustomerAddress.FirstOrDefaultAsync(x => x.AddressId == addressId && (customerId == 0 ? true : x.FkCustomerId == customerId));
                if (address == null)
                {
                    return false;
                }
                _context.TCustomerAddress.Remove(address);

                await _context.SaveChangesAsync();

                return true;

            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<UserTransactionWebGetDto> GetProfileAjyalCredit(PaginationFormDto pagination)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }

                var WebTransaction = new UserTransactionWebGetDto();
                var customer = await _context.TCustomer.FirstAsync(x => x.CustomerId == pagination.Id);
                WebTransaction.Credit = customer.Credit;
                WebTransaction.TransactionList = await _context.TUserTransaction
                .Where(x => x.FkUserId == token.UserId)
                .OrderByDescending(x => x.TransactionId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(e => e.FkTransactionType)
                .Select(x => new UserTransactionGetDto
                {
                    Balance =
                    Extentions.DecimalRound(
                        Extentions.CalculateBalancePrice(_context.TUserTransaction
                .Where(t => t.FkUserId == token.UserId && (t.FkTransactionTypeId == (int)TransactionTypeEnum.CashPayment
                || t.FkTransactionTypeId == (int)TransactionTypeEnum.Refund || t.FkTransactionTypeId == (int)TransactionTypeEnum.GiftCard)
                && t.TransactionId <= x.TransactionId)
                .Sum(t => t.Amount) , _context.TUserTransaction
                .Where(t => t.FkUserId == token.UserId &&  (t.FkTransactionTypeId == (int)TransactionTypeEnum.Purchased || t.FkTransactionTypeId == (int)TransactionTypeEnum.Withdraw)
                && t.TransactionId <= x.TransactionId).Sum(t => t.Amount))   / rate),
                    Amount = decimal.Round(x.Amount  / rate, 2, MidpointRounding.AwayFromZero),
                    Comment = x.Comment,
                    TransactionTypeId = x.FkTransactionTypeId,
                    TransactionId = x.TransactionId,
                    TransactionDateTime = Extentions.PersianDateString(x.TransactionDateTime),
                    TransactionType = JsonExtensions.JsonValue(x.FkTransactionType.TransactionTypeTitle, header.Language)

                })
                .AsNoTracking()
                .ToListAsync();
                return WebTransaction;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<CustomerAddressDto> AddProfileCustomerAddress(TCustomerAddress customerAddress)
        {
            try
            {

                await _context.TCustomerAddress.AddAsync(customerAddress);
                await _context.SaveChangesAsync();
                return await _context.TCustomerAddress
                .Include(x => x.FkCity)
                .Include(x => x.FkCountry)
                .Include(x => x.FkProvince)
                .Select(x => new CustomerAddressDto
                {
                    Address = x.Address,
                    AddressId = x.AddressId,
                    CityName = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                    ProvinceName = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language),
                    CountryName = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                    FkCityId = x.FkCityId,
                    FkCountryId = x.FkCountryId,
                    FkProvinceId = (int)x.FkProvinceId,
                    LocationX = x.LocationX,
                    LocationY = x.LocationY,
                    PostalCode = x.PostalCode,
                    TransfereeFamily = x.TransfereeFamily,
                    TransfereeMobile = x.TransfereeMobile,
                    TransfereeName = x.TransfereeName
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AddressId == customerAddress.AddressId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<int> GetProfileAjyalCreditCount(PaginationFormDto pagination)
        {
            try
            {

                return await _context.TUserTransaction
                .CountAsync(x => x.FkUserId == token.UserId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }



        public async Task<bool> SetDefualtAddress(int addressId)
        {
            try
            {

                var result = await _context.TCustomerAddress.Where(x => x.FkCustomerId == token.Id).ToListAsync();
                foreach (var item in result)
                {
                    item.IsDefualt = false;
                }
                var findAddress = result.First(x => x.AddressId == addressId);
                if (!findAddress.MobileVerifed)
                {
                    return false;
                }
                findAddress.IsDefualt = true;
                await _context.SaveChangesAsync();
                return true;

            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> ChangeMobileNumberAddress(int addressId, string mobileNumber)
        {
            try
            {

                var result = await _context.TCustomerAddress.FirstAsync(x => x.AddressId == addressId);
                result.TransfereeMobile = mobileNumber;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> SetCustomerMobileVerify(int addressId)
        {
            try
            {
                var result = await _context.TCustomerAddress.FirstAsync(x => x.AddressId == addressId);
                result.MobileVerifed = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<string> GetPhoneCodeWithCustomerAddress(int addressId)
        {
            try
            {
                var result = await _context.TCustomerAddress.Include(o => o.FkCountry).AsNoTracking().FirstAsync(x => x.AddressId == addressId);
                return result.FkCountry.PhoneCode;
            }
            catch (System.Exception)
            {
                return "0";
            }
        }
        public async Task<string> GetMobileNumberWithCustomerAddress(int addressId)
        {
            try
            {
                var result = await _context.TCustomerAddress.AsNoTracking().FirstAsync(x => x.AddressId == addressId);
                return result.TransfereeMobile;
            }
            catch (System.Exception)
            {
                return "0";
            }
        }

        public async Task<bool> CustomerEmailVerify(string email)
        {

            try
            {
                var customer = await _context.TCustomer.FirstOrDefaultAsync(x => x.Email == email);
                if (customer == null)
                {
                    return false;
                }
                else
                {
                    customer.EmailVerifed = true;
                    await _context.SaveChangesAsync();
                    return true;
                }

            }
            catch (System.Exception)
            {

                return false;
            }

        }

        public async Task<ShopGeneralDto> CallRequestGoodsAdd(int customerId, int goodsId, int providerId)
        {
            try
            {

                var shopInfo = await _context.TGoodsProvider.Include(s => s.FkShop).ThenInclude(c => c.FkCountry).FirstOrDefaultAsync(c => c.ProviderId == providerId);
                var shopDetail = new ShopGeneralDto();
                shopDetail.Address = JsonExtensions.JsonGet(shopInfo.FkShop.Address, header) ;
                shopDetail.Phone = "+" + shopInfo.FkShop.FkCountry.PhoneCode + shopInfo.FkShop.Phone;
                shopDetail.Iso = shopInfo.FkShop.FkCountry.Iso2;
                shopDetail.FullName = shopInfo.FkShop.FullName;
                shopDetail.StoreName = shopInfo.FkShop.StoreName;
                shopDetail.VendorUrlid = shopInfo.FkShop.VendorUrlid;
                var beforeData = await _context.TCallRequest.FirstOrDefaultAsync(x => x.FkGoodsId == goodsId && x.FkCustomerId == customerId && x.FkGoodsProviderId == providerId);
                if (beforeData != null)
                {
                    return shopDetail;
                }
                var data = new TCallRequest();
                data.FkGoodsId = goodsId;
                data.FkCustomerId = customerId;
                data.FkGoodsProviderId = providerId;
                data.FkStatusId = (int)CallRequestEnum.New;
                data.RequestDateTime = DateTime.Now;
                await _context.TCallRequest.AddAsync(data);
                await _context.SaveChangesAsync();


                return shopDetail;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}