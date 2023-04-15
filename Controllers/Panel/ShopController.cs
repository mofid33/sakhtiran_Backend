using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.Image;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ShopController : ControllerBase
    {
        public IShopService _shopService { get; }
        public IUserActivityService _userActivityService { get; }


        public ShopController(IShopService shopService,IUserActivityService userActivityService)
        {
            this._shopService = shopService;
            this._userActivityService = userActivityService;
        }

        // ثبت نام تامین کننده
        [Authorize(Roles="Admin")]
        [HttpPost("RegisterProvider")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopRegisterDto>))]
        public async Task<IActionResult> RegisterProvider([FromForm] ShopRegisterSerializeDto shopDto)
        {
            var result = await _userActivityService.RegisterProvider(shopDto);
            return new Response<ShopRegisterDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("List")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ShopListGetDto>>))]
        public async Task<IActionResult> GetShopList([FromQuery] ShopListPaginationDto pagination)
        {
            var result = await _shopService.GetShopList(pagination);
            return new Response<Pagination<ShopListGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("GeneralDetail/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopGeneralDto>))]
        public async Task<IActionResult> GetShopGeneralDetail([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopGeneralDetail(shopId);
            return new Response<ShopGeneralDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetShopStoreName/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopBaseDto>))]
        public async Task<IActionResult> GetShopStoreName([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopStoreName(shopId);
            return new Response<ShopBaseDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Statistics/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopStatisticsDto>))]
        public async Task<IActionResult> GetShopStatistics([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopStatistics(shopId);
            return new Response<ShopStatisticsDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("GeneralDetail")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopGeneralDto>))]
        public async Task<IActionResult> EditShopGeneralDetail([FromBody] ShopGeneralDto shopDto)
        {
            var result = await _shopService.EditShopGeneralDetail(shopDto);
            return new Response<ShopGeneralDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("Description")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShopDescription([FromBody] ShopDescriptionDto shopDto)
        {
            var result = await _shopService.EditShopDescription(shopDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("TermsAndConditions")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShopTermsAndConditions([FromBody] ShopDescriptionDto shopDto)
        {
            var result = await _shopService.EditShopTermsAndConditions(shopDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Description/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopDescriptionDto>))]
        public async Task<IActionResult> GetShopDescription([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopDescription(shopId);
            return new Response<ShopDescriptionDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("TermsAndConditions/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopDescriptionDto>))]
        public async Task<IActionResult> GetShopTermsAndConditions([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopTermsAndConditions(shopId);
            return new Response<ShopDescriptionDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Document/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopFilesGetDto>))]
        public async Task<IActionResult> GetShopDocument([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopDocument(shopId);
            return new Response<List<ShopFilesGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("Document/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShopFilesGetDto>>))]
        public async Task<IActionResult> EditShopDocument([FromRoute]int shopId,[FromForm] ShopSerializeDto shopDto)
        {
            var result = await _shopService.EditShopDocument(shopId,shopDto);
            return new Response<List<ShopFilesGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Balance")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopBalanceGetDto>))]
        public async Task<IActionResult> GetShopBalance([FromQuery] ShopBalancePagination pagination)
        {
            var result = await _shopService.GetShopBalance(pagination);
            return new Response<ShopBalanceGetDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("Profile")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopProfileDto>))]
        public async Task<IActionResult> EditShopProfile([FromForm] ShopProfileSerializeDto shopDto)
        {
            var result = await _shopService.EditShopProfile(shopDto);
            return new Response<ShopProfileDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Profile/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopProfileDto>))]
        public async Task<IActionResult> GetShopProfile([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopProfile(shopId);
            return new Response<ShopProfileDto>().ResponseSending(result);
        }


        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("Setting")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShopSetting([FromBody] ShopSetting shopDto)
        {
            var result = await _shopService.EditShopSetting(shopDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Setting/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopSetting>))]
        public async Task<IActionResult> GetShopSetting([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopSetting(shopId);
            return new Response<ShopSetting>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("BankInformation")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopBankInformationDto>))]
        public async Task<IActionResult> EditShopBankInformation([FromForm] ShopSerializeDto shopDto)
        {
            var result = await _shopService.EditShopBankInformation(shopDto);
            return new Response<ShopBankInformationDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("BankInformation/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopBankInformationDto>))]
        public async Task<IActionResult> GetShopBankInformation([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopBankInformation(shopId);
            return new Response<ShopBankInformationDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("Tax")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopTaxDto>))]
        public async Task<IActionResult> EditShopTax([FromForm] ShopSerializeDto shopDto)
        {
            var result = await _shopService.EditShopTax(shopDto);
            return new Response<ShopTaxDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Tax/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopTaxDto>))]
        public async Task<IActionResult> GetShopTax([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopTax(shopId);
            return new Response<ShopTaxDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("Plan/{shopId}/{planId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShopPlan([FromRoute] int shopId,[FromRoute]int planId)
        {
            var result = await _shopService.EditShopPlan(shopId,planId);
            return new Response<bool>().ResponseSending(result);
        }

        
        [Authorize(Roles = "Admin,Seller")]
        [HttpPost("InitPlanPayment")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> InitPlanPayment([FromBody] ShopPlanPaymentDto shopPlanPaymentDto)
        {
            var result = await _shopService.InitPlanPayment(shopPlanPaymentDto);
            return new Response<string>().ResponseSending(result);
        }
        
        [Authorize(Roles = "Seller")]
        [HttpGet("PayPlanPayment/{paymentId}/{payerId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> PayPlanPayment([FromRoute] string paymentId,[FromRoute]string payerId)
        {
            var result = await _shopService.GetStatusPlanPayment(paymentId , payerId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Plan/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<PlanShopDto>>))]
        public async Task<IActionResult> GetShopPlan([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopPlan(shopId);
            return new Response<List<PlanShopDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Plan/{planId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteShopPlan([FromRoute] int planId)
        {
            var result = await _shopService.ShopPlanDelete(planId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("ActivityCountry")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShopActivityCountry([FromBody] ShopActivityCountryEditDto shopDto)
        {
            var result = await _shopService.EditShopActivityCountry(shopDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ActivityCountry")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ShopActivityCountryGetDto>>))]
        public async Task<IActionResult> GetShopActivityCountry([FromQuery] PaginationFormDto pagination)
        {
            var result = await _shopService.GetShopActivityCountry(pagination);
            return new Response<Pagination<ShopActivityCountryGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("ActivityCity")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShopActivityCity([FromBody] ShopActivityCityEditDto shopDto)
        {
            var result = await _shopService.EditShopActivityCity(shopDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ActivityCity")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ShopActivityCityGetDto>>))]
        public async Task<IActionResult> GetShopActivityCity([FromQuery] PaginationFormDto pagination,[FromQuery] int provinceId)
        {
            var result = await _shopService.GetShopActivityCity(pagination,provinceId);
            return new Response<Pagination<ShopActivityCityGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("ActivityProvince")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShopActivityProvince([FromBody] ShopActivityCityEditDto shopDto)
        {
            var result = await _shopService.EditShopActivityProvince(shopDto);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ActivityProvince")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ShopActivityCityGetDto>>))]
        public async Task<IActionResult> GetShopActivityProvince([FromQuery] PaginationFormDto pagination)
        {
            var result = await _shopService.GetShopActivityProvince(pagination);
            return new Response<Pagination<ShopActivityCityGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("GetAllShopProvince")]
        public async Task<IActionResult> GetAllShopProvince()
        {
            var result = await _shopService.GetAllShopProvince();
            return new Response<List<ProvinceFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPost("Slider")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopSliderDto>))]
        public async Task<IActionResult> AddShopSlider([FromForm] UploadImageDto shopDto)
        {
            var result = await _shopService.AddShopSlider(shopDto);
            return new Response<ShopSliderDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpDelete("Slider/{sliderId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteShopSlider(int sliderId)
        {
            var result = await _shopService.DeleteShopSlider(sliderId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Slider/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShopSliderDto>>))]
        public async Task<IActionResult> GetShopSlider([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopSlider(shopId);
            return new Response<List<ShopSliderDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("SliderStatus")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeShopSliderStatus([FromBody] AcceptDto accept)
        {
            var result = await _shopService.ChangeShopSliderStatus(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("UserName/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> GetShopUserName([FromRoute] int shopId)
        {
            var result = await _shopService.GetShopUserName(shopId);
            return new Response<string>().ResponseSending(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("CheckShopAccess")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopAccessDto>))]
        public async Task<IActionResult> CheckShopAccess()
        {
            var result = await _shopService.CheckShopAccess();
            return new Response<ShopAccessDto>().ResponseSending(result);
        }        
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Category/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopCategoryPlanGetDto>))]
        public async Task<IActionResult> GetShopCategory([FromRoute]int shopId)
        {
            var result = await _shopService.GetShopCategory(shopId);
            return new Response<ShopCategoryPlanGetDto>().ResponseSending(result);
        }        
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpPost("Category/{shopId}/{category}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> AddShopCategory([FromRoute]int shopId,[FromRoute]int category)
        {
            var result = await _shopService.AddShopCategory(shopId,category);
            return new Response<bool>().ResponseSending(result);
        }


        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpPut("ChangeShopStatus")]
        public async Task<IActionResult> ChangeShopStatus([FromBody]AcceptNullDto accept)
        {
            var result = await _shopService.ChangeShopStatus(accept);
            return new Response<bool>().ResponseSending(result);
        }




        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("InactiveShopMessage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditInactiveShopMessage([FromBody] ShopDescriptionDto shopDto)
        {
            var result = await _shopService.EditInactiveShopMessage(shopDto);
            return new Response<bool>().ResponseSending(result);
        }


        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("InactiveShopMessage/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopDescriptionDto>))]
        public async Task<IActionResult> GetInactiveShopMessage([FromRoute] int shopId)
        {
            var result = await _shopService.GetInactiveShopMessage(shopId);
            return new Response<ShopDescriptionDto>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpDelete("DeleteShopCategory/{categoryId}/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteShopCategory([FromRoute] int categoryId,[FromRoute] int shopId)
        {
            var result = await _shopService.DeleteShopCategory(categoryId , shopId);
            return new Response<bool>().ResponseSending(result);
        }



        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteShop/{shopId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> DeleteShop([FromRoute] int shopId)
        {
            var result = await _shopService.ShopDelete(shopId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("ChangeAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAccept([FromBody] List<AcceptNullDto> accept)
        {
            var result = await _shopService.ChangeAccept(accept);
            return new Response<bool>().ResponseSending(result);
        }


    }
}