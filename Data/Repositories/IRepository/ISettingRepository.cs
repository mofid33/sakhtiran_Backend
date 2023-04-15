using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Data.Dtos.PaymentMethod;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface ISettingRepository
    {
        Task<WebsiteSettingWebDto> WebsiteSettingGet();

        Task<SettingLogoDto> EditSettingLogo(SettingLogoDto settingDto);
        Task<SettingLogoDto> GetSettingLogo();

        Task<SettingCompanyDto> EditSettingCompany(SettingCompanyDto settingDto);
        Task<SettingCompanyDto> GetSettingCompany();

        Task<SettingSeoDto> EditSettingSeo(SettingSeoDto settingDto);
        Task<SettingSeoDto> GetSettingSeo();

        Task<SettingSocialDto> EditSettingSocial(SettingSocialDto settingDto);
        Task<SettingSocialDto> GetSettingSocial();

        Task<SettingGeneralDto> EditSettingGeneral(SettingGeneralDto settingDto);
        Task<SettingGeneralDto> GetSettingGeneral();

        Task<SettingDescriptionDto> EditSettingAboutUs(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetSettingAboutUs();

        Task<SettingDescriptionDto> EditSettingShortDescription(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetSettingShortDescription();

        Task<SettingDescriptionDto> EditSettingWarrantyPolicy(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetSettingWarrantyPolicy();

        Task<SettingDescriptionDto> EditSettingTermOfUser(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetSettingTermOfUser();

        Task<SettingDescriptionDto> EditSettingTermOfSale(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetSettingTermOfSale();

        Task<SettingDescriptionDto> EditSettingPrivacyPolicy(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetSettingPrivacyPolicy();

        Task<SettingDescriptionDto> EditSettingCustomerRights(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetSettingCustomerRights();

        Task<SettingDescriptionDto> EditInactiveShopMessage(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetInactiveShopMessage();        
        Task<SettingDescriptionDto> EditShopWelcomeMessage(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetShopWelcomeMessage();

        Task<SettingDescriptionDto> EditSettingRegistrationFinalMessage(SettingDescriptionDto settingDto);
        Task<SettingDescriptionDto> GetSettingRegistrationFinalMessage();

        Task<SettingEmailDto> EditSettingEmail(SettingEmailDto settingDto);
        Task<SettingEmailDto> GetSettingEmail();


        
        Task<bool> AddShopPlan( TShopPlan shopPlan);
        Task<bool> EditShopPlan( TShopPlan shopPlan);
        Task<List<ShopPlanDto>> GetShopPlans();
        Task<ShopPlanDto> GetShopPlansOne(int planId);
        Task<bool> ChangeStatusShopPlan(AcceptDto accept);

        
        Task<bool> EditPaymentMethod(AcceptDto accept);
        Task<List<PaymentMethodDto>> GetPaymentMethod();

        Task<bool> ChangeAcceptShipingMethod(AcceptDto accept);
        Task<bool> EditShippingMethod(TShippingMethod shippingMethod);
         Task<bool> EditShippingMethodDesc(TShippingMethod shippingMethod);
        Task<List<ShippingMethodDto>> GetShippingMethod();
        Task<bool> ChangeShippingMethod(ShippingMethodChangeDto shippingMethod);
        Task<int> GeneralMinimumInventory();
        Task<TSettingPayment> BankInfGet();
        Task<string> GetDefaultLanguage();
        Task<string> GetDefaultCurrency();
        Task<decimal> GetCurrencyRate();
        Task<int> GetSettingIndexCollectionLastDay();
        Task<PostMethodDto> PostMethod(int shopId, int? cityId,int countryId, int? provinceId);

         Task<bool> EditShippingMethodCountryCost(List<TShippingOnCountry> shippingMethodOnCountry , int shippingMethodId);
         Task<bool> EditShippingMethodCityCost(List<TShippingOnCity> shippingMethodOnCity , int shippingMethodId);
         Task<List<ShippingOnCityDto>> GetShippingMethodProvinceCost(int shippingMethodId , int countryId);
         Task<List<ShippingOnCityDto>> GetShippingMethodCityCost(int shippingMethodId , int provinceId);
         Task<long> GetRandomNumber();
         
    }
}