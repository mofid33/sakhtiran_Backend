using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Survey;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IUserActivityService
    {
        Task<ApiResponse<bool>> LikeGoodsAdd(int goodsId);
        Task<ApiResponse<GoodsCommentAddDto>> GoodsCommentAdd(GoodsCommentAddDto goodsComment);
        Task<ApiResponse<bool>> ViewGoodsAdd(int goodsId , string ipAddress);
        Task<ApiResponse<UserGoodsCommentDto>> GetCustomerGoodsComment(int goodsId , long orderItemId);
        Task<ApiResponse<ShopRegisterDto>> RegisterProvider(ShopRegisterSerializeDto shopDto);
        Task<ApiResponse<CustomerSMSDto>> CheckShopEmail(string email, string mobileNumber , bool CheckMobileNumber, string captchaToken);

        Task<ApiResponse<CustomerAddressDto>> UpdateCustomerAddress(CustomerAddressDto customerAddress, bool forOrder);
        Task<ApiResponse<List<CustomerAddressDto>>> GetCustomerAddress(string type);
        Task<ApiResponse<CustomerSMSDto>> AddCustomerAddress(CustomerAddressDto customerAddress,bool forCart ,long? orderId);
        Task<ApiResponse<CustomerSMSDto>> AddProfileCustomerAddress(CustomerAddressDto customerAddress);
        Task<ApiResponse<bool>> DeleteCustomerAddress(int addressId);
        Task<ApiResponse<CustomerAddressDto>> CheckAddressArea(CustomerMapAddressDto customerAddress);

        Task<ApiResponse<UserTransactionWebGetDto>> GetProfileAjyalCredit(PaginationFormDto pagination);

         Task<ApiResponse<bool>> SetDefualtAddress(int addressId);
         Task<ApiResponse<CustomerSMSDto>> ChangeMobileNumberAddress(int addressId , string mobileNumber);
         Task<ApiResponse<CustomerSMSDto>> CheckVerifyMobileNumber(int addressId , string code , string requestId, bool forRegisterProvider , string mobileNumber);


         // verify kardn email karbar
        Task<ApiResponse<bool>> CustomerEmailVerify(string email , int code);

        // درخواست call request

         Task<ApiResponse<ShopGeneralDto>> CallRequestGoodsAdd(int goodsId , int providerId);


    }
}