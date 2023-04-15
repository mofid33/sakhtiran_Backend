using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IUserOrderRepository
    {
        Task<List<int>> GetGoodsIdsInOrder(int customerId, Guid cookieId);
        Task<RepRes<OrderAddReturnDto>> AddOrder(OrderAddDto addOrder,int customerId,Guid? cookieId);
        Task<RepRes<bool>> IncreaseOrderItem(OrderAddDto orderDto);
        Task<RepRes<bool>> DeleteOrderItem(long orderItem);
        Task<RepRes<WebsiteOrderGetDto>> GetOrderDetail(string code,bool forPay , int? setCurrency = null,string paymentId = null);
        Task<RepRes<WebsiteOrderGetDto>> OrderCityCountryDetail();
        Task<int> GetUserOrderCount(int customerId, Guid? cookieId);
        Task<bool> ChangeOrderItemsCustomer(Guid? cookieId, int customerId);
        Task<bool> ChangeAreaOrder(int cityId, int countryId, int provinceId);
        Task<RepRes<bool>> ChangeDestination(long orderId,bool forCart, int addressId);
        // ذخیره اطلاعات پرداخت
        Task<bool> InitOrderPayment(PayOrderDto PayDto);
        // آپدیت payment ID 
        Task<bool> UpdatePaymentIdWithTransactionId(string transactionID, string paymentId);
        Task<RepRes<WebsiteOrderGetDto>> PayOrder(PayOrderDto PayDto);
        Task<WebsiteOrderGetDto> GetOrderWithPaymentID (string paymentId);
        Task<TPaymentTransaction> GetPaymentTransaction(string paymentId);
        Task<TPaymentTransaction> GetPaymentTransactionWithOrderId(long orderId);
        // گرفتن کشور و شهر سفارش
        Task<string> GetOrderCountryAndCity();

        // سفارش های کاربر در پنل پروفایل
        Task<List<ProfileOrderGetDto>> GetProfileOrderDetail(PaginationFormDto pagination);
        Task<int> GetProfileOrderDetailCount(PaginationFormDto pagination);
        Task<ProfileOrderGetDto> GetProfileOrderItem(int orderId , string trackingCode = null);
        Task<List<ProfileOrderItemGetDto>> ProfileOrdersItemReturned();
        Task<List<ProfileOrderItemGetDto>> ProfileOrdersItemCanceled(long orderId);
        Task<ProfileOrderItemGetDto> ProfileProductReturned(int ItemId);
        Task<List<ProfileOrderItemReturnGetDto>> GetProfileReturnRequested(PaginationFormDto pagination , int type);
        Task<int> GetProfileReturnRequestedCount(PaginationFormDto pagination, int type);
        Task<RepRes<List<ProfileOrderItemGetDto>>> CancelOrder(List<TOrderCanceling> orderCanceling);
        Task<RepRes<bool>> ReturnOrder(TOrderReturning orderReturning);

        // for mobile

        Task<CustomerOrderCountDto> GetCustomerOrderCount();


        // send message for customer for order cart
        Task<bool> SendMessageForCustomerForUnUseCartOrder();


    }
}