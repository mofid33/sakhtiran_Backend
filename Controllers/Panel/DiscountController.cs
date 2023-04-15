using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Data.Dtos.Discount;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Enums;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        public IDiscountService _discountService { get; }
        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<DiscountPlanAddDto>))]
        [HttpPost]
        public async Task<IActionResult> Add(DiscountPlanAddDto discountDto)
        {
            var result = await _discountService.DiscountPlanAdd(discountDto);
            return new Response<DiscountPlanAddDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<DiscountPlanEditDto>))]
        [HttpPut]
        public async Task<IActionResult> Edit(DiscountPlanEditDto discountDto)
        {
            var result = await _discountService.DiscountPlanEdit(discountDto);
            return new Response<DiscountPlanEditDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<DiscountPlanGetDto>>))]
        [HttpGet] 
        public async Task<IActionResult> Get([FromQuery] DiscountFilterDto filterDto)
        {
            var result = await _discountService.DiscountPlanGet(filterDto);
            return new Response<Pagination<DiscountPlanGetDto>>().ResponseSending(result);
        }


        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<DiscountPlanGetOneDto>))]
        [HttpGet("{planId}")]
        public async Task<IActionResult> GetOne(int planId)
        {
            var result = await _discountService.GetOne(planId);
            return new Response<DiscountPlanGetOneDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<DiscountCodeExelDto>>))]
        [HttpGet("GetCoponCodeForExel/{planId}")]
        public async Task<IActionResult> GetCoponCodeForExel(int planId)
        {
            var result = await _discountService.GetCoponCodeForExel(planId);
            return new Response<List<DiscountCodeExelDto>>().ResponseSending(result);
        }


        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<DiscountCodeDetailDto>>))]
        [HttpGet("DiscountCodeDetail")]
        public async Task<IActionResult> GetDiscountCodeDetail([FromQuery] DiscountCodePaginationDto pagination)
        {
            var result = await _discountService.GetDiscountCodeDetail(pagination);
            return new Response<Pagination<DiscountCodeDetailDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("ChangeAccept")]
        public async Task<IActionResult> ChangeAccept(AcceptDto accept)
        {
            var result = await _discountService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpGet("DeleteDiscount/{planId}")]
        public async Task<IActionResult> DeleteDiscount(int planId)
        {
            var result = await _discountService.DeleteDiscount(planId);
            return new Response<bool>().ResponseSending(result);
        }

    }
}