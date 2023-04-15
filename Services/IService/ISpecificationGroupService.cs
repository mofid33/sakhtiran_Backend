using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface ISpecificationGroupService
    {
        Task<ApiResponse<SpecificationGroupDto>> SpecificationGroupAdd(SpecificationGroupDto specificationGroup);
        Task<ApiResponse<SpecificationGroupDto>> SpecificationGroupEdit(SpecificationGroupDto specificationGroup);
        Task<ApiResponse<bool>> SpecificationGroupDelete(int id);
        Task<ApiResponse<bool>> SpecificationGroupExist(int id);
        Task<ApiResponse<Pagination<SpecificationGroupDto>>> SpecificationGroupGetAll(PaginationDto pagination);
        Task<ApiResponse<Pagination<SpecificationGroupGetForGoodsDto>>> SpecificationGroupWithSpecGetAll(PaginationDto pagination);
        Task<ApiResponse<List<SpecificationGroupDto>>> PanelSpecificationGroupGet();
        Task<ApiResponse<List<SpecificationGroupFromDto>>> GroupGetByCatId(int categoryId);
        Task<ApiResponse<SpecificationGroupDto>> GroupGetById(int id);
    }
}