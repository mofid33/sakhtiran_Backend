using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Data.Dtos.Home;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.Survey;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Dtos.PupupItem;
//using Controllers.Models;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public IHomeService _homeService { get; }
        public IShopService _shopService { get; }
        public IPupupService _pupupService { get; }
        public HomeController(IHomeService homeService, IShopService shopService, IPupupService pupupService)
        {
            this._homeService = homeService;
            this._shopService = shopService;
            this._pupupService = pupupService;
        }

        [HttpGet("Home")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HomeGetDto>))]
        public async Task<IActionResult> Home()
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var result = await _homeService.HomeGet(ipAddress);
            return new Response<HomeGetDto>().ResponseSending(result);
        }

        [HttpGet("Category/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HomeCategoryGetDto>))]
        public async Task<IActionResult> HomeCategory(int id)
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var result = await _homeService.HomeCategoryGet(id, ipAddress);
            return new Response<HomeCategoryGetDto>().ResponseSending(result);
        }

        [HttpGet("Header")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HeaderGetDto>))]
        public async Task<IActionResult> Header([FromQuery] bool? isDesktop)
        {
            if (isDesktop == null)
            {
                isDesktop = true;
            }
            var result = await _homeService.HeaderGet(isDesktop);
            return new Response<HeaderGetDto>().ResponseSending(result);
        }


        [HttpGet("Footer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<FooterGetDto>))]
        public async Task<IActionResult> Footer()
        {
            var result = await _homeService.FooterGet(true);
            return new Response<FooterGetDto>().ResponseSending(result);
        }

        [HttpPost("FilterGoods")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<FilterGoodsGetDto>))]
        public async Task<IActionResult> FilterGoods([FromBody] WebsiteFilterDto filterDto)
        {
            var result = await _homeService.FilterGoodsGet(filterDto);
            return new Response<FilterGoodsGetDto>().ResponseSending(result);
        }

        [HttpPost("ShopGoods/{shop}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<FilterShopGoodsGetDto>))]

        public async Task<IActionResult> FilterShopGoods([FromRoute] string shop, [FromBody] WebsiteFilterDto filterDto)
        {
            var result = await _homeService.FilterShopGoodsGet(shop, filterDto);
            return new Response<FilterShopGoodsGetDto>().ResponseSending(result);
        }

        [HttpGet("GoodsDetails/{goodsId}/{providerId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsDetailesDto>))]
        public async Task<IActionResult> GoodsDetails([FromRoute] int goodsId, [FromRoute] int providerId)
        {
            var result = await _homeService.GoodsDetailsGet(goodsId, providerId);
            return new Response<GoodsDetailesDto>().ResponseSending(result);
        }

        [HttpGet("PostMethod/{shopId}/{countryId}/{cityId}/{provinceId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<PostMethodDto>))]
        public async Task<IActionResult> PostMethod([FromRoute] int shopId, [FromRoute] int countryId, [FromRoute] int? cityId, [FromRoute] int? provinceId)
        {
            var result = await _homeService.PostMethod(shopId, cityId, countryId, provinceId);
            return new Response<PostMethodDto>().ResponseSending(result);
        }

        // مشخصات کالا
        [HttpGet("Specifications/{goodsId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<SpecificationGroupGetForGoodsDto>>))]

        public async Task<IActionResult> SpecificationsGet([FromRoute] int goodsId)
        {
            var result = await _homeService.GoodsSpecifictionGet(goodsId);
            return new Response<List<SpecificationGroupGetForGoodsDto>>().ResponseSending(result);
        }

        // نظرات کاربران به کالا
        [HttpGet("CustomerComment")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsCommentWithSurveyDto>))]

        public async Task<IActionResult> CustomerComment([FromQuery] PaginationFormDto paginatin)
        {
            var result = await _homeService.CustomerCommentGet(paginatin);
            return new Response<GoodsCommentWithSurveyDto>().ResponseSending(result);
        }

        [HttpGet("CustomerLikes")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GoodsHomeDto>>))]
        public async Task<IActionResult> GetCustomerLikes()
        {
            var result = await _homeService.GetCustomerLikes();
            return new Response<List<GoodsHomeDto>>().ResponseSending(result);
        }

        [HttpGet("SearchAutoComplete/{search}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HomeSearchAutoComplete>))]
        public async Task<IActionResult> GetSearchAutoComplete([FromRoute] string search)
        {
            var result = await _homeService.GetSearchAutoComplete(search);
            return new Response<HomeSearchAutoComplete>().ResponseSending(result);
        }

        [HttpGet("HelpAutoComplete/{search}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<HelpArticleFormDto>>))]
        public async Task<IActionResult> HelpAutoComplete([FromRoute] string search)
        {
            var result = await _homeService.HelpAutoComplete(search);
            return new Response<List<HelpArticleFormDto>>().ResponseSending(result);
        }

        [HttpGet("Help")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<HomeHelpTopicListDto>>))]
        public async Task<IActionResult> GetHomeHelpTopic()
        {
            var result = await _homeService.GetHomeHelpTopic();
            return new Response<List<HomeHelpTopicListDto>>().ResponseSending(result);
        }

        [HttpGet("HelpTopic/{topicId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HomeHelpTopicChildDto>))]
        public async Task<IActionResult> GetHelpTopic([FromRoute] int topicId)
        {
            var result = await _homeService.GetHelpTopic(topicId);
            return new Response<HomeHelpTopicChildDto>().ResponseSending(result);
        }

        [HttpGet("HelpParentTopic/{topicId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HomeHelpTopicDto>))]
        public async Task<IActionResult> GetHelpParentTopic([FromRoute] int topicId)
        {
            var result = await _homeService.GetHelpParentTopic(topicId);
            return new Response<HomeHelpTopicDto>().ResponseSending(result);
        }

        [HttpGet("HelpArticle/{articleId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HomeHelpArticleDto>))]
        public async Task<IActionResult> GetHelpArticle([FromRoute] int articleId)
        {
            var result = await _homeService.GetHelpArticle(articleId);
            return new Response<HomeHelpArticleDto>().ResponseSending(result);
        }

        [HttpPut("HelpArticle")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HomeHelpArticleDto>))]
        public async Task<IActionResult> AddHelpFul([FromBody] AcceptDto accept)
        {
            var result = await _homeService.AddHelpFul(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("GetHelpImage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> GetHelpImage()
        {
            var result = await _homeService.GetHelpImage();
            return new Response<string>().ResponseSending(result);
        }


        [HttpGet("GetShopList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ShopGeneralDto>>))]
        public async Task<IActionResult> GetShopList([FromQuery] ShopListWebPaginationDto pagination)
        {
            var result = await _shopService.GetWebShopList(pagination);
            return new Response<Pagination<ShopGeneralDto>>().ResponseSending(result);
        }

        [HttpGet("GetOrderWithCode")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ProfileOrderGetDto>))]
        public async Task<IActionResult> GetOrderWithCode([FromQuery] string trackingCode)
        {
            var result = await _homeService.GetOrderWithCode(trackingCode);
            return new Response<ProfileOrderGetDto>().ResponseSending(result);
        }

        [HttpGet("GetFooterContent/{content}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> GetFooterContent([FromRoute] string content)
        {
            var result = await _homeService.GetFooterContent(content);
            return new Response<string>().ResponseSending(result);
        }


        [HttpGet("GetDefualtLanguageAndCurrency")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<DefualtLanguageCurrencyDto>))]
        public async Task<IActionResult> GetDefualtLanguageAndCurrency()
        {
            var result = await _homeService.GetDefualtLanguageAndCurrency();
            return new Response<DefualtLanguageCurrencyDto>().ResponseSending(result);
        }

        /// mobile section ///
        [HttpGet("GetMobileSplashData")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<MobileSplashDataDto>))]
        public async Task<IActionResult> GetMobileSplashData([FromQuery] MobileSplashDataParams model)
        {
            var result = await _homeService.GetMobileSplashData(model);
            return new Response<MobileSplashDataDto>().ResponseSending(result);
        }


        [HttpGet("MobileCategory")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CategoryWebGetDto>>))]
        public async Task<IActionResult> MobileCategory()
        {
            var result = await _homeService.MobileCategory();
            return new Response<List<CategoryWebGetDto>>().ResponseSending(result);
        }

        [HttpGet("MobileFooter")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<FooterGetDto>))]
        public async Task<IActionResult> MobileFooter()
        {
            var result = await _homeService.FooterGet(false);
            return new Response<FooterGetDto>().ResponseSending(result);
        }

        [HttpGet("GetMobileDescriptionPageData")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(string))]
        public async Task<IActionResult> GetMobileDescriptionPageData([FromQuery] MobileDescriptionPageParams model)
        {
            if (model.Type == "content")
            {
                var result2 = await _homeService.GetFooterContent(model.ContentType);
                return new Response<string>().ResponseSending(result2);
            }
            var result = await _homeService.GetMobileDescriptionPageData(model);
            return new Response<string>().ResponseSending(result);
        }


        
        [HttpGet("GetPupup")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<PupupItemDto>))]
        public async Task<IActionResult> getWebsitePupup()
        {
            var result = await _pupupService.GetWebsitePupup();
            return new Response<PupupItemDto>().ResponseSending(result);
        }

    }
}