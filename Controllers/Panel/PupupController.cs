using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.PupupItem;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;
using System.Collections.Generic;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    public class PupupController : ControllerBase
    {
        public IPupupService _pupupService { get; }
        public PupupController(IPupupService pupupService)
        {
            this._pupupService = pupupService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<PupupItemDto>))]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] PupupItemSerializeDto brandDto)
        {
            var result = await _pupupService.PupupItemAdd(brandDto);
            return new Response<PupupItemDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<PupupItemDto>))]
        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] PupupItemSerializeDto brandDto)
        {
            var result = await _pupupService.PupupItemEdit(brandDto);
            return new Response<PupupItemDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("ChangeAccept")]
        public async Task<IActionResult> ChangeAccept([FromBody]AcceptDto accept)
        {
            var result = await _pupupService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _pupupService.PupupItemDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<PupupItemDto>>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDto pagination)
        {
            var result = await _pupupService.PupupItemGetAll(pagination);
            return new Response<Pagination<PupupItemDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<PupupItemDto>))]
        [HttpGet("{brandId}")]
        public async Task<IActionResult> GetPupupItemById([FromRoute]int brandId)
        {
            var result = await _pupupService.GetPupupItemById(brandId);
            return new Response<PupupItemDto>().ResponseSending(result);
        }
    }
}