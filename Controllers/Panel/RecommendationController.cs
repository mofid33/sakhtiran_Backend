using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Recommendation;
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
    public class RecommendationController : ControllerBase
    {
        public IRecommendationService _recommendationService { get; }
        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPost("AddRecommendation")]
        public async Task<IActionResult> AddRecommendation([FromBody] RecommendationAddDto recommendation)
        {
            var result = await _recommendationService.AddRecommendation(recommendation);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("EditRecommendation")]
        public async Task<IActionResult> EditRecommendation([FromBody]  RecommendationAddDto recommendation)
        {
            var result = await _recommendationService.EditRecommendation(recommendation);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<RecommendationGetDto>>))]
        [HttpGet("GetRecommendation")]
        public async Task<IActionResult> GetRecommendation([FromQuery] PaginationRecommendationDto pagination)
        {
            var result = await _recommendationService.GetRecommendationList(pagination);
            return new Response<Pagination<RecommendationGetDto>>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<RecommendationGetDto>>))]
        [HttpGet("GetRecommendationWithId/{recommendationId}")]
        public async Task<IActionResult> GetRecommendationWithId(int recommendationId)
        {
            var result = await _recommendationService.GetRecommendationWithId(recommendationId);
            return new Response<RecommendationAddDto>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("RecommendationDelete/{recommendationId}")]
        public async Task<IActionResult> RecommendationDelete(int recommendationId)
        {
            var result = await _recommendationService.RecommendationDelete(recommendationId);
            return new Response<bool>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPost("RecommendationChangeStatus")]
        public async Task<IActionResult> RecommendationChangeStatus( [FromBody] AcceptDto accept)
        {
            var result = await _recommendationService.ChangeStatus(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<RecommendationItemDto>>))]
        [HttpGet("RecommendationItemType")]
        public async Task<IActionResult> GetRecommendationItemType()
        {
            var result = await _recommendationService.GetRecommendationItemType();
            return new Response<List<RecommendationItemDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<RecommendationItemDto>>))]
        [HttpGet("RecommendationCollectionType")]
        public async Task<IActionResult> GetRecommendationCollectionType()
        {
            var result = await _recommendationService.GetRecommendationCollectionType();
            return new Response<List<RecommendationItemDto>>().ResponseSending(result);
        }



    }
}