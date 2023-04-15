using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IOrderRepository
    {
        Task<List<OrderListDto>> GetOrderList(OrderListPaginationDto pagination);
        Task<int> GetOrderListCount(OrderListPaginationDto pagination);
        Task<decimal?> GetOrderListDiscount(OrderListPaginationDto pagination);
        Task<decimal?> GetOrderListComissionPrice(OrderListPaginationDto pagination);
        Task<decimal?> GetOrderListShipingCost(OrderListPaginationDto pagination);
        Task<decimal?> GetOrderListVatAmount(OrderListPaginationDto pagination);
        Task<decimal?> GetOrderListFinalPrice(OrderListPaginationDto pagination);
        Task<double?> GetOrderListItemCount(OrderListPaginationDto pagination);        
        
        Task<List<ShopOrderListDto>> GetShopOrderList(OrderListPaginationDto pagination);
        Task<int> GetShopOrderListCount(OrderListPaginationDto pagination);
        Task<decimal?> GetShopOrderListDiscount(OrderListPaginationDto pagination);
        Task<decimal?> GetShopOrderListShipingCost(OrderListPaginationDto pagination);
        Task<decimal?> GetShopOrderListVatAmount(OrderListPaginationDto pagination);
        Task<decimal?> GetShopOrderListFinalPrice(OrderListPaginationDto pagination);
        Task<double?> GetShopOrderListItemCount(OrderListPaginationDto pagination);

        Task<OrderDetailGetDto> GetOrderDetail(long orderId,decimal rate , int shopId);
        Task<List<OrderLogDto>> GetOrderLog(long orderId);

        Task<List<LiveCartListDto>> GetLiveCartList(LiveCartListPaginationDto pagination);
        Task<int> GetLiveCartListCount(LiveCartListPaginationDto pagination);
        Task<LiveCartDetailGetDto> GetLiveCartDetail(long orderId,decimal rate);
        Task<bool> DeleteLiveCart(long orderId);

        
        Task<ShippmentDto> GetShippmentList(ShippmentPaginationDto pagination);
        // Task<int> GetShippmentListCount(ShippmentPaginationDto pagination);
        Task<List<ShippmentDetailDto>> GetShippmentDetail(int shopId , int customerId);
        Task<RepRes<int>> ChangeStauts(long orderItemId, int statusId);
        Task<List<SalesListDto>> GeSalesList(SalesListPaginationDto pagination);
        Task<int> GeSalesListCount(SalesListPaginationDto pagination);
        Task<SalesDetailDto> GetSalesDetail(long itemId);

        Task<List<OrderCallRequestDto>> GetOrderCallRequest(OrderCallRequestPaginationDto pagination);
        Task<int> GetOrderCallRequestCount(OrderCallRequestPaginationDto pagination);
        Task<bool> ChangeCallRequestStatus(long callRequestId , int status);
    }
}