using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.OrderReturning;
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
    public class OrderReturnController : ControllerBase
    {
        public IOrderReturnService _orderReturnService { get; }

        public OrderReturnController(IOrderReturnService orderReturnService)
        {
            this._orderReturnService = orderReturnService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("List")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<OrderReturningListDto>>))]
        public async Task<IActionResult> GetOrderReturningList([FromQuery] OrderReturningPaginationDto pagination)
        {
            var result = await _orderReturnService.GetOrderReturningList(pagination);
            return new Response<Pagination<OrderReturningListDto>>().ResponseSending(result);
        }        
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Detail/{returnId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<OrderReturningDetailDto>>))]
        public async Task<IActionResult> GetOrderReturningDetail([FromRoute] int returnId)
        {
            var result = await _orderReturnService.GetOrderReturningDetail(returnId);
            return new Response<OrderReturningDetailDto>().ResponseSending(result);
        }        
        
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Log/{returnId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<OrderReturningLogDto>>))]
        public async Task<IActionResult> GetOrderReturningLog([FromRoute] int returnId)
        {
            var result = await _orderReturnService.GetOrderReturningLog(returnId);
            return new Response<List<OrderReturningLogDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("Change")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditOrderReturning([FromBody] OrderReturningChangeDto orderReturning)
        {
            var result = await _orderReturnService.EditOrderReturning(orderReturning);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("BlockAmount")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> BlockAmountOrderReturning([FromBody] AmountDto blockAmount)
        {
            var result = await _orderReturnService.BlockAmountOrderReturning(blockAmount);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("RefoundAmount")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> RefoundAmountOrderReturning([FromBody] AmountDto refoundAmount)
        {
            var result = await _orderReturnService.RefoundAmountOrderReturning(refoundAmount);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Amount/{returnId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<AmountDto>))]
        public async Task<IActionResult> BlockAmountOrderReturning([FromRoute] int returnId)
        {
            var result = await _orderReturnService.GetOrderReturningAmount(returnId);
            return new Response<AmountDto>().ResponseSending(result);
        }
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("SendEmailAndSMS")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> SendEmailAndSMS([FromQuery] int returnId , [FromQuery] string msg)
        {
            var result = await _orderReturnService.SendEmailAndSMS(returnId,msg);
            return new Response<bool>().ResponseSending(result);
        }
    }
}