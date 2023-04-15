using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using System.Collections.Generic;
using MarketPlace.API.Data.Enums;

namespace MarketPlace.API.Controllers.Panel
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GoodsSurveyQuestionsController : ControllerBase
    {
        public IGoodsSurveyQuestionsService _goodsSurveyQuestionsService { get; }

        public GoodsSurveyQuestionsController(IGoodsSurveyQuestionsService goodsSurveyQuestionsService)
        {
            this._goodsSurveyQuestionsService = goodsSurveyQuestionsService;
        }

        [HttpPost]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsSurveyQuestionsDto>))]
        public async Task<IActionResult> Add([FromBody]GoodsSurveyQuestionsDto goodsSurveyQuestionsAddDto)
        {
            var result = await _goodsSurveyQuestionsService.GoodsSurveyQuestionsAdd(goodsSurveyQuestionsAddDto);
            return new Response<GoodsSurveyQuestionsDto>().ResponseSending(result);
        }

        [HttpPut]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsSurveyQuestionsDto>))]
        public async Task<IActionResult> Edit([FromBody]GoodsSurveyQuestionsDto goodsSurveyQuestionsEditDto)
        {
            var result = await _goodsSurveyQuestionsService.GoodsSurveyQuestionsEdit(goodsSurveyQuestionsEditDto);
            return new Response<GoodsSurveyQuestionsDto>().ResponseSending(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var result = await _goodsSurveyQuestionsService.GoodsSurveyQuestionsDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GoodsSurveyQuestionsDto>>))]
        public async Task<IActionResult> GetAll([FromRoute]int categoryId)
        {
            var result = await _goodsSurveyQuestionsService.GoodsSurveyQuestionsGetAll(categoryId);
            return new Response<List<GoodsSurveyQuestionsDto>>().ResponseSending(result);
        }
    }
}