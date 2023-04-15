using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountingController : ControllerBase
    {
        public IAccountingService _accountingService { get; }
        public AccountingController(IAccountingService accountingService)
        {
            this._accountingService = accountingService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<AccountingGetDto>))]
        public async Task<IActionResult> GetAccountingList([FromQuery] AccountingListPaginationDto pagination)
        {
            var result = await _accountingService.GetAccountingList(pagination);
            return new Response<AccountingGetDto>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ShopWithdrawalRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ShopWithdrawalRequestGetDto>>))]
        public async Task<IActionResult> GetShopWithdrawalRequestList([FromQuery] ShopWithdrawalRequestPaginationDto pagination)
        {
            var result = await _accountingService.GetShopWithdrawalRequestList(pagination);
            return new Response<Pagination<ShopWithdrawalRequestGetDto>>().ResponseSending(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("ShopWithdrawalRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopWithdrawalRequestDto>))]
        public async Task<IActionResult> EditShopWithdrawalRequest([FromForm] ShopWithdrawalSerializeDto serializeDto)
        {
            var result = await _accountingService.EditShopWithdrawalRequest(serializeDto);
            return new Response<ShopWithdrawalRequestDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("Balance")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<decimal>))]
        public async Task<IActionResult> GetShopBalance()
        {
            var result = await _accountingService.GetShopBalance();
            return new Response<decimal>().ResponseSending(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("WithdrawalRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> AddShopWithdrawalRequest([FromBody] ShopAddWithdrawalRequestDto request)
        {
            var result = await _accountingService.AddShopWithdrawalRequest(request);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CustomerWithdrawalRequest")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> AddCustomerWithdrawalRequest([FromBody] CustomerAddWithdrawalRequestDto request)
        {
            var result = await _accountingService.AddCustomerWithdrawalRequest(request);
            return new Response<bool>().ResponseSending(result);
        }

    }
}