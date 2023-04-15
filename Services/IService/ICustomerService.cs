using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.CustomerBankCards;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface ICustomerService
    {
        Task<ApiResponse<Pagination<CustomerListDto>>> GetCustomerList(CustomerListPaginationDto pagination);
        Task<ApiResponse<CustomerGeneralDetailDto>> GetCustomerGeneralDetail(int customerId);
        Task<ApiResponse<List<CustomerAddressDto>>> GetCustomerAddress(int customerId);
        Task<ApiResponse<Pagination<CustomerWishListViewDateDto>>> GetCustomerWishList(CustomerPaginationDto pagination);
        Task<ApiResponse<Pagination<CustomerWishListViewDateDto>>> GetCustomerViewList(CustomerPaginationDto pagination);
        Task<ApiResponse<Pagination<CustomerCommentDto>>> GetCustomerCommentList(CustomerPaginationDto pagination);
        Task<ApiResponse<Pagination<CustomerCommentDto>>> GetAllCustomerCommentList(CustomerCommentPaginationDto pagination);
        Task<ApiResponse<Pagination<LiveCartListDto>>> GetCustomerLiveCartList(CustomerPaginationDto pagination);
        Task<ApiResponse<OrderListGetDto>> GetCustomerOrderList(CustomerPaginationDto pagination);
        Task<ApiResponse<CustomerBalanceGetDto>> GetCustomerBalance(CustomerPaginationDto pagination);
        Task<ApiResponse<string>> GetCustomerUserName(int customerId);
        Task<ApiResponse<CustomerGeneralDetailDto>> RegisterCustomer(CustomerGeneralDetailDto customer);
        Task<ApiResponse<bool>> CustomerDelete(int customerId);

        Task<ApiResponse<int>> CustomerRefundPreference();
        Task<ApiResponse<bool>> SetCustomerRefundPreference(int refundPreference);

        Task<ApiResponse<List<CustomerBankCardGetDto>>> GetCustomerBankCards();
        Task<ApiResponse<bool>> RemoveCustomerBankCard(int bankCartId);
        Task<ApiResponse<bool>> ChangeGoodsCommentIsAccept(int commentId, bool? isAccept);
    }
}