using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Province;
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

    public class ProvinceController : ControllerBase
    {
        public IProvinceService _ProvinceService { get; }
        public ProvinceController(IProvinceService ProvinceService)
        {
            this._ProvinceService = ProvinceService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ProvinceDto>))]
        public async Task<IActionResult> Add([FromBody] ProvinceDto ProvinceDto)
        {
            var result = await _ProvinceService.ProvinceAdd(ProvinceDto);
            return new Response<ProvinceDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ProvinceDto>))]
        public async Task<IActionResult> Edit([FromBody] ProvinceDto ProvinceDto)
        {
            var result = await _ProvinceService.ProvinceEdit(ProvinceDto);
            return new Response<ProvinceDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _ProvinceService.ProvinceDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProvinceGetDto>>))]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination)
        {
            var result = await _ProvinceService.ProvinceGetAll(pagination);
            return new Response<Pagination<ProvinceGetDto>>().ResponseSending(result);
        }

        [HttpGet("{ProvinceId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ProvinceGetDto>))]
        public async Task<IActionResult> GetProvinceById([FromRoute] int ProvinceId)
        {
            var result = await _ProvinceService.GetProvinceById(ProvinceId);
            return new Response<ProvinceGetDto>().ResponseSending(result);
        }

        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAccept([FromBody] List<AcceptDto> accept)
        {
            var result = await _ProvinceService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }
    }
}