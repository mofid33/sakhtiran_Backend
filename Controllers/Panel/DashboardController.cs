using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Dashboard;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        public IDashboardService _dashboardService { get; }
        public DashboardController(IDashboardService dashboardService)
        {
            this._dashboardService = dashboardService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet()]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<DashboardDto>))]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _dashboardService.GetDashboard();
            return new Response<DashboardDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Order/{statusId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<RecentOrderDto>>))]
        public async Task<IActionResult> GetDashboardOrder(int statusId)
        {
            var result = await _dashboardService.GetDashboardOrder(statusId);
            return new Response<List<RecentOrderDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ReturnOrder/{statusId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<RecentReturnOrderDto>>))]
        public async Task<IActionResult> GetDashboardReturnOrder(int statusId)
        {
            var result = await _dashboardService.GetDashboardReturnOrder(statusId);
            return new Response<List<RecentReturnOrderDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ShopRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShopRequestDto>>))]
        public async Task<IActionResult> GetDashboardShopRequest()
        {
            var result = await _dashboardService.GetDashboardShopRequest();
            return new Response<List<ShopRequestDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GoodsRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GoodsRequestDto>>))]
        public async Task<IActionResult> GetDashboardGoodsRequest()
        {
            var result = await _dashboardService.GetDashboardGoodsRequest();
            return new Response<List<GoodsRequestDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("BrandRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<BrandRequestDto>>))]
        public async Task<IActionResult> GetDashboardBrandRequest()
        {
            var result = await _dashboardService.GetDashboardBrandRequest();
            return new Response<List<BrandRequestDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GarenteeRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GarenteeRequestDto>>))]
        public async Task<IActionResult> GetDashboardGarenteeRequest()
        {
            var result = await _dashboardService.GetDashboardGarenteeRequest();
            return new Response<List<GarenteeRequestDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("WithDrawalRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<WithDrawalRequestDto>>))]
        public async Task<IActionResult> GetDashboardWithDrawalRequest()
        {
            var result = await _dashboardService.GetDashboardWithDrawalRequest();
            return new Response<List<WithDrawalRequestDto>>().ResponseSending(result);
        }

    }
}