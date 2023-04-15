using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Pagination;

namespace MarketPlace.API.Services.Service
{
    public class SpecificationGroupService : ISpecificationGroupService
    {
        public IMapper _mapper { get; }
        public ISpecificationGroupRepository _specificationGroupRepository { get; }
        public IMessageLanguageService _ms { get; set; }

        public SpecificationGroupService(IMapper mapper, IMessageLanguageService ms, ISpecificationGroupRepository specificationGroupRepository)
        {
            this._specificationGroupRepository = specificationGroupRepository;
            this._mapper = mapper;
            _ms = ms;
        }
        public async Task<ApiResponse<SpecificationGroupDto>> SpecificationGroupAdd(SpecificationGroupDto SpecificationGroup)
        {
            var mapSpecificationGroup = _mapper.Map<TSpecificationGroup>(SpecificationGroup);
            var craetedSpecificationGroup = await _specificationGroupRepository.SpecificationGroupAdd(mapSpecificationGroup);
            if (craetedSpecificationGroup == null)
            {
                return new ApiResponse<SpecificationGroupDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGroupAdding));
            }
            var mapCraetedSpecificationGroup = _mapper.Map<SpecificationGroupDto>(craetedSpecificationGroup);
            return new ApiResponse<SpecificationGroupDto>(ResponseStatusEnum.Success, mapCraetedSpecificationGroup, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> SpecificationGroupDelete(int id)
        {
            var exist = await this.SpecificationGroupExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, exist.Result, _ms.MessageService(exist.Message));
            }
            var result = await _specificationGroupRepository.SpecificationGroupDelete(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<SpecificationGroupDto>> SpecificationGroupEdit(SpecificationGroupDto SpecificationGroup)
        {
            var exist = await this.SpecificationGroupExist(SpecificationGroup.SpecGroupId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<SpecificationGroupDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var mapSpecificationGroup = _mapper.Map<TSpecificationGroup>(SpecificationGroup);
            var editedSpecificationGroup = await _specificationGroupRepository.SpecificationGroupEdit(mapSpecificationGroup);
            if (editedSpecificationGroup == null)
            {
                return new ApiResponse<SpecificationGroupDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGroupEditing));
            }
            var mapEditedSpecificationGroup = _mapper.Map<SpecificationGroupDto>(editedSpecificationGroup);
            return new ApiResponse<SpecificationGroupDto>(ResponseStatusEnum.Success, mapEditedSpecificationGroup, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> SpecificationGroupExist(int id)
        {
            var result = await _specificationGroupRepository.SpecificationGroupExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.SpecificationGroupNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<Pagination<SpecificationGroupDto>>> SpecificationGroupGetAll(PaginationDto pagination)
        {
            var data = await _specificationGroupRepository.SpecificationGroupGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<SpecificationGroupDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGroupGetting));
            }
            var count = await _specificationGroupRepository.SpecificationGroupGetAllCount(pagination);
            return new ApiResponse<Pagination<SpecificationGroupDto>>(ResponseStatusEnum.Success, new Pagination<SpecificationGroupDto>(count, data),  _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<SpecificationGroupFromDto>>> GroupGetByCatId(int categoryId)
        {
            var data = await _specificationGroupRepository.GroupGetByCatId(categoryId);
            if (data == null)
            {
                return new ApiResponse<List<SpecificationGroupFromDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGroupGetting));
            }
            return new ApiResponse<List<SpecificationGroupFromDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<SpecificationGroupDto>>> PanelSpecificationGroupGet()
        {
            var data = await _specificationGroupRepository.PanelSpecificationGroupGet();
            if (data == null)
            {
                return new ApiResponse<List<SpecificationGroupDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGroupGetting));
            }
            return new ApiResponse<List<SpecificationGroupDto>>(ResponseStatusEnum.Success,data,_ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<SpecificationGroupGetForGoodsDto>>> SpecificationGroupWithSpecGetAll(PaginationDto pagination)
        {
            var data = await _specificationGroupRepository.SpecificationGroupWithSpecGetAll(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<SpecificationGroupGetForGoodsDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGroupGetting));
            }
            var count = await _specificationGroupRepository.SpecificationGroupGetAllCount(pagination);
            return new ApiResponse<Pagination<SpecificationGroupGetForGoodsDto>>(ResponseStatusEnum.Success, new Pagination<SpecificationGroupGetForGoodsDto>(count, data),  _ms.MessageService(Message.Successfull));        
        }

        public async Task<ApiResponse<SpecificationGroupDto>> GroupGetById(int id)
        {
            var data = await _specificationGroupRepository.GroupGetById(id);
            if (data == null)
            {
                return new ApiResponse<SpecificationGroupDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGroupGetting));
            }
            return new ApiResponse<SpecificationGroupDto>(ResponseStatusEnum.Success, data,  _ms.MessageService(Message.Successfull));     
        }
    }
}