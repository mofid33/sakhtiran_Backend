using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.ReturningType;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReturningTypeController : ControllerBase
    {
        public IReturningTypeService _returningTypeService { get; }

        public ReturningTypeController(IReturningTypeService ReturningTypeService)
        {
            this._returningTypeService = ReturningTypeService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ReturningTypeDto>))]
        public async Task<IActionResult> Add([FromBody]ReturningTypeDto returningTypeDto)
        {
            var result = await _returningTypeService.ReturningTypeAdd(returningTypeDto);
            return new Response<ReturningTypeDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ReturningTypeDto>))]
        public async Task<IActionResult> Edit([FromBody]ReturningTypeDto returningTypeDto)
        {
            var result = await _returningTypeService.ReturningTypeEdit(returningTypeDto);
            return new Response<ReturningTypeDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _returningTypeService.ReturningTypeDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ReturningTypeDto>>))]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDto pagination)
        {
            var result = await _returningTypeService.ReturningTypeGetAll(pagination);
            return new Response<Pagination<ReturningTypeDto>>().ResponseSending(result);
        }
    }
}