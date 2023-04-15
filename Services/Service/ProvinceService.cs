using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Province;
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
    public class ProvinceService : IProvinceService
    {
        public IMapper _mapper { get; }
        public IProvinceRepository _ProvinceRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }


        public ProvinceService(
        IMapper mapper,
        IProvinceRepository ProvinceRepository,
        IMessageLanguageService ms,
        IHttpContextAccessor httpContextAccessor)
        {
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._ProvinceRepository = ProvinceRepository;
            _ms = ms;
            this._mapper = mapper;
        }

        public async Task<ApiResponse<ProvinceDto>> ProvinceAdd(ProvinceDto ProvinceDto)
        {
            var mapProvince = _mapper.Map<TProvince>(ProvinceDto);
            var craetedProvince = await _ProvinceRepository.ProvinceAdd(mapProvince);
            if (craetedProvince == null)
            {
                return new ApiResponse<ProvinceDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ProvinceAdding));
            }
            var mapCraetedProvince = _mapper.Map<ProvinceDto>(craetedProvince);
            return new ApiResponse<ProvinceDto>(ResponseStatusEnum.Success, mapCraetedProvince, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ProvinceDto>> ProvinceEdit(ProvinceDto ProvinceDto)
        {
            var exist = await this.ProvinceExist(ProvinceDto.ProvinceId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<ProvinceDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapProvince = _mapper.Map<TProvince>(ProvinceDto);
            var editedProvince = await _ProvinceRepository.ProvinceEdit(mapProvince);
            if (editedProvince == null)
            {
                return new ApiResponse<ProvinceDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ProvinceEditing));
            }
            var mapEditedProvince = _mapper.Map<ProvinceDto>(editedProvince);
            return new ApiResponse<ProvinceDto>(ResponseStatusEnum.Success, mapEditedProvince, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ProvinceDelete(int id)
        {
            var exist = await this.ProvinceExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result, _ms.MessageService(exist.Message));
            }
            var result = await _ProvinceRepository.ProvinceDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<bool>> ProvinceExist(int id)
        {
            var result = await _ProvinceRepository.ProvinceExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.ProvinceNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<ProvinceGetDto>>> ProvinceGetAll(PaginationDto pagination)
        {
            var data = await _ProvinceRepository.ProvinceGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ProvinceGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ProvinceGetting));
            }
            var count = await _ProvinceRepository.ProvinceGetAllCount(pagination);
            return new ApiResponse<Pagination<ProvinceGetDto>>(ResponseStatusEnum.Success, new Pagination<ProvinceGetDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<ProvinceGetDto>> GetProvinceById(int ProvinceId)
        {
            var data = await _ProvinceRepository.GetProvinceById(ProvinceId);
            if (data == null)
            {
                return new ApiResponse<ProvinceGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ProvinceGetting));
            }
            return new ApiResponse<ProvinceGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(List<AcceptDto> accept)
        {
            var data = await _ProvinceRepository.ChangeAccept(accept);
            if (data == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, false, _ms.MessageService(Message.ProvinceChangeStatus));
            }
            return new ApiResponse<bool>(ResponseStatusEnum.Success, true, _ms.MessageService(Message.Successfull));
        }
    }
}