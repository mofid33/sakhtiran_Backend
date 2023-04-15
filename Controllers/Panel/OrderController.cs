using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        public IOrderService _orderService { get; }

        public OrderController(IOrderService orderService)
        {
            this._orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("OrderList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<OrderListGetDto>))]
        public async Task<IActionResult> GetOrderList([FromQuery]OrderListPaginationDto pagination)
        {
            var result = await _orderService.GetOrderList(pagination);
            return new Response<OrderListGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("ShopOrderList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopOrderListGetDto>))]
        public async Task<IActionResult> GetShopOrderList([FromQuery]OrderListPaginationDto pagination)
        {
            var result = await _orderService.GetShopOrderList(pagination);
            return new Response<ShopOrderListGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("OrderDetail/{orderId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<OrderDetailGetDto>))]
        public async Task<IActionResult> GetOrderDetail([FromRoute]long orderId)
        {
            var result = await _orderService.GetOrderDetail(orderId);
            return new Response<OrderDetailGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("OrderLog/{orderId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<OrderLogDto>>))]
        public async Task<IActionResult> GetOrderLog([FromRoute]long orderId)
        {
            var result = await _orderService.GetOrderLog(orderId);
            return new Response<List<OrderLogDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("LiveCartList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<LiveCartListDto>>))]
        public async Task<IActionResult> GetLiveCartList([FromQuery]LiveCartListPaginationDto pagination)
        {
            var result = await _orderService.GetLiveCartList(pagination);
            return new Response<Pagination<LiveCartListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("LiveCartDetail/{orderId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<LiveCartDetailGetDto>))]
        public async Task<IActionResult> GetLiveCartDetail([FromRoute]long orderId)
        {
            var result = await _orderService.GetLiveCartDetail(orderId);
            return new Response<LiveCartDetailGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("LiveCart/{orderId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteLiveCart([FromRoute]long orderId)
        {
            var result = await _orderService.DeleteLiveCart(orderId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ShippmentList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ShippmentListDto>>))]
        public async Task<IActionResult> GetShippmentList([FromQuery]ShippmentPaginationDto pagination)
        {
            var result = await _orderService.GetShippmentList(pagination);
            return new Response<Pagination<ShippmentListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ShippmentDetail/{shopId}/{customerId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShippmentDetailDto>>))]
        public async Task<IActionResult> GetShippmentDetail([FromRoute]int shopId , [FromRoute]int customerId)
        {
            var result = await _orderService.GetShippmentDetail(shopId , customerId);
            return new Response<List<ShippmentDetailDto>>().ResponseSending(result);
        }        
        


        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("ChangeStauts/{orderItemId}/{statusId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> ChangeStauts([FromRoute]long orderItemId,[FromRoute]int statusId)
        {
            var result = await _orderService.ChangeStauts(orderItemId,statusId);
            return new Response<int>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("SalesList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<SalesListDto>>))]
        public async Task<IActionResult> GeSalesList([FromQuery]SalesListPaginationDto pagination)
        {
            var result = await _orderService.GeSalesList(pagination);
            return new Response<Pagination<SalesListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("SalesDetail/{itemId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SalesDetailDto>))]
        public async Task<IActionResult> GetSalesDetail([FromRoute]long itemId)
        {
            var result = await _orderService.GetSalesDetail(itemId);
            return new Response<SalesDetailDto>().ResponseSending(result);
        }

        // لیست درخواست های کاربران برای کالاهای بدون قیمت
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("GetOrderCallRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<OrderCallRequestDto>>))]
        public async Task<IActionResult> GetOrderCallRequest([FromQuery] OrderCallRequestPaginationDto pagination)
        {
            var result = await _orderService.GetOrderCallRequest(pagination);
            return new Response<Pagination<OrderCallRequestDto>>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("ChangeCallRequestStatus")]
        public async Task<IActionResult> ChangeCallRequestStatus([FromBody]AcceptNullDto accept)
        {
            var result = await _orderService.ChangeCallRequestStatus((long)accept.StatusId  , accept.Accept);
            return new Response<bool>().ResponseSending(result);
        }

    


    }
}