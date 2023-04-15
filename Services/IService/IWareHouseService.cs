using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.WareHouse;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IWareHouseService
    {
         Task<ApiResponse<bool>> AddWareHouseOperation(WareHouseOprationAddDto opration);
         Task<ApiResponse<Pagination<WareHouseOprationListDto>>> GetWareHouseOprationList(WareHouseOprationListPaginationDto pagination);
         Task<ApiResponse<Pagination<WareHouseOprationDetailDto>>> GetWareHouseOperationDetail(PaginationFormDto pagination);
    }
}