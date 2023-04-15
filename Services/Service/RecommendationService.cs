using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Recommendation;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class RecommendationService : IRecommendationService
    {
        public IMapper _mapper { get; }
        public IRecommendationRepository _recommendationRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public RecommendationService(IMapper mapper, 
         IRecommendationRepository recommendationRepository,
        IMessageLanguageService ms,
        IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._recommendationRepository = recommendationRepository;
            this._mapper = mapper;
            _ms = ms;
        }

        public async Task<ApiResponse<bool>> AddRecommendation(RecommendationAddDto recommendation)
        {
            var mapRecommendation = _mapper.Map<TRecommendation>(recommendation);
            var craetedmapRecommendation = await _recommendationRepository.AddRecommendation(mapRecommendation);
            if (craetedmapRecommendation == null)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.RecommendationAdding));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<bool>> EditRecommendation(RecommendationAddDto recommendation)
        {
            var mapRecommendation = _mapper.Map<TRecommendation>(recommendation);
            var craetedmapRecommendation = await _recommendationRepository.EditRecommendation(mapRecommendation);
            if (craetedmapRecommendation == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.RecommendationEditing));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<RecommendationGetDto>>> GetRecommendationList(PaginationRecommendationDto pagination)
        {
            var data = await _recommendationRepository.GetRecommendationList(pagination);
            if(data == null)
            {
                return new ApiResponse<Pagination<RecommendationGetDto>>(ResponseStatusEnum.BadRequest,null,_ms.MessageService(Message.RecommendationGetting));
            }
            var count = await _recommendationRepository.GetRecommendationListCount(pagination);
            return new ApiResponse<Pagination<RecommendationGetDto>>(ResponseStatusEnum.Success,new Pagination<RecommendationGetDto>(count,data),_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<RecommendationItemDto>>> GetRecommendationItemType()
        {
            var data = await _recommendationRepository.GetRecommendationItemType();
            return new ApiResponse<List<RecommendationItemDto>>(ResponseStatusEnum.Success,data,_ms.MessageService(Message.Successfull));        
        }

        public async Task<ApiResponse<List<RecommendationItemDto>>> GetRecommendationCollectionType()
        {
            var data = await _recommendationRepository.GetRecommendationCollectionType();
            return new ApiResponse<List<RecommendationItemDto>>(ResponseStatusEnum.Success,data,_ms.MessageService(Message.Successfull));     
        }

        public async Task<ApiResponse<RecommendationAddDto>> GetRecommendationWithId(int recommendationId)
        {
            var recommendation = await _recommendationRepository.GetRecommendationWithId(recommendationId);
            if (recommendation == null)
            {
                return new ApiResponse<RecommendationAddDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.RecommendationGetting));
            }
            return new ApiResponse<RecommendationAddDto>(ResponseStatusEnum.Success, recommendation,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> RecommendationDelete(int id)
        {
            var result = await _recommendationRepository.RecommendationDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result,_ms.MessageService(result.Message) );
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result,_ms.MessageService(result.Message) );
            }       
            
         }

        public async Task<ApiResponse<bool>> ChangeStatus(AcceptDto accept)
        {
            var result = await _recommendationRepository.ChangeStatus(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false,_ms.MessageService(Message.RecommendationGetting));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true,_ms.MessageService(Message.Successfull));
            }       
        }
    }
}