using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.WebModule;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Height;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Image;
using System.Collections.Generic;
using MarketPlace.API.Data.Enums;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class WebModuleController : ControllerBase
    {
        public IWebModuleService _webModuleService { get; }
        public WebModuleController(IWebModuleService webModuleService)
        {
            _webModuleService = webModuleService;
        }

        [HttpPost("WebIndexModuleList")] 
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebIndexModuleListAddDto>))]
        public async Task<IActionResult> AddWebIndexModuleList([FromBody]WebIndexModuleListAddDto webIndexModuleList)
        {
            var result = await _webModuleService.WebIndexModuleListAdd(webIndexModuleList);
            return new Response<WebIndexModuleListAddDto>().ResponseSending(result);
        }

        [HttpPut("EditWebIndexModuleList")] 
        public async Task<IActionResult> EditWebIndexModuleTitle([FromBody]WebIndexModuleListAddDto webIndexModuleList)
        {
            var result = await _webModuleService.WebIndexModuleListEdit(webIndexModuleList);
            return new Response<WebIndexModuleListAddDto>().ResponseSending(result);
        }

        [HttpPost("UploadWebIndexModuleListImage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UploadWebIndexModuleListImage([FromForm]WebIndexModuleListSerialieDto imageDto)
        {

            var result = await _webModuleService.UploadModuleListImage(imageDto);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("WebIndexModuleList/changePriority")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangePriorityOfWebIndexModuleList([FromBody]ChangePriorityDto changePriority)
        {
            var result = await _webModuleService.ChangePriorityOfWebIndexModuleList(changePriority);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("WebIndexModuleList/ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAcceptOfWebIndexModuleList([FromBody]AcceptDto accept)
        {

            var result = await _webModuleService.ChangeAcceptOfWebIndexModuleList(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("WebIndexModuleList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<WebHomeIndexModuleListDto>>))]
        public async Task<IActionResult> Get()
        {
            var result = await _webModuleService.WebIndexModuleListGet();
            return new Response<List<WebHomeIndexModuleListDto>>().ResponseSending(result);

        }         
        
        [HttpGet("CategoryWebIndexModuleList/{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<WebHomeIndexModuleListDto>>))]
        public async Task<IActionResult> GetCategoryWebIndexModuleList([FromRoute]int categoryId)
        {
            var result = await _webModuleService.CategoryWebIndexModuleListGet(categoryId);
            return new Response<List<WebHomeIndexModuleListDto>>().ResponseSending(result);

        } 

        [HttpGet("GetWebCollectionType")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<WebCollectionTypeDto>>))]
        public async Task<IActionResult> GetWebCollectionType()
        {
            var result = await _webModuleService.GetWebCollectionType(false);
            return new Response<List<WebCollectionTypeDto>>().ResponseSending(result);
        }

        [HttpGet("GetWebCollectionTypeSlider")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<WebCollectionTypeDto>>))]
        public async Task<IActionResult> GetWebCollectionTypeSlider()
        {
            var result = await _webModuleService.GetWebCollectionType(true);
            return new Response<List<WebCollectionTypeDto>>().ResponseSending(result);
        }

        [HttpDelete("WebIndexModuleList/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteWebIndexModuleList([FromRoute]int id)
        {
            var result = await _webModuleService.WebIndexModuleListDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPost("WebModuleCollections")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebModuleCollectionsAddDto>))]
        public async Task<IActionResult> AddWebModuleCollections([FromForm]WebModuleCollectionsSerializeDto collectionsDto)
        {
            var result = await _webModuleService.WebModuleCollectionsAdd(collectionsDto);
            return new Response<WebModuleCollectionsAddDto>().ResponseSending(result);
        }

        [HttpPut("WebModuleCollections")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebModuleCollectionsAddDto>))]
        public async Task<IActionResult> EditWebModuleCollections([FromForm]WebModuleCollectionsSerializeDto collectionDto)
        {
            var result = await _webModuleService.WebModuleCollectionsEdit(collectionDto);
            return new Response<WebModuleCollectionsAddDto>().ResponseSending(result);
        }

        [HttpPut("WebModuleCollections/changePriority")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangePriorityOfWebModuleCollections([FromBody]ChangePriorityDto changePriority)
        {
            var result = await _webModuleService.ChangePriorityOfWebModuleCollections(changePriority);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("WebIndexModuleList/changeHeight")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> changeHeightOfWebIndexModuleList([FromBody]ChangeHeight changeHeight)
        {
            var result = await _webModuleService.ChangeHeightOfWebIndexModuleList(changeHeight);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("WebModuleCollections/UploadImage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UploadGoodsImage([FromForm]UploadTowImageDto imageDto)
        {
            var result = await _webModuleService.UploadWebModuleCollectionsImage(imageDto);
            return new Response<bool>().ResponseSending(result); 
        }

        [HttpDelete("WebModuleCollections/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteWebModuleCollections([FromRoute]int id)
        {
            var result = await _webModuleService.WebModuleCollectionsDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("WebModuleCollections/{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebModuleCollectionsGetDto>))]
        public async Task<IActionResult> GetWebModuleCollections([FromRoute]int id)
        {
            var result = await _webModuleService.WebModuleCollectionsGetById(id);
            return new Response<WebModuleCollectionsGetDto>().ResponseSending(result);
        }

        [HttpGet("WebModuleCollectionsList/{moduleId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<WebHomeModuleCollectionsDto>>))]
        public async Task<IActionResult> GetWebModuleCollectionsList([FromRoute]int moduleId)
        {
            var result = await _webModuleService.WebModuleCollections(moduleId);
            return new Response<List<WebHomeModuleCollectionsDto>>().ResponseSending(result);
        }
    }
}