using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Survey;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IUserActivityRepository
    {
         Task<bool> LikeGoodsAdd(int customerId, int goodsId);
         // ثبت نظر کاربر
         Task<TGoodsComment> GoodsCommentAdd(GoodsCommentAddDto goodsComment);

         Task<bool> ViewGoodsAdd(int customerId, int goodsId , string ipAddress);
         Task<GoodsCommentGetDto> GetCustomerGoodsComment(long orderItemId, int customerId);
         Task<int> GetCustomerWishListCount(int customerId);
        Task<int> GetCustomerCartCount(int customerId , Guid? cookieId);

        
         Task<CustomerAddressDto> AddCustomerAddress(TCustomerAddress customerAddress,bool forCart,long? orderId);
         Task<CustomerAddressDto> AddProfileCustomerAddress(TCustomerAddress customerAddress);
         Task<CustomerAddressDto> UpdateCustomerAddress(TCustomerAddress customerAddress , bool forOrder);
         Task<List<CustomerAddressDto>> GetCustomerAddress(string type);
         Task<bool> DeleteCustomerAddress(int addressId , int customerId);

         // ajyal credit for customer profile
         Task<UserTransactionWebGetDto> GetProfileAjyalCredit(PaginationFormDto pagination);
         Task<int> GetProfileAjyalCreditCount(PaginationFormDto pagination);

         Task<bool> SetDefualtAddress(int addressId);
         Task<bool> ChangeMobileNumberAddress(int addressId, string mobileNumber);
         Task<bool> SetCustomerMobileVerify(int addressId);
         Task<string> GetPhoneCodeWithCustomerAddress(int addressId);
         Task<string> GetMobileNumberWithCustomerAddress(int addressId);
         Task<bool> CustomerEmailVerify(string email);

         // درخواست call request

         Task<ShopGeneralDto> CallRequestGoodsAdd(int customerId, int goodsId , int providerId);


    }
}