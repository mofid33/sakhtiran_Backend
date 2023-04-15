using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface ISpecificationService
    {
        Task<ApiResponse<SpecificationAddGetDto>> SpecificationAdd(SpecificationAddGetDto specification);
        Task<ApiResponse<bool>> ChangeKeyAndRequired(SpecificationKeyAndRequiredDto KeyAndRequired);
        Task<ApiResponse<bool>> EditPriorityGroup(ChangePriorityDto changePriority);
        Task<ApiResponse<bool>> EditPrioritySpec(ChangePriorityDto changePriority);
        Task<ApiResponse<bool>> SpecificationExist(int id);
        Task<ApiResponse<bool>> SpecificationDeletebyId(int id);
        Task<ApiResponse<bool>> SpecificationExistByCategoryId(int categoryId, int id);
        Task<ApiResponse<bool>> SpecificationDeletebyCategoryId(int categoryId, int id);
        Task<ApiResponse<List<CategorySpecificationGetDto>>> SpecificationGetByCategoryId(int categoryId);
        Task<ApiResponse<SpecificationGetDto>> SpecificationGetById(int id);
        Task<ApiResponse<SpecificationGetDto>> SpecificationEdit(SpecificationEditDto specificationEdit);
        Task<ApiResponse<List<SpecificationFormDto>>> GetSpecsByGroupId(int groupId);
        Task<ApiResponse<Pagination<SpecificationCatGroupDto>>> GetSpecs(SpecPagination pagination);
        Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept);
    }
}