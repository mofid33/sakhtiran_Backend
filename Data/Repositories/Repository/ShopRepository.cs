using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class ShopRepository : IShopRepository
    {
        public MarketPlaceDbContext _context { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMapper _mapper { get; set; }
        public IAccountingRepository _accountingRepository { get; }
        public INotificationService _notificationService { get; }

        public ShopRepository(MarketPlaceDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        INotificationService notificationService,
        IAccountingRepository accountingRepository)
        {
            this._context = context;
            header = new HeaderParseDto(httpContextAccessor);
            this._mapper = mapper;
            token = new TokenParseDto(httpContextAccessor);
            _accountingRepository = accountingRepository;
            _notificationService = notificationService;
        }

        public async Task<List<ShopListGetDto>> GetShopList(ShopListPaginationDto pagination)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }

                if (!string.IsNullOrWhiteSpace(pagination.PhoneNumber))
                {

                    pagination.PhoneNumber = pagination.PhoneNumber.Replace(" ", "");

                }

                return await _context.TShop
                .Include(x => x.TShopCategory).ThenInclude(t => t.FkCategory)
                .Where(x =>
                (pagination.CategoryId != 0 ? x.TShopCategory.Any(t => t.FkCategory.CategoryId == pagination.CategoryId) : true) &&
                (string.IsNullOrWhiteSpace(pagination.Email) ? true : x.Email.Contains(pagination.Email)) &&
                (string.IsNullOrWhiteSpace(pagination.StoreName) ? true : x.StoreName.Contains(pagination.StoreName)) &&
                (string.IsNullOrWhiteSpace(pagination.PhoneNumber) ? true : x.Phone.Contains(pagination.PhoneNumber)) &&
                (pagination.PlanId == 0 ? true : x.FkPlanId == pagination.PlanId) &&

                (pagination.Status == null ? true : x.FkStatusId == pagination.Status) &&
                (pagination.FromDate == (DateTime?)null ? true : x.RegisteryDateTime >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.RegisteryDateTime <= pagination.ToDate)
                )
                .OrderByDescending(x => x.ShopId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkPlan)
                .Include(x => x.FkStatus)
                .Select(x => new ShopListGetDto()
                {
                    Category = x.TShopCategory.Select(t => new Dtos.Category.CategoryFormGetDto()
                    {
                        CategoryId = t.FkCategory.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language)
                    }).ToList(),
                    CategoryTitle = string.Join(",", x.TShopCategory.Select(t => JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language))),
                    Email = x.Email,
                    FkPlanId = x.FkPlanId,
                    FkStatusId = x.FkStatusId,
                    Phone = x.Phone,
                    PlanTitle = JsonExtensions.JsonValue(x.FkPlan.PlanTitle, header.Language),
                    RegisteryDateTime = Extentions.PersianDateString(x.RegisteryDateTime),
                    ShopId = x.ShopId,
                    StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                    StoreName = x.StoreName,
                    Credit = decimal.Round(x.Credit  / rate, 2, MidpointRounding.AwayFromZero)
                }).AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetShopListCount(ShopListPaginationDto pagination)
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(pagination.PhoneNumber))
                {

                    pagination.PhoneNumber = pagination.PhoneNumber.Replace(" ", "");

                }

                return await _context.TShop
                .Include(x => x.TShopCategory).ThenInclude(t => t.FkCategory)
                .AsNoTracking()
                .CountAsync(x =>
                (pagination.CategoryId != 0 ? x.TShopCategory.Any(t => t.FkCategory.CategoryId == pagination.CategoryId) : true) &&
                (string.IsNullOrWhiteSpace(pagination.Email) ? true : x.Email.Contains(pagination.Email)) &&
                (string.IsNullOrWhiteSpace(pagination.StoreName) ? true : x.StoreName.Contains(pagination.StoreName)) &&
                (string.IsNullOrWhiteSpace(pagination.PhoneNumber) ? true : x.Phone.Contains(pagination.PhoneNumber)) &&
                (pagination.PlanId == 0 ? true : x.FkPlanId == pagination.PlanId) &&
                (pagination.Status == null ? true : x.FkStatusId == pagination.Status) &&
                (pagination.FromDate == (DateTime?)null ? true : x.RegisteryDateTime >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.RegisteryDateTime <= pagination.ToDate)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<ShopGeneralDto> GetShopGeneralDetail(int shopId, string userName = null)
        {
            try
            {
                return await _context.TShop
                .Include(x => x.TShopCategory).ThenInclude(t => t.FkCategory)
                .Include(x => x.FkStatus)
                .Include(x => x.TUser)
                .Select(x => new ShopGeneralDto()
                {
                    Address = JsonExtensions.JsonValue(x.Address, header.Language),
                    CompanyName = x.CompanyName,
                    Email = x.Email,
                    UserId = x.TUser.FirstOrDefault() == null ? new Guid() : x.TUser.FirstOrDefault().UserId,
                    UserName = x.TUser.FirstOrDefault() == null ? "" : x.TUser.FirstOrDefault().UserName,
                    FkCityId = (int)x.FkCityId,
                    FkCountryId = x.FkCountryId,
                    FkProvinceId = (int)x.FkProvinceId,
                    FkPersonId = x.FkPersonId,
                    FkStatusId = x.FkStatusId,
                    FullName = x.FullName,
                    LocationX = x.LocationX,
                    LocationY = x.LocationY,
                    Phone = x.Phone,
                    ShopId = x.ShopId,
                    StoreName = x.StoreName,
                    VendorUrlid = x.VendorUrlid,
                    ShopShippingCode = x.ShopShippingCode,
                    AutoAccountRecharge = x.AutoAccountRecharge,
                    RegisteryDateTime = Extentions.PersianDateString(x.RegisteryDateTime),
                    MaxSliderForShopWebPage = x.MaxSliderForShopWebPage != null ? x.MaxSliderForShopWebPage : (short)_context.TSetting.FirstOrDefault().MaxSliderForShopWebPage,
                    StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language)
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => shopId != 0 ? x.ShopId == shopId : x.UserName == userName);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<ShopGeneralDto>> EditShopGeneralDetail(ShopGeneralDto shopDto)
        {
            try
            {
                var data = await _context.TShop.Include(x => x.TUser).FirstOrDefaultAsync(x => x.ShopId == shopDto.ShopId);
                if (shopDto.FkPersonId == (int)PersontypeEnum.Natural)
                {
                    shopDto.CompanyName = null;
                }

                if (token.Rule == UserGroupEnum.Admin)
                {
                    data.FkStatusId = shopDto.FkStatusId;
                    if (shopDto.FkStatusId == (int)ShopStatusEnum.Disabled)
                    {
                        data.TUser.FirstOrDefault().Active = false;
                    }
                    else
                    {
                        data.TUser.FirstOrDefault().Active = true;
                    }
                }
                else
                {
                    if (data.FkStatusId == (int)ShopStatusEnum.InitialRegistration)
                    {
                        data.FkStatusId = (int)ShopStatusEnum.Waiting;
                    }
                }

                if (await _context.TShop.AnyAsync(x => x.VendorUrlid == shopDto.VendorUrlid && x.ShopId != shopDto.ShopId && !string.IsNullOrWhiteSpace(x.VendorUrlid)))
                {
                    return new RepRes<ShopGeneralDto>(Message.ShopUrlIdExist, false, null);
                }
                if (await _context.TUser.AnyAsync(x => x.UserName == shopDto.Email && x.FkShopId != shopDto.ShopId && x.FkShopId != null))
                {
                    return new RepRes<ShopGeneralDto>(Message.UserNameDupplicate, false, null);
                }
                if (await _context.TShop.AnyAsync(x => x.Email == shopDto.Email && x.ShopId != shopDto.ShopId))
                {
                    return new RepRes<ShopGeneralDto>(Message.UserNameDupplicate, false, null);
                }
                if (token.Rule == UserGroupEnum.Admin)
                {
                    if (shopDto.MaxSliderForShopWebPage == null)
                    {
                        data.MaxSliderForShopWebPage = (short)await _context.TSetting.Select(x => x.MaxSliderForShopWebPage).FirstOrDefaultAsync();
                    }
                    else
                    {
                        data.MaxSliderForShopWebPage = shopDto.MaxSliderForShopWebPage;
                    }
                }

                data.AutoAccountRecharge = shopDto.AutoAccountRecharge;
                data.Address = JsonExtensions.JsonEdit(shopDto.Address, data.Address, header);
                data.Email = shopDto.Email;
                data.CompanyName = shopDto.CompanyName;
                data.TUser.FirstOrDefault().UserName = shopDto.Email;
                data.FkCityId = shopDto.FkCityId;
                data.FkCountryId = shopDto.FkCountryId;
                data.FkProvinceId = shopDto.FkProvinceId;
                data.FkPersonId = shopDto.FkPersonId;
                data.FullName = shopDto.FullName;
                data.LocationX = shopDto.LocationX;
                data.LocationY = shopDto.LocationY;
                data.Phone = shopDto.Phone;
                data.StoreName = shopDto.StoreName;
                data.VendorUrlid = shopDto.VendorUrlid;
                data.ShopShippingCode = shopDto.ShopShippingCode;
                await _context.SaveChangesAsync();
                return new RepRes<ShopGeneralDto>(Message.Successfull, true, shopDto);

            }
            catch (System.Exception)
            {
                return new RepRes<ShopGeneralDto>(Message.ShopEditing, false, null);
            }
        }

        public async Task<decimal> GetAvailableBalance(int shopId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var data = await _context.TShop.AsNoTracking().Select(x => new { x.ShopId, x.Credit }).FirstOrDefaultAsync(x => x.ShopId == shopId);
                return decimal.Round(data.Credit  / rate, 2, MidpointRounding.AwayFromZero);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<ShopBalanceDto>> GetShopBalance(ShopBalancePagination pagination)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TUserTransaction.Where(x =>
                (x.FkUserId == _context.TUser.FirstOrDefault(u => u.FkShopId == pagination.ShopId).UserId) &&
                (pagination.Type == 0 ? true : x.FkTransactionTypeId == pagination.Type) &&
                (pagination.FromDate == (DateTime?)null ? true : x.TransactionDateTime >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.TransactionDateTime <= pagination.ToDate)
                )
                .OrderByDescending(x => x.TransactionId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkTransactionType)
                .Include(x => x.FkApprovalStatus)
                .Select(x => new ShopBalanceDto()
                {
                    Amount = decimal.Round(x.Amount  / rate, 2, MidpointRounding.AwayFromZero),
                    FkApprovalStatusId = x.FkApprovalStatusId,
                    ApprovalStatusTitle = JsonExtensions.JsonValue(x.FkApprovalStatus.StatusTitle, header.Language),
                    Comment = x.Comment,
                    FkTransactionTypeId = x.FkTransactionTypeId,
                    TransactionDateTime = Extentions.PersianDateString(x.TransactionDateTime),
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

        public async Task<int> GetShopBalanceCount(ShopBalancePagination pagination)
        {
            try
            {
                return await _context.TUserTransaction.AsNoTracking().CountAsync(x =>
                (x.FkUserId == _context.TUser.FirstOrDefault(u => u.FkShopId == pagination.ShopId).UserId) &&
                (pagination.Type == 0 ? true : x.FkTransactionTypeId == pagination.Type) &&
                (pagination.FromDate == (DateTime?)null ? true : x.TransactionDateTime >= pagination.FromDate) &&
                (pagination.ToDate == (DateTime?)null ? true : x.TransactionDateTime <= pagination.ToDate)
                );
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<RepRes<bool>> EditShopDescription(ShopDescriptionDto shopDto)
        {
            try
            {
                var data = await _context.TShop.FindAsync(shopDto.ShopId);
                data.AboutShop = shopDto.Description;
                if (data.FkStatusId == (int)ShopStatusEnum.InitialRegistration)
                {
                    data.FkStatusId = (int)ShopStatusEnum.Waiting;
                }
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.ShopEditing, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.ShopEditing, false, false);
            }
        }

        public async Task<RepRes<bool>> EditShopTermsAndConditions(ShopDescriptionDto shopDto)
        {
            try
            {
                var data = await _context.TShop.FindAsync(shopDto.ShopId);
                data.TermCondition = JsonExtensions.JsonEdit(shopDto.Description, data.TermCondition, header);
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.ShopEditing, false, false);
            }
        }

        public async Task<ShopDescriptionDto> GetShopDescription(int shopId)
        {
            try
            {
                return await _context.TShop.Select(x => new ShopDescriptionDto()
                {
                    Description = x.AboutShop,
                    ShopId = x.ShopId
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == shopId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<ShopDescriptionDto> GetShopTermsAndConditions(int shopId)
        {
            try
            {
                var data = await _context.TShop.Select(x => new ShopDescriptionDto()
                {
                    Description = x.TermCondition,
                    ShopId = x.ShopId
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == shopId);

                data.Description = JsonExtensions.JsonGet(data.Description, header);
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ShopFilesGetDto>> GetShopDocument(int shopId)
        {
            try
            {
                return await _context.TShopFiles
                .Include(x => x.FkDocumentType).ThenInclude(t => t.FkGroupdNavigation)
                .Where(x => x.FkShopId == shopId && x.FkDocumentType.FkGroupd == (int)DocumentGroupEnum.Identity)
                .Include(x => x.FkDocumentType).ThenInclude(t => t.FkPerson)
                .Select(x => new ShopFilesGetDto()
                {
                    DocumentTypeTitle = JsonExtensions.JsonValue(x.FkDocumentType.DocumentTitle, header.Language),
                    FileId = x.FileId,
                    FileUrl = x.FileUrl,
                    FkDocumentTypeId = x.FkDocumentTypeId,
                    FkGroupd = x.FkDocumentType.FkGroupd,
                    FkPersonId = x.FkDocumentType.FkPersonId,
                    FkShopId = x.FkShopId,
                    GroupTitle = JsonExtensions.JsonValue(x.FkDocumentType.FkGroupdNavigation.DocumentTypeTitle, header.Language),
                    PersonTitle = JsonExtensions.JsonValue(x.FkDocumentType.FkPerson.PersonTypeTitle, header.Language)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TShop>> EditShopProfile(string profile, string logo, int shopId, bool IsLogoNull, bool IsProfileNull)
        {
            try
            {
                var data = await _context.TShop.FindAsync(shopId);
                var newShop = new TShop();
                newShop.LogoImage = data.LogoImage;
                newShop.ProfileImage = data.ProfileImage;
                newShop.ShopId = data.ShopId;

                if (!string.IsNullOrWhiteSpace(profile))
                {
                    data.ProfileImage = profile;
                }
                if (!string.IsNullOrWhiteSpace(logo))
                {
                    data.LogoImage = logo;
                }

                if (IsLogoNull)
                {
                    data.LogoImage = null;
                }


                if (IsProfileNull)
                {
                    data.ProfileImage = null;
                }


                await _context.SaveChangesAsync();

                return new RepRes<TShop>(Message.Successfull, true, newShop);

            }
            catch (System.Exception)
            {
                return new RepRes<TShop>(Message.ShopEditing, false, null);
            }
        }

        public async Task<ShopProfileDto> GetShopProfile(int shopId)
        {
            try
            {
                return await _context.TShop
                .Select(x => new ShopProfileDto()
                {
                    Logo = x.LogoImage,
                    Profile = x.ProfileImage,
                    ShopId = x.ShopId
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == shopId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<ShopSetting> GetShopSetting(int shopId)
        {
            try
            {
                return await _context.TShop
                .Select(x => new ShopSetting()
                {
                    ShippingBaseWeight = x.ShippingBaseWeight,
                    ShippingPermission = x.ShippingPermission,
                    ShippingPossibilities = x.ShippingPossibilities,
                    ShopId = x.ShopId
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == shopId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<bool>> EditShopSetting(ShopSetting shopDto)
        {
            try
            {
                var data = await _context.TShop.FindAsync(shopDto.ShopId);
                data.ShippingBaseWeight = shopDto.ShippingBaseWeight;
                if (token.Rule == UserGroupEnum.Admin)
                {
                    data.ShippingPermission = shopDto.ShippingPermission;
                    if (data.ShippingPermission == false)
                    {
                        var ExistShippingByMarket = await _context.TShopActivityCity.AsNoTracking().AnyAsync(x => x.FkShippingMethodId == (int)ShippingMethodEnum.Market);
                        if (ExistShippingByMarket == true)
                        {
                            return new RepRes<bool>(Message.FirstGoChangeShopShippingMethod, false, false);
                        }
                    }
                }
                if (shopDto.ShippingPossibilities == false)
                {
                    data.ShippingPermission = shopDto.ShippingPermission;
                    if (data.ShippingPermission == false)
                    {
                        var ExistShippingByMarket = await _context.TShopActivityCity.AsNoTracking().AnyAsync(x => x.FkShippingMethodId == (int)ShippingMethodEnum.Market);
                        if (ExistShippingByMarket == true)
                        {
                            return new RepRes<bool>(Message.FirstGoChangeShopShippingMethod, false, false);
                        }
                    }
                }
                data.ShippingPossibilities = shopDto.ShippingPossibilities;
                if (data.FkStatusId == (int)ShopStatusEnum.InitialRegistration)
                {
                    data.FkStatusId = (int)ShopStatusEnum.Waiting;
                }
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.ShopEditing, false, false);
            }
        }

        public async Task<ShopBankInformationDto> GetShopBankInformation(int shopId)
        {
            try
            {
                return await _context.TShop
                .Include(x => x.TShopFiles).ThenInclude(t => t.FkDocumentType)
                .Select(x => new ShopBankInformationDto()
                {
                    BankAccountNumber = x.BankAccountNumber,
                    BankAdditionalInformation = x.BankAdditionalInformation,
                    BankBeneficiaryName = x.BankBeneficiaryName,
                    BankBranch = x.BankBranch,
                    BankIban = x.BankIban,
                    BankName = x.BankName,
                    BankSwiftCode = x.BankSwiftCode,
                    FkCurrencyId = x.FkCurrencyId,
                    ShopId = x.ShopId,
                    TShopFiles = x.TShopFiles.Where(t => t.FkDocumentType.FkGroupd == (int)DocumentGroupEnum.Bank).Select(t => new ShopFileDto()
                    {
                        FileId = t.FileId,
                        FileUrl = t.FileUrl,
                        FkDocumentTypeId = t.FkDocumentTypeId,
                        FkShopId = t.FkShopId
                    }).ToList()
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == shopId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<ShopTaxDto> GetShopTax(int shopId)
        {
            try
            {
                return await _context.TShop
                .Include(x => x.TShopFiles).ThenInclude(t => t.FkDocumentType)
                .Select(x => new ShopTaxDto()
                {
                    ShopId = x.ShopId,
                    GoodsIncludedVat = x.GoodsIncludedVat,
                    TaxRegistrationNumber = x.TaxRegistrationNumber,
                    TShopFiles = x.TShopFiles.Where(t => t.FkDocumentType.FkGroupd == (int)DocumentGroupEnum.Tax).Select(t => new ShopFileDto()
                    {
                        FileId = t.FileId,
                        FileUrl = t.FileUrl,
                        FkDocumentTypeId = t.FkDocumentTypeId,
                        FkShopId = t.FkShopId
                    }).ToList()
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == shopId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<bool>> EditShopActivityCountry(ShopActivityCountryEditDto shopDto)
        {
            try
            {
                var deletedData = await _context.TShopActivityCountry.Where(x => shopDto.Ids.Contains(x.Id) || (shopDto.ShopId == x.FkShopId && shopDto.ShopActivityCountry.Select(t => t.FkCountryId).ToList().Contains(x.FkCountryId))).ToListAsync();
                _context.TShopActivityCountry.RemoveRange(deletedData);
                await _context.SaveChangesAsync();

                var mapData = _mapper.Map<List<TShopActivityCountry>>(shopDto.ShopActivityCountry);
                foreach (var item in mapData)
                {
                    item.FkShopId = shopDto.ShopId;
                    item.Id = 0;
                }

                await _context.TShopActivityCountry.AddRangeAsync(mapData);
                var data = await _context.TShop.FirstOrDefaultAsync(x => x.ShopId == shopDto.ShopId);
                if (data.FkStatusId == (int)ShopStatusEnum.InitialRegistration)
                {
                    data.FkStatusId = (int)ShopStatusEnum.Waiting;
                }
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.EditShopActivityCountry, false, false);
            }
        }

        public async Task<RepRes<bool>> EditShopActivityCity(ShopActivityCityEditDto shopDto)
        {
            try
            {
                var deletedData = await _context.TShopActivityCity.Where(x => shopDto.Ids.Contains(x.Id) || (shopDto.ShopId == x.FkShopId && shopDto.ShopActivityCity.Select(t => t.FkCityId).ToList().Contains(x.FkCityId))).ToListAsync();
                _context.TShopActivityCity.RemoveRange(deletedData);
                await _context.SaveChangesAsync();
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var mapData = _mapper.Map<List<TShopActivityCity>>(shopDto.ShopActivityCity);
                foreach (var item in mapData)
                {
                    item.FkShopId = shopDto.ShopId;
                    item.Id = 0;
                    item.PostTimeoutDayByShop = item.PostTimeoutDayByShop == null ? 1 : item.PostTimeoutDayByShop;
                    if (item.FkShippingMethodId != (int)ShippingMethodEnum.Market)
                    {
                        item.ShippingPriceFewerBaseWeight = null;
                        item.ShippingPriceMoreBaseWeight = null;
                    }

                    if (item.ShippingPriceFewerBaseWeight != null)
                    {
                        item.ShippingPriceFewerBaseWeight = decimal.Round((decimal)item.ShippingPriceFewerBaseWeight / rate, 2, MidpointRounding.AwayFromZero);
                    }
                    if (item.ShippingPriceMoreBaseWeight != null)
                    {
                        item.ShippingPriceMoreBaseWeight = decimal.Round((decimal)item.ShippingPriceMoreBaseWeight / rate, 2, MidpointRounding.AwayFromZero);
                    }
                }

                await _context.TShopActivityCity.AddRangeAsync(mapData);
                var data = await _context.TShop.FirstOrDefaultAsync(x => x.ShopId == shopDto.ShopId);
                if (data.FkStatusId == (int)ShopStatusEnum.InitialRegistration)
                {
                    data.FkStatusId = (int)ShopStatusEnum.Waiting;
                }
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.EditShopActivityCity, false, false);
            }
        }

        public async Task<RepRes<bool>> EditShopActivityProvince(ShopActivityCityEditDto shopDto)
        {
            try
            {
                var deletedData = await _context.TShopActivityCity.Where(x => shopDto.Ids.Contains(x.Id) || (shopDto.ShopId == x.FkShopId && shopDto.ShopActivityCity.Select(t => t.fkProviceId).ToList().Contains(x.FkProviceId))).ToListAsync();
                _context.TShopActivityCity.RemoveRange(deletedData);
                await _context.SaveChangesAsync();
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var mapData = _mapper.Map<List<TShopActivityCity>>(shopDto.ShopActivityCity);
                foreach (var item in mapData)
                {
                    item.FkShopId = shopDto.ShopId;
                    item.Id = 0;
                    item.PostTimeoutDayByShop = item.PostTimeoutDayByShop == null ? 1 : item.PostTimeoutDayByShop;
                    if (item.FkShippingMethodId != (int)ShippingMethodEnum.Market)
                    {
                        item.ShippingPriceFewerBaseWeight = null;
                        item.ShippingPriceMoreBaseWeight = null;
                    }
                    if (item.ShippingPriceFewerBaseWeight != null)
                    {
                        item.ShippingPriceFewerBaseWeight = decimal.Round((decimal)item.ShippingPriceFewerBaseWeight / rate, 2, MidpointRounding.AwayFromZero);
                    }
                    if (item.ShippingPriceMoreBaseWeight != null)
                    {
                        item.ShippingPriceMoreBaseWeight = decimal.Round((decimal)item.ShippingPriceMoreBaseWeight / rate, 2, MidpointRounding.AwayFromZero);
                    }
                }

                await _context.TShopActivityCity.AddRangeAsync(mapData);
                var data = await _context.TShop.FirstOrDefaultAsync(x => x.ShopId == shopDto.ShopId);
                if (data.FkStatusId == (int)ShopStatusEnum.InitialRegistration)
                {
                    data.FkStatusId = (int)ShopStatusEnum.Waiting;
                }
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.EditShopActivityCity, false, false);
            }
        }

        public async Task<List<ShopActivityCountryGetDto>> GetShopActivityCountry(PaginationFormDto pagination)
        {
            try
            {
                return await _context.TCountry.Where(x => x.CountryId != _context.TShop.Find(pagination.Id).FkCountryId && x.Status == true)
                .OrderByDescending(x => x.CountryId)
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.TShopActivityCountry)
                .Include(x => x.TShippingOnCountry).ThenInclude(t => t.FkShippingMethod)
                .Select(x => new ShopActivityCountryGetDto()
                {
                    CountryTitle = JsonExtensions.JsonValue(x.CountryTitle, header.Language),
                    FkCountryId = x.CountryId,
                    FkShippingMethodId = x.TShopActivityCountry.FirstOrDefault(t => t.FkShopId == pagination.Id).FkShippingMethodId,
                    FkShopId = x.TShopActivityCountry.FirstOrDefault(t => t.FkShopId == pagination.Id).FkShopId,
                    Id = x.TShopActivityCountry.FirstOrDefault(t => t.FkShopId == pagination.Id).Id,
                    ReturningAllowed = x.TShopActivityCountry.FirstOrDefault(t => t.FkShopId == pagination.Id).ReturningAllowed,
                    ShippingMethodList = x.TShippingOnCountry.Where(t => t.FkShippingMethodId != (int)ShippingMethodEnum.Market && t.FkShippingMethod.Active == true).Select(t => new Dtos.ShippingMethod.ShippingMethodFormDto()
                    {
                        BaseWeight = t.FkShippingMethod.BaseWeight,
                        CashOnDelivery = t.FkShippingMethod.CashOnDelivery,
                        HaveOnlineService = t.FkShippingMethod.HaveOnlineService,
                        Id = t.FkShippingMethod.Id,
                        ShippingMethodTitle = JsonExtensions.JsonValue(t.FkShippingMethod.ShippingMethodTitle, header.Language),
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetShopActivityCountryCount(PaginationFormDto pagination)
        {
            try
            {
                return await _context.TCountry.AsNoTracking().CountAsync(x => x.Status == true && x.CountryId != _context.TShop.Find(pagination.Id).FkCountryId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<ShopActivityCityGetDto>> GetShopActivityCity(PaginationFormDto pagination, int provinceId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var shipingMethods = await _context.TShippingMethod.Where(x => x.Active == true).Select(x => new ShippingMethodFormDto()
                {
                    BaseWeight = x.BaseWeight,
                    CashOnDelivery = x.CashOnDelivery,
                    HaveOnlineService = x.HaveOnlineService,
                    Id = x.Id,
                    ShippingMethodTitle = JsonExtensions.JsonValue(x.ShippingMethodTitle, header.Language)
                }).ToListAsync();
                var shop = await _context.TShop.Select(x => new { x.ShippingPermission, x.ShopId, x.FkCountryId, x.ShippingPossibilities }).AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == pagination.Id);
                if (shop.ShippingPossibilities == false || shop.ShippingPermission == false)
                {
                    shipingMethods = shipingMethods.Where(x => x.Id != (int)ShippingMethodEnum.Market).ToList();
                }

                return await _context.TCity
                .Where(x => x.FkCountryId == shop.FkCountryId
                && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CityTitle, header.Language).Contains(pagination.Filter)))
                && (provinceId == 0 ? true : ((int)x.FkProvinceId == provinceId))
                && x.Status == true)
                .OrderBy(x => JsonExtensions.JsonValue(x.CityTitle, header.Language))
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.FkProvince)
                .Include(x => x.TShopActivityCity).ThenInclude(t => t.FkShop)
                .Select(x => new ShopActivityCityGetDto()
                {
                    CityTitle = JsonExtensions.JsonValue(x.CityTitle, header.Language),
                    ProvinceTitle = JsonExtensions.JsonValue(x.FkProvince.ProvinceName, header.Language),
                    FkCityId = x.CityId,
                    FkProviceId = (int)x.FkProvinceId,
                    FkShippingMethodId = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).FkShippingMethodId,
                    FkShopId = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).FkShopId,
                    Id = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).Id,
                    ReturningAllowed = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).ReturningAllowed,
                    PostTimeoutDayByShop = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).PostTimeoutDayByShop,
                    ShippingPriceFewerBaseWeight = decimal.Round((decimal)x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).ShippingPriceFewerBaseWeight  / rate, 2, MidpointRounding.AwayFromZero),
                    ShippingPriceMoreBaseWeight = decimal.Round((decimal)x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).ShippingPriceMoreBaseWeight  / rate, 2, MidpointRounding.AwayFromZero),
                    ShippingMethodList = shipingMethods
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetShopActivityCityCount(PaginationFormDto pagination, int provinceId)
        {
            try
            {
                return await _context.TCity.AsNoTracking()
                .CountAsync(x => x.Status == true
                && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.CityTitle, header.Language).Contains(pagination.Filter)))
                && (provinceId == 0 ? true : ((int)x.FkProvinceId == provinceId))
                && x.FkCountryId == _context.TShop.Find(pagination.Id).FkCountryId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<List<ShopActivityCityGetDto>> GetShopActivityProvince(PaginationFormDto pagination)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var shipingMethods = await _context.TShippingMethod.Where(x => x.Active == true).Select(x => new ShippingMethodFormDto()
                {
                    BaseWeight = x.BaseWeight,
                    CashOnDelivery = x.CashOnDelivery,
                    HaveOnlineService = x.HaveOnlineService,
                    Id = x.Id,
                    ShippingMethodTitle = JsonExtensions.JsonValue(x.ShippingMethodTitle, header.Language)
                }).ToListAsync();
                var shop = await _context.TShop.Select(x => new { x.ShippingPermission, x.ShopId, x.FkCountryId, x.ShippingPossibilities }).AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == pagination.Id);
                if (shop.ShippingPossibilities == false || shop.ShippingPermission == false)
                {
                    shipingMethods = shipingMethods.Where(x => x.Id != (int)ShippingMethodEnum.Market).ToList();
                }

                return await _context.TProvince
                .Where(x => x.FkCountryId == shop.FkCountryId
                && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.ProvinceName, header.Language).Contains(pagination.Filter)))
                && x.Status == true)
                .OrderBy(x => JsonExtensions.JsonValue(x.ProvinceName, header.Language))
                .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                .Include(x => x.TShopActivityCity).ThenInclude(t => t.FkShop)
                .Select(x => new ShopActivityCityGetDto()
                {
                    ProvinceTitle = JsonExtensions.JsonValue(x.ProvinceName, header.Language),
                    FkProviceId = x.ProvinceId,
                    FkCityId = null,
                    FkShippingMethodId = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).FkShippingMethodId,
                    FkShopId = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).FkShopId,
                    Id = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).Id,
                    ReturningAllowed = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).ReturningAllowed,
                    PostTimeoutDayByShop = x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).PostTimeoutDayByShop,
                    ShippingPriceFewerBaseWeight = decimal.Round((decimal)x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).ShippingPriceFewerBaseWeight  / rate, 2, MidpointRounding.AwayFromZero),
                    ShippingPriceMoreBaseWeight = decimal.Round((decimal)x.TShopActivityCity.FirstOrDefault(t => t.FkShopId == pagination.Id).ShippingPriceMoreBaseWeight  / rate, 2, MidpointRounding.AwayFromZero),
                    ShippingMethodList = shipingMethods
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetShopActivityProvinceCount(PaginationFormDto pagination)
        {
            try
            {
                return await _context.TProvince.AsNoTracking()
                .CountAsync(x => x.Status == true
                && (string.IsNullOrWhiteSpace(pagination.Filter) ? true : (JsonExtensions.JsonValue(x.ProvinceName, header.Language).Contains(pagination.Filter)))
                && x.FkCountryId == _context.TShop.Find(pagination.Id).FkCountryId);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }



        public async Task<List<ProvinceFormDto>> GetAllShopProvince(int shopId)
        {
            try
            {

                var shop = await _context.TShop.Select(x => new { x.ShopId, x.FkCountryId }).AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == shopId);

                return await _context.TProvince
                .Where(x => x.FkCountryId == shop.FkCountryId
                && x.Status == true)
                .OrderBy(x => JsonExtensions.JsonValue(x.ProvinceName, header.Language))
                .Include(x => x.TShopActivityCity).ThenInclude(t => t.FkShop)
                .Select(x => new ProvinceFormDto()
                {
                    ProvinceName = JsonExtensions.JsonValue(x.ProvinceName, header.Language),
                    ProvinceId = x.ProvinceId
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }




        public async Task<bool> CanAddSlider(int shopId)
        {
            try
            {
                var maxItem = 0;
                var data = await _context.TShop.Select(x => new { x.MaxSliderForShopWebPage, x.ShopId, x.FkPlanId }).FirstOrDefaultAsync(x => x.ShopId == shopId);

                if (data.MaxSliderForShopWebPage != null)
                {
                    maxItem = (int)data.MaxSliderForShopWebPage;
                }
                else
                {
                    maxItem = (int)await _context.TSetting.Select(x => x.MaxSliderForShopWebPage).FirstOrDefaultAsync();
                }

                var count = await _context.TShopSlider.CountAsync(x => x.FkShopId == shopId);

                if (count >= maxItem)
                {
                    return false;
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<TShopSlider> AddShopSlider(TShopSlider shopSlider)
        {
            try
            {
                await _context.TShopSlider.AddAsync(shopSlider);
                await _context.SaveChangesAsync();
                return shopSlider;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TShopSlider>> DeleteShopSlider(int sliderId, int shopId)
        {
            try
            {
                var data = await _context.TShopSlider.FirstOrDefaultAsync(x => x.SliderId == sliderId && (shopId == 0 ? true : x.FkShopId == shopId));
                _context.TShopSlider.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TShopSlider>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return new RepRes<TShopSlider>(Message.DeleteShopSlider, false, null);
            }
        }

        public async Task<List<ShopSliderDto>> GetShopSlider(int shopId)
        {
            try
            {
                return await _context.TShopSlider.Where(x => x.FkShopId == shopId)
                .OrderByDescending(x => x.SliderId)
                .Select(x => new ShopSliderDto()
                {
                    FkShopId = x.FkShopId,
                    ImageUrl = x.ImageUrl,
                    SliderId = x.SliderId,
                    Status = x.Status
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<bool>> ChangeShopSliderStatus(AcceptDto accept, int shopId)
        {
            try
            {
                var data = await _context.TShopSlider.FirstOrDefaultAsync(x => x.SliderId == accept.Id && (shopId == 0 ? true : x.FkShopId == shopId));
                data.Status = accept.Accept;
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.ShopSliderChangeAccept, false, false);
            }
        }

        public async Task<RepRes<List<TShopFiles>>> EditShopBankInformation(ShopBankInformationDto shopDto, List<int> shopFileDeleted)
        {
            try
            {
                var data = await _context.TShop.Include(x => x.TShopFiles).FirstOrDefaultAsync(x => x.ShopId == shopDto.ShopId);
                data.BankAccountNumber = shopDto.BankAccountNumber;
                data.BankAdditionalInformation = shopDto.BankAdditionalInformation;
                data.BankBeneficiaryName = shopDto.BankBeneficiaryName;
                data.BankBranch = shopDto.BankBranch;
                data.BankIban = shopDto.BankIban;
                data.BankName = shopDto.BankName;
                data.BankSwiftCode = shopDto.BankSwiftCode;
                data.FkCurrencyId = shopDto.FkCurrencyId;

                var oldFileIds = shopDto.TShopFiles.Select(x => x.FkDocumentTypeId).ToList();
                var oldFiles = data.TShopFiles.Where(x => oldFileIds.Contains(x.FkDocumentTypeId) && x.FkShopId == shopDto.ShopId).ToList();

                _context.TShopFiles.RemoveRange(oldFiles);

                await _context.TShopFiles.AddRangeAsync(_mapper.Map<List<TShopFiles>>(shopDto.TShopFiles));

                if (data.FkStatusId == (int)ShopStatusEnum.InitialRegistration)
                {
                    data.FkStatusId = (int)ShopStatusEnum.Waiting;
                }

                await _context.SaveChangesAsync();


                if (shopFileDeleted.Count > 0)
                {

                    var filesImageDeleted = _context.TShopFiles.Where(x => shopFileDeleted.Contains(x.FileId)).ToList();
                    _context.TShopFiles.RemoveRange(filesImageDeleted);
                    await _context.SaveChangesAsync();

                }


                return new RepRes<List<TShopFiles>>(Message.Successfull, true, oldFiles);


            }
            catch (System.Exception)
            {
                return new RepRes<List<TShopFiles>>(Message.EditShopBankInformation, false, null);
            }
        }

        public async Task<RepRes<List<TShopFiles>>> EditShopTax(ShopTaxDto shopDto, List<int> shopFileDeleted)
        {
            try
            {
                var data = await _context.TShop.Include(x => x.TShopFiles).FirstOrDefaultAsync(x => x.ShopId == shopDto.ShopId);
                if (data.GoodsIncludedVat == true && shopDto.GoodsIncludedVat == false)
                {
                    var allGoodsProviderOfShops = await _context.TGoodsProvider.Where(x => x.FkShopId == shopDto.ShopId).ToListAsync();
                    foreach (var item in allGoodsProviderOfShops)
                    {
                        item.Vatfree = true;
                        item.Vatamount = 0;
                    }
                }
                data.GoodsIncludedVat = shopDto.GoodsIncludedVat;
                data.TaxRegistrationNumber = shopDto.TaxRegistrationNumber;

                var oldFileIds = shopDto.TShopFiles.Select(x => x.FkDocumentTypeId).ToList();
                var oldFiles = data.TShopFiles.Where(x => oldFileIds.Contains(x.FkDocumentTypeId) && x.FkShopId == shopDto.ShopId).ToList();

                _context.TShopFiles.RemoveRange(oldFiles);

                await _context.TShopFiles.AddRangeAsync(_mapper.Map<List<TShopFiles>>(shopDto.TShopFiles));

                if (data.FkStatusId == (int)ShopStatusEnum.InitialRegistration)
                {
                    data.FkStatusId = (int)ShopStatusEnum.Waiting;
                }

                await _context.SaveChangesAsync();



                if (shopFileDeleted.Count > 0)
                {

                    var filesImageDeleted = _context.TShopFiles.Where(x => shopFileDeleted.Contains(x.FileId)).ToList();
                    _context.TShopFiles.RemoveRange(filesImageDeleted);
                    await _context.SaveChangesAsync();

                }



                return new RepRes<List<TShopFiles>>(Message.Successfull, true, oldFiles);


            }
            catch (System.Exception)
            {
                return new RepRes<List<TShopFiles>>(Message.EditShopTax, false, null);
            }
        }

        public async Task<RepRes<TShopPlan>> EditShopPlan(ShopPlanPaymentDto shopPlanPaymentDto, bool pay, int? setCurrency = null)
        {
            try
            {
                var rate = (decimal)1.00;
                var extend = false; // baraye tamdid

                var data = await _context.TShop.Include(x => x.TShopCategory).ThenInclude(t => t.FkCategory).Include(x => x.TUser)
                .FirstOrDefaultAsync(x => x.ShopId == shopPlanPaymentDto.ShopId);
                if (data == null)
                {
                    return new RepRes<TShopPlan>(Message.EditShopPlan, false, null);
                }
                if (data.FkPlanId == shopPlanPaymentDto.PlanId && data.ExpirationDate != null)
                {
                    if (data.ExpirationDate.Value.Date >= DateTime.Now.Date)
                    {
                        extend = true;
                        //   return new RepRes<TShopPlan>(Message.ThisIsYourCurrentPlanNow, false, null);
                    }
                }
                var plan = await _context.TShopPlan.AsNoTracking().FirstAsync(x => x.PlanId == shopPlanPaymentDto.PlanId && x.Status == true);
                if (pay == false && plan.Microstore && string.IsNullOrWhiteSpace(data.VendorUrlid))
                {
                    return new RepRes<TShopPlan>(Message.AddVendorUrlid, false, null);
                }

                if (token.Rule == UserGroupEnum.Admin)
                {
                    var ShopGoodsCount = await _context.TGoods.Where(x => x.FkOwnerId == shopPlanPaymentDto.ShopId).AsNoTracking().CountAsync();
                    if (plan.MaxProduct != null)
                    {
                        if (plan.MaxProduct < ShopGoodsCount)
                        {
                            return new RepRes<TShopPlan>(Message.YouCantChooseThisPlanBecauseYourProductIsMoreThanMaximumOfPlanProduct, false, null);
                        }
                    }

                    if (plan.MaxCategory != null)
                    {
                        if (plan.MaxCategory < data.TShopCategory.Count)
                        {
                            return new RepRes<TShopPlan>(Message.YouCantChooseThisPlanBecauseYourCategoryIsMoreThanMaximumOfPlanCategory, false, null);
                        }
                    }

                    data.RentPerMonth = (plan.RentPerMonth == null || plan.RentPerMonth == (decimal)0.00) ? 0 : (plan.RentPerMonth * shopPlanPaymentDto.Month);
                    data.Microstore = plan.Microstore;
                    data.CommissionFraction = plan.PercentOfCommission;
                    data.MaxProduct = plan.MaxProduct;
                    data.MaxCategory = plan.MaxCategory;
                    data.ExpirationDate = extend ? (data.ExpirationDate.Value.Date.AddMonths((int)shopPlanPaymentDto.Month)) : (DateTime.Now.AddMonths((int)shopPlanPaymentDto.Month));
                    data.FkPlanId = shopPlanPaymentDto.PlanId;
                    foreach (var item in data.TShopCategory)
                    {
                        item.ContractCommissionFee = (decimal)((item.FkCategory.CommissionFee * plan.PercentOfCommission) / 100);
                    }
                }
                else
                {

                    var ShopGoodsCount = await _context.TGoods.Where(x => x.FkOwnerId == shopPlanPaymentDto.ShopId).AsNoTracking().CountAsync();
                    if (plan.MaxProduct != null)
                    {
                        if (plan.MaxProduct < ShopGoodsCount)
                        {
                            return new RepRes<TShopPlan>(Message.YouCantChooseThisPlanBecauseYourProductIsMoreThanMaximumOfPlanProduct, false, null);
                        }
                    }

                    if (plan.MaxCategory != null)
                    {
                        if (plan.MaxCategory < data.TShopCategory.Count)
                        {
                            return new RepRes<TShopPlan>(Message.YouCantChooseThisPlanBecauseYourCategoryIsMoreThanMaximumOfPlanCategory, false, null);
                        }
                    }

                    var setPlan = true;
                    if (plan.RentPerMonth != null && plan.RentPerMonth != (decimal)0.00)
                    {
                        // if (data.FkStatusId == (int)ShopStatusEnum.Active)
                        //  {
                        //can transaction if can status active else status pending
                        if (data.Credit >= (plan.RentPerMonth * shopPlanPaymentDto.Month) && shopPlanPaymentDto.UseCredit)
                        {
                            await _accountingRepository.AddTransaction((int)TransactionTypeEnum.RentAmount, token.UserId, null, null, null, (int)TransactionStatusEnum.Completed, (decimal)plan.RentPerMonth, " پرداخت " + Extentions.DecimalRoundWithZiro((decimal)plan.RentPerMonth) + "تومان برای اجاره ی ماهیانه");
                        }
                        else if (data.Credit < (plan.RentPerMonth * shopPlanPaymentDto.Month) && shopPlanPaymentDto.UseCredit)
                        {
                            return new RepRes<TShopPlan>(Message.CreditNotEnough, false, null);
                        }
                        else if (pay)
                        {
                            setPlan = true;
                        }
                        else
                        {
                            setPlan = false;
                        }
                        //}
                    }
                    // else
                    // {
                    //     if (data.FkStatusId == (int)ShopStatusEnum.InitialRegistration)
                    //     {
                    //         shopPlanPaymentDto.Month = 12;
                    //     }
                    // }

                    if (setPlan == true)
                    {

                        data.RentPerMonth = (plan.RentPerMonth == null || plan.RentPerMonth == (decimal)0.00) ? 0 : (plan.RentPerMonth * shopPlanPaymentDto.Month);
                        data.Microstore = plan.Microstore;
                        data.CommissionFraction = plan.PercentOfCommission;
                        data.MaxProduct = plan.MaxProduct;
                        data.MaxCategory = plan.MaxCategory;
                        data.ExpirationDate = extend ? (data.ExpirationDate.Value.Date.AddMonths((int)shopPlanPaymentDto.Month)) : (DateTime.Now.AddMonths((int)shopPlanPaymentDto.Month));
                        data.FkPlanId = shopPlanPaymentDto.PlanId;
                        foreach (var item in data.TShopCategory)
                        {
                            item.ContractCommissionFee = (decimal)((item.FkCategory.CommissionFee * plan.PercentOfCommission) / 100);
                        }
                        try
                        {
                            await _notificationService.SendNotification((int)NotificationSettingTypeEnum.ConfirmationSubscriptionRenewalAfterPaymentRentProvider, data.TUser.FirstOrDefault().ProviderFirebasePushNotificationKey, null, data.Email, data.Phone, data.TUser.FirstOrDefault().UserId);
                        }
                        catch (System.Exception)
                        {

                        }


                    }
                    // else
                    // {
                    //     data.FkPlanId = planId;
                    //     data.ExpirationDate = DateTime.Now.AddDays(-1);

                    // }

                }

                if ((pay && data.FkStatusId == (int)ShopStatusEnum.InitialRegistration) || ((plan.RentPerMonth == null || plan.RentPerMonth == (decimal)0.00) && data.FkStatusId == (int)ShopStatusEnum.InitialRegistration))
                {
                    data.FkStatusId = (int)ShopStatusEnum.Waiting;
                }
                else if (pay)
                {
                    data.FkStatusId = (int)ShopStatusEnum.Active;
                }
                if (!string.IsNullOrWhiteSpace(shopPlanPaymentDto.PaymentPayId) && pay)
                {
                    var paymentTransaction = await _context.TPaymentTransaction.FirstOrDefaultAsync(c => c.PaymentId == shopPlanPaymentDto.PaymentPayId);
                    if (paymentTransaction != null)
                    {
                        paymentTransaction.Status = true;
                    }
                }
                await _context.SaveChangesAsync();
                plan.RentPerMonth = (plan.RentPerMonth * shopPlanPaymentDto.Month)  / rate;
                return new RepRes<TShopPlan>(Message.Successfull, true, plan);
            }
            catch (System.Exception)
            {
                return new RepRes<TShopPlan>(Message.EditShopPlan, false, null);
            }
        }

        public async Task<List<PlanShopDto>> GetShopPlan(int shopId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                var shopPlan = await _context.TShop.Include(x => x.TShopPlanExclusive).ThenInclude(x => x.FkPlan).Include(x => x.TShopCategory).ThenInclude(t => t.FkCategory).AsNoTracking().FirstAsync(x => x.ShopId == shopId);
                var plans = new List<PlanShopDto>();
                if (shopPlan.TShopPlanExclusive.Count > 0)
                {
                    plans = shopPlan.TShopPlanExclusive.Where(x => x.FkPlan.Status == true)
                   .Select(x => new PlanShopDto()
                   {
                       Available = x.FkPlan.Status,
                       CommissionFraction = x.FkPlan.PercentOfCommission,
                       CurrentPlan = shopPlan.FkPlanId == null ? (false) : (shopPlan.FkPlanId == x.FkPlan.PlanId ? true : false),
                       ExpDate = shopPlan.FkPlanId == null ? (null) : (shopPlan.FkPlanId == x.FkPlan.PlanId ? (shopPlan.ExpirationDate != null ? Extentions.PersianDateString((DateTime)shopPlan.ExpirationDate) : null) : null),
                       Desription = JsonExtensions.JsonGet(x.FkPlan.Desription, header),
                       PlanTitle = JsonExtensions.JsonGet(x.FkPlan.PlanTitle, header),
                       MaxCategory = x.FkPlan.MaxCategory,
                       MaxProduct = x.FkPlan.MaxProduct,
                       Microstore = x.FkPlan.Microstore,
                       PlanId = x.FkPlan.PlanId,
                       ShopCredit = shopPlan.Credit  / rate,
                       RentPerMonth = x.FkPlan.RentPerMonth == null ? 0 : decimal.Round((decimal)(x.FkPlan.RentPerMonth  / rate), 2, MidpointRounding.AwayFromZero),
                   }).ToList();

                }
                else
                {

                    plans = await _context.TShopPlan.Where(x => x.Status == true)
                   .Select(x => new PlanShopDto()
                   {
                       Available = x.Status,
                       CommissionFraction = x.PercentOfCommission,
                       CurrentPlan = shopPlan.FkPlanId == null ? (false) : (shopPlan.FkPlanId == x.PlanId ? true : false),
                       ExpDate = shopPlan.FkPlanId == null ? (null) : (shopPlan.FkPlanId == x.PlanId ? (shopPlan.ExpirationDate != null ? Extentions.PersianDateString((DateTime)shopPlan.ExpirationDate) : null) : null),
                       Desription = JsonExtensions.JsonValue(x.Desription, header.Language),
                       PlanTitle = JsonExtensions.JsonValue(x.PlanTitle, header.Language),
                       MaxCategory = x.MaxCategory,
                       MaxProduct = x.MaxProduct,
                       Microstore = x.Microstore,
                       PlanId = x.PlanId,
                       ShopCredit = shopPlan.Credit  / rate,
                       RentPerMonth = x.RentPerMonth == null ? 0 : decimal.Round((decimal)(x.RentPerMonth  / rate), 2, MidpointRounding.AwayFromZero),
                   })
                   .AsNoTracking().ToListAsync();
                }

                var ShopGoodsCount = await _context.TGoods.Where(x => x.FkOwnerId == shopId).AsNoTracking().CountAsync();
                foreach (var item in plans)
                {
                    if (item.Available == true)
                    {
                        if (item.MaxProduct != null)
                        {
                            if (item.MaxProduct < ShopGoodsCount)
                            {
                                item.Available = false;
                            }
                        }

                        if (item.MaxCategory != null)
                        {
                            if (item.MaxCategory < shopPlan.TShopCategory.Count)
                            {
                                item.Available = false;
                            }
                        }
                    }
                    item.Category = shopPlan.TShopCategory
                    .Select(x => new ShopCategoryPlanDto()
                    {
                        CategoryId = x.FkCategoryId,
                        CategoryTitle = JsonExtensions.JsonGet(x.FkCategory.CategoryTitle, header),
                        Commission = (decimal)((x.FkCategory.CommissionFee * item.CommissionFraction) / 100)
                    }).ToList();
                }

                return plans;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetShopActiveOrderCount(int shopId)
        {
            try
            {
                return await _context.TOrderItem.AsNoTracking().CountAsync(x => x.FkShopId == shopId && x.FkShopId >= (int)OrderStatusEnum.Ongoing && x.FkShopId < (int)OrderStatusEnum.Completed);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<int> GetShopActiveProductCount(int shopId)
        {
            try
            {
                return await _context.TGoods.Include(x => x.TGoodsProvider).AsNoTracking()
                .CountAsync(x => x.TGoodsProvider.Any(t => t.FkShopId == shopId && t.HasInventory == true));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<double> GetShopIncome(int shopId)
        {
            try
            {
                var rate = 1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? 1.0 : currency.RatesAgainstOneDollar;
                }
                return await _context.TOrderItem.Where(x => x.FkStatusId > (int)OrderStatusEnum.Cart && x.ComissionPrice != null && x.FkShopId == shopId).SumAsync(x => (double)x.ComissionPrice)  / rate;

            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<int> GetShopOrdersCount(int shopId)
        {
            try
            {
                return await _context.TOrderItem.AsNoTracking()
                .CountAsync(x => x.FkShopId == shopId && x.FkStatusId > (int)OrderStatusEnum.Cart);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<int> GetShopOutOfStuckCount(int shopId)
        {
            try
            {
                return await _context.TGoods.Include(x => x.TGoodsProvider).AsNoTracking()
                .CountAsync(x => x.TGoodsProvider.Where(t => t.FkShopId == shopId).All(t => t.HasInventory == false));
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<double> GetShopSales(int shopId)
        {
            try
            {
                var rate = 1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? 1.0 : currency.RatesAgainstOneDollar;
                }
                return await _context.TOrderItem.AsNoTracking()
                .Where(x => x.FkShopId == shopId && x.FkStatusId > (int)OrderStatusEnum.Cart).SumAsync(x => (double)x.FinalPrice)  / rate;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<string> GetShopUserName(int shopId)
        {
            try
            {
                var data = await _context.TUser.Select(x => new { x.FkShopId, x.UserName }).AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == shopId);
                return data.UserName;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<ShopWebsiteDetailDto> GetShopByUrl(string url)
        {
            try
            {
                var setting = await _context.TSetting.FirstOrDefaultAsync();
                var defualtImage = "";
                if (setting != null)
                {
                    defualtImage = setting.ShopDefaultBannerImageUrl;
                }

                return await _context.TShop
                .Where(x => x.VendorUrlid == url && x.Microstore == true)
                .Include(x => x.FkCity)
                .Include(x => x.FkCountry)
                .Include(x => x.TShopSlider)
                .Include(x => x.TOrderItem)
                .Include(x => x.TShopCategory).ThenInclude(t => t.FkCategory)
                .Select(x => new ShopWebsiteDetailDto()
                {
                    Address = JsonExtensions.JsonValue(x.Address, header.Language),
                    ShopStatus = x.FkStatusId,
                    CityTitle = JsonExtensions.JsonValue(x.FkCity.CityTitle, header.Language),
                    CountryTitle = JsonExtensions.JsonValue(x.FkCountry.CountryTitle, header.Language),
                    TermCondition = JsonExtensions.JsonValue(x.TermCondition, header.Language),
                    LocationX = x.LocationX,
                    LocationY = x.LocationY,
                    LogoImage = string.IsNullOrWhiteSpace(x.LogoImage) ? defualtImage : x.LogoImage,
                    IsDefualtImage = (x.LogoImage == "ShopDefaultBanner.png") ? true : false,
                    Phone = x.Phone,
                    ProfileImage = x.ProfileImage,
                    RegisteryDateTime = Extentions.PersianDateString(x.RegisteryDateTime),
                    ShopId = x.ShopId,
                    Iso = x.FkCountry.Iso2,
                    SurveyScore = (double)x.SurveyScore,
                    ProductsSold = x.TOrderItem.Count(v => v.FkStatusId != (int)OrderStatusEnum.Cart
                                                    && v.FkStatusId != (int)OrderStatusEnum.Cancelled
                                                    && v.FkStatusId != (int)OrderStatusEnum.ReturnProcessing
                                                    && v.FkStatusId != (int)OrderStatusEnum.ReturnComplete),
                    CategoryIds = x.TShopCategory.Where(x => x.FkCategory.IsActive == true).Select(x => x.FkCategoryId).ToList(),
                    ShopSlider = x.TShopSlider.Where(t => t.Status == true).Select(t => new ShopSliderGetDto()
                    {
                        FkShopId = t.FkShopId,
                        ImageUrl = t.ImageUrl
                    }).ToList(),
                    StoreName = x.StoreName,
                })
                .FirstOrDefaultAsync();

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TShop>> RegisterShop(TShop shopRegister)
        {
            try
            {
                // if (await _context.TShop.AnyAsync(x => x.StoreName == shopRegister.StoreName))
                // {
                //     return new RepRes<TShop>(Message.ShopStoreName, false, null);
                // }
                var setting = await _context.TSetting.FirstOrDefaultAsync();
                if (setting != null)
                {
                    shopRegister.InactiveShopMessage = setting.ShopWelcomeMessage;
                    shopRegister.LogoImage = setting.ShopDefaultBannerImageUrl;
                }
                shopRegister.StoreName = shopRegister.StoreName;
                shopRegister.VendorUrlid = null;
                shopRegister.RegisteryDateTime = DateTime.Now;
                shopRegister.Address = JsonExtensions.JsonAdd(shopRegister.Address, header);
                shopRegister.FkStatusId = (int)ShopStatusEnum.InitialRegistration;

                await _context.TShop.AddAsync(shopRegister);
                await _context.SaveChangesAsync();
                shopRegister.StoreName = shopRegister.StoreName;
                // return shopRegister;
                return new RepRes<TShop>(Message.Successfull, true, shopRegister);

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteShop(int shopId)
        {
            try
            {
                var shop = await _context.TShop.Include(x => x.TShopFiles).FirstAsync(x => x.ShopId == shopId);
                _context.TShopFiles.RemoveRange(shop.TShopFiles);
                await _context.SaveChangesAsync();
                _context.TShop.Remove(shop);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;

            }
        }

        public async Task<bool> ChangeShopStatus()
        {
            try
            {
                var shop = await _context.TShop.Where(x => x.FkStatusId == (int)ShopStatusEnum.Active && x.ExpirationDate.Value.Date < DateTime.Now.Date)
                .Include(x => x.TShopCategory).ThenInclude(t => t.FkCategory)
                .Include(x => x.FkPlan)
                .Include(x => x.TUser)
                .ToListAsync();
                var settingData = await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = x.InactiveShopMessage
                }).AsNoTracking().FirstOrDefaultAsync();
                if (shop.Count > 0)
                {
                    foreach (var item in shop)
                    {
                        if (item.AutoAccountRecharge == true)
                        {
                            if (item.FkPlan.Status == true)
                            {
                                var userId = item.TUser.FirstOrDefault().UserId;
                                if (item.FkPlan.RentPerMonth == null)
                                {
                                    var ShopGoodsCount = await _context.TGoods.Where(x => x.FkOwnerId == item.ShopId).AsNoTracking().CountAsync();
                                    if (item.FkPlan.MaxProduct != null)
                                    {
                                        if (item.FkPlan.MaxProduct < ShopGoodsCount)
                                        {
                                            item.FkStatusId = (int)ShopStatusEnum.Pending;
                                            item.InactiveShopMessage = settingData.Description;
                                            continue;
                                        }
                                    }

                                    if (item.FkPlan.MaxCategory != null)
                                    {
                                        if (item.FkPlan.MaxProduct < item.TShopCategory.Count)
                                        {
                                            item.FkStatusId = (int)ShopStatusEnum.Pending;
                                            item.InactiveShopMessage = settingData.Description;
                                            continue;
                                        }
                                    }

                                    item.RentPerMonth = item.FkPlan.RentPerMonth;
                                    item.Microstore = item.FkPlan.Microstore;
                                    item.CommissionFraction = item.FkPlan.PercentOfCommission;
                                    item.MaxProduct = item.FkPlan.MaxProduct;
                                    item.MaxCategory = item.FkPlan.MaxCategory;
                                    item.ExpirationDate = DateTime.Now.AddMonths(1);
                                    item.FkPlanId = item.FkPlan.PlanId;
                                    foreach (var item2 in item.TShopCategory)
                                    {
                                        item2.ContractCommissionFee = (decimal)((item2.FkCategory.CommissionFee * item.FkPlan.PercentOfCommission) / 100);
                                    }
                                }
                                else
                                {
                                    if (item.Credit >= item.FkPlan.RentPerMonth)
                                    {
                                        var ShopGoodsCount = await _context.TGoods.Where(x => x.FkOwnerId == item.ShopId).AsNoTracking().CountAsync();
                                        if (item.FkPlan.MaxProduct != null)
                                        {
                                            if (item.FkPlan.MaxProduct < ShopGoodsCount)
                                            {
                                                item.FkStatusId = (int)ShopStatusEnum.Pending;
                                                item.InactiveShopMessage = settingData.Description;
                                                continue;
                                            }
                                        }

                                        if (item.FkPlan.MaxCategory != null)
                                        {
                                            if (item.FkPlan.MaxProduct < item.TShopCategory.Count)
                                            {
                                                item.FkStatusId = (int)ShopStatusEnum.Pending;
                                                item.InactiveShopMessage = settingData.Description;
                                                continue;
                                            }
                                        }

                                        item.RentPerMonth = item.FkPlan.RentPerMonth;
                                        item.Microstore = item.FkPlan.Microstore;
                                        item.CommissionFraction = item.FkPlan.PercentOfCommission;
                                        item.MaxProduct = item.FkPlan.MaxProduct;
                                        item.MaxCategory = item.FkPlan.MaxCategory;
                                        item.ExpirationDate = DateTime.Now.AddMonths(1);
                                        item.FkPlanId = item.FkPlan.PlanId;
                                        foreach (var item2 in item.TShopCategory)
                                        {
                                            item2.ContractCommissionFee = (decimal)((item2.FkCategory.CommissionFee * item.FkPlan.PercentOfCommission) / 100);
                                        }
                                        await _accountingRepository.AddTransaction((int)TransactionTypeEnum.RentAmount, userId, null, null, null, (int)TransactionStatusEnum.Completed, (decimal)item.RentPerMonth, "پرداخت" + Extentions.DecimalRoundWithZiro((decimal)item.RentPerMonth) + "تومان برای اجاره ی ماهیانه");
                                    }
                                    else
                                    {
                                        item.FkStatusId = (int)ShopStatusEnum.Pending;
                                        item.InactiveShopMessage = settingData.Description;
                                    }
                                }
                            }
                            else
                            {
                                item.FkStatusId = (int)ShopStatusEnum.Pending;
                                item.InactiveShopMessage = settingData.Description;

                            }
                        }
                        {
                            item.FkStatusId = (int)ShopStatusEnum.Pending;
                            item.InactiveShopMessage = settingData.Description;

                        }
                    }
                    await _context.SaveChangesAsync();
                }

                else
                {
                    var alertShop = await _context.TShop.Where(x => x.FkStatusId == (int)ShopStatusEnum.Active && x.ExpirationDate.Value.Date.AddDays(7) == DateTime.Now.Date)
                   .Include(x => x.TUser)
                   .ToListAsync();

                    foreach (var item in alertShop)
                    {
                        await _notificationService.SendNotification((int)NotificationSettingTypeEnum.RentPaymentNoticeProvider, item.TUser.FirstOrDefault().ProviderFirebasePushNotificationKey, null, item.Email, item.Phone, item.TUser.FirstOrDefault().UserId);
                    }

                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<RepRes<List<TShopFiles>>> EditShopDocument(int shopId, List<ShopFileDto> shopDto, List<int> shopFileDeleted)
        {
            try
            {
                var data = await _context.TShop.Include(x => x.TShopFiles).FirstOrDefaultAsync(x => x.ShopId == shopId);

                var oldFileIds = shopDto.Select(x => x.FkDocumentTypeId).ToList();
                var oldFiles = data.TShopFiles.Where(x => oldFileIds.Contains(x.FkDocumentTypeId) && x.FkShopId == shopId).ToList();

                _context.TShopFiles.RemoveRange(oldFiles);

                await _context.TShopFiles.AddRangeAsync(_mapper.Map<List<TShopFiles>>(shopDto));

                await _context.SaveChangesAsync();

                if (shopFileDeleted.Count > 0)
                {

                    var filesImageDeleted = _context.TShopFiles.Where(x => shopFileDeleted.Contains(x.FileId)).ToList();
                    _context.TShopFiles.RemoveRange(filesImageDeleted);
                    await _context.SaveChangesAsync();

                }
                return new RepRes<List<TShopFiles>>(Message.Successfull, true, oldFiles);
            }
            catch (System.Exception)
            {
                return new RepRes<List<TShopFiles>>(Message.EditShopTax, false, null);
            }
        }

        public async Task<RepRes<bool>> AddShopCategory(int shopId, int categoryId)
        {
            try
            {
                var data = await _context.TShop.Include(x => x.TShopCategory).FirstOrDefaultAsync(x => x.ShopId == shopId);
                if (data == null)
                {
                    return new RepRes<bool>(Message.ShopEditing, false, false);
                }

                if (data.FkPlanId == null)
                {
                    return new RepRes<bool>(Message.FirstGoAndChooseYourPlanToAddCategory, false, false);
                }
                if (data.ExpirationDate.Value.Date < DateTime.Now.Date)
                {
                    return new RepRes<bool>(Message.YourPlanExpire, false, false);
                }
                if (data.TShopCategory.Any(x => x.FkCategoryId == categoryId))
                {
                    return new RepRes<bool>(Message.YouAddThisCategoryBefore, false, false);
                }

                if (data.MaxCategory != null)
                {
                    if (data.MaxCategory <= data.TShopCategory.Count)
                    {
                        return new RepRes<bool>(Message.YouShouldChangeYourPlanToAddCategory, false, false);
                    }
                }


                var category = await _context.TCategory.FindAsync(categoryId);
                var shopCategory = new TShopCategory();
                shopCategory.FkCategoryId = category.CategoryId;
                shopCategory.FkShopId = shopId;
                shopCategory.ContractCommissionFee = ((decimal)(category.CommissionFee * data.CommissionFraction) / 100);

                await _context.TShopCategory.AddAsync(shopCategory);

                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.Successfull, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.ShopEditing, false, false);
            }
        }

        public async Task<ShopCategoryPlanGetDto> GetShopCategory(int shopId)
        {
            try
            {
                var data = await _context.TShop.Where(x => x.ShopId == shopId)
                .Include(x => x.TShopCategory).ThenInclude(t => t.FkCategory)
                .Select(x => new ShopCategoryPlanGetDto()
                {
                    HasPlan = x.FkPlanId == null ? false : true,
                    MaxCategory = x.MaxCategory,
                    ShopCategory = x.TShopCategory.Select(t => new ShopCategoryPlanDto()
                    {
                        CategoryId = t.FkCategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(t.FkCategory.CategoryTitle, header.Language),
                        Commission = t.ContractCommissionFee
                    }).ToList(),
                    Commission = x.CommissionFraction
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

                if (data == null)
                {
                    return null;
                }

                if (data.HasPlan)
                {
                    data.Category = await _context.TCategory.Where(x => x.FkParentId == null && x.IsActive == true && !data.ShopCategory.Select(x => x.CategoryId).ToList().Contains(x.CategoryId))
                    .Select(x => new ShopCategoryPlanDto()
                    {
                        CategoryId = x.CategoryId,
                        CategoryTitle = JsonExtensions.JsonValue(x.CategoryTitle, header.Language),
                        Commission = (decimal)(x.CommissionFee * data.Commission) / 100,
                    }).AsNoTracking().ToListAsync();
                }

                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<TShopPlan>> ShopPlanDelete(int id)
        {
            try
            {
                var data = await _context.TShopPlan.FirstOrDefaultAsync(x => x.PlanId == id);
                if (data == null)
                {
                    return new RepRes<TShopPlan>(Message.ShopPlanGetting, false, null);
                }
                var hasRelation = await _context.TShop.AsNoTracking().AnyAsync(x => x.FkPlanId == id);
                if (hasRelation)
                {
                    return new RepRes<TShopPlan>(Message.ShopPlanCantDelete, false, null);
                }
                var shopPlanExclusive = await _context.TShopPlanExclusive.Where(x => x.FkPlanId == id).ToListAsync();

                _context.TShopPlanExclusive.RemoveRange(shopPlanExclusive);
                _context.TShopPlan.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TShopPlan>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return null;
            }
        }




        public async Task<List<ShopGeneralDto>> GetWebShopList(ShopListWebPaginationDto pagination)
        {
            try
            {

                var shopList = new List<ShopGeneralDto>();
                var setting = await _context.TSetting.FirstOrDefaultAsync();
                var countryDefualt = await _context.TCountry.FirstOrDefaultAsync(x => x.DefualtPreCode == true);
                if (countryDefualt != null)
                {

                    pagination.CountryId = countryDefualt.CountryId;

                }
                if (setting != null)
                {
                    pagination.PageSize = setting.VendorCountInStoreList == null ? 10 : (int)setting.VendorCountInStoreList;
                }
                else
                {
                    pagination.PageSize = 10;
                }

                // var minLat = 0.0;
                // var maxLat = 0.0;
                // var minLon = 0.0;
                // var maxLon = 0.0;

                // if (pagination.Lat != null && pagination.Lng != null)
                // {
                //     var minMilePerLat = 68.703;
                //     var milePerLon = Math.Cos((double)pagination.Lat) * 69.172;
                //     minLat = (double)pagination.Lat - (double)setting.ShopSearchZoneKilometer / minMilePerLat;
                //     maxLat = (double)pagination.Lat + (double)setting.ShopSearchZoneKilometer / minMilePerLat;
                //     minLon = (double)pagination.Lng - (double)setting.ShopSearchZoneKilometer / milePerLon;
                //     maxLon = (double)pagination.Lng + (double)setting.ShopSearchZoneKilometer / milePerLon;
                // }

                if (pagination.Sort == (int)ShopWebFilterEnum.MostReviewed)
                {
                    shopList = _context.TShop
                    .Include(t => t.FkCountry)
                    .Include(t => t.FkProvince)
                    .Include(t => t.FkCity)
                    .Where(x =>
                    (x.FkStatusId == (int)ShopStatusEnum.Active) &&
                     x.Microstore == true &&
                    !string.IsNullOrWhiteSpace(x.VendorUrlid) &&
                    (string.IsNullOrWhiteSpace(pagination.StoreName) ? true : x.StoreName.Contains(pagination.StoreName)) &&
                    (pagination.CategoryId == 0 ? true : x.TShopCategory.Any(c => c.FkCategoryId == pagination.CategoryId)) &&
                    (pagination.CountryId == 0 ? true : x.FkCountry.CountryId == pagination.CountryId) &&
                    (pagination.ProvinceId == 0 ? true : x.FkProvince.ProvinceId == pagination.ProvinceId) &&
                    (pagination.CityId == 0 ? true : x.FkCity.CityId == pagination.CityId)
                    // ((pagination.Lat == null || pagination.Lng == null) ? true :
                    // (minLat <= x.LocationX && x.LocationX <= maxLat) && (minLon <= x.LocationY && x.LocationY <= maxLon)
                    // )

                    )
                    .Include(t => t.TShopComment)
                    .OrderByDescending(x => x.TShopComment.Count())
                    .AsEnumerable()
                    .Select(x => new ShopGeneralDto()
                    {

                        Email = x.Email,
                        Phone = x.Phone,
                        RegisteryDateTime = Extentions.PersianDateString(x.RegisteryDateTime),
                        StoreName = x.StoreName,
                        Address = JsonExtensions.JsonGet(x.Address, header),
                        VendorUrlid = x.VendorUrlid,
                        ProfileImage = x.ProfileImage,
                        LogoImage = x.LogoImage,
                        Iso = x.FkCountry.Iso2,
                        ShopId = x.ShopId,
                        Dist = Extentions.DistanceTo((double)(pagination.Lat == null ? 0 : pagination.Lat),(double)(pagination.Lng == null ? 0 : pagination.Lng), (double)(x.LocationX == null ? 0 : x.LocationX), (double)(x.LocationY == null ? 0 : x.LocationY))
                    })
                    .Where(x => ((pagination.Lat == null || pagination.Lng == null) ? true : x.Dist <= (int)setting.ShopSearchZoneKilometer))
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .ToList();
                }
                else if (pagination.Sort == (int)ShopWebFilterEnum.MostRecent)
                {
                    shopList = _context.TShop
                    .Include(t => t.FkCountry)
                    .Include(t => t.FkProvince)
                    .Include(t => t.FkCity)
                    .Where(x =>
                    (x.FkStatusId == (int)ShopStatusEnum.Active) &&
                    x.Microstore == true &&
                    !string.IsNullOrWhiteSpace(x.VendorUrlid) &&
                    (string.IsNullOrWhiteSpace(pagination.StoreName) ? true : x.StoreName.Contains(pagination.StoreName)) &&
                    (pagination.CategoryId == 0 ? true : x.TShopCategory.Any(c => c.FkCategoryId == pagination.CategoryId)) &&
                    (pagination.CountryId == 0 ? true : x.FkCountry.CountryId == pagination.CountryId) &&
                    (pagination.ProvinceId == 0 ? true : x.FkProvince.ProvinceId == pagination.ProvinceId) &&
                    (pagination.CityId == 0 ? true : x.FkCity.CityId == pagination.CityId) 
                    // ((pagination.Lat == null || pagination.Lng == null) ? true :
                    // (minLat <= x.LocationX && x.LocationX <= maxLat) && (minLon <= x.LocationY && x.LocationY <= maxLon)
                    // )
                    )
                    .AsEnumerable()
                    .Select(x => new ShopGeneralDto()
                    {

                        Email = x.Email,
                        Phone = x.Phone,
                        RegisteryDateTime = Extentions.PersianDateString(x.RegisteryDateTime),
                        StoreName = x.StoreName,
                        Address = JsonExtensions.JsonGet(x.Address, header),
                        VendorUrlid = x.VendorUrlid,
                        ProfileImage = x.ProfileImage,
                        LogoImage = x.LogoImage,
                        Iso = x.FkCountry.Iso2,
                        ShopId = x.ShopId,
                        Dist = Extentions.DistanceTo((double)(pagination.Lat == null ? 0 : pagination.Lat),(double)(pagination.Lng == null ? 0 : pagination.Lng), (double)(x.LocationX == null ? 0 : x.LocationX), (double)(x.LocationY == null ? 0 : x.LocationY))

                    })
                    .Where(x => ((pagination.Lat == null || pagination.Lng == null) ? true : x.Dist <= (int)setting.ShopSearchZoneKilometer))
                    .OrderByDescending(x => x.ShopId)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .ToList();
                }
                else
                {
                    shopList = _context.TShop
                    .Include(t => t.FkCountry)
                    .Include(t => t.FkProvince)
                    .Include(t => t.FkCity)
                    .Where(x =>
                    (x.FkStatusId == (int)ShopStatusEnum.Active) &&
                    x.Microstore == true &&
                    !string.IsNullOrWhiteSpace(x.VendorUrlid) &&
                    (string.IsNullOrWhiteSpace(pagination.StoreName) ? true : x.StoreName.Contains(pagination.StoreName)) &&
                    (pagination.CategoryId == 0 ? true : x.TShopCategory.Any(c => c.FkCategoryId == pagination.CategoryId)) &&
                    (pagination.CountryId == 0 ? true : x.FkCountry.CountryId == pagination.CountryId) &&
                    (pagination.ProvinceId == 0 ? true : x.FkProvince.ProvinceId == pagination.ProvinceId) &&
                    (pagination.CityId == 0 ? true : x.FkCity.CityId == pagination.CityId) 
                    // ((pagination.Lat == null || pagination.Lng == null) ? true :
                    // (minLat <= x.LocationX && x.LocationX <= maxLat) && (minLon <= x.LocationY && x.LocationY <= maxLon)
                    // )
                    )
                    .AsEnumerable()
                    .Select(x => new ShopGeneralDto()
                    {

                        Email = x.Email,
                        Phone = x.Phone,
                        RegisteryDateTime = Extentions.PersianDateString(x.RegisteryDateTime),
                        StoreName = x.StoreName,
                        Address = JsonExtensions.JsonGet(x.Address, header),
                        VendorUrlid = x.VendorUrlid,
                        ProfileImage = x.ProfileImage,
                        LogoImage = x.LogoImage,
                        Iso = x.FkCountry.Iso2,
                        ShopId = x.ShopId,
                        SurveyScore = x.SurveyScore,
                        Dist = Extentions.DistanceTo((double)(pagination.Lat == null ? 0 : pagination.Lat),(double)(pagination.Lng == null ? 0 : pagination.Lng), (double)(x.LocationX == null ? 0 : x.LocationX), (double)(x.LocationY == null ? 0 : x.LocationY))
                    })
                    .Where(x => ((pagination.Lat == null || pagination.Lng == null) ? true : x.Dist <= (double)setting.ShopSearchZoneKilometer))
                    .OrderByDescending(x => x.SurveyScore)
                    .Skip(pagination.PageSize * (pagination.PageNumber - 1)).Take(pagination.PageSize)
                    .ToList();
                }


                return shopList;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<int> GetWebShopListCount(ShopListWebPaginationDto pagination)
        {
            try
            {
                var setting = await _context.TSetting.FirstOrDefaultAsync();


                return  _context.TShop
                .Where(x =>
                (x.FkStatusId == (int)ShopStatusEnum.Active) &&
                x.Microstore == true &&
                !string.IsNullOrWhiteSpace(x.VendorUrlid) &&
                (string.IsNullOrWhiteSpace(pagination.StoreName) ? true : x.StoreName.Contains(pagination.StoreName)) &&
                (pagination.CategoryId == 0 ? true : x.TShopCategory.Any(c => c.FkCategoryId == pagination.CategoryId)) &&
                (pagination.CountryId == 0 ? true : x.FkCountry.CountryId == pagination.CountryId) &&
                (pagination.ProvinceId == 0 ? true : x.FkProvince.ProvinceId == pagination.ProvinceId) &&
                (pagination.CityId == 0 ? true : x.FkCity.CityId == pagination.CityId)
                // ((pagination.Lat == null || pagination.Lng == null) ? true :
                // (minLat <= x.LocationX && x.LocationX <= maxLat) && (minLon <= x.LocationY && x.LocationY <= maxLon)
                // )
                )
                .AsEnumerable()
                .Select(x => new ShopGeneralDto()
                    {
                        Dist = Extentions.DistanceTo((double)(pagination.Lat == null ? 0 : pagination.Lat),(double)(pagination.Lng == null ? 0 : pagination.Lng), (double)(x.LocationX == null ? 0 : x.LocationX), (double)(x.LocationY == null ? 0 : x.LocationY))
                    })
                    .Where(x => ((pagination.Lat == null || pagination.Lng == null) ? true : x.Dist <= (double)setting.ShopSearchZoneKilometer))
                   .Count();
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        public async Task<bool> ChangeShopStatus(AcceptNullDto accept)
        {
            try
            {
                var data = await _context.TShop.Include(x => x.TUser).FirstAsync(c => c.ShopId == accept.Id);
                data.FkStatusId = (short)accept.StatusId;
                await _context.SaveChangesAsync();
                if (data.FkStatusId == (int)ShopStatusEnum.Active)
                {
                    try
                    {
                        await _notificationService.SendNotification((int)NotificationSettingTypeEnum.CompleteRegistrationOrPlanSelectionProvider, data.TUser.FirstOrDefault().ProviderFirebasePushNotificationKey, null, data.Email, data.Phone, data.TUser.FirstOrDefault().UserId);
                    }
                    catch (System.Exception)
                    {

                    }
                }

                if (data.FkStatusId != (int)ShopStatusEnum.Active)
                {
                    var shopOrder = await _context.TOrder.Include(v => v.TOrderItem).ThenInclude(v => v.FkGoods).Include(v => v.FkCustomer).Include(v => v.AdFkCountry).Include(v => v.TPaymentTransaction).Where(x => x.TOrderItem.Any(c => c.FkShopId == accept.Id && c.FkStatusId == (int)OrderStatusEnum.Cart) && x.FkOrderStatusId == (int)OrderStatusEnum.Cart).ToListAsync();
                    var customerIds = new List<int>();
                    foreach (var item in shopOrder)
                    {

                        if (!string.IsNullOrEmpty(item.FkCustomer.MobileNumber))
                            Extentions.SendPodinisSmsForProvider("کابر گرامی ساخت ایران ، کالاهای شما در سبد خرید به علت غیرفعال شدن فروشنده یا عدم موجودی از سبد خرید حذف شدند ، لطفا برای سفارش جدید به سایت ساخت ایران مراجعه فرمایید", item.AdFkCountry.PhoneCode + item.FkCustomer.MobileNumber);

                        if (item.TOrderItem.Count == 1)
                        {
                            _context.TOrderItem.RemoveRange(item.TOrderItem);
                            _context.TPaymentTransaction.RemoveRange(item.TPaymentTransaction);
                            _context.TOrder.Remove(item);

                        }
                        else
                        {
                            _context.TOrderItem.RemoveRange(item.TOrderItem);
                        }


                    }

                    await _context.SaveChangesAsync();

                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<ShopBaseDto> GetShopStoreName(int shopId)
        {
            try
            {
                return await _context.TShop
                .Include(t => t.FkStatus)
                .Select(x => new ShopBaseDto()
                {
                    StoreName = x.StoreName,
                    ShopId = x.ShopId,
                    PlanId = x.FkPlanId,
                    RegisteryDateTime = Extentions.PersianDateString(x.RegisteryDateTime),
                    FkStatusId = x.FkStatusId,
                    VendorUrlid = x.VendorUrlid,
                    StatusTitle = JsonExtensions.JsonValue(x.FkStatus.StatusTitle, header.Language),
                    StatusDesc = JsonExtensions.JsonValue(x.FkStatus.Comment, header.Language),
                    ShopMessage = JsonExtensions.JsonValue(x.InactiveShopMessage, header.Language)
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ShopId == shopId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<RepRes<bool>> EditInactiveShopMessage(ShopDescriptionDto shopDto)
        {
            try
            {
                var data = await _context.TShop.FindAsync(shopDto.ShopId);
                // data.InactiveShopMessage = shopDto.Description;
                data.InactiveShopMessage = JsonExtensions.JsonEdit(shopDto.Description, data.InactiveShopMessage, header);
                await _context.SaveChangesAsync();
                return new RepRes<bool>(Message.ShopEditing, true, true);
            }
            catch (System.Exception)
            {
                return new RepRes<bool>(Message.ShopEditing, false, false);
            }
        }

        public async Task<ShopDescriptionDto> GetInactiveShopMessage(int shopId)
        {
            try
            {
                return await _context.TShop.Select(x => new ShopDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.InactiveShopMessage, header.Language),
                    ShopId = x.ShopId
                })
                .AsNoTracking().FirstOrDefaultAsync(x => x.ShopId == shopId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }




        public async Task<bool> TransactionPlanPayment(TPaymentTransaction paymentTransaction)
        {
            try
            {

                await _context.TPaymentTransaction.AddAsync(paymentTransaction);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }

        public async Task<TPaymentTransaction> planShopDetailsWithPaymentId(string paymentId)
        {
            try
            {
                var paymentTransaction = await _context.TPaymentTransaction.Include(t => t.FkShop).Include(t => t.FkPlan).FirstAsync(x => x.PaymentId == paymentId);
                return paymentTransaction;
            }
            catch (System.Exception)
            {
                return null;
            }

        }

        public async Task<RepRes<TShopCategory>> DeleteShopCategory(List<int> categoryIdWithChild, int parentCategory, int shopId)
        {
            try
            {
                var data = await _context.TShopCategory.FirstOrDefaultAsync(x => x.FkCategoryId == parentCategory && x.FkShopId == shopId);
                var anyGoods = await _context.TGoodsProvider.Include(d => d.FkGoods).AnyAsync(c => categoryIdWithChild.Contains(c.FkGoods.FkCategoryId) && c.FkShopId == shopId);
                if (data != null && anyGoods == false)
                {
                    _context.TShopCategory.Remove(data);
                    await _context.SaveChangesAsync();
                    return new RepRes<TShopCategory>(Message.Successfull, true, data);
                }
                else
                {
                    return new RepRes<TShopCategory>(Message.ShopCategoryCantDelete, false, null);

                }
            }
            catch (System.Exception)
            {
                return new RepRes<TShopCategory>(Message.ShopCategoryCantDelete, false, null);
            }
        }



        public async Task<RepRes<TShop>> ShopDelete(int shopId)
        {
            try
            {
                var data = await _context.TShop.FirstOrDefaultAsync(x => x.ShopId == shopId);
                if (data == null)
                {
                    return new RepRes<TShop>(Message.ShopGetting, false, null);
                }
                var hasRelation = await _context.TGoods.AsNoTracking().AnyAsync(x => x.FkOwnerId == shopId);
                if (hasRelation)
                {
                    return new RepRes<TShop>(Message.ShopCantDelete, false, null);
                }
                var hasRelation2 = await _context.TGoodsProvider.AsNoTracking().AnyAsync(x => x.FkShopId == shopId);
                if (hasRelation2)
                {
                    return new RepRes<TShop>(Message.ShopCantDelete, false, null);
                }
                var hasRelation3 = await _context.TOrderItem.AsNoTracking().AnyAsync(x => x.FkShopId == shopId);
                if (hasRelation3)
                {
                    return new RepRes<TShop>(Message.ShopCantDeleteOrder, false, null);
                }
                var discountPlan = await _context.TDiscountPlan.Where(x => x.FkShopId == shopId).ToListAsync();
                var discountShops = await _context.TDiscountShops.Where(x => x.FkShopId == shopId).ToListAsync();
                var shopActivityCity = await _context.TShopActivityCity.Where(x => x.FkShopId == shopId).ToListAsync();
                var shopActivityCountry = await _context.TShopActivityCountry.Where(x => x.FkShopId == shopId).ToListAsync();
                var shopCategory = await _context.TShopCategory.Where(x => x.FkShopId == shopId).ToListAsync();
                var shopComment = await _context.TShopComment.Where(x => x.FkShopId == shopId).ToListAsync();
                var shopFiles = await _context.TShopFiles.Where(x => x.FkShopId == shopId).ToListAsync();
                var shopSlider = await _context.TShopSlider.Where(x => x.FkShopId == shopId).ToListAsync();
                var shopPayment = await _context.TPaymentTransaction.Where(x => x.FkShopId == shopId).ToListAsync();
                var shopSurveyAnswers = await _context.TShopSurveyAnswers.Where(x => x.FkShopId == shopId).ToListAsync();
                var shopWithdrawalRequest = await _context.TShopWithdrawalRequest.Where(x => x.FkShopId == shopId).ToListAsync();
                var user = await _context.TUser.Include(c => c.TMessageRecipient)
                .Include(c => c.TMessage).ThenInclude(v => v.TMessageAttachment)
                .Include(c => c.TMessage).ThenInclude(v => v.TMessageRecipient)
                .Where(x => x.FkShopId == shopId).ToListAsync();

                _context.TDiscountPlan.RemoveRange(discountPlan);
                _context.TDiscountShops.RemoveRange(discountShops);
                _context.TShopActivityCity.RemoveRange(shopActivityCity);
                _context.TShopActivityCountry.RemoveRange(shopActivityCountry);
                _context.TShopCategory.RemoveRange(shopCategory);
                _context.TShopComment.RemoveRange(shopComment);
                _context.TShopFiles.RemoveRange(shopFiles);
                _context.TShopSlider.RemoveRange(shopSlider);
                _context.TShopSurveyAnswers.RemoveRange(shopSurveyAnswers);
                _context.TShopWithdrawalRequest.RemoveRange(shopWithdrawalRequest);
                _context.TPaymentTransaction.RemoveRange(shopPayment);
                foreach (var item in user)
                {
                    _context.TMessageRecipient.RemoveRange(item.TMessageRecipient);
                    foreach (var message in item.TMessage)
                    {
                        _context.TMessageAttachment.RemoveRange(message.TMessageAttachment);
                        _context.TMessageRecipient.RemoveRange(message.TMessageRecipient);
                    }
                    _context.TMessage.RemoveRange(item.TMessage);
                }
                _context.TUser.RemoveRange(user);
                _context.TShop.Remove(data);
                await _context.SaveChangesAsync();
                return new RepRes<TShop>(Message.Successfull, true, data);
            }
            catch (System.Exception)
            {
                return null;
            }
        }


        public async Task<bool> ChangeAccept(List<AcceptNullDto> accept)
        {
            try
            {
                var shopIds = new List<int>();
                foreach (var item in accept)
                {
                    shopIds.Add(item.Id);
                }
                var data = await _context.TShop.Where(x => shopIds.Contains(x.ShopId)).ToListAsync();
                for (int i = 0; i < data.Count; i++)
                {
                    if (accept[0].Accept == 0)
                    {
                        data[i].FkStatusId = (int)ShopStatusEnum.Disabled;
                        var shopOrder = await _context.TOrder.Include(v => v.TOrderItem).Include(v => v.TPaymentTransaction).Include(v => v.FkCustomer).Include(v => v.AdFkCountry).Where(x => x.TOrderItem.Any(c => c.FkShopId == data[i].ShopId && c.FkStatusId == (int)OrderStatusEnum.Cart) && x.FkOrderStatusId == (int)OrderStatusEnum.Cart).ToListAsync();
                        foreach (var item in shopOrder)
                        {

                            if (!string.IsNullOrEmpty(item.FkCustomer.MobileNumber))
                                Extentions.SendPodinisSmsForProvider("کابر گرامی ساخت ایران ، کالاهای شما در سبد خرید به علت غیرفعال شدن فروشنده یا عدم موجودی از سبد خرید حذف شدند ، لطفا برای سفارش جدید به سایت ساخت ایران مراجعه فرمایید", item.AdFkCountry.PhoneCode + item.FkCustomer.MobileNumber);

                            if (item.TOrderItem.Count == 1)
                            {
                                _context.TOrderItem.RemoveRange(item.TOrderItem);
                                _context.TPaymentTransaction.RemoveRange(item.TPaymentTransaction);
                                _context.TOrder.Remove(item);
                            }
                            else
                            {
                                _context.TOrderItem.RemoveRange(item.TOrderItem);
                            }
                        }

                    }
                    else if (accept[0].Accept == 1)
                    {
                        data[i].FkStatusId = (int)ShopStatusEnum.Active;
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


    }
}