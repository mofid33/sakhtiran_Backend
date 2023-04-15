using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Data.Dtos.WebSlider;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Image;
using MarketPlace.API.Data.Enums;
//using Controllers.Models;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class WebSliderController : ControllerBase
    {
        public IWebSliderService _webSliderService { get; }

        public WebSliderController(IWebSliderService webSliderService)
        {
            _webSliderService = webSliderService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebSliderAddDto>))]
        public async Task<IActionResult> Add([FromForm] WebSliderSerializeDto webSliderDto)
        {
            var result = await _webSliderService.SliderAdd(webSliderDto);
            return new Response<WebSliderAddDto>().ResponseSending(result);
        }

        [HttpPut("changePriority")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangePrioritySlider([FromBody] ChangePriorityDto changePriority)
        {
            var result = await _webSliderService.ChangePrioritySlider(changePriority);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebSliderAddDto>))]
        public async Task<IActionResult> SliderEdit([FromForm] WebSliderSerializeDto SliderDto)
        {
            var result = await _webSliderService.SliderEdit(SliderDto);
            return new Response<WebSliderAddDto>().ResponseSending(result);
        }


        [HttpPut("UploadImage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UploadSliderImage([FromForm] UploadTowImageDto imageDto)
        {
            var result = await _webSliderService.UploadSliderImage(imageDto);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _webSliderService.SliderDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<WebSliderGetDto>))]
        public async Task<IActionResult> GetSliderById([FromRoute] int id)
        {
            var result = await _webSliderService.SliderGetById(id);
            return new Response<WebSliderGetDto>().ResponseSending(result);
        }

        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<WebSliderGetListDto>>))]
        public async Task<IActionResult> GetSliders()
        {
            var result = await _webSliderService.SliderGet();
            return new Response<List<WebSliderGetListDto>>().ResponseSending(result);
        }

        [HttpGet("Category/{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<WebSliderGetListDto>>))]
        public async Task<IActionResult> CategorySliderGet(int categoryId)
        {
            var result = await _webSliderService.CategorySliderGet(categoryId);
            return new Response<List<WebSliderGetListDto>>().ResponseSending(result);
        }

    }
}