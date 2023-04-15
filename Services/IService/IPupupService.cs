using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.PupupItem;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Helper;
using System.Collections.Generic;

namespace MarketPlace.API.Services.IService
{
    public interface IPupupService
    {
        Task<ApiResponse<PupupItemDto>> PupupItemAdd(PupupItemSerializeDto pupupItemDto);
        Task<ApiResponse<PupupItemDto>> PupupItemEdit(PupupItemSerializeDto pupupItemDto);
        Task<ApiResponse<bool>> PupupItemDelete(int id);
        Task<ApiResponse<bool>> ChangeAccept(AcceptDto  accept);
        Task<ApiResponse<bool>> PupupItemExist(int id);
        Task<ApiResponse<Pagination<PupupItemDto>>> PupupItemGetAll(PaginationDto pagination);
        Task<ApiResponse<PupupItemDto>> GetPupupItemById(int pupupItemId);
        Task<ApiResponse<PupupItemDto>> GetWebsitePupup();
    }
}