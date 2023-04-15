using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Enums;

namespace MarketPlace.API.Services.Service
{
    public class GoodsSurveyQuestionsService : IGoodsSurveyQuestionsService
    {
        public IMapper _mapper { get; }
        public IGoodsSurveyQuestionsRepository _goodsSurveyQuestionsRepository { get; }
        public IMessageLanguageService _ms { get; }

        public GoodsSurveyQuestionsService(IMapper mapper, IGoodsSurveyQuestionsRepository goodsSurveyQuestionsRepository,IMessageLanguageService ms)
        {
            this._goodsSurveyQuestionsRepository = goodsSurveyQuestionsRepository;
            this._mapper = mapper;
            _ms = ms;
        }
        public async Task<ApiResponse<GoodsSurveyQuestionsDto>> GoodsSurveyQuestionsAdd(GoodsSurveyQuestionsDto GoodsSurveyQuestions)
        {
            var mapGoodsSurveyQuestions = _mapper.Map<TGoodsSurveyQuestions>(GoodsSurveyQuestions);
            var craetedGoodsSurveyQuestions = await _goodsSurveyQuestionsRepository.GoodsSurveyQuestionsAdd(mapGoodsSurveyQuestions);
            if (craetedGoodsSurveyQuestions == null)
            {
                return new ApiResponse<GoodsSurveyQuestionsDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.GoodsSurveyQuestionsAdding));
            }
            var mapCraetedGoodsSurveyQuestions = _mapper.Map<GoodsSurveyQuestionsDto>(craetedGoodsSurveyQuestions);
            return new ApiResponse<GoodsSurveyQuestionsDto>(ResponseStatusEnum.Success, mapCraetedGoodsSurveyQuestions,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> GoodsSurveyQuestionsDelete(int id)
        {
            var exist = await this.GoodsSurveyQuestionsExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result,  _ms.MessageService(exist.Message));
            }
            var result = await _goodsSurveyQuestionsRepository.GoodsSurveyQuestionsDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result,  _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result,  _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<GoodsSurveyQuestionsDto>> GoodsSurveyQuestionsEdit(GoodsSurveyQuestionsDto GoodsSurveyQuestions)
        {
            var exist = await this.GoodsSurveyQuestionsExist(GoodsSurveyQuestions.QueId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<GoodsSurveyQuestionsDto>((ResponseStatusEnum)exist.Status, null, exist.Message);
            }
            var mapGoodsSurveyQuestions = _mapper.Map<TGoodsSurveyQuestions>(GoodsSurveyQuestions);
            var editedGoodsSurveyQuestions = await _goodsSurveyQuestionsRepository.GoodsSurveyQuestionsEdit(mapGoodsSurveyQuestions);
            if (editedGoodsSurveyQuestions == null)
            {
                return new ApiResponse<GoodsSurveyQuestionsDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.GoodsSurveyQuestionsEditing));
            }
            var mapEditedGoodsSurveyQuestions = _mapper.Map<GoodsSurveyQuestionsDto>(editedGoodsSurveyQuestions);
           return new ApiResponse<GoodsSurveyQuestionsDto>(ResponseStatusEnum.Success, mapEditedGoodsSurveyQuestions,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> GoodsSurveyQuestionsExist(int id)
        {
            var result = await _goodsSurveyQuestionsRepository.GoodsSurveyQuestionsExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result,_ms.MessageService(Message.GoodsSurveyQuestionsNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result,_ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<List<GoodsSurveyQuestionsDto>>> GoodsSurveyQuestionsGetAll(int categoryId)
        {
            var data = await _goodsSurveyQuestionsRepository.GoodsSurveyQuestionsGetAll(categoryId);
            if (data == null)
            {
                return new ApiResponse<List<GoodsSurveyQuestionsDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.GoodsSurveyQuestionsGetting));
            }
            return new ApiResponse<List<GoodsSurveyQuestionsDto>>(ResponseStatusEnum.Success,data,_ms.MessageService(Message.Successfull));
          
        }
    }
}