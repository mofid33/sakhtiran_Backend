using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.ShopStatus;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IShopRepository
    {
         Task<List<ShopListGetDto>> GetShopList(ShopListPaginationDto pagination);
         Task<int> GetShopListCount(ShopListPaginationDto pagination);
         Task<ShopGeneralDto> GetShopGeneralDetail(int shopId, string userName = null);
         Task<ShopBaseDto> GetShopStoreName(int shopId);
         Task<RepRes<ShopGeneralDto>> EditShopGeneralDetail(ShopGeneralDto shopDto);
         Task<decimal> GetAvailableBalance(int shopId);
         Task<List<ShopBalanceDto>> GetShopBalance(ShopBalancePagination pagination);
         Task<int> GetShopBalanceCount(ShopBalancePagination pagination);
         Task<RepRes<bool>> EditShopDescription(ShopDescriptionDto shopDto);
         Task<RepRes<bool>> EditShopTermsAndConditions(ShopDescriptionDto shopDto);
         Task<RepRes<TShop>> EditShopProfile(string profile, string logo,int shopId,bool IsLogoNull , bool IsProfileNull);
         Task<ShopDescriptionDto> GetShopDescription(int shopId);
         Task<ShopDescriptionDto> GetShopTermsAndConditions(int shopId);
         Task<List<ShopFilesGetDto>> GetShopDocument(int shopId);
         Task<RepRes<List<TShopFiles>>> EditShopDocument(int shopId,List<ShopFileDto> shopDto ,List<int> shopFileDeleted);

         Task<ShopProfileDto> GetShopProfile(int shopId);
         Task<ShopSetting> GetShopSetting(int shopId);
         Task<RepRes<bool>> EditShopSetting(ShopSetting shopDto);

         Task<ShopBankInformationDto> GetShopBankInformation(int shopId);
         Task<ShopTaxDto> GetShopTax(int shopId);


         Task<RepRes<bool>> EditShopActivityCountry(ShopActivityCountryEditDto shopDto);
         Task<RepRes<bool>> EditShopActivityCity(ShopActivityCityEditDto shopDto);
         Task<RepRes<bool>> EditShopActivityProvince(ShopActivityCityEditDto shopDto);

         Task<List<ShopActivityCountryGetDto>> GetShopActivityCountry(PaginationFormDto pagination);
         Task<int> GetShopActivityCountryCount(PaginationFormDto pagination);
         Task<List<ShopActivityCityGetDto>> GetShopActivityCity(PaginationFormDto pagination,int provinceId);
         Task<int> GetShopActivityCityCount(PaginationFormDto pagination , int provinceId);
        Task<List<ShopActivityCityGetDto>> GetShopActivityProvince(PaginationFormDto pagination);
        Task<List<ProvinceFormDto>> GetAllShopProvince(int shopId);
         Task<int> GetShopActivityProvinceCount(PaginationFormDto pagination);
         Task<bool> CanAddSlider(int shopId);
         Task<TShopSlider> AddShopSlider(TShopSlider shopSlider);
         Task<RepRes<TShopSlider>> DeleteShopSlider(int sliderId,int shopId);
         Task<List<ShopSliderDto>> GetShopSlider(int shopId);
         Task<RepRes<bool>> ChangeShopSliderStatus(AcceptDto accept,int shopId);
         Task<RepRes<List<TShopFiles>>> EditShopBankInformation(ShopBankInformationDto shopDto, List<int> shopFileDeleted);
         Task<RepRes<List<TShopFiles>>> EditShopTax(ShopTaxDto shopDto, List<int> shopFileDeleted);

         Task<RepRes<TShopPlan>> EditShopPlan(ShopPlanPaymentDto shopPlanPaymentDto ,  bool pay , int? setCurrency = null);
         Task<List<PlanShopDto>> GetShopPlan(int shopId);
        Task<RepRes<TShopPlan>> ShopPlanDelete(int id);

         Task<int> GetShopActiveOrderCount(int shopId);
         Task<int> GetShopActiveProductCount(int shopId);
         Task<double> GetShopIncome(int shopId);
         Task<int> GetShopOrdersCount(int shopId);
         Task<int> GetShopOutOfStuckCount(int shopId);
         Task<double> GetShopSales(int shopId);
         Task<string> GetShopUserName(int shopId);
         Task<ShopWebsiteDetailDto> GetShopByUrl(string url);

         // ثبت نام تامین کننده
         Task<RepRes<TShop>> RegisterShop(TShop shopRegister);
         Task<bool> DeleteShop(int shopId);
         Task<bool> ChangeShopStatus();
         Task<ShopCategoryPlanGetDto> GetShopCategory(int shopId);
         Task<RepRes<bool>> AddShopCategory(int shopId, int categoryId);


         // لیست فروشنده ها
         Task<List<ShopGeneralDto>> GetWebShopList(ShopListWebPaginationDto pagination);
         Task<int> GetWebShopListCount(ShopListWebPaginationDto pagination);


         Task<bool> ChangeShopStatus(AcceptNullDto accept);


         Task<RepRes<bool>> EditInactiveShopMessage(ShopDescriptionDto shopDto);

         Task<ShopDescriptionDto> GetInactiveShopMessage(int shopId);
         Task<bool> TransactionPlanPayment(TPaymentTransaction paymentTransaction);
         Task<TPaymentTransaction> planShopDetailsWithPaymentId(string paymentId);


         Task<RepRes<TShopCategory>> DeleteShopCategory(List<int> categoryIdWithChild, int parentCategory ,int shopId);


         Task<RepRes<TShop>> ShopDelete(int shopId);

        Task<bool> ChangeAccept(List<AcceptNullDto> accept);

    }
}