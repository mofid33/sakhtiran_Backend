using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Recommendation;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IRecommendationService
    {
         Task<ApiResponse<bool>> AddRecommendation(RecommendationAddDto recommendation);
         Task<ApiResponse<bool>> EditRecommendation(RecommendationAddDto recommendation);
         Task<ApiResponse<Pagination<RecommendationGetDto>>> GetRecommendationList(PaginationRecommendationDto pagination);
         Task<ApiResponse<RecommendationAddDto>> GetRecommendationWithId(int recommendationId);

         Task<ApiResponse<List<RecommendationItemDto>>> GetRecommendationItemType();
         Task<ApiResponse<List<RecommendationItemDto>>> GetRecommendationCollectionType();
        Task<ApiResponse<bool>> RecommendationDelete(int id);
        Task<ApiResponse<bool>> ChangeStatus(AcceptDto accept);
    }
}