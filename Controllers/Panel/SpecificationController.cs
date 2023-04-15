using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Enums;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SpecificationController : ControllerBase
    {
        public ISpecificationService _specificationService { get; }
        public ISpecificationGroupService _specificationGroupService { get; }
        public SpecificationController(ISpecificationService specificationService,
        ISpecificationGroupService specificationGroupService)
        {
            _specificationGroupService = specificationGroupService;
            _specificationService = specificationService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SpecificationAddGetDto>))]
        public async Task<IActionResult> Add([FromBody]SpecificationAddGetDto specificationAdd)
        {
            var result = await _specificationService.SpecificationAdd(specificationAdd);
            return new Response<SpecificationAddGetDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SpecificationGetDto>))]
        public async Task<IActionResult> Edit([FromBody]SpecificationEditDto SpecificationEdit)
        {
            var result = await _specificationService.SpecificationEdit(SpecificationEdit);
            return new Response<SpecificationGetDto>().ResponseSending(result);
        }

        [HttpPut("EditKeyAndRequired")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditKeyAndRequired([FromBody]SpecificationKeyAndRequiredDto KeyAndRequired)
        {
            var result = await _specificationService.ChangeKeyAndRequired(KeyAndRequired);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("EditPriorityGroup")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditPriorityGroup([FromBody]ChangePriorityDto changePriority)
        {
            var result = await _specificationService.EditPriorityGroup(changePriority);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("EditPrioritySpec")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditPrioritySpec([FromBody]ChangePriorityDto changePriority)
        {
            var result = await _specificationService.EditPrioritySpec(changePriority);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("ByCategoryId/{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CategorySpecificationGetDto>>))]
        public async Task<IActionResult> ByCategoryId([FromRoute]int categoryId)
        {
            var result = await _specificationService.SpecificationGetByCategoryId(categoryId);
            return new Response<List<CategorySpecificationGetDto>>().ResponseSending(result);
        } 
        
        [HttpGet("ById/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SpecificationGetDto>))]
        public async Task<IActionResult> ById([FromRoute]int id)
        { 
            var result = await _specificationService.SpecificationGetById(id);
            return new Response<SpecificationGetDto>().ResponseSending(result);
        }

        [HttpDelete("byId/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _specificationService.SpecificationDeletebyId(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpDelete("byCategoryId/{categoryid}/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute]int categoryid ,[FromRoute] int id)
        {
            var result = await _specificationService.SpecificationDeletebyCategoryId(categoryid,id);
            return new Response<bool>().ResponseSending(result);
        }


        [HttpGet("byGroupId/{groupId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<SpecificationFormDto>>))]
        public async Task<IActionResult> GetSpecsByGroupId([FromRoute]int groupId)
        {
            var result = await _specificationService.GetSpecsByGroupId(groupId);
            return new Response<List<SpecificationFormDto>>().ResponseSending(result);
        } 


        [HttpGet("GetSpecs")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<SpecificationCatGroupDto>>))]
        public async Task<IActionResult> GetSpecs([FromQuery]SpecPagination pagination)
        {
            var result = await _specificationService.GetSpecs(pagination);
            return new Response<Pagination<SpecificationCatGroupDto>>().ResponseSending(result);
        }

        
        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeAccept([FromBody]AcceptDto accept)
        {
            var result = await _specificationService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }
    }
}
