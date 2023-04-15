using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Country;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class CountryController : ControllerBase
    {
        public ICountryService _countryService { get; }
        public CountryController(ICountryService countryService)
        {
            this._countryService = countryService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CountryDto>))]
        public async Task<IActionResult> Add([FromForm] CountrySerializeDto countryDto)
        {
            var result = await _countryService.CountryAdd(countryDto);
            return new Response<CountryDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CountryDto>))]
        public async Task<IActionResult> Edit([FromForm] CountrySerializeDto countryDto)
        {
            var result = await _countryService.CountryEdit(countryDto);
            return new Response<CountryDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _countryService.CountryDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CountryDto>>))]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination)
        {
            var result = await _countryService.CountryGetAll(pagination);
            return new Response<Pagination<CountryDto>>().ResponseSending(result);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CountryDto>))]
        public async Task<IActionResult> GetCountryById([FromRoute] int countryId)
        {
            var result = await _countryService.GetCountryById(countryId);
            return new Response<CountryDto>().ResponseSending(result);
        }

        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAccept([FromBody] List<AcceptDto> accept)
        {
            var result = await _countryService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }
    }
}