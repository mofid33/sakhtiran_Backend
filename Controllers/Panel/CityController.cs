using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.City;
using MarketPlace.API.Data.Dtos.Pagination;
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
    [Authorize(Roles = "Admin")]

    public class CityController : ControllerBase
    {
        public ICityService _cityService { get; }
        public CityController(ICityService cityService)
        {
            this._cityService = cityService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CityDto>))]
        public async Task<IActionResult> Add([FromBody] CityDto cityDto)
        {
            var result = await _cityService.CityAdd(cityDto);
            return new Response<CityDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CityDto>))]
        public async Task<IActionResult> Edit([FromBody] CityDto cityDto)
        {
            var result = await _cityService.CityEdit(cityDto);
            return new Response<CityDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _cityService.CityDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CityGetDto>>))]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination)
        {
            var result = await _cityService.CityGetAll(pagination);
            return new Response<Pagination<CityGetDto>>().ResponseSending(result);
        }

        [HttpGet("{CityId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CityGetDto>))]
        public async Task<IActionResult> GetCityById([FromRoute] int CityId)
        {
            var result = await _cityService.GetCityById(CityId);
            return new Response<CityGetDto>().ResponseSending(result);
        }

        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAccept([FromBody] List<AcceptDto> accept)
        {
            var result = await _cityService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }


        [HttpPost("AddShippingMethodAreaCode")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CityDto>))]
        public async Task<IActionResult> AddShippingMethodAreaCode([FromBody] CityShippingMethodAreaCodeDto cityShippingMethodAreaCodeDto)
        {
            var result = await _cityService.AddShippingMethodAreaCode(cityShippingMethodAreaCodeDto);
            return new Response<CityShippingMethodAreaCodeDto>().ResponseSending(result);
        }

        [HttpPut("UpdateShippingMethodAreaCode")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CityDto>))]
        public async Task<IActionResult> UpdateShippingMethodAreaCode([FromBody] CityShippingMethodAreaCodeDto cityShippingMethodAreaCodeDto)
        {
            var result = await _cityService.UpdateShippingMethodAreaCode(cityShippingMethodAreaCodeDto);
            return new Response<CityShippingMethodAreaCodeDto>().ResponseSending(result);
        }

        [HttpDelete("DeleteShippingMethodAreaCode/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteShippingMethodAreaCode([FromRoute] int id)
        {
            var result = await _cityService.DeleteShippingMethodAreaCode(id);
            return new Response<bool>().ResponseSending(result);
        }

   
        [HttpGet("GetAllShippingMethodAreaCode")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CityShippingMethodAreaCodeDto>>))]
        public async Task<IActionResult> GetAllShippingMethodAreaCode([FromQuery] PaginationDto pagination)
        {
            var result = await _cityService.ShippingMethodAreaGetAll(pagination);
            return new Response<Pagination<CityShippingMethodAreaCodeDto>>().ResponseSending(result);
        }
    

    }
}