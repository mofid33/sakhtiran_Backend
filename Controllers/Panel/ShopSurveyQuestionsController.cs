using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using System.Collections.Generic;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Setting;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ShopSurveyQuestionsController : ControllerBase
    {
        public IShopSurveyQuestionsService _ShopSurveyQuestionsService { get; }

        public ShopSurveyQuestionsController(IShopSurveyQuestionsService ShopSurveyQuestionsService)
        {
            this._ShopSurveyQuestionsService = ShopSurveyQuestionsService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopSurveyQuestionsDto>))]
        public async Task<IActionResult> Add([FromBody]ShopSurveyQuestionsDto ShopSurveyQuestionsAddDto)
        {
            var result = await _ShopSurveyQuestionsService.ShopSurveyQuestionsAdd(ShopSurveyQuestionsAddDto);
            return new Response<ShopSurveyQuestionsDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopSurveyQuestionsDto>))]
        public async Task<IActionResult> Edit([FromBody]ShopSurveyQuestionsDto ShopSurveyQuestionsEditDto)
        {
            var result = await _ShopSurveyQuestionsService.ShopSurveyQuestionsEdit(ShopSurveyQuestionsEditDto);
            return new Response<ShopSurveyQuestionsDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _ShopSurveyQuestionsService.ShopSurveyQuestionsDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet()]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShopSurveyQuestionsDto>>))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ShopSurveyQuestionsService.ShopSurveyQuestionsGetAll();
            return new Response<List<ShopSurveyQuestionsDto>>().ResponseSending(result);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopSurveyQuestionsDto>))]
        public async Task<IActionResult> GetOne([FromRoute]int id)
        {
            var result = await _ShopSurveyQuestionsService.ShopSurveyQuestionsGetOne(id);
            return new Response<ShopSurveyQuestionsDto>().ResponseSending(result);
        }
        
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("ChangeAccept")]
        public async Task<IActionResult> ChangeAccept([FromBody] AcceptDto accept)
        {
            var result = await _ShopSurveyQuestionsService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        [HttpPut("EditShopCalculateComment")]
        public async Task<IActionResult> EditShopCalculateComment([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _ShopSurveyQuestionsService.EditShopCalculateComment(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        [HttpGet("GetShopCalculateComment")]
        public async Task<IActionResult> GetShopCalculateComment()
        {
            var result = await _ShopSurveyQuestionsService.GetShopCalculateComment();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

    }
}