using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.City;
using MarketPlace.API.Data.Dtos.Header;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class CityService : ICityService
    {
        public IMapper _mapper { get; }
        public ICityRepository _cityRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }


        public CityService(
        IMapper mapper,
        ICityRepository cityRepository,
        IMessageLanguageService ms,
        IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._cityRepository = cityRepository;
            _ms = ms;
            this._mapper = mapper;
        }

        public async Task<ApiResponse<CityDto>> CityAdd(CityDto cityDto)
        {
            var mapCity = _mapper.Map<TCity>(cityDto);
            var craetedCity = await _cityRepository.CityAdd(mapCity);
            if (craetedCity == null)
            {
                return new ApiResponse<CityDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CityAdding));
            }
            var mapCraetedCity = _mapper.Map<CityDto>(craetedCity);
            return new ApiResponse<CityDto>(ResponseStatusEnum.Success, mapCraetedCity, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<CityDto>> CityEdit(CityDto cityDto)
        {
            var exist = await this.CityExist(cityDto.CityId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<CityDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapCity = _mapper.Map<TCity>(cityDto);
            var editedCity = await _cityRepository.CityEdit(mapCity);
            if (editedCity == null)
            {
                return new ApiResponse<CityDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CityEditing));
            }
            var mapEditedCity = _mapper.Map<CityDto>(editedCity);
            return new ApiResponse<CityDto>(ResponseStatusEnum.Success, mapEditedCity, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> CityDelete(int id)
        {
            var exist = await this.CityExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result, _ms.MessageService(exist.Message));
            }
            var result = await _cityRepository.CityDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<bool>> CityExist(int id)
        {
            var result = await _cityRepository.CityExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.CityNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<CityGetDto>>> CityGetAll(PaginationDto pagination)
        {
            var data = await _cityRepository.CityGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CityGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CityGetting));
            }
            var count = await _cityRepository.CityGetAllCount(pagination);
            return new ApiResponse<Pagination<CityGetDto>>(ResponseStatusEnum.Success, new Pagination<CityGetDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<CityGetDto>> GetCityById(int CityId)
        {
            var data = await _cityRepository.GetCityById(CityId);
            if (data == null)
            {
                return new ApiResponse<CityGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CityGetting));
            }
            return new ApiResponse<CityGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(List<AcceptDto> accept)
        {
            var data = await _cityRepository.ChangeAccept(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.CityChangeStatus));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }


        public async Task<ApiResponse<CityShippingMethodAreaCodeDto>> AddShippingMethodAreaCode(CityShippingMethodAreaCodeDto ShippingMethodAreaCode)
        {
            var mapShipping = _mapper.Map<TShippingMethodAreaCode>(ShippingMethodAreaCode);
            var find = await _cityRepository.ShippingMethodAreaCodeExist(0,mapShipping.Code , mapShipping.FkShippingMethodId , mapShipping.FkCityId);
            if(find) {
                return new ApiResponse<CityShippingMethodAreaCodeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.AreaFoundById));
            }

            var craetedShipping = await _cityRepository.AddShippingMethodAreaCode(mapShipping);
            if (craetedShipping == null)
            {
                return new ApiResponse<CityShippingMethodAreaCodeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.AreaAdding));
            }
            var mapCraetedShipping = _mapper.Map<CityShippingMethodAreaCodeDto>(craetedShipping);
            return new ApiResponse<CityShippingMethodAreaCodeDto>(ResponseStatusEnum.Success, mapCraetedShipping, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<CityShippingMethodAreaCodeDto>> UpdateShippingMethodAreaCode(CityShippingMethodAreaCodeDto ShippingMethodAreaCode)
        {
            var mapShipping = _mapper.Map<TShippingMethodAreaCode>(ShippingMethodAreaCode);
            var find = await _cityRepository.ShippingMethodAreaCodeExist(mapShipping.PostAreaCodeId,mapShipping.Code , mapShipping.FkShippingMethodId , mapShipping.FkCityId);
            if(find) {
                return new ApiResponse<CityShippingMethodAreaCodeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.AreaFoundById));
            }
            var editedShipping = await _cityRepository.UpdateShippingMethodAreaCode(mapShipping);
            if (editedShipping == null)
            {
                return new ApiResponse<CityShippingMethodAreaCodeDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.AreaEditing));
            }
            var mapEditedShipping = _mapper.Map<CityShippingMethodAreaCodeDto>(editedShipping);
            return new ApiResponse<CityShippingMethodAreaCodeDto>(ResponseStatusEnum.Success, mapEditedShipping, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> DeleteShippingMethodAreaCode(int id)
        {
            var result = await _cityRepository.DeleteShippingMethodAreaCode(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }
        public async Task<ApiResponse<Pagination<CityShippingMethodAreaCodeDto>>> ShippingMethodAreaGetAll(PaginationDto pagination)
        {
            var data = await _cityRepository.ShippingMethodGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CityShippingMethodAreaCodeDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.AreaGetting));
            }
            var count = await _cityRepository.ShippingMethodGetAllCount(pagination);
            return new ApiResponse<Pagination<CityShippingMethodAreaCodeDto>>(ResponseStatusEnum.Success, new Pagination<CityShippingMethodAreaCodeDto>(count, data), _ms.MessageService(Message.Successfull));
        }




    }
}