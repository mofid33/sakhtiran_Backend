using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Help;
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
    public class HelpController : ControllerBase
    {
        public IHelpService _helpService { get; }
        public HelpController(IHelpService helpService)
        {
            _helpService = helpService;
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPost("HelpTopic")]
        public async Task<IActionResult> AddHelpTopic([FromForm] HelpTopicSerializeDto helpTopicDto)
        {
            var result = await _helpService.AddHelpTopic(helpTopicDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("HelpTopic")]
        public async Task<IActionResult> EditHelpTopic([FromForm] HelpTopicSerializeDto helpTopicDto)
        {
            var result = await _helpService.EditHelpTopic(helpTopicDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<HelpTopicListDto>>))]
        [HttpGet("HelpTopic")]
        public async Task<IActionResult> GetHelpTopicList([FromQuery] PaginationFormDto pagination)
        {
            var result = await _helpService.GetHelpTopicList(pagination);
            return new Response<Pagination<HelpTopicListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HelpTopicGetDto>))]
        [HttpGet("HelpTopic/{topicId}")]
        public async Task<IActionResult> GetHelpTopicById([FromRoute] int topicId)
        {
            var result = await _helpService.GetHelpTopicById(topicId);
            return new Response<HelpTopicGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPost("HelpArticle")]
        public async Task<IActionResult> AddHelpArticle([FromBody] HelpArticleAddDto helpArticleDto)
        {
            var result = await _helpService.AddHelpArticle(helpArticleDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("HelpArticle")]
        public async Task<IActionResult> EditHelpArticle([FromBody]HelpArticleAddDto helpArticleDto)
        {
            var result = await _helpService.EditHelpArticle(helpArticleDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<HelpArticleListDto>>))]
        [HttpGet("HelpArticle")]
        public async Task<IActionResult> GetHelpArticleList([FromQuery] PaginationFormDto pagination)
        {
            var result = await _helpService.GetHelpArticleList(pagination);
            return new Response<Pagination<HelpArticleListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<HelpArticleGetDto>))]
        [HttpGet("HelpArticle/{articleId}")]
        public async Task<IActionResult> GetHelpArticleById([FromRoute] int articleId)
        {
            var result = await _helpService.GetHelpArticleById(articleId);
            return new Response<HelpArticleGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("HelpArticleDelete/{articleId}")]
        public async Task<IActionResult> HelpArticleDelete([FromRoute]int articleId)
        {
            var result = await _helpService.ArticleDelete(articleId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("HelpTopicDelete/{topicId}")]
        public async Task<IActionResult> HelpTopicDelete([FromRoute]int topicId)
        {
            var result = await _helpService.TopicDelete(topicId);
            return new Response<bool>().ResponseSending(result);
        }
    

    }
}