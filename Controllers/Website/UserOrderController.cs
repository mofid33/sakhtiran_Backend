using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserOrderController : ControllerBase
    {
        public IUserOrderService _userOrderService { get; }

        public UserOrderController(IUserOrderService userOrderService )
        {
            this._userOrderService = userOrderService;

        }

        [HttpPost("Order")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<OrderCountDto>))]
        public async Task<IActionResult> AddOrder([FromBody]OrderAddDto orderDto)
        {
            var result = await _userOrderService.AddOrder(orderDto);
            return new Response<OrderCountDto>().ResponseSending(result);
        }


        // [Authorize(Roles = "Customer")]
        [HttpPut("IncreaseOrderItem")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<OrderCountDto>))]
        public async Task<IActionResult> IncreaseOrderItem(OrderAddDto orderDto)
        {
            var result = await _userOrderService.IncreaseOrderItem(orderDto);
            return new Response<OrderCountDto>().ResponseSending(result);
        }


        // [Authorize(Roles = "Customer")]
        [HttpDelete("OrderItem/{orderItemId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<OrderCountDto>))]
        public async Task<IActionResult> DeleteOrderItem([FromRoute]long orderItemId)
        {
            var result = await _userOrderService.DeleteOrderItem(orderItemId);
            return new Response<OrderCountDto>().ResponseSending(result);
        }

        // [Authorize(Roles = "Customer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebsiteOrderGetDto>))]
        [HttpGet("CartDetail")]
        public async Task<IActionResult> GetCartDetail([FromQuery] string code,int? cityId,int? countryId,int? provinceId)
        {
            var result = await _userOrderService.GetOrderDetail(code,cityId,countryId , provinceId ,false);
            return new Response<WebsiteOrderGetDto>().ResponseSending(result);
        }

        // [Authorize(Roles = "Customer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebsiteOrderGetDto>))]
        [HttpGet("OrderDetail")]
        public async Task<IActionResult> GetOrderDetail([FromQuery] string code,int? cityId,int? countryId,int? provinceId)
        {
            var result = await _userOrderService.GetOrderDetail(code,cityId,countryId,provinceId,true);
            return new Response<WebsiteOrderGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Customer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebsiteOrderGetDto>))]
        [HttpGet("OrderCityCountryDetail")]
        public async Task<IActionResult> OrderCityCountryDetail()
        {
            var result = await _userOrderService.OrderCityCountryDetail();
            return new Response<WebsiteOrderGetDto>().ResponseSending(result);
        }

        // [Authorize(Roles = "Customer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("ChangeDestination/{addressId}")]
        public async Task<IActionResult> ChangeDestination(int addressId)
        { 
            var result = await _userOrderService.ChangeDestination(addressId);
            return new Response<bool>().ResponseSending(result);
        }

        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<string>))]
        [HttpPost("InitOrderPayment")]
        public async Task<IActionResult> InitOrderPayment(PayOrderDto PayDto)
        {
            var result  = await _userOrderService.InitOrderPayment(PayDto);
            return new Response<string>().ResponseSending(result);
            
        }

        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebsiteOrderGetDto>))]
        [HttpPost("PayOrder")]
        public async Task<IActionResult> PayOrder([FromBody] PayOrderDto PayDto)
        {
            var result  = await _userOrderService.PayOrder(PayDto);
            return new Response<WebsiteOrderGetDto>().ResponseSending(result);

        }

        [HttpGet("CheckPayOrder")]
        public async Task<IActionResult> CheckPayOrder([FromQuery] long OrderId)
        {
            var result  = await _userOrderService.CheckPayOrder(OrderId);
            return new Response<PayOrderDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("CancelOrder")]
        public async Task<IActionResult> CancelOrder([FromBody]List<OrderCancelingAddDto> orderCanceling)
        { 
            var result  = await _userOrderService.CancelOrder(orderCanceling);
            return new Response<List<ProfileOrderItemGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("ReturnOrder")]
        public async Task<IActionResult> ReturnOrder([FromBody]OrderReturningAddDto orderReturning)
        { 
            var result  = await _userOrderService.ReturnOrder(orderReturning);
            return new Response<bool>().ResponseSending(result);
        }


       


    }
}