using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Controllers.Models;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CustomerController : ControllerBase
    {
        public ICustomerService _customerService { get; }
        public CustomerController(ICustomerService customerService)
        {
            this._customerService = customerService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("List")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CustomerListDto>>))]
        public async Task<IActionResult> GetCustomerList([FromQuery] CustomerListPaginationDto pagination)
        {
            var result = await _customerService.GetCustomerList(pagination);
            return new Response<Pagination<CustomerListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GeneralDetail/{customerId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerGeneralDetailDto>))]
        public async Task<IActionResult> GetCustomerGeneralDetail([FromRoute] int customerId)
        {
            var result = await _customerService.GetCustomerGeneralDetail(customerId);
            return new Response<CustomerGeneralDetailDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Address/{customerId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CustomerAddressDto>>))]
        public async Task<IActionResult> GetCustomerAddress([FromRoute] int customerId)
        {
            var result = await _customerService.GetCustomerAddress(customerId);
            return new Response<List<CustomerAddressDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("OrderList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<OrderListGetDto>))]
        public async Task<IActionResult> GetCustomerOrderList([FromQuery] CustomerPaginationDto pagination)
        {
            var result = await _customerService.GetCustomerOrderList(pagination);
            return new Response<OrderListGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("WishList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CustomerWishListViewDateDto>>))]
        public async Task<IActionResult> GetCustomerWishList([FromQuery] CustomerPaginationDto pagination)
        {
            var result = await _customerService.GetCustomerWishList(pagination);
            return new Response<Pagination<CustomerWishListViewDateDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ViewList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CustomerWishListViewDateDto>>))]
        public async Task<IActionResult> GetCustomerViewList([FromQuery] CustomerPaginationDto pagination)
        {
            var result = await _customerService.GetCustomerViewList(pagination);
            return new Response<Pagination<CustomerWishListViewDateDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("CommentList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CustomerCommentDto>>))]
        public async Task<IActionResult> GetCustomerCommentList([FromQuery] CustomerPaginationDto pagination)
        {
            var result = await _customerService.GetCustomerCommentList(pagination);
            return new Response<Pagination<CustomerCommentDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("LiveCartList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<LiveCartListDto>>))]
        public async Task<IActionResult> GetCustomerLiveCartList([FromQuery] CustomerPaginationDto pagination)
        {
            var result = await _customerService.GetCustomerLiveCartList(pagination);
            return new Response<Pagination<LiveCartListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Balance")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerBalanceGetDto>))]
        public async Task<IActionResult> GetCustomerBalance([FromQuery] CustomerPaginationDto pagination)
        {
            var result = await _customerService.GetCustomerBalance(pagination);
            return new Response<CustomerBalanceGetDto>().ResponseSending(result);
        }


        [Authorize(Roles = "Admin,Seller")]
        // همه ی نظرات مشتری
        [HttpGet("AllCommentList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CustomerCommentDto>>))]
        public async Task<IActionResult> GetCustomerAllCommentList([FromQuery] CustomerCommentPaginationDto pagination)
        {
            var result = await _customerService.GetAllCustomerCommentList(pagination);
            return new Response<Pagination<CustomerCommentDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("UserName/{customerId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> GetCustomerUserName([FromRoute] int customerId)
        {
            var result = await _customerService.GetCustomerUserName(customerId);
            return new Response<string>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> CustomerDelete([FromRoute] int customerId)
        {
            var result = await _customerService.CustomerDelete(customerId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("ChangeGoodsCommentIsAccept")]
        public async Task<IActionResult> changeGoodsCommentIsAccept([FromBody] AcceptDto model)
        {
            var result = await _customerService.ChangeGoodsCommentIsAccept(model.Id, model.Accept);
            return new Response<bool>().ResponseSending(result);
        }

    }
}