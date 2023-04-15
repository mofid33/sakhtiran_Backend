using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.CustomerBankCards;
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
    public class CustomerRepository : ICustomerRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }


        public CustomerRepository(MarketPlaceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            this._context = context;
            token = new TokenParseDto(httpContextAccessor);
        }

        public async Task<List<CustomerListDto>> GetCustomerList(CustomerListPaginationDto pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TCustomer
                    .Include(x => x.FkCity)
                    .Include(x => x.FkCountry)
                    .Where(x =>
                    x.CustomerId != (int)CustomerTypeEnum.Unknown&&
                    (pagination.CityId == 0? true : x.FkCityId == pagination.CityId ) &&
                    (pagination.CountryId == 0? true : x.FkCountryId == pagination.CountryId ) &&
                    (pagination.ProvinceId == 0? true : x.FkProvinceId == pagination.ProvinceId ) &&
                    (string.IsNullOrWhiteSpace(pagination.Email) ? true : x.Email.Contains(pagination.Email)) &&
                    (string.IsNullOrWhiteSpace(pagination.Name) ? true : (x.Name.Contains(pagination.Name) || x.Family.Contains(pagination.Name))) &&
                    (string.IsNullOrWhiteSpace(pagination.Phone) ? true : x.MobileNumber.Contains(pagination.Phone))
                    )
                    .OrderByDescending(x => x.CustomerId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.TUser)
                    .Select(x => new CustomerListDto()
                    {
                        CityName = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                        CountryName = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                        CustomerId = x.CustomerId,
                        Email = x.Email,
                        Family = x.Family,
                        Credit = x.Credit,
                        FkCountryId = x.FkCountryId,
                        FkProvinceId = x.FkProvinceId,
                        LastLogin =  Extentions.PersianDateString((DateTime) x.TUser.FirstOrDefault().LastLoginDatetime),
                        MobileNumber = x.MobileNumber,
                        Name = x.Name
                    })
                    .AsNoTracking().ToListAsync();
                }
                else
                {
                    return await _context.TCustomer
                    .Include(x => x.FkCity)
                    .Include(x => x.FkCountry)
                    .Include(x => x.TOrder).ThenInclude(t=>t.TOrderItem)
                    .Where(x =>
                    x.CustomerId != (int)CustomerTypeEnum.Unknown&&
                    x.TOrder.Any(t=>t.TOrderItem.Any(i=>i.FkShopId == token.Id))&&
                    (pagination.CityId == 0? true : x.FkCityId == pagination.CityId ) &&
                    (pagination.CountryId == 0? true : x.FkCountryId == pagination.CountryId ) &&
                    (pagination.ProvinceId == 0? true : x.FkProvinceId == pagination.ProvinceId ) &&
                    (string.IsNullOrWhiteSpace(pagination.Email) ? true : x.Email.Contains(pagination.Email)) &&
                    (string.IsNullOrWhiteSpace(pagination.Name) ? true : (x.Name.Contains(pagination.Name) || x.Family.Contains(pagination.Name))) &&
                    (string.IsNullOrWhiteSpace(pagination.Phone) ? true : x.MobileNumber.Contains(pagination.Phone))
                    )
                    .OrderByDescending(x => x.CustomerId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .Include(x => x.TUser)
                    .Select(x => new CustomerListDto()
                    {
                        CityName = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                        CountryName = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                        CustomerId = x.CustomerId,
                        Email = x.Email,
                        Family = x.Family,
                        FkCountryId = x.FkCountryId,
                        FkProvinceId = x.FkProvinceId,
                        LastLogin =  Extentions.PersianDateString((DateTime) x.TUser.FirstOrDefault().LastLoginDatetime ),
                        MobileNumber = x.MobileNumber,
                        Name = x.Name
                    })
                    .AsNoTracking().ToListAsync();
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetCustomerListCount(CustomerListPaginationDto pagination)
        {
            try
            {
                if (token.Rule == UserGroupEnum.Admin)
                {
                    return await _context.TCustomer
                    .Include(x => x.FkCity)
                    .Include(x => x.FkCountry)
                    .AsNoTracking()
                    .CountAsync(x =>
                    x.CustomerId != (int)CustomerTypeEnum.Unknown &&
                    (pagination.CityId == 0? true : x.FkCityId == pagination.CityId ) &&
                    (pagination.CountryId == 0? true : x.FkCountryId == pagination.CountryId ) &&
                    (pagination.ProvinceId == 0? true : x.FkProvinceId == pagination.ProvinceId ) &&
                    (string.IsNullOrWhiteSpace(pagination.Email) ? true : x.Email.Contains(pagination.Email)) &&
                    (string.IsNullOrWhiteSpace(pagination.Name) ? true : (x.Name.Contains(pagination.Name) || x.Family.Contains(pagination.Name))) &&
                    (string.IsNullOrWhiteSpace(pagination.Phone) ? true : x.MobileNumber.Contains(pagination.Phone))
                    );
                }
               else
               {
                    return await _context.TCustomer
                    .Include(x => x.FkCity)
                    .Include(x => x.FkCountry)
                    .Include(x => x.TOrder).ThenInclude(t=>t.TOrderItem)
                    .AsNoTracking()
                    .CountAsync(x =>
                    x.CustomerId != (int)CustomerTypeEnum.Unknown &&
                    x.TOrder.Any(t=>t.TOrderItem.Any(i=>i.FkShopId == token.Id))&&
                    (pagination.CityId == 0? true : x.FkCityId == pagination.CityId ) &&
                    (pagination.CountryId == 0? true : x.FkCountryId == pagination.CountryId ) &&
                    (pagination.ProvinceId == 0? true : x.FkProvinceId == pagination.ProvinceId ) &&
                    (string.IsNullOrWhiteSpace(pagination.Email) ? true : x.Email.Contains(pagination.Email)) &&
                    (string.IsNullOrWhiteSpace(pagination.Name) ? true : (x.Name.Contains(pagination.Name) || x.Family.Contains(pagination.Name))) &&
                    (string.IsNullOrWhiteSpace(pagination.Phone) ? true : x.MobileNumber.Contains(pagination.Phone))
                    );
               }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<CustomerGeneralDetailDto> GetCustomerGeneralDetail(int customerId)
        {
            try
            {
                return await _context.TCustomer
                .Include(x => x.TUser)
                .Include(x => x.FkCity)
                .Include(x => x.FkProvince)
                .Include(x => x.FkCountry)
                .Select(x => new CustomerGeneralDetailDto()
                {
                    BirthDate = x.BirthDate,
                    CustomerId = x.CustomerId,
                    Email = x.Email,
                    Credit = x.Credit,
                    UserId = x.TUser.FirstOrDefault().UserId,
                    EmailVerifed = (bool)x.EmailVerifed,
                    Family = x.Family,
                    FkCountryId = x.FkCountryId,
                    FkProvinceId = x.FkProvinceId,
                    FkCityId = x.FkCityId,
                    LastLogin =  Extentions.PersianDateString((DateTime)x.TUser.FirstOrDefault().LastLoginDatetime),
                    MobileNumber = x.MobileNumber,
                    MobileVerifed = (bool) x.MobileVerifed,
                    Name = x.Name,
                    Iso = x.FkCountry.Iso2,
                    PhoneCode = x.FkCountry.PhoneCode,
                    NationalCode = x.NationalCode,
                    RegisteryDate =  Extentions.PersianDateString((DateTime)x.RegisteryDate),
                    CityName = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                    ProvinceName = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language),
                    CountryName = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                    RefundPreference = x.RefundPreference == 1 ? "بازگشت به کیف پول" : ( x.RefundPreference == 2 ? "بازگشت به کارت بانکی" : "") ,
                    RefundPreferenceId = (int) x.RefundPreference,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<CustomerAddressDto>> GetCustomerAddress(int customerId)
        {
            try
            {
                return await _context.TCustomerAddress.Where(x => x.FkCustomerId == customerId)
                .Include(x => x.FkCity)
                .Include(x => x.FkCountry)
                .Include(x => x.FkProvince)
                .Select(x => new CustomerAddressDto()
                {
                    Address = x.Address,
                    AddressId = x.AddressId,
                    CityName = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                    ProvinceName = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language),
                    CountryName = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                    FkCityId = x.FkCityId,
                    FkCountryId = x.FkCountryId,
                    FkProvinceId = (int) x.FkProvinceId,
                    LocationX = x.LocationX,
                    LocationY = x.LocationY,
                    PostalCode = x.PostalCode,
                    TransfereeMobile = x.TransfereeMobile,
                    TransfereeFamily = x.TransfereeFamily,
                    TransfereeName = x.TransfereeName
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<CustomerWishListViewDateDto>> GetCustomerWishList(CustomerPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsLike.Where(x => x.FkCustomerId == pagination.CustomerId)
                .OrderByDescending(x => x.LikeId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkGoods).ThenInclude(t => t.FkCategory)
                .Include(x => x.FkGoods).ThenInclude(t => t.FkBrand)
                .Include(x => x.FkGoods).ThenInclude(t => t.TGoodsProvider).ThenInclude(i => i.FkShop)
                .Select(x => new CustomerWishListViewDateDto()
                {
                    Brand = JsonExtensions.JsonValue(x.FkGoods.FkBrand.BrandTitle, header.Language),
                    Category = JsonExtensions.JsonValue(x.FkGoods.FkCategory.CategoryTitle, header.Language),
                    Title = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    Date =  Extentions.PersianDateString((DateTime)x.LikeDate),
                    GoodsCode = x.FkGoods.GoodsCode,
                    GoodsId = x.FkGoodsId,
                    Image = x.FkGoods.ImageUrl,
                    SerialNumber = x.FkGoods.SerialNumber,
                    Shop = x.FkGoods.TGoodsProvider.Select(t => new Dtos.Shop.ShopFormDto()
                    {
                        ShopId = t.FkShop.ShopId,
                        ShopTitle = t.FkShop.StoreName,
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetCustomerWishListCount(CustomerPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsLike.AsNoTracking().CountAsync(x => x.FkCustomerId == pagination.CustomerId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<CustomerWishListViewDateDto>> GetCustomerViewList(CustomerPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsView.Where(x => x.FkCustomerId == pagination.CustomerId)
                .OrderByDescending(x => x.ViewId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkGoods).ThenInclude(t => t.FkCategory)
                .Include(x => x.FkGoods).ThenInclude(t => t.FkBrand)
                .Include(x => x.FkGoods).ThenInclude(t => t.TGoodsProvider).ThenInclude(i => i.FkShop)
                .Select(x => new CustomerWishListViewDateDto()
                {
                    Brand = JsonExtensions.JsonValue(x.FkGoods.FkBrand.BrandTitle, header.Language),
                    Category = JsonExtensions.JsonValue(x.FkGoods.FkCategory.CategoryTitle, header.Language),
                    Title = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    Date =  Extentions.PersianDateString((DateTime)x.ViewDate),
                    GoodsCode = x.FkGoods.GoodsCode,
                    GoodsId = x.FkGoodsId,
                    Image = x.FkGoods.ImageUrl,
                    SerialNumber = x.FkGoods.SerialNumber,
                    Shop = x.FkGoods.TGoodsProvider.Select(t => new Dtos.Shop.ShopFormDto()
                    {
                        ShopId = t.FkShop.ShopId,
                        ShopTitle = t.FkShop.StoreName,
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetCustomerViewListCount(CustomerPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsView.AsNoTracking().CountAsync(x => x.FkCustomerId == pagination.CustomerId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<CustomerCommentDto>> GetCustomerCommentList(CustomerPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsComment.Where(x => x.FkCustomerId == pagination.CustomerId)
                .OrderByDescending(x => x.CommentId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkGoods).ThenInclude(t => t.TGoodsProvider).ThenInclude(i => i.FkShop)
                .Include(x => x.FkCustomer)
                .Select(x => new CustomerCommentDto()
                {
                    CommentDate =  Extentions.PersianDateString((DateTime)x.CommentDate),
                    CommentId = x.CommentId,
                    CommentText = x.CommentText,
                    FkGoodsId = x.FkGoodsId,
                    GoodsCode = x.FkGoods.GoodsCode,
                    CustomerName = x.FkCustomer.Name,
                    CustomerFamily = x.FkCustomer.Family,
                    Image = x.FkGoods.ImageUrl,
                    IsAccepted = x.IsAccepted,
                    Points = x.TGoodsCommentPoints.Select(t => new Dtos.Survey.GoodsCommentPointsDto()
                    {
                        PointId = t.PointId,
                        PointText = t.PointText,
                        PointType = t.PointType
                    }).ToList(),
                    ReviewPoint = x.ReviewPoint,
                    SerialNumber = x.FkGoods.SerialNumber,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    Shop = x.FkGoods.TGoodsProvider.Select(t => new Dtos.Shop.ShopFormDto()
                    {
                        ShopId = t.FkShop.ShopId,
                        ShopTitle = t.FkShop.StoreName,
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetCustomerCommentListCount(CustomerPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsComment.AsNoTracking().CountAsync(x => x.FkCustomerId == pagination.CustomerId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<decimal> GetAvailableBalance(int customerId)
        {
            try
            {
                var rate = (decimal) 1.00;
                if ( header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.0 : (decimal) currency.RatesAgainstOneDollar;
                }
                var data = await _context.TCustomer.AsNoTracking().Select(x => new { x.CustomerId, x.Credit }).FirstOrDefaultAsync(x => x.CustomerId == customerId);
                return decimal.Round(data.Credit / rate, 2, MidpointRounding.AwayFromZero) ;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<CustomerBalanceDto>> GetCustomerBalance(CustomerPaginationDto pagination)
        {
            try
            {
                var rate = (decimal) 1.00;
                if (header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal) 1.0 : (decimal) currency.RatesAgainstOneDollar;
                }
                var customerUser = await _context.TUser.FirstOrDefaultAsync(y => y.FkCustumerId == pagination.CustomerId);
                
                return await _context.TUserTransaction.Where(x =>
                (x.FkUserId == customerUser.UserId)
                )
                .OrderByDescending(x => x.TransactionId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkTransactionType)
                .Include(x => x.FkApprovalStatus)
                .Select(x => new CustomerBalanceDto()
                {
                Balance = 
                    Extentions.DecimalRound(
                        Extentions.CalculateBalancePrice(_context.TUserTransaction
                .Where(t =>  (t.FkUserId == customerUser.UserId) && (t.FkTransactionTypeId == (int)TransactionTypeEnum.CashPayment
                || t.FkTransactionTypeId == (int)TransactionTypeEnum.Refund || t.FkTransactionTypeId == (int)TransactionTypeEnum.GiftCard)
                && t.TransactionId <= x.TransactionId)
                .Sum(t => t.Amount) , _context.TUserTransaction
                .Where(t => (t.FkUserId == customerUser.UserId) && (t.FkTransactionTypeId == (int)TransactionTypeEnum.Purchased || t.FkTransactionTypeId == (int)TransactionTypeEnum.Withdraw)
                && t.TransactionId <= x.TransactionId).Sum(t => t.Amount))  / rate),
                    Amount = decimal.Round(x.Amount / rate, 2, MidpointRounding.AwayFromZero) ,
                    FkApprovalStatusId = x.FkApprovalStatusId,
                    ApprovalStatusTitle = JsonExtensions.JsonValue(x.FkApprovalStatus.StatusTitle, header.Language),
                    Comment = x.Comment,
                    FkTransactionTypeId = x.FkTransactionTypeId,
                    TransactionDateTime = Extentions.PersianDateString( (DateTime) (x.TransactionDateTime)) ,
                    TransactionId = x.TransactionId,
                    TransactionTypeTitle = JsonExtensions.JsonValue(x.FkTransactionType.TransactionTypeTitle, header.Language)
                })
                .AsNoTracking().ToListAsync();

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetCustomerBalanceCount(CustomerPaginationDto pagination)
        {
            try
            {
                return await _context.TUserTransaction.AsNoTracking().CountAsync(x =>
                    (x.FkUserId == _context.TUser.FirstOrDefault(y => y.FkCustumerId == pagination.CustomerId).UserId)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        // همه ی نظرات مشتری
        public async Task<List<CustomerCommentDto>> GetAllCustomerCommentList(CustomerCommentPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsComment
                .Include(c => c.FkGoods)
                .ThenInclude(d => d.TGoodsProvider)
                .Where(x =>
                (pagination.CategoryId == 0 ? true : x.FkGoods.FkCategoryId == pagination.CategoryId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.Status == null ? true : x.IsAccepted == (pagination.Status == 1 ? true : false)) &&
                (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                (token.Rule != UserGroupEnum.Seller ? true : x.IsAccepted == true) &&
                (pagination.VendorId == 0 ? true : x.FkGoods.TGoodsProvider.Any(p => p.FkShopId == pagination.VendorId))
                &&
                (pagination.FromDate == (DateTime?)null ? true : (x.CommentDate >= pagination.FromDate))
                && (pagination.ToDate == (DateTime?)null ? true : (x.CommentDate <= pagination.ToDate))
                )
                .OrderByDescending(x => x.CommentId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkGoods).ThenInclude(t => t.TGoodsProvider).ThenInclude(i => i.FkShop)
                .Include(x => x.FkCustomer)
                .Select(x => new CustomerCommentDto()
                {
                    CommentDate =  Extentions.PersianDateString((DateTime)x.CommentDate),
                    CommentId = x.CommentId,
                    CommentText = x.CommentText,
                    FkGoodsId = x.FkGoodsId,
                    GoodsCode = x.FkGoods.GoodsCode,
                    Image = x.FkGoods.ImageUrl,
                    CustomerName = x.FkCustomer.Name,
                    CustomerFamily = x.FkCustomer.Family,
                    IsAccepted = x.IsAccepted,
                    Points = x.TGoodsCommentPoints.Select(t => new Dtos.Survey.GoodsCommentPointsDto()
                    {
                        PointId = t.PointId,
                        PointText = t.PointText,
                        PointType = t.PointType
                    }).ToList(),
                    ReviewPoint = x.ReviewPoint,
                    SerialNumber = x.FkGoods.SerialNumber,
                    GoodsTitle = JsonExtensions.JsonValue(x.FkGoods.Title, header.Language),
                    Shop = x.FkGoods.TGoodsProvider.Select(t => new Dtos.Shop.ShopFormDto()
                    {
                        ShopId = t.FkShop.ShopId,
                        ShopTitle = t.FkShop.StoreName,
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetAllCustomerCommentListCount(CustomerCommentPaginationDto pagination)
        {
            try
            {
                return await _context.TGoodsComment.AsNoTracking()
                .CountAsync(x =>
                (pagination.CategoryId == 0 ? true : x.FkGoods.FkCategoryId == pagination.CategoryId) &&
                (pagination.GoodsId == 0 ? true : x.FkGoodsId == pagination.GoodsId) &&
                (pagination.Status == null ? true : x.IsAccepted == (pagination.Status == 1 ? true : false)) &&
                (pagination.CustomerId == 0 ? true : x.FkCustomerId == pagination.CustomerId) &&
                (token.Rule != UserGroupEnum.Seller ? true : x.IsAccepted == true) &&
                (pagination.VendorId == 0 ? true : x.FkGoods.TGoodsProvider.Any(p => p.FkShopId == pagination.VendorId))
                &&
                (pagination.FromDate == (DateTime?)null ? true : (x.CommentDate >= pagination.FromDate))
                && (pagination.ToDate == (DateTime?)null ? true : (x.CommentDate <= pagination.ToDate))
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<string> GetCustomerUserName(int customerId)
        {
            try
            {
                var data = await _context.TUser.Select(x => new { x.FkCustumerId, x.UserName }).AsNoTracking().FirstOrDefaultAsync(x => x.FkCustumerId == customerId);
                return data.UserName;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        // ثبت مشتری جدید
        public async Task<TCustomer> RegisterCustomer(TCustomer customer)
        {
            try
            {
                await _context.TCustomer.AddAsync(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteCustomer(int customerId)
        {
            try
            {
                var customer = await _context.TCustomer.FirstAsync(x=>x.CustomerId == customerId);
                _context.TCustomer.Remove(customer);
                await _context.SaveChangesAsync();
                return true;      
            }
            catch (System.Exception)
            {
                
                return false;
            }
    
        }

        public async Task<TCustomer> ExistCustomer(int customerId)
        {
            try
            {
                var data = await _context.TCustomer.Include(c=>c.TUser).FirstOrDefaultAsync(x => x.CustomerId == customerId);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }   
        }

        public async Task<TCustomer> UpdateCustomer(TCustomer customer)
        {
            try
            {
                var data = await _context.TCustomer.FirstAsync(x => x.CustomerId == customer.CustomerId);
                _context.Entry(data).CurrentValues.SetValues(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (System.Exception)
            {
                return null;
            }        
        }


         public async Task<RepRes<TCustomer>> CustomerDelete(int customerId)
        {
            try
            {
                var canDelete = await _context.TOrder.AsNoTracking().AnyAsync(x => x.FkCustomerId == customerId);
                if (canDelete)
                {
                    return new RepRes<TCustomer>(Message.CustomerCantDelete, false, null);
                }
                var customerDiscount = await _context.TDiscountCustomers.Where(x => x.FkCustomerId == customerId).ToListAsync();
                var customerAddress = await _context.TCustomerAddress.Where(x => x.FkCustomerId == customerId).ToListAsync();
                var customerComment = await _context.TGoodsComment.Where(x => x.FkCustomerId == customerId).ToListAsync();
                var customerLike = await _context.TGoodsLike.Where(x => x.FkCustomerId == customerId).ToListAsync();
                var customerQueAns = await _context.TGoodsQueAns.Where(x => x.FkCustomerId == customerId).ToListAsync();
                var customerSurvey = await _context.TGoodsSurveyAnswers.Where(x => x.FkCustomerId == customerId).ToListAsync();
                var customerView = await _context.TGoodsView.Where(x => x.FkCustomerId == customerId).ToListAsync();
                var customerUser = await _context.TUser.FirstAsync(x => x.FkCustumerId == customerId);
                var customer = await _context.TCustomer.FindAsync(customerId);
                _context.TDiscountCustomers.RemoveRange(customerDiscount);
                _context.TCustomerAddress.RemoveRange(customerAddress);
                _context.TGoodsComment.RemoveRange(customerComment);
                _context.TGoodsLike.RemoveRange(customerLike);
                _context.TGoodsQueAns.RemoveRange(customerQueAns);
                _context.TGoodsSurveyAnswers.RemoveRange(customerSurvey);
                _context.TGoodsView.RemoveRange(customerView);
                _context.TCustomer.Remove(customer);
                _context.TUser.Remove(customerUser);
                await _context.SaveChangesAsync();
                return new RepRes<TCustomer>(Message.Successfull, true, customer);
            }
            catch (System.Exception)
            {
                return new RepRes<TCustomer>(Message.CustomerCantDelete, false, null);
            }
        }

        public async Task<int> CustomerRefundPreference(int customerId)
        {
            try
            {
                var data = await _context.TCustomer.AsNoTracking().FirstAsync(x => x.CustomerId == customerId);
                return (int) data.RefundPreference;
            }
            catch (System.Exception)
            {
                return 0;
            }   
        }

        public async Task<bool> SetCustomerRefundPreference(int customerId , int refundPreference)
        {
            try
            {
                var data = await _context.TCustomer.FirstAsync(x => x.CustomerId == customerId);
                 data.RefundPreference = (int) refundPreference;
                 await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }           
        }

        public async Task<bool> SaveCustomerBankCard(TCustomerBankCard customerBankCard)
        {
            try
            {
                await _context.TCustomerBankCard.AddAsync(customerBankCard);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false ;
            }
        }
    
        public async Task<List<CustomerBankCardGetDto>> GetCustomerBankCards(int customerId)
        {
            try
            {
                var result = await _context.TCustomerBankCard.AsNoTracking()
                                .Include(i => i.FkCustumer)
                                .Include(i => i.FkPaymentMethod)
                                .Where(x => x.FkCustumerId == customerId)
                                .Select(x => new CustomerBankCardGetDto() {
                                    BankCardId = x.BankCardId,
                                    BankCardMonth = x.BankCardMonth,
                                    BankCardName = x.BankCardName,
                                    BankCardNumber = "**** **** **** " + x.BankCardNumber.Substring(x.BankCardNumber.Length - 4),
                                    BankCardYear = x.BankCardYear,
                                    FkCustumerId = x.FkCustumerId,
                                    FkPaymentMethodId = x.FkPaymentMethodId,
                                    PaymentMethodImageName = x.FkPaymentMethod.MethodImageUrl,
                                    ZipCode = x.ZipCode,
                                })
                                .ToListAsync();
                return result;
            }
            catch (System.Exception)
            {
                return null ;
            }
        }

        public async Task<bool> RemoveCustomerBankCard(int bankCardId)
        {
            try
            {
                var bankCard = await _context.TCustomerBankCard.FindAsync(bankCardId);
                _context.TCustomerBankCard.Remove(bankCard);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false ;
            }
        }

        
        public async Task<bool> VerifyCustomerMobileNumber(int customerId)
        {
            try
            {
                var customer = await _context.TCustomer.FirstAsync(x=>x.CustomerId == customerId);
                customer.MobileVerifed = true;
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