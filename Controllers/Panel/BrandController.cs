using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;
using System.Collections.Generic;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        public IBrandService _brandService { get; }
        public BrandController(IBrandService brandService)
        {
            this._brandService = brandService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<BrandDto>))]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] BrandSerializeDto brandDto)
        {
            var result = await _brandService.BrandAdd(brandDto);
            return new Response<BrandDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<BrandDto>))]
        [HttpPut]
        public async Task<IActionResult> Edit([FromForm] BrandSerializeDto brandDto)
        {
            var result = await _brandService.BrandEdit(brandDto);
            return new Response<BrandDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("ChangeAccept")]
        public async Task<IActionResult> ChangeAccept([FromBody]List<AcceptNullDto> accept)
        {
            var result = await _brandService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _brandService.BrandDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<BrandDto>>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDto pagination)
        {
            var result = await _brandService.BrandGetAll(pagination);
            return new Response<Pagination<BrandDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<BrandGetOneDto>))]
        [HttpGet("{brandId}")]
        public async Task<IActionResult> GetBrandById([FromRoute]int brandId)
        {
            var result = await _brandService.GetBrandById(brandId);
            return new Response<BrandGetOneDto>().ResponseSending(result);
        }
    }
}