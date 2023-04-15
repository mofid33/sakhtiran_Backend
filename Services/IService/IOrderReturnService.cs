using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.OrderReturning;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IOrderReturnService
    {
        Task<ApiResponse<Pagination<OrderReturningListDto>>> GetOrderReturningList(OrderReturningPaginationDto pagination);
        Task<ApiResponse<List<OrderReturningLogDto>>> GetOrderReturningLog(int returnId);
        Task<ApiResponse<OrderReturningDetailDto>> GetOrderReturningDetail(int returnId);
        Task<ApiResponse<bool>> EditOrderReturning(OrderReturningChangeDto orderReturning);
        Task<ApiResponse<bool>> BlockAmountOrderReturning(AmountDto amount);
        Task<ApiResponse<bool>> RefoundAmountOrderReturning(AmountDto amount);
        Task<ApiResponse<AmountDto>> GetOrderReturningAmount(int returnId);
        Task<ApiResponse<bool>> SendEmailAndSMS(int returnId , string msg);
    }
}