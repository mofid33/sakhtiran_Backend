using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Services.Service
{
    public class SpecificationService : ISpecificationService
    {
        public IMapper _mapper { get; }
        public ISpecificationRepository _specificationRepository { get; }
        public ICategoryRepository _categoryRepository { get; }
        public IMessageLanguageService _ms { get; set; }

        public SpecificationService(IMapper mapper, IMessageLanguageService ms, ICategoryRepository categoryRepository,ISpecificationRepository specificationRepository)
        {
            this._specificationRepository = specificationRepository;
            this._categoryRepository = categoryRepository;
            this._mapper = mapper;
            _ms = ms;
        }
        public async Task<ApiResponse<SpecificationAddGetDto>> SpecificationAdd(SpecificationAddGetDto specification)
        {
            var mapSpecification = _mapper.Map<TSpecification>(specification);
            var craetedSpecification = await _specificationRepository.SpecificationAdd(mapSpecification);
            if (craetedSpecification == null)
            {
                return new ApiResponse<SpecificationAddGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationAdding));
            }
            var mapCraetedSpecification = _mapper.Map<SpecificationAddGetDto>(craetedSpecification);
            return new ApiResponse<SpecificationAddGetDto>(ResponseStatusEnum.Success, mapCraetedSpecification, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeKeyAndRequired(SpecificationKeyAndRequiredDto KeyAndRequired)
        {
            var exist = await this.SpecificationExist(KeyAndRequired.SpecId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _specificationRepository.ChangeKeyAndRequired(KeyAndRequired);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.SpecificationChangeKeyAndRequired));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> EditPriorityGroup(ChangePriorityDto changePriority)
        {
            var result = await _specificationRepository.EditPriorityGroup(changePriority);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.SpecificationGroupChangePriority));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> EditPrioritySpec(ChangePriorityDto changePriority)
        {
            var exist = await this.SpecificationExist(changePriority.Id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _specificationRepository.EditPrioritySpec(changePriority);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result, _ms.MessageService(Message.SpecificationChangePriority));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<List<CategorySpecificationGetDto>>> SpecificationGetByCategoryId(int categoryId)
        {
            var categorySpecification = await _specificationRepository.SpecificationGetByCategoryId(categoryId);
            if (categorySpecification == null)
            {
                return new ApiResponse<List<CategorySpecificationGetDto>>(ResponseStatusEnum.BadRequest, categorySpecification, _ms.MessageService(Message.SpecificationGetting));
            }
            return new ApiResponse<List<CategorySpecificationGetDto>>(ResponseStatusEnum.Success, categorySpecification, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> SpecificationExist(int id)
        {
            var result = await _specificationRepository.SpecificationExist(id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.SpecificationNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> SpecificationDeletebyId(int id)
        {
            var exist = await this.SpecificationExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var result = await _specificationRepository.SpecificationDeletebyId(id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }





        public async Task<ApiResponse<bool>> SpecificationExistByCategoryId(int categoryId, int id)
        {
            var result = await _specificationRepository.SpecificationExistByCategoryId(categoryId, id);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.SpecificationNotFoundById));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        }

        public async Task<ApiResponse<bool>> SpecificationDeletebyCategoryId(int categoryId, int id)
        {
            var exist = await this.SpecificationExist(id);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist.Status, false, _ms.MessageService(exist.Message));
            }
            var exist2 = await this.SpecificationExistByCategoryId(categoryId, id);
            if (exist2.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<bool>((ResponseStatusEnum)exist2.Status, false, _ms.MessageService(exist2.Message));
            }
            var result = await _specificationRepository.SpecificationDeletebyCategoryId(categoryId, id);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }
        }

        public async Task<ApiResponse<SpecificationGetDto>> SpecificationGetById(int id)
        {
            var data = await _specificationRepository.SpecificationGetById(id);
            if (data == null)
            {
                return new ApiResponse<SpecificationGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGetting));
            }
            return new ApiResponse<SpecificationGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<SpecificationGetDto>> SpecificationEdit(SpecificationEditDto specificationEdit)
        {
            var exist = await this.SpecificationExist(specificationEdit.SpecId);
            if (exist.Status == (int)ResponseStatusEnum.NotFound)
            {
                return new ApiResponse<SpecificationGetDto>((ResponseStatusEnum)exist.Status, null, _ms.MessageService(exist.Message));
            }
            var getSpec = await _specificationRepository.SpecificationGetData(specificationEdit.SpecId);
            if ((getSpec.IsSelectable != specificationEdit.IsSelectable))
            {
                //delete all options
                var deleteAllOptions = await _specificationRepository.DeleteAllOptionsBySpecId(specificationEdit.SpecId);
                if (!deleteAllOptions)
                {
                    return new ApiResponse<SpecificationGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationEditing));
                }
            }

            if (specificationEdit.OptionId.Count > 0)
            {
                var DeleteOptionsByIds = await _specificationRepository.DeleteOptionsByIds(specificationEdit.OptionId);
                if (!DeleteOptionsByIds)
                {
                    return new ApiResponse<SpecificationGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationEditing));
                }
            }
            //
            if (specificationEdit.Gcsid.Count > 0)
            {
                var DeleteCatSpecByIds = await _specificationRepository.DeleteCatSpecByIds(specificationEdit.Gcsid, getSpec.SpecId, getSpec.FkSpecGroupId);
                if (!DeleteCatSpecByIds)
                {
                    return new ApiResponse<SpecificationGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationEditing));
                }
            }

       
            if (specificationEdit.TSpecificationOptions.Count > 0)
            {
                var mapSpecOpt = _mapper.Map<List<TSpecificationOptions>>(specificationEdit.TSpecificationOptions);
                foreach (var item in mapSpecOpt)
                {
                    item.FkSpecId = specificationEdit.SpecId;
                }
                var AddSpecificationOptions = await _specificationRepository.AddSpecificationOptions(mapSpecOpt , specificationEdit.SpecId);
                if (!AddSpecificationOptions)
                {
                    return new ApiResponse<SpecificationGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationEditing));
                }
               // specificationEdit.EditOptions.AddRange(AddSpecificationOptions);
            }


            //
            if (specificationEdit.TCategorySpecification.Count > 0)
            {
                var mapCatSpec = _mapper.Map<List<TCategorySpecification>>(specificationEdit.TCategorySpecification);
                foreach (var item in mapCatSpec)
                {
                    item.FkSpecId = specificationEdit.SpecId;
                    item.Gcsid = 0;
                }
                var AddCategorySpecification = await _specificationRepository.AddCategorySpecification(mapCatSpec, specificationEdit.SpecId, getSpec.FkSpecGroupId);
                if (!AddCategorySpecification)
                {
                    return new ApiResponse<SpecificationGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationEditing));
                }
            }

            var result = await _specificationRepository.SpecificationEdit(specificationEdit);
            if (!result)
            {
                return new ApiResponse<SpecificationGetDto>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationEditing));
            }

            var data = await _specificationRepository.SpecificationGetById(specificationEdit.SpecId);
            return new ApiResponse<SpecificationGetDto>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<SpecificationFormDto>>> GetSpecsByGroupId(int groupId)
        {
            var data = await _specificationRepository.GetSpecsByGroupId(groupId);
            if (data == null)
            {
                return new ApiResponse<List<SpecificationFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGetting));
            }
            return new ApiResponse<List<SpecificationFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<SpecificationCatGroupDto>>> GetSpecs(SpecPagination pagination)
        {
            if(pagination.CategoryId!=0)
            {
                pagination.CatChilds = await _categoryRepository.GetCategoriesChilds(pagination.CategoryId);
            }
            var data = await _specificationRepository.GetSpecs(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<SpecificationCatGroupDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.SpecificationGetting));
            }
            var count = await _specificationRepository.GetSpecsCount(pagination);
            return new ApiResponse<Pagination<SpecificationCatGroupDto>>(ResponseStatusEnum.Success,new Pagination<SpecificationCatGroupDto>(count,data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept)
        {
            var result = await _specificationRepository.ChangeAccept(accept);
            if (result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.NotFound, result, _ms.MessageService(Message.CategoryChangeStatus));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result, _ms.MessageService(Message.Successfull));
            }
        } 
    }
}