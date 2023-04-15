using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IAccountingService
    {
        Task<ApiResponse<AccountingGetDto>> GetAccountingList(AccountingListPaginationDto pagination);
        Task<ApiResponse<Pagination<ShopWithdrawalRequestGetDto>>> GetShopWithdrawalRequestList(ShopWithdrawalRequestPaginationDto pagination);
        Task<ApiResponse<ShopWithdrawalRequestDto>> EditShopWithdrawalRequest(ShopWithdrawalSerializeDto serializeDto);
        Task<ApiResponse<bool>> AddShopWithdrawalRequest(ShopAddWithdrawalRequestDto request);
        Task<ApiResponse<bool>> AddCustomerWithdrawalRequest(CustomerAddWithdrawalRequestDto request);
        Task<ApiResponse<decimal>> GetShopBalance();
    }
}