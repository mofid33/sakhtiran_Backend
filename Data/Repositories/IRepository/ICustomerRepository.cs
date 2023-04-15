using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.CustomerBankCards;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface ICustomerRepository
    {
        Task<List<CustomerListDto>> GetCustomerList(CustomerListPaginationDto pagination);
        Task<int> GetCustomerListCount(CustomerListPaginationDto pagination);
        Task<CustomerGeneralDetailDto> GetCustomerGeneralDetail(int customerId);
        Task<List<CustomerAddressDto>> GetCustomerAddress(int customerId);
        Task<List<CustomerWishListViewDateDto>> GetCustomerWishList(CustomerPaginationDto pagination);
        Task<int> GetCustomerWishListCount(CustomerPaginationDto pagination);
        Task<List<CustomerWishListViewDateDto>> GetCustomerViewList(CustomerPaginationDto pagination);
        Task<int> GetCustomerViewListCount(CustomerPaginationDto pagination);
        Task<List<CustomerCommentDto>> GetCustomerCommentList(CustomerPaginationDto pagination);
        Task<int> GetCustomerCommentListCount(CustomerPaginationDto pagination);
        Task<decimal> GetAvailableBalance(int customerId);
        Task<List<CustomerBalanceDto>> GetCustomerBalance(CustomerPaginationDto pagination);
        Task<int> GetCustomerBalanceCount(CustomerPaginationDto pagination);

        Task<List<CustomerCommentDto>> GetAllCustomerCommentList(CustomerCommentPaginationDto pagination);
        Task<int> GetAllCustomerCommentListCount(CustomerCommentPaginationDto pagination);
        Task<string> GetCustomerUserName(int customerId);

        Task<TCustomer> RegisterCustomer(TCustomer customer);
        Task<TCustomer> UpdateCustomer(TCustomer customer);
        Task<bool> DeleteCustomer(int customerId);

        Task<TCustomer> ExistCustomer(int customerId);

        Task<RepRes<TCustomer>> CustomerDelete(int customerId);

        Task<int> CustomerRefundPreference(int customerId);
        Task<bool> SetCustomerRefundPreference(int customerId, int refundPreference);

        Task<bool> SaveCustomerBankCard(TCustomerBankCard customerBankCard);

        Task<List<CustomerBankCardGetDto>> GetCustomerBankCards(int customerId);
        Task<bool> RemoveCustomerBankCard(int bankCardId);
        Task<bool> VerifyCustomerMobileNumber(int customerId);

    }
}