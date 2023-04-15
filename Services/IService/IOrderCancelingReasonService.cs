using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.OrderCancelingReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IOrderCancelingReasonService
    {
        Task<ApiResponse<OrderCancelingReasonDto>> OrderCancelingReasonAdd(OrderCancelingReasonDto OrderCancelingReason);
        Task<ApiResponse<OrderCancelingReasonDto>> OrderCancelingReasonEdit(OrderCancelingReasonDto OrderCancelingReason);
        Task<ApiResponse<bool>> OrderCancelingReasonDelete(int id);
        Task<ApiResponse<bool>> OrderCancelingReasonExist(int id);
        Task<ApiResponse<Pagination<OrderCancelingReasonDto>>> OrderCancelingReasonGetAll(PaginationDto pagination);
        Task<ApiResponse<List<OrderCancelingReasonDto>>> GetOrderCancelingReason( );
        Task<ApiResponse<bool>> ChangeAccept(AcceptDto accept);
    }
}