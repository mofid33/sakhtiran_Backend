using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IAuthService
    {
        Task<ApiResponse<TokenDto>> Login(UserLoginDto userLogin , int type);
        Task<ApiResponse<string>> GetCurrentUserUserName();
        
        Task<ApiResponse<TokenDto>> ChangeUserPass(ChangeUserPasswordDto passwordDto,UserGroupEnum type);
        Task<ApiResponse<CustomerSMSDto>> SendVerifyMobileNumberCustomer(UserVerficationDto userVerfication , bool isResend);

        Task<ApiResponse<TokenDto>> Register(UserRegisterDto userRegister);
        // پروفایل کاربر
        Task<ApiResponse<bool>>  UpdateUser(UserUpdateDto userUpdate);

        Task<ApiResponse<Pagination<UserAccessDto>>> GetEmployeeUsers(PaginationFormDto pagination);
        Task<ApiResponse<bool>> ChangeActiveEmployeUser(UserAcceptDto userAccept);
        Task<ApiResponse<List<UserMenuDto>>> GetMenu();
        Task<ApiResponse<bool>> AccessMenu(string path);

        Task<ApiResponse<UserAccessDto>> AddNewEmployeUser(UserAccessDto newUser);

        Task<ApiResponse<UserAccessDto>> UpdateNewEmployeUser(UserAccessDto userUpdate);

        Task<ApiResponse<AuthWebDetailsDto>> GetWebSiteAuthDetials();


        Task<ApiResponse<TokenDto>> CustomerRegisterGoogleFacebook(SocialRegisterDto userRegister);
        Task<ApiResponse<bool>> SendEmail(int type ,string email);
        Task<ApiResponse<bool>> CustomerEmailVerify(string email, int code);

        Task<ApiResponse<bool>> ChangeCustomerPassForForget(ChangeUserPasswordDto passwordDto);

        Task<ApiResponse<bool>> UpdateUserNotificationKey(UpdateUserNotificationKeyDto model, Guid userId);

        Task<ApiResponse<CustomerSMSDto>> VerifyCustomerMobileNumber();
        Task<ApiResponse<bool>> CheckVerifyCustomerMobileNumber(int VerifyCode , string requestId);

         Task<ApiResponse<CustomerSMSDto>> SendCodeToVendorForForgetPassword(string email);
         Task<ApiResponse<TokenDto>> CheckVerifyVendorForForgetPassword(int VerifyCode , string requestId,string email, string notificationKey);
    }
}