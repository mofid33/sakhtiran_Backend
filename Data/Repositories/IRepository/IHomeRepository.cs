using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.WebSlider;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IHomeRepository
    {
        //this repositories is for home 
        Task<List<GoodsHomeDto>> GetNewGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetAllGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetMostLikesGoodsHome(int? ids, int number, DateTime DateTime, DateTime toDay, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetMostExpensiveGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetCheapestGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetMostSellGoodsHome(int? ids, int number, DateTime fromDay, DateTime toDay, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetMostDiscountGoodsHome(int? ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetMostViewGoodsHome(int? ids, int number, DateTime fromDay, DateTime toDay, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetSpecialOfferGoodsHome(int customerId, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetSpecialGoodsHome(List<int> ids, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetSpecialSaleGoodsHome(int couponId, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);
        Task<List<GoodsHomeDto>> GetLastViewGoodsHome(int customerId, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo, string ipAddress);
        Task<List<GoodsHomeDto>> GetCustomerLikeGoodsHome(int customerId, int number, decimal rate, short? criteriaType, decimal? criteriaFrom, decimal? criteriaTo);

        //Filtered goods website
        Task<List<GoodsHomeDto>> FilterGoodsGetType1(WebsiteFilterDto filterDto, WebSliderGetDto slider, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, decimal rate, bool allow);
        Task<List<GoodsHomeDto>> FilterGoodsGetType2(WebsiteFilterDto filterDto, List<int> catIds, decimal rate);
        Task<List<GoodsHomeDto>> FilterGoodsGetType3(WebsiteFilterDto filterDto, WebModuleCollectionsGetDto module, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, decimal rate, bool allow);
        Task<List<GoodsHomeDto>> FilterGoodsGetType4(WebsiteFilterDto filterDto, decimal rate, List<int> catIds);
        Task<List<GoodsHomeDto>> FilterGoodsGetType5(WebsiteFilterDto filterDto, decimal rate);

        Task<int> FilterGoodsGetType1Count(WebsiteFilterDto filterDto, WebSliderGetDto slider, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, bool allow);
        Task<decimal> MaxPriceGetType1(WebsiteFilterDto filterDto, WebSliderGetDto slider, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, decimal rate, bool allow);
        Task<int> FilterGoodsGetType2Count(WebsiteFilterDto filterDto, List<int> catIds);
        Task<decimal> MaxPriceGetType2(WebsiteFilterDto filterDto, List<int> catIds, decimal rate);
        Task<int> FilterGoodsGetType3Count(WebsiteFilterDto filterDto, WebModuleCollectionsGetDto module, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, bool allow);
        Task<decimal> MaxPriceGetType3(WebsiteFilterDto filterDto, WebModuleCollectionsGetDto module, List<int> catIds, List<int> goodsIds, DateTime fromDay, DateTime toDay, decimal rate, bool allow);
        Task<int> FilterGoodsGetType4Count(WebsiteFilterDto filterDto, List<int> catIds);
        Task<decimal> MaxPriceGetType4(WebsiteFilterDto filterDto, decimal rate, List<int> catIds);
        Task<int> FilterGoodsGetType5Count(WebsiteFilterDto filterDto);
        Task<decimal> MaxPriceGetType5(WebsiteFilterDto filterDto, decimal rate);
        Task<int> GetAllGoodsCountForSelling(int shopId);
        Task<List<GoodsHomeDto>> GetCustomerLike(int customerId);

        Task<HomeSearchAutoComplete> GetSearchAutoComplete(string search);


        // // //وبسایت جزئیات کالا
        Task<GoodsDetailesDto> GoodsDetailsGet(int goodsId, int providerId);
        // // // مشخصات کالا وبسایت
        Task<List<SpecificationGroupGetForGoodsDto>> GoodsSpecifictionGet(int goodsId);
        Task<DefualtLanguageCurrencyDto> GetDefualtLanguageAndCurrency();


        // for mobile 
        Task<MobileSplashDataDto> GetMobileSplashData();

        // footer content
        Task<string> GetFooterContent(string content);
        Task<string> GetMobileDescriptionPageData(MobileDescriptionPageParams model);
    }
}