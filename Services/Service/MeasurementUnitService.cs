using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.MeasurementUnit;
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
    public class MeasurementUnitService : IMeasurementUnitService
    {
        public IMapper _mapper { get; }
        public IMeasurementUnitRepository _MeasurementUnitRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public MeasurementUnitService(IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
        IMessageLanguageService ms, IMeasurementUnitRepository MeasurementUnitRepository)
        {
            this._MeasurementUnitRepository = MeasurementUnitRepository;
            this._mapper = mapper;
            header = new HeaderParseDto(httpContextAccessor);
            this._ms = ms;
            token = new TokenParseDto(httpContextAccessor);
        }
        public async Task<ApiResponse<MeasurementUnitDto>> MeasurementUnitAdd(MeasurementUnitDto MeasurementUnit)
        {
            var mapMeasurementUnit = _mapper.Map<TMeasurementUnit>(MeasurementUnit);
            var craetedMeasurementUnit = await _MeasurementUnitRepository.MeasurementUnitAdd(mapMeasurementUnit);
            if (craetedMeasurementUnit == null)
            {
                return new ApiResponse<MeasurementUnitDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MeasurementUnitAdding));
            }
            var mapCraetedMeasurementUnit = _mapper.Map<MeasurementUnitDto>(craetedMeasurementUnit);
            return new ApiResponse<MeasurementUnitDto>(ResponseStatusEnum.Success, mapCraetedMeasurementUnit, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> MeasurementUnitDelete(int id)
        {
            var exist = await this.MeasurementUnitExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _MeasurementUnitRepository.MeasurementUnitDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<MeasurementUnitDto>> MeasurementUnitEdit(MeasurementUnitDto MeasurementUnit)
        {
            var exist = await this.MeasurementUnitExist(MeasurementUnit.UnitId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<MeasurementUnitDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapMeasurementUnit = _mapper.Map<TMeasurementUnit>(MeasurementUnit);
            var editedMeasurementUnit = await _MeasurementUnitRepository.MeasurementUnitEdit(mapMeasurementUnit);
            if (editedMeasurementUnit == null)
            {
                return new ApiResponse<MeasurementUnitDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MeasurementUnitEditing));
            }
            var mapEditedMeasurementUnit = _mapper.Map<MeasurementUnitDto>(editedMeasurementUnit);
            return new ApiResponse<MeasurementUnitDto>(ResponseStatusEnum.Success, mapEditedMeasurementUnit, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> MeasurementUnitExist(int id)
        {
            var result = await _MeasurementUnitRepository.MeasurementUnitExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.MeasurementUnitNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<MeasurementUnitDto>>> MeasurementUnitGetAll(PaginationDto pagination)
        {
            var data = await _MeasurementUnitRepository.MeasurementUnitGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<MeasurementUnitDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MeasurementUnitGetting));
            }
            var count = await _MeasurementUnitRepository.MeasurementUnitGetAllCount(pagination);
            return new ApiResponse<Pagination<MeasurementUnitDto>>(ResponseStatusEnum.Success, new Pagination<MeasurementUnitDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<MeasurementUnitDto>>> GetMeasurementUnit()
        {
            var data = await _MeasurementUnitRepository.MeasurementUnitGetAllForm();
            if (data == null)
            {
                return new ApiResponse<List<MeasurementUnitDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MeasurementUnitGetting));
            }
            return new ApiResponse<List<MeasurementUnitDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }
    }
}