using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.ReturningType;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Token;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class ReturningTypeService : IReturningTypeService
    {
        public IMapper _mapper { get; }
        public IReturningTypeRepository _ReturningTypeRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public ReturningTypeService(IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms, IReturningTypeRepository ReturningTypeRepository)
        {
            this._ReturningTypeRepository = ReturningTypeRepository;
            this._mapper = mapper;
            header = new HeaderParseDto(httpContextAccessor);
            this._ms = ms;
            token = new TokenParseDto(httpContextAccessor);
        }
        public async Task<ApiResponse<ReturningTypeDto>> ReturningTypeAdd(ReturningTypeDto ReturningType)
        {
            var mapReturningType = _mapper.Map<TReturningAction>(ReturningType);
            var craetedReturningType = await _ReturningTypeRepository.ReturningTypeAdd(mapReturningType);
            if (craetedReturningType == null)
            {
                return new ApiResponse<ReturningTypeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningTypeAdding));
            }
            var mapCraetedReturningType = _mapper.Map<ReturningTypeDto>(craetedReturningType);
            return new ApiResponse<ReturningTypeDto>(ResponseStatusEnum.Success, mapCraetedReturningType, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ReturningTypeDelete(int id)
        {
            var exist = await this.ReturningTypeExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _ReturningTypeRepository.ReturningTypeDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<ReturningTypeDto>> ReturningTypeEdit(ReturningTypeDto ReturningType)
        {
            var exist = await this.ReturningTypeExist(ReturningType.ReturningTypeId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<ReturningTypeDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapReturningType = _mapper.Map<TReturningAction>(ReturningType);
            var editedReturningType = await _ReturningTypeRepository.ReturningTypeEdit(mapReturningType);
            if (editedReturningType == null)
            {
                return new ApiResponse<ReturningTypeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningTypeEditing));
            }
            var mapEditedReturningType = _mapper.Map<ReturningTypeDto>(editedReturningType);
            return new ApiResponse<ReturningTypeDto>(ResponseStatusEnum.Success, mapEditedReturningType, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ReturningTypeExist(int id)
        {
            var result = await _ReturningTypeRepository.ReturningTypeExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.ReturningTypeNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<ReturningTypeDto>>> ReturningTypeGetAll(PaginationDto pagination)
        {
            var data = await _ReturningTypeRepository.ReturningTypeGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ReturningTypeDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningTypeGetting));
            }
            var count = await _ReturningTypeRepository.ReturningTypeGetAllCount(pagination);
            return new ApiResponse<Pagination<ReturningTypeDto>>(ResponseStatusEnum.Success, new Pagination<ReturningTypeDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ReturningTypeDto>>> GetReturningType()
        {
            var data = await _ReturningTypeRepository.ReturningTypeGetAllForm();
            if (data == null)
            {
                return new ApiResponse<List<ReturningTypeDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningTypeGetting));
            }
            return new ApiResponse<List<ReturningTypeDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

    }
}