using MarketPlace.API.Data.Enums;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Services.Service;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Helper
{
    public class Response<T> : ControllerBase 
    {
        public ActionResult ResponseSending(ApiResponse<T> response)
        {
            switch ((ResponseStatusEnum)response.Status)
            {
                case ResponseStatusEnum.Success:
                    return Ok(response);
                case ResponseStatusEnum.BadRequest:
                    return BadRequest(response);
                case ResponseStatusEnum.NotFound:
                    return NotFound(response);
                case ResponseStatusEnum.Forbidden:
                    return Forbid();
                default:
                    return BadRequest(response);
            }
        }
    }
}