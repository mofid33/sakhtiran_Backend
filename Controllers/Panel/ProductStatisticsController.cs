using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.ProductsStatistics;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Pagination;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStatisticsController : ControllerBase
    {
        public IProductStatisticsService _productStatisticsService { get; }
        public ProductStatisticsController(IProductStatisticsService productStatisticsService)
        {
            _productStatisticsService = productStatisticsService;
        }


        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProductStatisticsDto>>))]
        [HttpGet("GetMostPopularGoods")]
        public async Task<IActionResult> GetMostPopularGoods([FromQuery]ProductStatisticsPaginationDto pagination)
        {
            var result = await _productStatisticsService.GetMostPopularGoods(pagination);
            return new Response<Pagination<ProductStatisticsDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProductStatisticsDto>>))]
        [HttpGet("GetMostVisitedGoods")]
        public async Task<IActionResult> GetMostVisitedGoods([FromQuery]ProductStatisticsPaginationDto pagination)
        {
            var result = await _productStatisticsService.GetMostVisitedGoods(pagination);
            return new Response<Pagination<ProductStatisticsDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProductStatisticsDto>>))]
        [HttpGet("GetBestSellerGoods")]
        public async Task<IActionResult> GetBestSellerGoods([FromQuery]ProductStatisticsPaginationDto pagination)
        {
            var result = await _productStatisticsService.GetBestSellerGoods(pagination);
            return new Response<Pagination<ProductStatisticsDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProductStatisticsDetailsDto>>))]
        [HttpGet("GetGoodsCustomerLike")]
        public async Task<IActionResult> GetGoodsCustomerLike([FromQuery]PaginationFormDto pagination)
        {
            var result = await _productStatisticsService.GetGoodsCustomerLikeAndView(pagination , "like");
            return new Response<Pagination<ProductStatisticsDetailsDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProductStatisticsDetailsDto>>))]
        [HttpGet("GetGoodsCustomerView")]
        public async Task<IActionResult> GetGoodsCustomerView([FromQuery]PaginationFormDto pagination)
        {
            var result = await _productStatisticsService.GetGoodsCustomerLikeAndView(pagination , "view");
            return new Response<Pagination<ProductStatisticsDetailsDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProductStatisticsDto>>))]
        [HttpGet("GetNoSelleGoods")]
        public async Task<IActionResult> GetNoSelleGoods([FromQuery]ProductStatisticsPaginationDto pagination)
        {
            var result = await _productStatisticsService.GetNoSelleGoods(pagination);
            return new Response<Pagination<ProductStatisticsDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProductStatisticsDetailsDto>>))]
        [HttpGet("GetGoodsSellDetails")]
        public async Task<IActionResult> GetGoodsSellDetails([FromQuery]PaginationFormDto pagination)
        {
            var result = await _productStatisticsService.GetGoodsSellDetails(pagination);
            return new Response<Pagination<ProductStatisticsDetailsDto>>().ResponseSending(result);
        }



    }
}