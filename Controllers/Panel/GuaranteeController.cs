using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Enums;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuaranteeController : ControllerBase
    {
        public IGuaranteeService _guaranteeService { get; }

        public GuaranteeController(IGuaranteeService guaranteeService)
        {
            this._guaranteeService = guaranteeService;
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GuaranteeDto>))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] GuaranteeDto GuaranteeDto)
        {
            var result = await _guaranteeService.GuaranteeAdd(GuaranteeDto);
            return new Response<GuaranteeDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GuaranteeDto>))]
        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] GuaranteeDto GuaranteeEditDto)
        {
            var result = await _guaranteeService.GuaranteeEdit(GuaranteeEditDto);
            return new Response<GuaranteeDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _guaranteeService.GuaranteeDelete(id);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<GuaranteeDto>>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination)
        {
            var result = await _guaranteeService.GuaranteeGetAll(pagination);
            return new Response<Pagination<GuaranteeDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("ChangeAccept")]
        public async Task<IActionResult> ChangeAccept([FromBody] AcceptNullDto accept)
        {
            var result = await _guaranteeService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<GuaranteeGetOneDto>))]
        [HttpGet("{guaranteeId}")]
        public async Task<IActionResult> GetGuaranteeById([FromRoute] int guaranteeId)
        {
            var result = await _guaranteeService.GetGuaranteeById(guaranteeId);
            return new Response<GuaranteeGetOneDto>().ResponseSending(result);
        }
    }
}