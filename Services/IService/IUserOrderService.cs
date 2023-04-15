using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Helper;
using Microsoft.Extensions.Configuration;

namespace MarketPlace.API.Services.IService
{
    public interface IUserOrderService
    {
        Task<ApiResponse<OrderCountDto>> AddOrder(OrderAddDto orderDto);
        Task<ApiResponse<OrderCountDto>> IncreaseOrderItem(OrderAddDto orderDto);
        Task<ApiResponse<OrderCountDto>> DeleteOrderItem(long orderItemId);
        Task<ApiResponse<WebsiteOrderGetDto>> GetOrderDetail(string code, int? cityId, int? countryId,int? provinceId, bool forPay);
        Task<ApiResponse<WebsiteOrderGetDto>> OrderCityCountryDetail();
        Task<ApiResponse<bool>> ChangeDestination(int addressId);
        Task<ApiResponse<string>> InitOrderPayment(PayOrderDto PayDto);
        Task<ApiResponse<WebsiteOrderGetDto>> PayOrderMellatCheck(string refId, string resCode, string saleOrderId, string saleReferenceId);
        Task<ApiResponse<WebsiteOrderGetDto>> PayOrder(PayOrderDto PayDto);
        Task<ApiResponse<PayOrderDto>> CheckPayOrder(long OrderId);
        Task<ApiResponse<Pagination<ProfileOrderGetDto>>> GetProfileOrderDetail(PaginationFormDto pagination);
        Task<ApiResponse<ProfileOrderGetDto>> GetProfileOrderItem(int orderId);
        Task<ApiResponse<List<ProfileOrderItemGetDto>>> ProfileOrdersItemReturned();
        Task<ApiResponse<List<ProfileOrderItemGetDto>>> ProfileOrdersItemCanceled(long orderId);
        Task<ApiResponse<ProfileOrderItemGetDto>> ProfileProductReturned(int ItemId);

        Task<ApiResponse<List<ProfileOrderItemGetDto>>> CancelOrder(List<OrderCancelingAddDto> orderCanceling);
        Task<ApiResponse<bool>> ReturnOrder(OrderReturningAddDto orderReturning);

        Task<ApiResponse<Pagination<ProfileOrderItemReturnGetDto>>> GetProfileReturnRequested(PaginationFormDto pagination , int type);

        // for mobile
        Task<ApiResponse<CustomerOrderCountDto>> GetCustomerOrderCount();


    }
}