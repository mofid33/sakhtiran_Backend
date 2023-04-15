using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.MeasurementUnit;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class MeasurementUnitController : ControllerBase
    {
        public IMeasurementUnitService _MeasurementUnitService { get; }

        public MeasurementUnitController(IMeasurementUnitService MeasurementUnitService)
        {
            this._MeasurementUnitService = MeasurementUnitService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<MeasurementUnitDto>))]
        public async Task<IActionResult> Add([FromForm]MeasurementUnitDto MeasurementUnitDto)
        {
            var result = await _MeasurementUnitService.MeasurementUnitAdd(MeasurementUnitDto);
            return new Response<MeasurementUnitDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<MeasurementUnitDto>))]
        public async Task<IActionResult> Edit([FromForm]MeasurementUnitDto MeasurementUnitDto)
        {
            var result = await _MeasurementUnitService.MeasurementUnitEdit(MeasurementUnitDto);
            return new Response<MeasurementUnitDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _MeasurementUnitService.MeasurementUnitDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<MeasurementUnitDto>>))]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDto pagination)
        {
            var result = await _MeasurementUnitService.MeasurementUnitGetAll(pagination);
            return new Response<Pagination<MeasurementUnitDto>>().ResponseSending(result);
        }
    }
}