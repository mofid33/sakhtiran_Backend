using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Enums;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SpecificationGroupController : ControllerBase
    {
        public ISpecificationGroupService _specificationGroupService { get; }
        public SpecificationGroupController(ISpecificationGroupService specificationGroupService)
        {
            this._specificationGroupService = specificationGroupService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SpecificationGroupDto>))]
        public async Task<IActionResult> Add([FromBody]SpecificationGroupDto SpecificationGroupAddDto)
        {
            var result = await _specificationGroupService.SpecificationGroupAdd(SpecificationGroupAddDto);
            return new Response<SpecificationGroupDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SpecificationGroupDto>))]
        public async Task<IActionResult> Edit([FromBody]SpecificationGroupDto SpecificationGroupEditDto)
        {
            var result = await _specificationGroupService.SpecificationGroupEdit(SpecificationGroupEditDto);
            return new Response<SpecificationGroupDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _specificationGroupService.SpecificationGroupDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<SpecificationGroupDto>>))]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDto pagination)
        {
            var result = await _specificationGroupService.SpecificationGroupGetAll(pagination);
            return new Response<Pagination<SpecificationGroupDto>>().ResponseSending(result);
        }

        [HttpGet("SpecificationGroupWithSpecGetAll")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<SpecificationGroupGetForGoodsDto>>))]
        public async Task<IActionResult> GetAllSpecificationGroupWithSpec([FromQuery]PaginationDto pagination)
        {
            var result = await _specificationGroupService.SpecificationGroupWithSpecGetAll(pagination);
            return new Response<Pagination<SpecificationGroupGetForGoodsDto>>().ResponseSending(result);
        }

        [HttpGet("byCategoryId/{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<SpecificationGroupFromDto>>))]
        public async Task<IActionResult> byCategoryId([FromRoute]int categoryId)
        {
            var result = await _specificationGroupService.GroupGetByCatId(categoryId);
            return new Response<List<SpecificationGroupFromDto>>().ResponseSending(result);
        }
        [HttpGet("byId/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SpecificationGroupDto>))]
        public async Task<IActionResult> bySpecGroupId([FromRoute]int id)
        {
            var result = await _specificationGroupService.GroupGetById(id);
            return new Response<SpecificationGroupDto>().ResponseSending(result);
        }
    }
}