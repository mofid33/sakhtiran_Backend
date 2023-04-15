using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.OrderCancelingReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class OrderCancelingReasonController : ControllerBase
    {
        public IOrderCancelingReasonService _OrderCancelingReasonService { get; }

        public OrderCancelingReasonController(IOrderCancelingReasonService OrderCancelingReasonService)
        {
            this._OrderCancelingReasonService = OrderCancelingReasonService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<OrderCancelingReasonDto>))]
        public async Task<IActionResult> Add([FromBody] OrderCancelingReasonDto orderCancelingReasonDto)
        {
            var result = await _OrderCancelingReasonService.OrderCancelingReasonAdd(orderCancelingReasonDto);
            return new Response<OrderCancelingReasonDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<OrderCancelingReasonDto>))]
        public async Task<IActionResult> Edit([FromBody] OrderCancelingReasonDto OrderCancelingReasonEditDto)
        {
            var result = await _OrderCancelingReasonService.OrderCancelingReasonEdit(OrderCancelingReasonEditDto);
            return new Response<OrderCancelingReasonDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _OrderCancelingReasonService.OrderCancelingReasonDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<OrderCancelingReasonDto>>))]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination)
        {
            var result = await _OrderCancelingReasonService.OrderCancelingReasonGetAll(pagination);
            return new Response<Pagination<OrderCancelingReasonDto>>().ResponseSending(result);
        }

        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAccept([FromBody] AcceptDto accept)
        {
            var result = await _OrderCancelingReasonService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }
    }
}