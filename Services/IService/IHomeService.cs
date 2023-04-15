using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.Survey;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IHomeService
    {
        Task<ApiResponse<HomeGetDto>> HomeGet(string ipAddress);
        Task<ApiResponse<HomeCategoryGetDto>> HomeCategoryGet(int categoryId, string ipAddress);
        Task<ApiResponse<HeaderGetDto>> HeaderGet(bool? isDesktop);
        Task<ApiResponse<List<CategoryWebGetDto>>> MobileCategory();
        Task<ApiResponse<FooterGetDto>> FooterGet(bool getFooterCategory);
        Task<ApiResponse<FilterGoodsGetDto>> FilterGoodsGet(WebsiteFilterDto filterDto);
        Task<ApiResponse<FilterShopGoodsGetDto>> FilterShopGoodsGet(string shop, WebsiteFilterDto filterDto);
        Task<ApiResponse<GoodsDetailesDto>> GoodsDetailsGet(int goodsId, int providerId);
        Task<ApiResponse<PostMethodDto>> PostMethod(int shopId, int? cityId, int countryId, int? provinceId);
        Task<ApiResponse<List<SpecificationGroupGetForGoodsDto>>> GoodsSpecifictionGet(int goodsId);
        Task<ApiResponse<GoodsCommentWithSurveyDto>> CustomerCommentGet(PaginationFormDto paginatin);
        Task<ApiResponse<List<GoodsHomeDto>>> GetCustomerLikes();
        Task<ApiResponse<HomeSearchAutoComplete>> GetSearchAutoComplete(string search);
        Task<ApiResponse<List<HelpArticleFormDto>>> HelpAutoComplete(string search);
        Task<ApiResponse<List<HomeHelpTopicListDto>>> GetHomeHelpTopic();
        Task<ApiResponse<HomeHelpTopicChildDto>> GetHelpTopic(int topicId);
        Task<ApiResponse<HomeHelpTopicDto>> GetHelpParentTopic(int topicId);
        Task<ApiResponse<HomeHelpArticleDto>> GetHelpArticle(int articleId);
        Task<ApiResponse<bool>> AddHelpFul(AcceptDto accept);
        Task<ApiResponse<string>> GetHelpImage();
        Task<ApiResponse<ProfileOrderGetDto>> GetOrderWithCode(string trackingCode);

        Task<ApiResponse<DefualtLanguageCurrencyDto>> GetDefualtLanguageAndCurrency();

        // for mobile
        Task<ApiResponse<MobileSplashDataDto>> GetMobileSplashData(MobileSplashDataParams model);

        Task<ApiResponse<string>> GetFooterContent(string content);
        Task<ApiResponse<string>> GetMobileDescriptionPageData(MobileDescriptionPageParams model);

    }
}