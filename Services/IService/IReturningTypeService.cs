using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.ReturningType;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IReturningTypeService
    {
        Task<ApiResponse<ReturningTypeDto>> ReturningTypeAdd(ReturningTypeDto ReturningType);
        Task<ApiResponse<ReturningTypeDto>> ReturningTypeEdit(ReturningTypeDto ReturningType);
        Task<ApiResponse<bool>> ReturningTypeDelete(int id);
        Task<ApiResponse<bool>> ReturningTypeExist(int id);
        Task<ApiResponse<Pagination<ReturningTypeDto>>> ReturningTypeGetAll(PaginationDto pagination);
        Task<ApiResponse<List<ReturningTypeDto>>> GetReturningType();
    }
}