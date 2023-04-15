using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.ChangePriority;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface ICategoryService
    {
        Task<ApiResponse<CategoryAddGetDto>> CategoryAdd(CategorySerializeDto categoryDto);
        Task<ApiResponse<Pagination<CategoryGetDto>>> GetAllCategoryGrid(CategoryPaginationDto categoryPagination);
        Task<ApiResponse<CategoryAddGetDto>> CategoryGetForEdit(int categoryId);
        Task<ApiResponse<List<CategoryTreeViewDto>>> CategoryGetOne(int categoryId);
        Task<ApiResponse<CategoryEditDto>> CategoryEdit(CategorySerializeDto categoryDto);
        Task<ApiResponse<CategoryGetDto>> GetCategoryById(int categoryId);
        Task<ApiResponse<List<CategoryTreeView>>> CategoryGet();
        Task<ApiResponse<List<CategoryTreeView>>> CategoryGetbyGoodsId(int goodsId);
        Task<ApiResponse<bool>> CategoryExist(int id);
        Task<ApiResponse<bool>> CategoryDelete(int id);
        Task<ApiResponse<bool>> ChangePriority(ChangePriorityDto changePriority);
        Task<ApiResponse<bool>> ChangeAccept(List<AcceptDto> accept);
        Task<ApiResponse<bool>> ChangeReturning(AcceptDto accept);
        Task<ApiResponse<bool>> ChangeAppearInFooter(AcceptDto accept);
        Task<ApiResponse<bool>> ChangeDisplay(AcceptDto accept);
        Task<ApiResponse<List<CategoryTreeViewDto>>> GetCategoryChildsByCatIdAndPath(CategoryPathDto categoryPath);
        Task<ApiResponse<List<CategoryTreeViewDto>>> GetTrueStatusCategoryChildsByCatIdAndPath(CategoryPathDto categoryPath);
        Task<ApiResponse<List<CategoryTreeViewDto>>> GetParentCategoryForWebsite(int categoryId,string categoryPath);
        Task<ApiResponse<List<CategoryTreeView>>> GetCategoryTreeView(List<int> catIds);
        Task<ApiResponse<List<CategorySettingPathDto>>> GetFooter();
        
    }
}