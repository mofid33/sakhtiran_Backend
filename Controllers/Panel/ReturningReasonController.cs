using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.ReturningReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReturningReasonController : ControllerBase
    {
        public IReturningReasonService _ReturningReasonService { get; }

        public ReturningReasonController(IReturningReasonService ReturningReasonService)
        {
            this._ReturningReasonService = ReturningReasonService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ReturningReasonDto>))]
        public async Task<IActionResult> Add([FromBody] ReturningReasonDto returningReasonDto)
        {
            var result = await _ReturningReasonService.ReturningReasonAdd(returningReasonDto);
            return new Response<ReturningReasonDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ReturningReasonDto>))]
        public async Task<IActionResult> Edit([FromBody] ReturningReasonDto returningReasonEditDto)
        {
            var result = await _ReturningReasonService.ReturningReasonEdit(returningReasonEditDto);
            return new Response<ReturningReasonDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _ReturningReasonService.ReturningReasonDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ReturningReasonDto>>))]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination)
        {
            var result = await _ReturningReasonService.ReturningReasonGetAll(pagination);
            return new Response<Pagination<ReturningReasonDto>>().ResponseSending(result);
        }

        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAccept([FromBody] AcceptDto accept)
        {
            var result = await _ReturningReasonService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }
    }
}