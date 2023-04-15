using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.Variation;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class VariationService : IVariationService
    {
        public IMapper _mapper { get; }
        public IVariationRepository _variationRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }
        public ICategoryRepository _categoryRepository { get; set; }


        public VariationService(
        IMapper mapper,
        IVariationRepository variationRepository,
        ICategoryRepository categoryRepository,
        IMessageLanguageService ms,
        IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._variationRepository = variationRepository;
            _ms = ms;
            this._mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<VariationParameterDto>> VariationParameterAdd(VariationParameterDto variationParameterDto)
        {
            var mapVariationParameter = _mapper.Map<TVariationParameter>(variationParameterDto);
            var craetedVariationParameter = await _variationRepository.VariationParameterAdd(mapVariationParameter);
            if (craetedVariationParameter == null)
            {
                return new ApiResponse<VariationParameterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterAdding));
            }
            var mapCraetedVariationParameter = _mapper.Map<VariationParameterDto>(craetedVariationParameter);
            return new ApiResponse<VariationParameterDto>(ResponseStatusEnum.Success, mapCraetedVariationParameter, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<VariationParameterDto>> VariationParameterEdit(VariationParameterDto variationParameterDto)
        {
            var exist = await this.VariationParameterExist(variationParameterDto.ParameterId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<VariationParameterDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapVariationParameter = _mapper.Map<TVariationParameter>(variationParameterDto);
            var editedVariationParameter = await _variationRepository.VariationParameterEdit(mapVariationParameter);
            if (editedVariationParameter == null)
            {
                return new ApiResponse<VariationParameterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterEditing));
            }
            var mapEditedVariationParameter = _mapper.Map<VariationParameterDto>(editedVariationParameter);
            return new ApiResponse<VariationParameterDto>(ResponseStatusEnum.Success, mapEditedVariationParameter, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> VariationParameterDelete(int id)
        {
            var exist = await this.VariationParameterExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result, _ms.MessageService(exist.Message));
            }
            var result = await _variationRepository.VariationParameterDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<bool>> VariationParameterExist(int id)
        {
            var result = await _variationRepository.VariationParameterExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.VariationParameterNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<VariationParameterGetDto>>> VariationParameterGetAll(PaginationDto pagination)
        {
            if(pagination.Id != 0)
            {
                pagination.ChildIds = await _categoryRepository.GetCategoriesChilds(pagination.Id);
            }
            var data = await _variationRepository.VariationParameterGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<VariationParameterGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterGetting));
            }
            var count = await _variationRepository.VariationParameterGetAllCount(pagination);
            return new ApiResponse<Pagination<VariationParameterGetDto>>(ResponseStatusEnum.Success, new Pagination<VariationParameterGetDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<VariationParameterDto>> GetVariationParameterById(int variationParameterId)
        {
            var data = await _variationRepository.GetVariationParameterById(variationParameterId);
            if (data == null)
            {
                return new ApiResponse<VariationParameterDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterGetting));
            }
            return new ApiResponse<VariationParameterDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        //values
        public async Task<ApiResponse<VariationParameterValuesDto>> VariationParameterValuesAdd(VariationParameterValuesDto valuesDto)
        {
            var mapVariationParameterValues = _mapper.Map<TVariationParameterValues>(valuesDto);
            var craetedVariationParameterValues = await _variationRepository.VariationParameterValuesAdd(mapVariationParameterValues);
            if (craetedVariationParameterValues == null)
            {
                return new ApiResponse<VariationParameterValuesDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterValuesAdding));
            }
            var mapCraetedVariationParameterValues = _mapper.Map<VariationParameterValuesDto>(craetedVariationParameterValues);
            return new ApiResponse<VariationParameterValuesDto>(ResponseStatusEnum.Success, mapCraetedVariationParameterValues, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<VariationParameterValuesDto>> VariationParameterValuesEdit(VariationParameterValuesDto valuesDto)
        {
            var exist = await this.VariationParameterValuesExist(valuesDto.ValueId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<VariationParameterValuesDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapVariationParameterValues = _mapper.Map<TVariationParameterValues>(valuesDto);
            var editedVariationParameterValues = await _variationRepository.VariationParameterValuesEdit(mapVariationParameterValues);
            if (editedVariationParameterValues == null)
            {
                return new ApiResponse<VariationParameterValuesDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterValuesEditing));
            }
            var mapEditedVariationParameterValues = _mapper.Map<VariationParameterValuesDto>(editedVariationParameterValues);
            return new ApiResponse<VariationParameterValuesDto>(ResponseStatusEnum.Success, mapEditedVariationParameterValues, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> VariationParameterValuesDelete(int id)
        {
            var exist = await this.VariationParameterValuesExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result, _ms.MessageService(exist.Message));
            }
            var result = await _variationRepository.VariationParameterValuesDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<bool>> VariationParameterValuesExist(int id)
        {
            var result = await _variationRepository.VariationParameterValuesExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.VariationParameterValuesNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<VariationParameterValuesDto>>> VariationParameterValuesGetAll(PaginationDto pagination)
        {
            var data = await _variationRepository.VariationParameterValuesGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<VariationParameterValuesDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterValuesGetting));
            }
            var count = await _variationRepository.VariationParameterValuesGetAllCount(pagination);
            return new ApiResponse<Pagination<VariationParameterValuesDto>>(ResponseStatusEnum.Success, new Pagination<VariationParameterValuesDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<VariationParameterValuesDto>> GetVariationParameterValuesById(int valuesId)
        {
            var data = await _variationRepository.GetVariationParameterValuesById(valuesId);
            if (data == null)
            {
                return new ApiResponse<VariationParameterValuesDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VariationParameterValuesGetting));
            }
            return new ApiResponse<VariationParameterValuesDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
    }
}