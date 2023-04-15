using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.PaymentMethod;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface ISettingService
    {
        Task<ApiResponse<SettingLogoDto>> EditSettingLogo(SettingSerializeDto settingDto);
        Task<ApiResponse<SettingLogoDto>> GetSettingLogo();

        Task<ApiResponse<SettingCompanyDto>> EditSettingCompany(SettingCompanyDto settingDto);
        Task<ApiResponse<SettingCompanyDto>> GetSettingCompany();

        Task<ApiResponse<SettingSeoDto>> EditSettingSeo(SettingSeoDto settingDto);
        Task<ApiResponse<SettingSeoDto>> GetSettingSeo();

        Task<ApiResponse<SettingSocialDto>> EditSettingSocial(SettingSocialDto settingDto);
        Task<ApiResponse<SettingSocialDto>> GetSettingSocial();

        Task<ApiResponse<SettingGeneralDto>> EditSettingGeneral(SettingGeneralDto settingDto);
        Task<ApiResponse<SettingGeneralDto>> GetSettingGeneral();

        Task<ApiResponse<SettingDescriptionDto>> EditSettingAboutUs(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingAboutUs();

        Task<ApiResponse<SettingDescriptionDto>> EditSettingShortDescription(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingShortDescription();

        Task<ApiResponse<SettingDescriptionDto>> EditSettingWarrantyPolicy(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingWarrantyPolicy();

        Task<ApiResponse<SettingDescriptionDto>> EditSettingTermOfUser(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingTermOfUser();

        Task<ApiResponse<SettingDescriptionDto>> EditSettingTermOfSale(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingTermOfSale();

        Task<ApiResponse<SettingDescriptionDto>> EditSettingPrivacyPolicy(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingPrivacyPolicy();

        Task<ApiResponse<SettingDescriptionDto>> EditSettingCustomerRights(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingCustomerRights();

        Task<ApiResponse<SettingDescriptionDto>> EditSettingRegistrationFinalMessage(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingRegistrationFinalMessage();

        Task<ApiResponse<SettingDescriptionDto>> EditSettingInactiveShopMessage(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingInactiveShopMessage();        
        Task<ApiResponse<SettingDescriptionDto>> EditSettingShopWelcomeMessage(SettingDescriptionDto settingDto);
        Task<ApiResponse<SettingDescriptionDto>> GetSettingShopWelcomeMessage();

        Task<ApiResponse<SettingEmailDto>> EditSettingEmail(SettingEmailDto settingDto);
        Task<ApiResponse<SettingEmailDto>> GetSettingEmail();
        Task<ApiResponse<int>> GeneralMinimumInventory();

        
        Task<ApiResponse<bool>> EditShopPlan( ShopPlanDto shopPlan);
        Task<ApiResponse<bool>> AddShopPlan( ShopPlanDto shopPlan);
        Task<ApiResponse<bool>> ChangeStatusShopPlan( AcceptDto accept);
        Task<ApiResponse<List<ShopPlanDto>>> GetShopPlans();
        Task<ApiResponse<ShopPlanDto>> GetShopPlansOne(int planId);

        
        Task<ApiResponse<bool>> EditPaymentMethod(AcceptDto accept);
        Task<ApiResponse<List<PaymentMethodDto>>> GetPaymentMethod();

        Task<ApiResponse<bool>> ChangeAcceptShipingMethod(AcceptDto accept);
        Task<ApiResponse<bool>> EditShippingMethod(ShippingMethodDto shippingMethod);
        Task<ApiResponse<bool>> EditShippingMethodDesc(ShippingMethodDto shippingMethod);
        Task<ApiResponse<bool>> ChangeShippingMethod(ShippingMethodChangeDto shippingMethod);
        Task<ApiResponse<List<ShippingMethodDto>>> GetShippingMethod();
        Task<ApiResponse<List<ShippingOnCityDto>>> GetShippingMethodCityCost(int shippingMethodId , int provinceId);
        Task<ApiResponse<List<ShippingOnCityDto>>> GetShippingMethodProvinceCost(int shippingMethodId , int countryId);

        Task<ApiResponse<bool>> EditShippingMethodCountryCost(List<ShippingOnCountryDto> shippingMethodOnCountry, int shippingMethodId);
        Task<ApiResponse<bool>> EditShippingMethodCityCost(List<ShippingOnCityDto> shippingMethodOnCity, int shippingMethodId);
        
     
    }
}