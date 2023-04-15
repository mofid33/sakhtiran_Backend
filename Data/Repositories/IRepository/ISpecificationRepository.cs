using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Specification;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface ISpecificationRepository
    {
        Task<TSpecification> SpecificationAdd(TSpecification specification);
        Task<bool> ChangeKeyAndRequired(SpecificationKeyAndRequiredDto KeyAndRequired);
        Task<bool> EditPriorityGroup(ChangePriorityDto changePriority);
        Task<bool> EditPrioritySpec(ChangePriorityDto changePriority);
        Task<List<CategorySpecificationGetDto>> SpecificationGetByCategoryId(int categoryId);
        Task<bool> SpecificationExist(int id);
        Task<RepRes<TSpecification>> SpecificationDeletebyId(int id);
        Task<bool> SpecificationExistByCategoryId(int categoryId, int id);
        Task<RepRes<TSpecification>> SpecificationDeletebyCategoryId(int categoryId, int id);
        Task<SpecificationGetDto> SpecificationGetById(int id);
        Task<TSpecification> SpecificationGetData(int id);
        Task<bool> SpecificationEdit(SpecificationEditDto SpecificationEdit);
        Task<bool> DeleteAllOptionsBySpecId(int id);
        Task<bool> DeleteOptionsByIds(List<int> optionIds);
        Task<bool> DeleteCatSpecByIds(List<int> CatSpecByIds,int specId , int groupId);

        Task<bool> AddSpecificationOptions(List<TSpecificationOptions> specificationOptions ,  int specId);

        Task<bool> AddCategorySpecification(List<TCategorySpecification> categorySpecification,int specId , int groupId);
        Task<List<SpecificationFormDto>> GetSpecsByGroupId(int groupId);
        Task<List<WebsiteSpecificationDto>> GetSpecsForWebsite(List<int> categoryIds);
        Task<List<SpecificationCatGroupDto>> GetSpecs(SpecPagination pagination);
        Task<int> GetSpecsCount(SpecPagination pagination);
        Task<bool> ChangeAccept(AcceptDto accept);
    }
}