using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Image;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IShopService
    {
        Task<ApiResponse<Pagination<ShopListGetDto>>> GetShopList(ShopListPaginationDto pagination);
        Task<ApiResponse<ShopGeneralDto>> GetShopGeneralDetail(int shopId);
        Task<ApiResponse<ShopBaseDto>> GetShopStoreName(int shopId);
        Task<ApiResponse<ShopGeneralDto>> EditShopGeneralDetail(ShopGeneralDto shopDto);
        Task<ApiResponse<ShopBalanceGetDto>> GetShopBalance(ShopBalancePagination pagination);
        Task<ApiResponse<bool>> EditShopDescription(ShopDescriptionDto shopDto);
        Task<ApiResponse<bool>> EditShopTermsAndConditions(ShopDescriptionDto shopDto);
        Task<ApiResponse<ShopDescriptionDto>> GetShopDescription(int shopId);
        Task<ApiResponse<ShopDescriptionDto>> GetShopTermsAndConditions(int shopId);
        Task<ApiResponse<List<ShopFilesGetDto>>> GetShopDocument(int shopId);
        Task<ApiResponse<List<ShopFilesGetDto>>> EditShopDocument(int shopId,ShopSerializeDto shopDto);

        Task<ApiResponse<ShopProfileDto>> GetShopProfile(int shopId);
        Task<ApiResponse<ShopProfileDto>> EditShopProfile(ShopProfileSerializeDto shopDto);
        Task<ApiResponse<bool>> EditShopSetting(ShopSetting shopDto);
        Task<ApiResponse<ShopSetting>> GetShopSetting(int shopId);


        Task<ApiResponse<ShopBankInformationDto>> EditShopBankInformation(ShopSerializeDto shopDto);
        Task<ApiResponse<ShopBankInformationDto>> GetShopBankInformation(int shopId);

        Task<ApiResponse<ShopTaxDto>> EditShopTax(ShopSerializeDto shopDto);
        Task<ApiResponse<ShopTaxDto>> GetShopTax(int shopId);



        Task<ApiResponse<bool>> EditShopActivityCountry(ShopActivityCountryEditDto shopDto);
        Task<ApiResponse<Pagination<ShopActivityCountryGetDto>>> GetShopActivityCountry(PaginationFormDto pagination);

        Task<ApiResponse<bool>> EditShopActivityCity(ShopActivityCityEditDto shopDto);
        Task<ApiResponse<Pagination<ShopActivityCityGetDto>>> GetShopActivityCity(PaginationFormDto pagination , int provinceId);

        Task<ApiResponse<bool>> EditShopActivityProvince(ShopActivityCityEditDto shopDto);
        Task<ApiResponse<Pagination<ShopActivityCityGetDto>>> GetShopActivityProvince(PaginationFormDto pagination);
        Task<ApiResponse<List<ProvinceFormDto>>> GetAllShopProvince();

        Task<ApiResponse<ShopSliderDto>> AddShopSlider(UploadImageDto shopDto);
        Task<ApiResponse<bool>> DeleteShopSlider(int shopId);
        Task<ApiResponse<List<ShopSliderDto>>> GetShopSlider(int shopId);
        Task<ApiResponse<bool>> ChangeShopSliderStatus(AcceptDto accept);


        Task<ApiResponse<bool>> EditShopPlan(int shopId,int planId);
        Task<ApiResponse<List<PlanShopDto>>> GetShopPlan(int shopId);
        Task<ApiResponse<bool>> ShopPlanDelete(int planId);

        Task<ApiResponse<ShopStatisticsDto>> GetShopStatistics(int shopId);
        Task<ApiResponse<string>> GetShopUserName(int shopId);
        Task<ApiResponse<ShopAccessDto>> CheckShopAccess();
        Task<ApiResponse<ShopCategoryPlanGetDto>> GetShopCategory(int shopId);
        Task<ApiResponse<bool>> AddShopCategory(int shopId, int categoryId);

        Task<ApiResponse<Pagination<ShopGeneralDto>>> GetWebShopList(ShopListWebPaginationDto pagination);


        Task<ApiResponse<bool>> ChangeShopStatus(AcceptNullDto accept);


        Task<ApiResponse<bool>> EditInactiveShopMessage(ShopDescriptionDto shopDto);
        Task<ApiResponse<ShopDescriptionDto>> GetInactiveShopMessage(int shopId);

        Task<ApiResponse<string>> InitPlanPayment(ShopPlanPaymentDto shopPlanPaymentDto);
        Task<ApiResponse<bool>> PayPlanPayment(string refId, string resCode, string saleId, string saleReferenceId);
        Task<ApiResponse<bool>> GetStatusPlanPayment(string paymentId, string payerId);

        Task<ApiResponse<bool>> DeleteShopCategory(int categoryId , int shopId);

        Task<ApiResponse<bool>> ShopDelete(int shopId);

        Task<ApiResponse<bool>> ChangeAccept(List<AcceptNullDto> accept);

    }
}