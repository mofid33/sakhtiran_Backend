using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.WareHouse;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IWareHouseRepository
    {
        Task<RepRes<bool>> AddWareHouseOperation(WareHouseOprationAddDto opration);
        Task<List<WareHouseOprationListDto>> GetWareHouseOprationList(WareHouseOprationListPaginationDto pagination);
        Task<int> GetWareHouseOprationListCount(WareHouseOprationListPaginationDto pagination);
        Task<List<WareHouseOprationDetailDto>> GetWareHouseOperationDetail(PaginationFormDto pagination);
        Task<int> GetWareHouseOperationDetailCount(PaginationFormDto pagination);
        Task<bool> AddStockOpration(int fkOperationTypeId, int fkStockId, long? fkOrderItem,  double operationStockCount, decimal? saleUnitPrice, string operationComment);
    }
}