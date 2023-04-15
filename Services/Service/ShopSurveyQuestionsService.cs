using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Setting;

namespace MarketPlace.API.Services.Service
{
    public class ShopSurveyQuestionsService : IShopSurveyQuestionsService
    {
        public IMapper _mapper { get; }
        public IShopSurveyQuestionsRepository _ShopSurveyQuestionsRepository { get; }
        public IMessageLanguageService _ms { get; }

        public ShopSurveyQuestionsService(IMapper mapper, IShopSurveyQuestionsRepository ShopSurveyQuestionsRepository,IMessageLanguageService ms)
        {
            this._ShopSurveyQuestionsRepository = ShopSurveyQuestionsRepository;
            this._mapper = mapper;
            _ms = ms;
        }
        public async Task<ApiResponse<ShopSurveyQuestionsDto>> ShopSurveyQuestionsAdd(ShopSurveyQuestionsDto ShopSurveyQuestions)
        {
            var mapShopSurveyQuestions = _mapper.Map<TShopSurveyQuestions>(ShopSurveyQuestions);
            var craetedShopSurveyQuestions = await _ShopSurveyQuestionsRepository.ShopSurveyQuestionsAdd(mapShopSurveyQuestions);
            if (craetedShopSurveyQuestions == null)
            {
                return new ApiResponse<ShopSurveyQuestionsDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.ShopSurveyQuestionsAdding));
            }
            var mapCraetedShopSurveyQuestions = _mapper.Map<ShopSurveyQuestionsDto>(craetedShopSurveyQuestions);
            return new ApiResponse<ShopSurveyQuestionsDto>(ResponseStatusEnum.Success, mapCraetedShopSurveyQuestions,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ShopSurveyQuestionsDelete(int id)
        {
            var exist = await this.ShopSurveyQuestionsExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result,  _ms.MessageService(exist.Message));
            }
            var result = await _ShopSurveyQuestionsRepository.ShopSurveyQuestionsDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result,  _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result,  _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<ShopSurveyQuestionsDto>> ShopSurveyQuestionsEdit(ShopSurveyQuestionsDto ShopSurveyQuestions)
        {
            var exist = await this.ShopSurveyQuestionsExist(ShopSurveyQuestions.QueId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<ShopSurveyQuestionsDto>((ResponseStatusEnum)exist.Status, null, exist.Message);
            }
            var mapShopSurveyQuestions = _mapper.Map<TShopSurveyQuestions>(ShopSurveyQuestions);
            var editedShopSurveyQuestions = await _ShopSurveyQuestionsRepository.ShopSurveyQuestionsEdit(mapShopSurveyQuestions);
            if (editedShopSurveyQuestions == null)
            {
                return new ApiResponse<ShopSurveyQuestionsDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.ShopSurveyQuestionsEditing));
            }
            var mapEditedShopSurveyQuestions = _mapper.Map<ShopSurveyQuestionsDto>(editedShopSurveyQuestions);
           return new ApiResponse<ShopSurveyQuestionsDto>(ResponseStatusEnum.Success, mapEditedShopSurveyQuestions,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ShopSurveyQuestionsExist(int id)
        {
            var result = await _ShopSurveyQuestionsRepository.ShopSurveyQuestionsExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result,_ms.MessageService(Message.ShopSurveyQuestionsNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result,_ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<List<ShopSurveyQuestionsDto>>> ShopSurveyQuestionsGetAll()
        {
            var data = await _ShopSurveyQuestionsRepository.ShopSurveyQuestionsGetAll();
            if (data == null)
            {
                return new ApiResponse<List<ShopSurveyQuestionsDto>>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.ShopSurveyQuestionsGetting));
            }
            return new ApiResponse<List<ShopSurveyQuestionsDto>>(ResponseStatusEnum.Success,data,_ms.MessageService(Message.Successfull));
          
        }


        public async Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept)
        {
            var result = await _ShopSurveyQuestionsRepository.ChangeAccept(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ShopSurveyQuestionsGetting));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
            }
        }


        
        public async Task<ApiResponse<SettingDescriptionDto>> EditShopCalculateComment(SettingDescriptionDto settingDto)
        {
            var data = await _ShopSurveyQuestionsRepository.EditShopCalculateComment(settingDto);
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingEdit));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SettingDescriptionDto>> GetShopCalculateComment()
        {
            var data = await _ShopSurveyQuestionsRepository.GetShopCalculateComment();
            if (data == null)
            {
                return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.SettingGetting));
            }
            return new ApiResponse<SettingDescriptionDto>(ResponseStatusEnum.Success, data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ShopSurveyQuestionsDto>> ShopSurveyQuestionsGetOne(int id)
        {
            var data = await _ShopSurveyQuestionsRepository.ShopSurveyQuestionsGetOne(id);
            if (data == null)
            {
                return new ApiResponse<ShopSurveyQuestionsDto>(ResponseStatusEnum.BadRequest, null,_ms.MessageService(Message.ShopSurveyQuestionsGetting));
            }
            return new ApiResponse<ShopSurveyQuestionsDto>(ResponseStatusEnum.Success,data,_ms.MessageService(Message.Successfull));
                  
        }
    }
}