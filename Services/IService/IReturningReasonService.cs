using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.ReturningReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;
using MarketPlace.API.Data.Dtos.Accept;

namespace MarketPlace.API.Services.IService
{
    public interface IReturningReasonService
    {
        Task<ApiResponse<ReturningReasonDto>> ReturningReasonAdd(ReturningReasonDto ReturningReason);
        Task<ApiResponse<ReturningReasonDto>> ReturningReasonEdit(ReturningReasonDto ReturningReason);
        Task<ApiResponse<bool>> ReturningReasonDelete(int id);
        Task<ApiResponse<bool>> ReturningReasonExist(int id);
        Task<ApiResponse<Pagination<ReturningReasonDto>>> ReturningReasonGetAll(PaginationDto pagination);
        Task<ApiResponse<List<ReturningReasonDto>>> GetReturningReason();
        Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept);
    }
}