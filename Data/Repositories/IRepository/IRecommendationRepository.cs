using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Recommendation;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IRecommendationRepository
    {
         Task<TRecommendation> AddRecommendation(TRecommendation recommendationDto);
         Task<bool> EditRecommendation(TRecommendation recommendationDto);
         Task<List<RecommendationGetDto>> GetRecommendationList(PaginationRecommendationDto pagination);
         Task<RecommendationAddDto> GetRecommendationWithId(int recommendationId);
         Task<int> GetRecommendationListCount(PaginationRecommendationDto pagination);
         Task<List<GoodsHomeDto>> GetRecommendationGoods(List<int> categoryIds, int goodsId);

         Task<List<RecommendationItemDto>> GetRecommendationItemType();
         Task<List<RecommendationItemDto>> GetRecommendationCollectionType();

         Task<RepRes<TRecommendation>> RecommendationDelete(int recommendationId);
         Task<bool> ChangeStatus(AcceptDto accept);
    }
}