
using System.Threading.Tasks;
using MarketPlace.API.Helper;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Services.IService;

using MarketPlace.API.Data.Enums;
using Microsoft.AspNetCore.Authorization;

using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Dtos.UserOrder;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Transaction;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.CustomerBankCards;
//using Controllers.Models;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class ProfileController : ControllerBase
    {
        public ICustomerService _customerService { get; }
        public IAuthService _authService { get; }
        public IUserOrderService _userOrderService { get; }
        public IUserActivityService _userActivityService { get; }
        public ProfileController(
        ICustomerService customerService
        , IAuthService authService
        , IUserActivityService userActivityService,
          IUserOrderService userOrderService)
        {
            this._customerService = customerService;
            this._authService = authService;
            this._userOrderService = userOrderService;
            this._userActivityService = userActivityService;
        }

        // اطلاعات کاربر
        [HttpGet("CustomerDetailsProfile")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerGeneralDetailDto>))]

        public async Task<IActionResult> CustomerDetailsProfile()
        {
            var result = await _customerService.GetCustomerGeneralDetail(0);
            return new Response<CustomerGeneralDetailDto>().ResponseSending(result);
        }

        // ادیت کاربر
        [HttpPut("EditCustomerProfile")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerGeneralDetailDto>))]

        public async Task<IActionResult> EditCustomerProfile([FromBody] UserUpdateDto userUpdate)
        {
            var result = await _authService.UpdateUser(userUpdate);
            return new Response<bool>().ResponseSending(result);
        }

        // سفارش های کاربر
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProfileOrderGetDto>>))]
        [HttpGet("ProfileOrdersList")]
        public async Task<IActionResult> GetProfileOrdersList([FromQuery] PaginationFormDto pagination)
        {
            var result = await _userOrderService.GetProfileOrderDetail(pagination);
            return new Response<Pagination<ProfileOrderGetDto>>().ResponseSending(result);
        }

        // سفارش های کاربر جزئیات
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ProfileOrderGetDto>))]
        [HttpGet("ProfileOrdersItem")]
        public async Task<IActionResult> ProfileOrdersItem([FromQuery] int orderId)
        {
            var result = await _userOrderService.GetProfileOrderItem(orderId);
            return new Response<ProfileOrderGetDto>().ResponseSending(result);
        }

        // سفارش هایی که برای مرجوعی درخواست داده شده
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProfileOrderItemReturnGetDto>>))]
        [HttpGet("ProfileReturnRequested")]
        public async Task<IActionResult> GetProfileReturnRequested([FromQuery] PaginationFormDto pagination)
        {
            var result = await _userOrderService.GetProfileReturnRequested(pagination, 0);
            return new Response<Pagination<ProfileOrderItemReturnGetDto>>().ResponseSending(result);
        }

        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ProfileOrderItemReturnGetDto>>))]
        [HttpGet("ProfileReturnDeliverd")]
        public async Task<IActionResult> ProfileReturnDeliverd([FromQuery] PaginationFormDto pagination)
        {
            var result = await _userOrderService.GetProfileReturnRequested(pagination, 1);
            return new Response<Pagination<ProfileOrderItemReturnGetDto>>().ResponseSending(result);
        }

        // ajyal credit
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<UserTransactionWebGetDto>))]
        [HttpGet("ProfileAjyalCredit")]
        public async Task<IActionResult> GetProfileAjyalCredit([FromQuery] PaginationFormDto pagination)
        {
            var result = await _userActivityService.GetProfileAjyalCredit(pagination);
            return new Response<UserTransactionWebGetDto>().ResponseSending(result);
        }

        // کالا های مجاز مرجوعی
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ProfileOrderItemGetDto>>))]
        [HttpGet("ProfileOrdersItemReturned")]
        public async Task<IActionResult> ProfileOrdersItemReturned()
        {
            var result = await _userOrderService.ProfileOrdersItemReturned();
            return new Response<List<ProfileOrderItemGetDto>>().ResponseSending(result);
        }

        // کالای مجاز مرجوعی
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ProfileOrderItemGetDto>))]
        [HttpGet("ProfileProductReturned/{itemId}")]
        public async Task<IActionResult> ProfileProductReturned([FromRoute] int ItemId)
        {
            var result = await _userOrderService.ProfileProductReturned(ItemId);
            return new Response<ProfileOrderItemGetDto>().ResponseSending(result);
        }

        // کالا های مجاز کنسلی
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ProfileOrderItemGetDto>>))]
        [HttpGet("ProfileOrdersItemCanceled/{orderId}")]
        public async Task<IActionResult> ProfileOrdersItemCanceled([FromRoute]long orderId)
        {
            var result = await _userOrderService.ProfileOrdersItemCanceled(orderId);
            return new Response<List<ProfileOrderItemGetDto>>().ResponseSending(result);
        }


        [HttpGet("Address")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CustomerAddressDto>>))]
        public async Task<IActionResult> GetAllCustomerAddress()
        {
            var result = await _userActivityService.GetCustomerAddress("profile");
            return new Response<List<CustomerAddressDto>>().ResponseSending(result);
        }

        [HttpPost("AddCustomerAddress")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddCustomerAddress([FromBody] CustomerAddressDto customerAddress)
        {
            var result = await _userActivityService.AddProfileCustomerAddress(customerAddress);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }


        // set deafualt address

        [HttpGet("SetDefualtAddress")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> SetDefualtAddress([FromQuery] int addressId)
        {
            var result = await _userActivityService.SetDefualtAddress(addressId);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("ChangeMobileNumberAddress")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        public async Task<IActionResult> ChangeMobileNumberAddress([FromQuery] int addressId, [FromQuery] string mobileNumber)
        {
            var result = await _userActivityService.ChangeMobileNumberAddress(addressId, mobileNumber);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }

        [HttpGet("VerifyMobileNumberAddress")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        public async Task<IActionResult> VerifyMobileNumberAddress([FromQuery] int addressId, [FromQuery] string code, [FromQuery] string requestId)
        {
            var result = await _userActivityService.CheckVerifyMobileNumber(addressId, code, requestId, false, null);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }

        [HttpGet("SendEmailVerify")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> SendEmailVerify([FromQuery] string email)
        {
            var result = await _authService.SendEmail((int)VerificationTypeEnum.Varify, email);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("VerifyCustomerEmail")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> VerifyCustomerEmail([FromQuery] string email, [FromQuery] int code)
        {
            var result = await _userActivityService.CustomerEmailVerify(email, code);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("CustomerRefundPreference")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> CustomerRefundPreference()
        {
            var result = await _customerService.CustomerRefundPreference();
            return new Response<int>().ResponseSending(result);
        }

        [HttpPut("SetCustomerRefundPreference/{PreferenceId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> SetCustomerRefundPreference(int PreferenceId)
        {
            var result = await _customerService.SetCustomerRefundPreference(PreferenceId);
            return new Response<bool>().ResponseSending(result);
        }




        // verify kardan shomare mobile customer dar panele khodesh

       
        [HttpGet("VerifyCustomerMobileNumber")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        public async Task<IActionResult> VerifyCustomerMobileNumber()
        {
            var result = await _authService.VerifyCustomerMobileNumber();
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }
       
        [HttpGet("CheckVerifyCustomerMobileNumber")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> CheckVerifyCustomerMobileNumber([FromQuery] int VerifyCode , [FromQuery] string requestId)
        {
            var result = await _authService.CheckVerifyCustomerMobileNumber(VerifyCode , requestId);
            return new Response<bool>().ResponseSending(result);
        }







        // for mobile

        [HttpGet("GetCustomerProfileHeader")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerOrderCountDto>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerOrderCount()
        {
            var result = await _userOrderService.GetCustomerOrderCount();
            return new Response<CustomerOrderCountDto>().ResponseSending(result);
        }

        [HttpGet("GetCustomerBankCards")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CustomerBankCardGetDto>>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerBankCards()
        {
            var result = await _customerService.GetCustomerBankCards();
            return new Response<List<CustomerBankCardGetDto>>().ResponseSending(result);
        }

        [HttpDelete("RemoveCustomerBankCard/{bankCardId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveCustomerBankCard(int bankCardId)
        {
            var result = await _customerService.RemoveCustomerBankCard(bankCardId);
            return new Response<bool>().ResponseSending(result);
        }

    }
}