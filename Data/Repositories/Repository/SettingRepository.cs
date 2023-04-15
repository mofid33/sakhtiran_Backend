using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using AutoMapper;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Header;
using Microsoft.AspNetCore.Http;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.ShopPlan;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.PaymentMethod;
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.Home;
using System;
using System.Globalization;

namespace MarketPlace.API.Data.Repositories.Repository
{
    public class SettingRepository : ISettingRepository
    {
        public MarketPlaceDbContext _context { get; }
        public IMapper _mapper { get; }
        public HeaderParseDto header { get; set; }
        public SettingRepository(MarketPlaceDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            _mapper = mapper;
            header = new HeaderParseDto(httpContextAccessor);
        }

        public async Task<SettingLogoDto> EditSettingLogo(SettingLogoDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(settingDto.LogoUrlLoginPage))
                {
                    data.LogoUrlLoginPage = settingDto.LogoUrlLoginPage;
                }
                if (!string.IsNullOrWhiteSpace(settingDto.LogoUrlShopFooter))
                {
                    data.LogoUrlShopFooter = settingDto.LogoUrlShopFooter;
                }
                if (!string.IsNullOrWhiteSpace(settingDto.LogoUrlShopHeader))
                {
                    data.LogoUrlShopHeader = settingDto.LogoUrlShopHeader;
                }
                if (!string.IsNullOrWhiteSpace(settingDto.CustomerLoginPageBackgroundImage))
                {
                    data.CustomerLoginPageBackgroundImage = settingDto.CustomerLoginPageBackgroundImage;
                }
                if (!string.IsNullOrWhiteSpace(settingDto.HelpPageBackgroundImage))
                {
                    data.HelpPageBackgroundImage = settingDto.HelpPageBackgroundImage;
                }
                if (!string.IsNullOrWhiteSpace(settingDto.ShopDefaultBannerImageUrl))
                {
                    data.ShopDefaultBannerImageUrl = settingDto.ShopDefaultBannerImageUrl;
                }
                await _context.SaveChangesAsync();
                return settingDto;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingLogoDto> GetSettingLogo()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingLogoDto()
                {
                    LogoUrlLoginPage = x.LogoUrlLoginPage,
                    LogoUrlShopFooter = x.LogoUrlShopFooter,
                    LogoUrlShopHeader = x.LogoUrlShopHeader,
                    CustomerLoginPageBackgroundImage = x.CustomerLoginPageBackgroundImage,
                    HelpPageBackgroundImage = x.HelpPageBackgroundImage,
                    ShopDefaultBannerImageUrl = x.ShopDefaultBannerImageUrl
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingCompanyDto> EditSettingCompany(SettingCompanyDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.ShopTitle = JsonExtensions.JsonEdit(settingDto.ShopTitle, data.ShopTitle, header);
                data.Address = JsonExtensions.JsonEdit(settingDto.Address, data.Address, header);
                data.Phone = settingDto.Phone;
                data.Email = settingDto.Email;
                data.Fax = settingDto.Fax;
                data.SupportPhone = settingDto.SupportPhone;

                await _context.SaveChangesAsync();

                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingCompanyDto> GetSettingCompany()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingCompanyDto()
                {
                    Address = JsonExtensions.JsonValue(x.Address, header.Language),
                    ShopTitle = JsonExtensions.JsonValue(x.ShopTitle, header.Language),
                    Email = x.Email,
                    Fax = x.Fax,
                    Phone = x.Phone,
                    SupportPhone = x.SupportPhone
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingSeoDto> EditSettingSeo(SettingSeoDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.MetaDescription = JsonExtensions.JsonEdit(settingDto.MetaDescription, data.MetaDescription, header);
                data.MetaKeyword = JsonExtensions.JsonEdit(settingDto.MetaKeyword, data.MetaKeyword, header);
                data.MetaTitle = JsonExtensions.JsonEdit(settingDto.MetaTitle, data.MetaTitle, header);
                data.PageTitle = JsonExtensions.JsonEdit(settingDto.PageTitle, data.PageTitle, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingSeoDto> GetSettingSeo()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingSeoDto()
                {
                    MetaDescription = JsonExtensions.JsonValue(x.MetaDescription, header.Language),
                    MetaKeyword = JsonExtensions.JsonValue(x.MetaKeyword, header.Language),
                    MetaTitle = JsonExtensions.JsonValue(x.MetaTitle, header.Language),
                    PageTitle = JsonExtensions.JsonValue(x.PageTitle, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingSocialDto> EditSettingSocial(SettingSocialDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.AparatUrl = settingDto.AparatUrl;
                data.FacebookUrl = settingDto.FacebookUrl;
                data.InstagramUrl = settingDto.InstagramUrl;
                data.LinkedinUrl = settingDto.LinkedinUrl;
                data.TelegramUrl = settingDto.TelegramUrl;
                data.TwitterUrl = settingDto.TwitterUrl;

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingSocialDto> GetSettingSocial()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingSocialDto()
                {
                    AparatUrl = x.AparatUrl,
                    FacebookUrl = x.FacebookUrl,
                    InstagramUrl = x.InstagramUrl,
                    LinkedinUrl = x.LinkedinUrl,
                    TelegramUrl = x.TelegramUrl,
                    TwitterUrl = x.TwitterUrl
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingGeneralDto> EditSettingGeneral(SettingGeneralDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.FooterMaxItem = settingDto.FooterMaxItem;
                data.ShopSearchZoneKilometer = settingDto.ShopSearchZoneKilometer;
                data.FooterMaxItemPerColumn = settingDto.FooterMaxItemPerColumn;
                data.IndexCollectionCount = settingDto.IndexCollectionCount;
                data.IndexCollectionLastDay = settingDto.IndexCollectionLastDay;
                data.MaxDeadlineDayToReturning = settingDto.MaxDeadlineDayToReturning;
                data.MaxSliderForShopWebPage = settingDto.MaxSliderForShopWebPage;
                data.DefaultMinimumInventory = settingDto.DefaultMinimumInventory;
                data.RecommendationMaxItemCount = settingDto.RecommendationMaxItemCount;
                data.SysAutoActiveBrand = settingDto.SysAutoActiveBrand;
                data.SysAutoActiveGoods = settingDto.SysAutoActiveGoods;
                data.SysAutoActiveGuarantee = settingDto.SysAutoActiveGuarantee;
                data.SysDisplayCategoriesWithoutGoods = settingDto.SysDisplayCategoriesWithoutGoods;
                data.DashboardTablesRows = settingDto.DashboardTablesRows;
                data.VendorCountInStoreList = settingDto.VendorCountInStoreList;
                data.LiveChatStatus = settingDto.LiveChatStatus;

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingGeneralDto> GetSettingGeneral()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingGeneralDto()
                {
                    FooterMaxItem = x.FooterMaxItem,
                    FooterMaxItemPerColumn = x.FooterMaxItemPerColumn,
                    IndexCollectionCount = x.IndexCollectionCount,
                    ShopSearchZoneKilometer = x.ShopSearchZoneKilometer,
                    IndexCollectionLastDay = x.IndexCollectionLastDay,
                    MaxDeadlineDayToReturning = x.MaxDeadlineDayToReturning,
                    MaxSliderForShopWebPage = x.MaxSliderForShopWebPage,
                    RecommendationMaxItemCount = x.RecommendationMaxItemCount,
                    DefaultMinimumInventory = (int)x.DefaultMinimumInventory,
                    SysAutoActiveBrand = x.SysAutoActiveBrand,
                    SysAutoActiveGoods = x.SysAutoActiveGoods,
                    SysAutoActiveGuarantee = x.SysAutoActiveGuarantee,
                    SysDisplayCategoriesWithoutGoods = x.SysDisplayCategoriesWithoutGoods,
                    DashboardTablesRows = x.DashboardTablesRows,
                    VendorCountInStoreList = (int)x.VendorCountInStoreList,
                    LiveChatStatus = (bool)x.LiveChatStatus
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> EditSettingAboutUs(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.AboutUs = JsonExtensions.JsonEdit(settingDto.Description, data.AboutUs, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetSettingAboutUs()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.AboutUs, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> EditSettingShortDescription(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.ShortDescription = JsonExtensions.JsonEdit(settingDto.Description, data.ShortDescription, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetSettingShortDescription()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.ShortDescription, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> EditSettingWarrantyPolicy(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.FooterWarrantyPolicy = JsonExtensions.JsonEdit(settingDto.Description, data.FooterWarrantyPolicy, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetSettingWarrantyPolicy()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.FooterWarrantyPolicy, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> EditSettingTermOfUser(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.FooterTermOfUser = JsonExtensions.JsonEdit(settingDto.Description, data.FooterTermOfUser, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetSettingTermOfUser()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.FooterTermOfUser, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> EditSettingTermOfSale(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.FooterTermOfSale = JsonExtensions.JsonEdit(settingDto.Description, data.FooterTermOfSale, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetSettingTermOfSale()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.FooterTermOfSale, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> EditSettingPrivacyPolicy(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.FooterPrivacyPolicy = JsonExtensions.JsonEdit(settingDto.Description, data.FooterPrivacyPolicy, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetSettingPrivacyPolicy()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.FooterPrivacyPolicy, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> EditSettingCustomerRights(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.FooterCustomerRights = JsonExtensions.JsonEdit(settingDto.Description, data.FooterCustomerRights, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetSettingCustomerRights()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.FooterCustomerRights, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> EditInactiveShopMessage(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.InactiveShopMessage = JsonExtensions.JsonEdit(settingDto.Description, data.InactiveShopMessage, header);
                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetInactiveShopMessage()
        {
            try
            {
                var settingData = await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = x.InactiveShopMessage
                }).AsNoTracking().FirstOrDefaultAsync();
                settingData.Description = JsonExtensions.JsonGet(settingData.Description, header);
                return settingData;
            }

            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> EditShopWelcomeMessage(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.ShopWelcomeMessage = JsonExtensions.JsonEdit(settingDto.Description, data.ShopWelcomeMessage, header);
                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetShopWelcomeMessage()
        {
            try
            {
                var settingData = await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = x.ShopWelcomeMessage
                }).AsNoTracking().FirstOrDefaultAsync();
                settingData.Description = JsonExtensions.JsonGet(settingData.Description, header);
                return settingData;
            }

            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingEmailDto> EditSettingEmail(SettingEmailDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.SmtpHost = settingDto.SmtpHost;
                data.SmtpPassword = settingDto.SmtpPassword;
                data.SmtpUsername = settingDto.SmtpUsername;

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingEmailDto> GetSettingEmail()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingEmailDto()
                {
                    SmtpHost = x.SmtpHost,
                    SmtpPassword = x.SmtpPassword,
                    SmtpUsername = x.SmtpUsername
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddShopPlan(TShopPlan shopPlan)
        {
            try
            {
                shopPlan.PlanTitle = JsonExtensions.JsonAdd(shopPlan.PlanTitle, header);
                shopPlan.Desription = JsonExtensions.JsonAdd(shopPlan.Desription, header);
                if (shopPlan.RentPerMonth != null)
                {
                    var rate = (decimal)1.00;
                    if (  header.CurrencyNum != CurrencyEnum.TMN)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                        rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                    }
                    shopPlan.RentPerMonth = shopPlan.RentPerMonth / rate;
                }
                await _context.TShopPlan.AddAsync(shopPlan);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> EditShopPlan(TShopPlan shopPlan)
        {
            try
            {

                var data = await _context.TShopPlan.FirstAsync(x => x.PlanId == shopPlan.PlanId);
                shopPlan.Exclusive = data.Exclusive;
                var shopPlanExclusive = await _context.TShopPlanExclusive.Include(c => c.FkShop).Where(x => x.FkPlanId == shopPlan.PlanId).ToListAsync();
                shopPlan.PlanTitle = JsonExtensions.JsonEdit(shopPlan.PlanTitle, data.PlanTitle, header);
                shopPlan.Desription = JsonExtensions.JsonEdit(shopPlan.Desription, data.Desription, header);
                if (shopPlan.RentPerMonth != null)
                {
                    var rate = (decimal)1.00;
                    if (  header.CurrencyNum != CurrencyEnum.TMN)
                    {
                        var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                        rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                    }
                    shopPlan.RentPerMonth = shopPlan.RentPerMonth / rate;
                }
                if (shopPlan.Exclusive == true)
                {
                    var allShopPlanExclusive = await _context.TShopPlanExclusive.Include(c => c.FkShop).Where(x => x.FkPlanId == shopPlan.PlanId).ToListAsync();
                    foreach (var item in shopPlan.TShopPlanExclusive)
                    {
                        var result = shopPlanExclusive.FirstOrDefault(c => c.FkPlanId == shopPlan.PlanId && c.FkShopId == item.FkShopId);
                        if (result != null)
                        {
                            allShopPlanExclusive.Remove(result);
                        }
                        item.FkPlanId = shopPlan.PlanId;
                    }
                    foreach (var item in allShopPlanExclusive)
                    {
                        var currentShop = await _context.TShop.FirstOrDefaultAsync(x => x.ShopId == item.FkShopId && x.FkPlanId == item.FkPlanId);
                        if (currentShop != null)
                        {
                            currentShop.FkPlanId = null;
                            currentShop.RentPerMonth = null;
                            currentShop.CommissionFraction = null;
                            currentShop.Microstore = null;
                            currentShop.MaxProduct = null;
                            currentShop.MaxCategory = null;
                            currentShop.ExpirationDate = null;
                            currentShop.FkStatusId = (int)ShopStatusEnum.Pending;
                        }
                    }
                    _context.TShopPlanExclusive.RemoveRange(shopPlanExclusive);
                    _context.TShopPlanExclusive.AddRange(shopPlan.TShopPlanExclusive);
                }
                _context.Entry(data).CurrentValues.SetValues(shopPlan);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<ShopPlanDto>> GetShopPlans()
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TShopPlan
                .Include(x => x.TShop)
                .Select(x => new ShopPlanDto()
                {
                    Desription = JsonExtensions.JsonValue(x.Desription, header.Language),
                    PlanTitle = JsonExtensions.JsonValue(x.PlanTitle, header.Language),
                    PlanId = x.PlanId,
                    Status = x.Status,
                    PercentOfCommission = x.PercentOfCommission,
                    MaxCategory = x.MaxCategory,
                    MaxProduct = x.MaxProduct,
                    Microstore = x.Microstore,
                    Exclusive = (bool)x.Exclusive,
                    RentPerMonth = decimal.Round((decimal)x.RentPerMonth  / rate, 2, MidpointRounding.AwayFromZero),
                    VendorCount = x.TShop.Where(x => x.FkStatusId == (int)ShopStatusEnum.Active).Count()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<ShopPlanDto> GetShopPlansOne(int planId)
        {
            try
            {
                var rate = (decimal)1.00;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TShopPlan
                    .Select(x => new ShopPlanDto()
                    {
                        Desription = JsonExtensions.JsonValue(x.Desription, header.Language),
                        PlanTitle = JsonExtensions.JsonValue(x.PlanTitle, header.Language),
                        PlanId = x.PlanId,
                        Status = x.Status,
                        PercentOfCommission = x.PercentOfCommission,
                        MaxCategory = x.MaxCategory,
                        MaxProduct = x.MaxProduct,
                        Microstore = x.Microstore,
                        Exclusive = (bool)x.Exclusive,
                        RentPerMonth = decimal.Round((decimal)x.RentPerMonth  / rate, 2, MidpointRounding.AwayFromZero),
                        TShopPlanExclusive = x.TShopPlanExclusive.Select(c => new ShopPlanExclusiveDto
                        {
                            ShopPlanExclusiveId = c.ShopPlanExclusiveId,
                            FkShopId = c.FkShopId,
                            FkPlanId = c.FkPlanId
                        }).ToList()
                    })
                    .AsNoTracking().FirstOrDefaultAsync(x => x.PlanId == planId);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> EditPaymentMethod(AcceptDto accept)
        {
            try
            {
                var data = await _context.TPaymentMethod.FindAsync(accept.Id);
                data.Active = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<List<PaymentMethodDto>> GetPaymentMethod()
        {
            try
            {
                return await _context.TPaymentMethod
                .Select(x => new PaymentMethodDto()
                {
                    Active = x.Active,
                    MethodId = x.MethodId,
                    MethodImageUrl = x.MethodImageUrl,
                    MethodTitle = JsonExtensions.JsonValue(x.MethodTitle, header.Language)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ChangeAcceptShipingMethod(AcceptDto accept)
        {
            try
            {
                if (accept.Accept == false)
                {
                    var existInShop = await _context.TShopActivityCity.AsNoTracking().AnyAsync(x => x.FkShippingMethodId == accept.Id);
                    if (existInShop == true)
                    {
                        return false;
                    }
                    existInShop = await _context.TShopActivityCountry.AsNoTracking().AnyAsync(x => x.FkShippingMethodId == accept.Id);
                    if (existInShop == true)
                    {
                        return false;
                    }
                }
                var data = await _context.TShippingMethod.FindAsync(accept.Id);
                data.Active = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> EditShippingMethod(TShippingMethod shippingMethod)
        {
            try
            {
                var data = await _context.TShippingMethod
                .Include(x => x.TShippingOnCountry)
                .FirstOrDefaultAsync(x => x.Id == shippingMethod.Id);
                data.BaseWeight = shippingMethod.BaseWeight;
                data.CashOnDelivery = shippingMethod.CashOnDelivery;
                data.HaveOnlineService = shippingMethod.HaveOnlineService;


                _context.TShippingOnCountry.RemoveRange(data.TShippingOnCountry);
                await _context.SaveChangesAsync();

                await _context.TShippingOnCountry.AddRangeAsync(shippingMethod.TShippingOnCountry);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> EditShippingMethodCountryCost(List<TShippingOnCountry> shippingMethodOnCountry, int shippingMethodId)
        {
            try
            {
                var data = await _context.TShippingOnCountry
                .Where(x => x.FkShippingMethodId == shippingMethodId).ToArrayAsync();
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar })
                    .AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }




                foreach (var item in data)
                {
                    foreach (var shippingMethod in shippingMethodOnCountry)
                    {
                        if (item.Id == shippingMethod.Id)
                        {
                            if (shippingMethod.ShippingPriceFewerBaseWeight != null)
                            {
                                item.ShippingPriceFewerBaseWeight = shippingMethod.ShippingPriceFewerBaseWeight / rate;
                            }
                            if (shippingMethod.ShippingPriceMoreBaseWeight != null)
                            {
                                item.ShippingPriceMoreBaseWeight = shippingMethod.ShippingPriceMoreBaseWeight / rate;
                            }
                            if (shippingMethod.PostTimeoutDay != null)
                            {
                                item.PostTimeoutDay = shippingMethod.PostTimeoutDay;
                            }
                        }

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

        public async Task<bool> EditShippingMethodCityCost(List<TShippingOnCity> shippingMethodOnCity, int shippingMethodId)
        {
            try
            {
                var checkCity = shippingMethodOnCity.Any(c => c.FkCityId != null);
                var data = new List<TShippingOnCity>();
                if (checkCity)
                {
                    data = await _context.TShippingOnCity
                   .Where(x => x.FkShippingMethodId == shippingMethodId &&
                    shippingMethodOnCity.Select(t => t.FkCityId).ToList().Contains(x.FkCityId)).ToListAsync();
                }
                else
                {
                    data = await _context.TShippingOnCity
                    .Where(x => x.FkShippingMethodId == shippingMethodId &&
                    shippingMethodOnCity.Select(t => t.FkProviceId).ToList().Contains(x.FkProviceId))
                    .ToListAsync();
                }
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar })
                    .AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                _context.TShippingOnCity.RemoveRange(data);
                await _context.SaveChangesAsync();

                foreach (var shippingMethod in shippingMethodOnCity)
                {
                    shippingMethod.Id = 0;
                    if (shippingMethod.ShippingPriceFewerBaseWeight != null)
                    {
                        shippingMethod.ShippingPriceFewerBaseWeight = shippingMethod.ShippingPriceFewerBaseWeight / rate;
                    }
                    if (shippingMethod.ShippingPriceMoreBaseWeight != null)
                    {
                        shippingMethod.ShippingPriceMoreBaseWeight = shippingMethod.ShippingPriceMoreBaseWeight / rate;
                    }
                }

                await _context.TShippingOnCity.AddRangeAsync(shippingMethodOnCity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<bool> EditShippingMethodDesc(TShippingMethod shippingMethod)
        {
            try
            {
                var data = await _context.TShippingMethod
                .FirstOrDefaultAsync(x => x.Id == shippingMethod.Id);
                data.Description = JsonExtensions.JsonEdit(shippingMethod.Description, data.Description, header);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }



        public async Task<List<ShippingMethodDto>> GetShippingMethod()
        {
            try
            {
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TShippingMethod
                .Include(x => x.TShippingOnCountry).ThenInclude(i => i.FkCountry)
                .Select(x => new ShippingMethodDto()
                {
                    Active = x.Active,
                    BaseWeight = x.BaseWeight,
                    CashOnDelivery = x.CashOnDelivery,
                    HaveOnlineService = x.HaveOnlineService,
                    Id = x.Id,
                    Image = JsonExtensions.JsonValue(x.Image, header.Language),
                    Description = JsonExtensions.JsonValue(x.Description, header.Language),
                    ShippingMethodTitle = JsonExtensions.JsonValue(x.ShippingMethodTitle, header.Language),
                    TShippingOnCountry = x.TShippingOnCountry.Select(t => new ShippingOnCountryDto()
                    {
                        CountryTitle = JsonExtensions.JsonValue(t.FkCountry.CountryTitle, header.Language),
                        FkCountryId = t.FkCountryId,
                        FkShippingMethodId = t.FkShippingMethodId,
                        Id = t.Id,
                        PostTimeoutDay = (int)t.PostTimeoutDay,
                        ShippingPriceFewerBaseWeight = Extentions.DecimalRound((decimal)t.ShippingPriceFewerBaseWeight  / rate),
                        ShippingPriceMoreBaseWeight = Extentions.DecimalRound((decimal)t.ShippingPriceMoreBaseWeight  / rate)
                    }).ToList()
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ShippingOnCityDto>> GetShippingMethodProvinceCost(int shippingMethodId, int countryId)
        {
            try
            {
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TProvince
                .Include(x => x.TShippingOnCity)
                .Where(b => b.FkCountryId == countryId)
                .Select(x => new ShippingOnCityDto()
                {
                    Id = x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId) == null ? 0 :
                         x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId).Id,
                    FkShippingMethodId = shippingMethodId,
                    FkProviceId = x.ProvinceId,
                    ProvinceTitle = JsonExtensions.JsonValue(x.ProvinceName, header.Language),
                    FkCityId = null,
                    PostTimeoutDay = x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId) == null ? 0 :
                         x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId).PostTimeoutDay,
                    ShippingPriceFewerBaseWeight = Extentions.DecimalRound((decimal)(x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId) == null ? (decimal)0 :
                         x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId).ShippingPriceFewerBaseWeight)  / rate),
                    ShippingPriceMoreBaseWeight = Extentions.DecimalRound((decimal)(x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId) == null ? (decimal)0 :
                         x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId).ShippingPriceMoreBaseWeight)  / rate)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<List<ShippingOnCityDto>> GetShippingMethodCityCost(int shippingMethodId, int provinceId)
        {
            try
            {
                var rate = (decimal)1.0;
                if (  header.CurrencyNum != CurrencyEnum.TMN)
                {
                    var currency = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.CurrencyNum.ToString());
                    rate = currency == null ? (decimal)1.0 : (decimal)currency.RatesAgainstOneDollar;
                }
                return await _context.TCity
                .Include(x => x.TShippingOnCity)
                .Where(b => b.FkProvinceId == provinceId)
                .Select(x => new ShippingOnCityDto()
                {
                    Id = x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId) == null ? 0 :
                         x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId).Id,
                    FkShippingMethodId = shippingMethodId,
                    FkProviceId = provinceId,
                    FkCityId = x.CityId,
                    CityTitle = JsonExtensions.JsonValue(x.CityTitle, header.Language),
                    PostTimeoutDay = x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId) == null ? 0 :
                         x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId).PostTimeoutDay,
                    ShippingPriceFewerBaseWeight = Extentions.DecimalRound((decimal)(x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId) == null ? (decimal)0 :
                         x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId).ShippingPriceFewerBaseWeight)  / rate),
                    ShippingPriceMoreBaseWeight = Extentions.DecimalRound((decimal)(x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId) == null ? (decimal)0 :
                         x.TShippingOnCity.FirstOrDefault(v => v.FkShippingMethodId == shippingMethodId).ShippingPriceMoreBaseWeight)  / rate)
                })
                .AsNoTracking().ToListAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<bool> ChangeShippingMethod(ShippingMethodChangeDto shippingMethod)
        {
            try
            {
                var City = await _context.TShopActivityCity.AsNoTracking().Where(x => x.FkShippingMethodId == shippingMethod.OldId).ToListAsync();
                var Country = await _context.TShopActivityCountry.AsNoTracking().Where(x => x.FkShippingMethodId == shippingMethod.OldId).ToListAsync();

                foreach (var item in City)
                {
                    item.FkShippingMethodId = shippingMethod.NewId;
                }
                foreach (var item in Country)
                {
                    item.FkShippingMethodId = shippingMethod.NewId;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }


        public async Task<TSettingPayment> BankInfGet()
        {
            try
            {
                return await _context.TSettingPayment.AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<WebsiteSettingWebDto> WebsiteSettingGet()
        {
            try
            {
                var data = await _context.TSetting
                .Select(x => new WebsiteSettingWebDto()
                {
                    SettingId = x.SettingId,
                    IndexCollectionCount = x.IndexCollectionCount,
                    IndexCollectionLastDay = x.IndexCollectionLastDay,
                    RecommendationMaxItemCount = x.RecommendationMaxItemCount,
                    ShopTitle = JsonExtensions.JsonValue(x.ShopTitle, header.Language),
                    InstagramUrl = x.InstagramUrl,
                    FacebookUrl = x.FacebookUrl,
                    TelegramUrl = x.TelegramUrl,
                    AparatUrl = x.AparatUrl,
                    LinkedinUrl = x.LinkedinUrl,
                    TwitterUrl = x.TwitterUrl,
                    Phone = x.Phone,
                    SupportPhone = x.SupportPhone,
                    Fax = x.Fax,
                    Email = x.Email,
                    LiveChatStatus = (bool)x.LiveChatStatus,
                    Address = JsonExtensions.JsonValue(x.Address, header.Language),
                    LogoUrlShopHeader = x.LogoUrlShopHeader,
                    LogoUrlShopFooter = x.LogoUrlShopFooter,
                    LogoUrlLoginPage = x.LogoUrlLoginPage,
                    MetaTitle = JsonExtensions.JsonValue(x.MetaTitle, header.Language),
                    MetaDescription = JsonExtensions.JsonValue(x.MetaDescription, header.Language),
                    MetaKeyword = JsonExtensions.JsonValue(x.MetaKeyword, header.Language),
                    PageTitle = JsonExtensions.JsonValue(x.PageTitle, header.Language),
                    DescriptionCalculateShopRatePrice = JsonExtensions.JsonValue(x.ShopCalculateComment, header.Language),
                    FooterMaxItem = x.FooterMaxItem,
                    FooterMaxItemPerColumn = x.FooterMaxItemPerColumn
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
                return data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<string> GetDefaultLanguage()
        {
            try
            {
                var data = await _context.TLanguage.Select(x => new { x.DefaultLanguage, x.LanguageCode }).AsNoTracking().FirstOrDefaultAsync(x => x.DefaultLanguage == true);
                return data.LanguageCode;
            }
            catch (System.Exception)
            {
                return LanguageEnum.En.ToString();
            }
        }

        public async Task<string> GetDefaultCurrency()
        {
            try
            {
                var data = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.DefaultCurrency }).AsNoTracking().FirstOrDefaultAsync(x => x.DefaultCurrency == true);
                return data.CurrencyCode;
            }
            catch (System.Exception)
            {
                return CurrencyEnum.USD.ToString();
            }
        }

        public async Task<decimal> GetCurrencyRate()
        {
            try
            {
                if (header.CurrencyNum == CurrencyEnum.USD && header.CurrencyNum == CurrencyEnum.TMN)
                {
                    return (decimal)1.0;
                }
                var data = await _context.TCurrency.Select(x => new { x.CurrencyCode, x.RatesAgainstOneDollar }).AsNoTracking().FirstOrDefaultAsync(x => x.CurrencyCode == header.Currency);
                if (data == null)
                {
                    return (decimal)1.0;
                }
                return (decimal)data.RatesAgainstOneDollar;
            }
            catch (System.Exception)
            {
                return (decimal)1.0;
            }
        }

        public async Task<int> GetSettingIndexCollectionLastDay()
        {
            try
            {
                return await _context.TSetting.Select(x => x.IndexCollectionLastDay).FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return 10;
            }
        }
        public async Task<PostMethodDto> PostMethod(int shopId, int? cityId, int countryId, int? provinceId)
        {
            try
            {
                CultureInfo ci = new CultureInfo("en-US");
                if (header.Language == "$." + LanguageEnum.Ar.ToString())
                {
                    ci = new CultureInfo("ar-BH");
                }
                if (header.Language == "$." + LanguageEnum.Fa.ToString())
                {
                    ci = new CultureInfo("fa-IR");
                }
                if (cityId != null)
                {
                    var data = await _context.TShopActivityCity.Include(c => c.FkShippingMethod).AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == shopId && x.FkCityId == cityId);
                    var post = new PostMethodDto();
                    if (data == null)
                    {
                        var provinces = await _context.TShopActivityCity.Include(c => c.FkShippingMethod).AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == shopId && x.FkProviceId == provinceId);
                        if (provinces == null)
                        {
                            // post.PostMethodType = (int)PostMethodEnum.NotPossible;
                            var dataCountry = await _context.TShopActivityCountry.Include(c => c.FkShippingMethod).AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == shopId && x.FkCountryId == countryId);
                            var postCountry = new PostMethodDto();
                            if (dataCountry == null)
                            {
                                postCountry.PostMethodType = (int)PostMethodEnum.NotPossible;
                            }
                            else
                            {
                                postCountry.PostMethodType = dataCountry.FkShippingMethodId;
                                postCountry.ShippingMethodImage = JsonExtensions.JsonGet(dataCountry.FkShippingMethod.Image, header);
                                postCountry.ShippingMethodDesc = JsonExtensions.JsonGet(dataCountry.FkShippingMethod.Description, header);
                                if (postCountry.PostMethodType == (int)ShippingMethodEnum.Podinis)
                                {
                                    var timeCountry = await _context.TShippingOnCountry.FirstOrDefaultAsync(c => c.FkCountryId == countryId);
                                    var postTimeoutDay = timeCountry == null ? 1 : timeCountry.PostTimeoutDay;
                                    post.PostTimeoutDay = DateTime.Now.AddDays((double)postTimeoutDay).ToString("dddd,MMMM dd", ci);

                                }
                                else
                                {

                                    postCountry.PostTimeoutDay = null;
                                }
                            }
                            return postCountry;
                        }
                        else
                        {
                            post.PostMethodType = provinces.FkShippingMethodId;
                            post.ShippingMethodImage = JsonExtensions.JsonGet(provinces.FkShippingMethod.Image, header);
                            post.ShippingMethodDesc = JsonExtensions.JsonGet(provinces.FkShippingMethod.Description, header);
                            if (post.PostMethodType == (int)ShippingMethodEnum.Podinis)
                            {
                                var timeCountry = await _context.TShippingOnCity.FirstOrDefaultAsync(c => c.FkProviceId == provinceId);
                                var postTimeoutDay = timeCountry == null ? 1 : timeCountry.PostTimeoutDay;
                                post.PostTimeoutDay = DateTime.Now.AddDays((double)postTimeoutDay).ToString("dddd,MMMM dd", ci);

                            }
                            else
                            {

                                post.PostTimeoutDay = provinces.FkShippingMethod.HaveOnlineService == false ? DateTime.Now.AddDays(provinces.PostTimeoutDayByShop == null ? 1 : (double)provinces.PostTimeoutDayByShop).ToString("dddd,MMMM dd", ci) : null;
                            }
                        }
                    }
                    else
                    {
                        post.PostMethodType = data.FkShippingMethodId;
                        post.ShippingMethodImage = JsonExtensions.JsonGet(data.FkShippingMethod.Image, header);
                        post.ShippingMethodDesc = JsonExtensions.JsonGet(data.FkShippingMethod.Description, header);
                        if (post.PostMethodType == (int)ShippingMethodEnum.Podinis)
                        {
                            var timeCountry = await _context.TShippingOnCity.FirstOrDefaultAsync(c => c.FkCityId == cityId);
                            var postTimeoutDay = timeCountry == null ? 1 : timeCountry.PostTimeoutDay;
                            post.PostTimeoutDay = DateTime.Now.AddDays((double)postTimeoutDay).ToString("dddd,MMMM dd", ci);

                        }
                        else
                        {
                            post.PostTimeoutDay = data.FkShippingMethod.HaveOnlineService == false ? DateTime.Now.AddDays(data.PostTimeoutDayByShop == null ? 1 : (double)data.PostTimeoutDayByShop).ToString("dddd,MMMM dd", ci) : null;
                        }
                    }
                    return post;
                }
                else
                {
                    var data = await _context.TShopActivityCountry.Include(c => c.FkShippingMethod).AsNoTracking().FirstOrDefaultAsync(x => x.FkShopId == shopId && x.FkCountryId == countryId);
                    var post = new PostMethodDto();
                    if (data == null)
                    {
                        post.PostMethodType = (int)PostMethodEnum.NotPossible;
                    }
                    else
                    {
                        post.PostMethodType = data.FkShippingMethodId;
                        post.ShippingMethodImage = JsonExtensions.JsonGet(data.FkShippingMethod.Image, header);
                        post.ShippingMethodDesc = JsonExtensions.JsonGet(data.FkShippingMethod.Description, header);
                        if (post.PostMethodType == (int)ShippingMethodEnum.Podinis)
                        {
                            var timeCountry = await _context.TShippingOnCountry.FirstOrDefaultAsync(c => c.FkCountryId == countryId);
                            var postTimeoutDay = timeCountry == null ? 1 : timeCountry.PostTimeoutDay;
                            post.PostTimeoutDay = DateTime.Now.AddDays((double)postTimeoutDay).ToString("dddd,MMMM dd", ci);
                        }
                        else
                        {
                            post.PostTimeoutDay = null;
                        }
                    }
                    return post;
                }
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        public async Task<bool> ChangeStatusShopPlan(AcceptDto accept)
        {
            try
            {
                var data = await _context.TShopPlan.FindAsync(accept.Id);
                data.Status = accept.Accept;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }


        public async Task<int> GeneralMinimumInventory()
        {
            try
            {
                var data = await _context.TSetting.Select(x => new SettingGeneralDto()
                {
                    DefaultMinimumInventory = (int)x.DefaultMinimumInventory
                }).AsNoTracking().FirstOrDefaultAsync();
                return data.DefaultMinimumInventory;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }


        public async Task<SettingDescriptionDto> EditSettingRegistrationFinalMessage(SettingDescriptionDto settingDto)
        {
            try
            {
                var data = await _context.TSetting.FirstOrDefaultAsync();
                data.RegistrationFinalMessage = JsonExtensions.JsonEdit(settingDto.Description, data.RegistrationFinalMessage, header);

                await _context.SaveChangesAsync();
                return settingDto;

            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<SettingDescriptionDto> GetSettingRegistrationFinalMessage()
        {
            try
            {
                return await _context.TSetting.Select(x => new SettingDescriptionDto()
                {
                    Description = JsonExtensions.JsonValue(x.RegistrationFinalMessage, header.Language)
                }).AsNoTracking().FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public async Task<long> GetRandomNumber()
        {
            try
            {
                var randomCode = await _context.TCodeRepository.Where(x => x.CodeLength == 10).FirstOrDefaultAsync();
                var code = randomCode.DiscountCode;
                _context.Remove(randomCode);
                await _context.SaveChangesAsync();
                return Int64.Parse(code);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }
    }
}

