using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.WareHouse;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseController : ControllerBase
    {
        public IWareHouseService _wareHouseService { get; }
        public WareHouseController(IWareHouseService wareHouseService)
        {
            this._wareHouseService = wareHouseService;
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("Operation")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> AddWareHouseOperation([FromBody]WareHouseOprationAddDto opration)
        {
            var result = await _wareHouseService.AddWareHouseOperation(opration);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Operation")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<WareHouseOprationListDto>>))]
        public async Task<IActionResult> GetWareHouseOprationList([FromQuery] WareHouseOprationListPaginationDto pagination)
        {
            var result = await _wareHouseService.GetWareHouseOprationList(pagination);
            return new Response<Pagination<WareHouseOprationListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")] 
        [HttpGet("OperationDetail")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<WareHouseOprationDetailDto>>))]
        public async Task<IActionResult> GetWareHouseOperationDetail([FromQuery] PaginationFormDto pagination)
        {
            var result = await _wareHouseService.GetWareHouseOperationDetail(pagination);
            return new Response<Pagination<WareHouseOprationDetailDto>>().ResponseSending(result);
        }
    }
}