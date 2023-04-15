using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface ISpecificationGroupRepository
    {
        Task<TSpecificationGroup> SpecificationGroupAdd(TSpecificationGroup specificationGroup);
        Task<TSpecificationGroup> SpecificationGroupEdit(TSpecificationGroup specificationGroup);
        Task<RepRes<TSpecificationGroup>> SpecificationGroupDelete(int id);
        Task<bool> SpecificationGroupExist(int id);
        Task<List<SpecificationGroupDto>> SpecificationGroupGetAll(PaginationDto pagination);
        Task<List<SpecificationGroupGetForGoodsDto>> SpecificationGroupWithSpecGetAll(PaginationDto pagination);

        Task<List<SpecificationGroupDto>> PanelSpecificationGroupGet();
        Task<int> SpecificationGroupGetAllCount(PaginationDto pagination);
        Task<List<SpecificationGroupGetForGoodsDto>> SpecificationGroupGetWithCategoryId(List<int> categoryId , int goodsId);
        Task<List<SpecificationGroupFromDto>> GroupGetByCatId(int categoryId);
        Task<SpecificationGroupDto> GroupGetById(int id);

    }
}