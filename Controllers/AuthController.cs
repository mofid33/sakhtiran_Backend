using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.Pagination;
using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.FirebaseServices;
using MarketPlace.API.Data.Constants;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IAuthService _authService { get; }
        public TokenParseDto token { get; set; }

        public AuthController(IAuthService authService,
        IHttpContextAccessor httpContextAccessor)
        {
            token = new TokenParseDto(httpContextAccessor);
            _authService = authService;
        }

        [HttpPost("Login")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<TokenDto>))]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
            var result = await _authService.Login(userLogin, (int)UserGroupEnum.Admin);
            return new Response<TokenDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("GetCurrentUserUserName")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> GetCurrentUserUserName()
        {
            var result = await _authService.GetCurrentUserUserName();
            return new Response<string>().ResponseSending(result);
        }



        [Authorize(Roles = "Admin,Customer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<TokenDto>))]
        [HttpPost("ChangeCustomerPassword")]
        public async Task<IActionResult> ChangeCustomerPassword([FromBody] ChangeUserPasswordDto passwordDto)
        {
            var result = await _authService.ChangeUserPass(passwordDto, UserGroupEnum.Customer);
            return new Response<TokenDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<TokenDto>))]
        [HttpPut("ChangeShopUserPass")]
        public async Task<IActionResult> ChangeShopUserPass([FromBody] ChangeUserPasswordDto passwordDto)
        {
            var result = await _authService.ChangeUserPass(passwordDto, UserGroupEnum.Seller);
            return new Response<TokenDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<TokenDto>))]
        [HttpPut("ChangeAdminUserPass")]
        public async Task<IActionResult> ChangeAdminUserPass([FromBody] ChangeUserPasswordDto passwordDto)
        {
            var result = await _authService.ChangeUserPass(passwordDto, UserGroupEnum.Admin);
            return new Response<TokenDto>().ResponseSending(result);
        }

        // لاگین کاربر
        [HttpPost("CustomerLogin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<TokenDto>))]
        public async Task<IActionResult> CustomerLogin([FromBody] UserLoginDto userLogin)
        {
            var result = await _authService.Login(userLogin, (int)UserGroupEnum.Customer);
            return new Response<TokenDto>().ResponseSending(result);
        }


        // ارسال اس ام اس به کاربر
        [HttpPost("SendVerifyMobileNumberCustomer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        public async Task<IActionResult> SendVerifyMobileNumberCustomer([FromBody] UserVerficationDto userVerfication)
        {
            var result = await _authService.SendVerifyMobileNumberCustomer(userVerfication, false);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }
        // ارسال دوباره اس ام اس
        [HttpPost("ReSendVerifyMobileNumber")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        public async Task<IActionResult> ReSendVerifyMobileNumber([FromBody] UserVerficationDto userVerfication)
        {
            var result = await _authService.SendVerifyMobileNumberCustomer(userVerfication, true);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }

        // ثبت نام کاربر
        [HttpPost("CustomerRegister")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<TokenDto>))]
        public async Task<IActionResult> CustomerRegister([FromBody] UserRegisterDto userRegister)
        {
            var result = await _authService.Register(userRegister);
            return new Response<TokenDto>().ResponseSending(result);
        }

        // با گوگل و فیس بوک ثبت نام یا لاگین کاربر
        [HttpPost("CustomerRegisterGoogleFacebook")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<TokenDto>))]
        public async Task<IActionResult> CustomerRegisterGoogleFacebook([FromBody] SocialRegisterDto userRegister)
        {
            var result = await _authService.CustomerRegisterGoogleFacebook(userRegister);
            return new Response<TokenDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetEmployeeUsers")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<UserAccessDto>>))]
        public async Task<IActionResult> GetEmployeeUsers([FromQuery] PaginationFormDto pagination)
        {
            var result = await _authService.GetEmployeeUsers(pagination);
            return new Response<Pagination<UserAccessDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ChangeActiveEmployeUser")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeActiveEmployeUser([FromBody] UserAcceptDto userAccept)
        {
            var result = await _authService.ChangeActiveEmployeUser(userAccept);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetMenu")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<UserMenuDto>>))]
        public async Task<IActionResult> GetMenu()
        {
            var result = await _authService.GetMenu();
            return new Response<List<UserMenuDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AccessMenu")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> AccessMenu([FromQuery] string path)
        {
            var result = await _authService.AccessMenu(path);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddNewEmployeUser")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> AddNewEmployeUser([FromBody] UserAccessDto newUser)
        {
            var result = await _authService.AddNewEmployeUser(newUser);
            return new Response<UserAccessDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateEmployeUser")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UpdateEmployeUser([FromBody] UserAccessDto newUser)
        {
            var result = await _authService.UpdateNewEmployeUser(newUser);
            return new Response<UserAccessDto>().ResponseSending(result);
        }


        // لوگوی صفحه ی لاگین و عکس بک گراند
        [HttpGet("GetWebSiteAuthDetials")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<AuthWebDetailsDto>))]
        public async Task<IActionResult> GetWebSiteAuthDetials()
        {
            var result = await _authService.GetWebSiteAuthDetials();
            return new Response<AuthWebDetailsDto>().ResponseSending(result);
        }

        [HttpGet("SendEmailForgetPassword")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> SendForgetPassword([FromQuery] string email)
        {
            var result = await _authService.SendEmail((int)VerificationTypeEnum.ForgetPassword, email);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("VerifyCodeEmail")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> VerifyMobileNumberAddress([FromQuery] string email, [FromQuery] int code)
        {
            var result = await _authService.CustomerEmailVerify(email, code);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("ChangeCustomerEmailForgetPass")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeCustomerEmailForgetPass([FromBody] ChangeUserPasswordDto passwordDto)
        {
            var result = await _authService.ChangeCustomerPassForForget(passwordDto);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("UpdateUserNotificationKey")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> UpdateUserNotificationKey([FromBody] UpdateUserNotificationKeyDto model)
        {
            try
            {
                var result = await _authService.UpdateUserNotificationKey(model, token.UserId);
                return new Response<bool>().ResponseSending(result);
            }
            catch (System.Exception)
            {
                return new Response<bool>().ResponseSending(new ApiResponse<bool>(ResponseStatusEnum.Success, true, ""));
            }
        }



        // ارسال اس ام اس به تامین برای فراموشی رمز عبور
        [HttpGet("SendCodeToVendorForForgetPassword")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        public async Task<IActionResult> SendCodeToVendorForForgetPassword([FromQuery] string email)
        {
            var result = await _authService.SendCodeToVendorForForgetPassword(email);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }
        //  تایید کد فرستاده شده به تامین کننده برای فراموشی رمز عبور
        [HttpGet("CheckVerifyVendorForForgetPassword")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<TokenDto>))]
        public async Task<IActionResult> CheckVerifyVendorForForgetPassword([FromQuery] string email, [FromQuery] int VerifyCode, [FromQuery] string requestId, [FromQuery] string notificationKey)
        {
            var result = await _authService.CheckVerifyVendorForForgetPassword( VerifyCode ,  requestId, email,notificationKey);
            return new Response<TokenDto>().ResponseSending(result);
        }



    }
}
