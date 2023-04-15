using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Variation;
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
    public class VariationController : ControllerBase
    {
        public IVariationService _variationService { get; }
        public VariationController(IVariationService variationService)
        {
            this._variationService = variationService;
        }

        [HttpPost("VariationParameter")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<VariationParameterDto>))]
        public async Task<IActionResult> VariationParameterAdd([FromBody] VariationParameterDto variationDto)
        {
            var result = await _variationService.VariationParameterAdd(variationDto);
            return new Response<VariationParameterDto>().ResponseSending(result);
        }

        [HttpPut("VariationParameter")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<VariationParameterDto>))]
        public async Task<IActionResult> VariationParameterEdit([FromBody] VariationParameterDto variationDto)
        {
            var result = await _variationService.VariationParameterEdit(variationDto);
            return new Response<VariationParameterDto>().ResponseSending(result);
        }

        [HttpDelete("VariationParameter/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> VariationParameterDelete([FromRoute] int id)
        {
            var result = await _variationService.VariationParameterDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("VariationParameter")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<VariationParameterGetDto>>))]
        public async Task<IActionResult> VariationParameterGetAll([FromQuery] PaginationDto pagination)
        {
            var result = await _variationService.VariationParameterGetAll(pagination);
            return new Response<Pagination<VariationParameterGetDto>>().ResponseSending(result);
        }

        [HttpGet("VariationParameter/{variationId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<VariationParameterDto>))]
        public async Task<IActionResult> GetVariationParameterById([FromRoute] int variationId)
        {
            var result = await _variationService.GetVariationParameterById(variationId);
            return new Response<VariationParameterDto>().ResponseSending(result);
        }


        //values
        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<VariationParameterValuesDto>))]
        public async Task<IActionResult> VariationParameterValuesAdd([FromBody] VariationParameterValuesDto values)
        {
            var result = await _variationService.VariationParameterValuesAdd(values);
            return new Response<VariationParameterValuesDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<VariationParameterValuesDto>))]
        public async Task<IActionResult> VariationParameterValuesEdit([FromBody] VariationParameterValuesDto values)
        {
            var result = await _variationService.VariationParameterValuesEdit(values);
            return new Response<VariationParameterValuesDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> VariationParameterValuesDelete([FromRoute] int id)
        {
            var result = await _variationService.VariationParameterValuesDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<VariationParameterValuesDto>>))]
        public async Task<IActionResult> VariationParameterValuesGetAll([FromQuery] PaginationDto pagination)
        {
            var result = await _variationService.VariationParameterValuesGetAll(pagination);
            return new Response<Pagination<VariationParameterValuesDto>>().ResponseSending(result);
        }

        [HttpGet("{valuesId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<VariationParameterValuesDto>))]
        public async Task<IActionResult> GetVariationParameterValuesById([FromRoute] int valuesId)
        {
            var result = await _variationService.GetVariationParameterValuesById(valuesId);
            return new Response<VariationParameterValuesDto>().ResponseSending(result);
        }
    }
}