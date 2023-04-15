using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface ICategoryRepository
    {
        Task<RepRes<TCategory>> CategoryAdd(TCategory category);
        Task<List<CategoryGetDto>> GetAllCategoryGrid(CategoryPaginationDto categoryPagination);
        Task<int> GetAllCategoryGridCount(CategoryPaginationDto categoryPagination);
        Task<CategoryAddGetDto> CategoryGetForEdit(int categoryId);
        Task<List<CategoryGetDto>> GetForWebsite();
        Task<List<CategoryTreeViewDto>> CategoryGetOne(int parentId);
        Task<CategoryGetDto> GetCategoryById(int categoryId);
        Task<List<TCategory>> GetCategoryByIds(List<int> categoryIds);
        Task<bool> CategoryExist(int id);
        Task<RepRes<TCategory>> CategoryDelete(int id);
        Task<RepRes<bool>> CategoryEdit(TCategory category);
        Task<bool> ChangeAccept(List<AcceptDto> accept);
        Task<bool> ChangeDisplay(AcceptDto accept);
        Task<bool> ChangeReturning(AcceptDto accept);
        Task<bool> ChangeAppearInFooter(AcceptDto accept);
        Task<List<int>> GetParentCatIds(int childId);
        Task<List<int>> GetCategoriesChilds(int parentId);
        Task<List<int>> GetCategoriesDirectChilds(int parentId);
        Task<List<int>> GetCategoriesChildsTrueStatus(int parentId);
        Task<List<int>> GetParentsAndChildsId(int categoryId);
        Task<List<int>> GetShopCatIds(int shopId);
        Task<CategoryTreeViewFilterDto> GetChildCategoryForWebsite(int categoryId);
        Task<List<CategoryTreeViewDto>> GetParentCategoryForWebsite(List<int> categoryId);
        Task<bool> ChangePriority(ChangePriorityDto changePriority);
        Task<int> GetGoodsCategoryId(int goodsId);
        Task<List<CategoryFormDto>> GetFooterForWebsite(int? MaxFooterItem,int? MaxItemPerFooterColumn);
        Task<List<CategoryWebGetDto>> GetCategoryAndBrandForWebsite();
        Task<List<CategorySettingPathDto>> GetFooter();
        Task<bool> CanAddNewCategoryInFooter();
        Task<CategoryWebDto> GetCategoryAndBrandForCategoryPageWebsite(int categoryId);

        // mobile section
        Task<List<CategoryWebGetDto>> GetCategoryForMobile();

    }
}