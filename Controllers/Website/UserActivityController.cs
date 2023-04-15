using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.Survey;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.Customer;
using System.Collections.Generic;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActivityController : ControllerBase
    {
        public IUserActivityService _userActivityService { get; }
        public UserActivityController(IUserActivityService userActivityService)
        {
            this._userActivityService = userActivityService;
        }

        [Authorize(Roles = "Customer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPost("LikeGoods/{goodsId}")]
        public async Task<IActionResult> LikeGoods([FromRoute] int goodsId)
        {
            var result = await _userActivityService.LikeGoodsAdd(goodsId);
            return new Response<bool>().ResponseSending(result);
        }

        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPost("ViewGoods/{goodsId}")]
        public async Task<IActionResult> ViewGoods([FromRoute] int goodsId)
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var result = await _userActivityService.ViewGoodsAdd(goodsId , ipAddress);
            return new Response<bool>().ResponseSending(result);
        }

        // ثبت نظر کاربر
        [HttpPost("CustomerGoodsComment")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GoodsCommentAddDto>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CustomerGoodsComment([FromBody] GoodsCommentAddDto goodsComment)
        {
            var result = await _userActivityService.GoodsCommentAdd(goodsComment);
            return new Response<GoodsCommentAddDto>().ResponseSending(result);
        }


        [HttpGet("CustomerGoodsComment/{goodsId}/{orderItemId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<UserGoodsCommentDto>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CustomerGoodsComment([FromRoute] int goodsId ,[FromRoute] int orderItemId)
        {
            var result = await _userActivityService.GetCustomerGoodsComment(goodsId , orderItemId);
            return new Response<UserGoodsCommentDto>().ResponseSending(result);
        }

        // ثبت نام تامین کننده
        [HttpPost("RegisterProvider")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopRegisterDto>))]
        public async Task<IActionResult> RegisterProvider([FromForm] ShopRegisterSerializeDto shopDto)
        {
            var result = await _userActivityService.RegisterProvider(shopDto);
            return new Response<ShopRegisterDto>().ResponseSending(result);
        }
        [HttpGet("CheckShopEmail")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        public async Task<IActionResult> CheckShopEmail([FromQuery] string Email , [FromQuery] string MobileNumber, [FromQuery] bool CheckMobileNumber, [FromQuery] string CaptchaToken )
        {
            var result = await _userActivityService.CheckShopEmail(Email , MobileNumber , CheckMobileNumber,CaptchaToken);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }
        
        [HttpGet("VerifyProviderMobileNumber")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        public async Task<IActionResult> VerifyProviderMobileNumber([FromQuery]string MobileNumber ,[FromQuery]string Code , [FromQuery] string RequestId)
        {
            var result = await _userActivityService.CheckVerifyMobileNumber(0 ,Code, RequestId , true , MobileNumber);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }


        [HttpPost("Address/{orderId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddCustomerAddress([FromBody]CustomerAddressDto customerAddress,[FromRoute] long? orderId)
        {
            var result = await _userActivityService.AddCustomerAddress(customerAddress,false, orderId);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }

        [HttpPost("AddressCard")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerSMSDto>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddCustomerAddressToCart([FromBody]CustomerAddressDto customerAddress)
        {
            var result = await _userActivityService.AddCustomerAddress(customerAddress,true ,null);
            return new Response<CustomerSMSDto>().ResponseSending(result);
        }


        [HttpPut("Address")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerAddressDto>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomerAddress([FromBody]CustomerAddressDto customerAddress)
        {
            var result = await _userActivityService.UpdateCustomerAddress(customerAddress , false);
            return new Response<CustomerAddressDto>().ResponseSending(result);
        }
        [HttpPut("AddressOrder")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerAddressDto>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomerAddressOrder([FromBody]CustomerAddressDto customerAddress)
        {
            var result = await _userActivityService.UpdateCustomerAddress(customerAddress , true);
            return new Response<CustomerAddressDto>().ResponseSending(result);
        }

        [HttpDelete("Address/{addressId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteCustomerAddress([FromRoute]int addressId)
        {
            var result = await _userActivityService.DeleteCustomerAddress(addressId);
            return new Response<bool>().ResponseSending(result);
        }

        //only for order
        [HttpGet("Address")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CustomerAddressDto>>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> OrderCustomerAddress()
        {
            var result = await _userActivityService.GetCustomerAddress("order");
            return new Response<List<CustomerAddressDto>>().ResponseSending(result);
        }

        [HttpGet("CheckAddressArea")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<CustomerAddressDto>))]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CheckAddressArea([FromQuery]CustomerMapAddressDto customerAddress)
        {
            var result = await _userActivityService.CheckAddressArea(customerAddress);
            return new Response<CustomerAddressDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Customer")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopGeneralDto>))]
        [HttpPost("CallRequestGoods/{goodsId}/{providerId}")]
        public async Task<IActionResult> CallRequestGoods([FromRoute] int goodsId , [FromRoute] int providerId)
        {
            var result = await _userActivityService.CallRequestGoodsAdd(goodsId , providerId);
            return new Response<ShopGeneralDto>().ResponseSending(result);
        }

        

    }
}