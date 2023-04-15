using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderListGetDto>> GetOrderList(OrderListPaginationDto pagination);
        Task<ApiResponse<ShopOrderListGetDto>> GetShopOrderList(OrderListPaginationDto pagination);
        Task<ApiResponse<OrderDetailGetDto>> GetOrderDetail(long orderId);
        Task<ApiResponse<List<OrderLogDto>>> GetOrderLog(long orderId);
        Task<ApiResponse<Pagination<LiveCartListDto>>> GetLiveCartList(LiveCartListPaginationDto pagination);
        Task<ApiResponse<LiveCartDetailGetDto>> GetLiveCartDetail(long orderId);
        Task<ApiResponse<bool>> DeleteLiveCart(long orderId);
        Task<ApiResponse<Pagination<ShippmentListDto>>> GetShippmentList(ShippmentPaginationDto pagination);
        Task<ApiResponse<List<ShippmentDetailDto>>> GetShippmentDetail(int shopId , int customerId);
        Task<ApiResponse<int>> ChangeStauts(long orderItemId,int statusId);
        Task<ApiResponse<Pagination<SalesListDto>>> GeSalesList(SalesListPaginationDto pagination);
        Task<ApiResponse<SalesDetailDto>> GetSalesDetail(long itemId);
        Task<ApiResponse<Pagination<OrderCallRequestDto>>> GetOrderCallRequest(OrderCallRequestPaginationDto pagination);
        Task<ApiResponse<bool>> ChangeCallRequestStatus(long callRequestId , int status);

    }
}