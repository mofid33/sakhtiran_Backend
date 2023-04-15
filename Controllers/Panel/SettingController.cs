using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.PaymentMethod;
using MarketPlace.API.Data.Dtos.Accept;
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Enums;
//using Controllers.Models;

namespace Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class SettingController : ControllerBase
    {
        public ISettingService _settingService { get; }
        public SettingController(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        [HttpPut("Logos")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingLogoDto>))]
        public async Task<IActionResult> EditSettingLogo([FromForm] SettingSerializeDto settingDto)
        {
            var result = await _settingService.EditSettingLogo(settingDto);
            return new Response<SettingLogoDto>().ResponseSending(result);
        }

        [HttpGet("Logos")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingLogoDto>))]
        public async Task<IActionResult> GetSettingLogo()
        {
            var result = await _settingService.GetSettingLogo();
            return new Response<SettingLogoDto>().ResponseSending(result);
        }


        [HttpPut("Company")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingCompanyDto>))]
        public async Task<IActionResult> EditSettingCompany([FromBody] SettingCompanyDto settingDto)
        {
            var result = await _settingService.EditSettingCompany(settingDto);
            return new Response<SettingCompanyDto>().ResponseSending(result);
        }

        [HttpGet("Company")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingCompanyDto>))]
        public async Task<IActionResult> GetSettingCompany()
        {
            var result = await _settingService.GetSettingCompany();
            return new Response<SettingCompanyDto>().ResponseSending(result);
        }

        [HttpPut("Seo")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingSeoDto>))]
        public async Task<IActionResult> EditSettingSeo([FromBody] SettingSeoDto settingDto)
        {
            var result = await _settingService.EditSettingSeo(settingDto);
            return new Response<SettingSeoDto>().ResponseSending(result);
        }

        [HttpGet("Seo")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingSeoDto>))]
        public async Task<IActionResult> GetSettingSeo()
        {
            var result = await _settingService.GetSettingSeo();
            return new Response<SettingSeoDto>().ResponseSending(result);
        }

        [HttpPut("Social")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingSocialDto>))]
        public async Task<IActionResult> EditSettingSocial([FromBody] SettingSocialDto settingDto)
        {
            var result = await _settingService.EditSettingSocial(settingDto);
            return new Response<SettingSocialDto>().ResponseSending(result);
        }

        [HttpGet("Social")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingSocialDto>))]
        public async Task<IActionResult> GetSettingSocial()
        {
            var result = await _settingService.GetSettingSocial();
            return new Response<SettingSocialDto>().ResponseSending(result);
        }

        [HttpPut("General")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingGeneralDto>))]
        public async Task<IActionResult> EditSettingGeneral([FromBody] SettingGeneralDto settingDto)
        {
            var result = await _settingService.EditSettingGeneral(settingDto);
            return new Response<SettingGeneralDto>().ResponseSending(result);
        }

        [HttpGet("General")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingGeneralDto>))]
        public async Task<IActionResult> GetSettingGeneral()
        {
            var result = await _settingService.GetSettingGeneral();
            return new Response<SettingGeneralDto>().ResponseSending(result);
        }



        [HttpPut("AboutUs")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingAboutUs([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingAboutUs(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("AboutUs")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingAboutUs()
        {
            var result = await _settingService.GetSettingAboutUs();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpPut("ShortDescription")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingShortDescription([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingShortDescription(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("ShortDescription")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingShortDescription()
        {
            var result = await _settingService.GetSettingShortDescription();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpPut("WarrantyPolicy")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingWarrantyPolicy([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingWarrantyPolicy(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("WarrantyPolicy")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingWarrantyPolicy()
        {
            var result = await _settingService.GetSettingWarrantyPolicy();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpPut("TermOfUser")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingTermOfUser([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingTermOfUser(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("TermOfUser")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingTermOfUser()
        {
            var result = await _settingService.GetSettingTermOfUser();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpPut("TermOfSale")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingTermOfSale([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingTermOfSale(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("TermOfSale")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingTermOfSale()
        {
            var result = await _settingService.GetSettingTermOfSale();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpPut("PrivacyPolicy")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingPrivacyPolicy([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingPrivacyPolicy(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("PrivacyPolicy")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingPrivacyPolicy()
        {
            var result = await _settingService.GetSettingPrivacyPolicy();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpPut("CustomerRights")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingCustomerRights([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingCustomerRights(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("CustomerRights")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingCustomerRights()
        {
            var result = await _settingService.GetSettingCustomerRights();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpPut("InactiveShopMessage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingInactiveShopMessage([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingInactiveShopMessage(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("InactiveShopMessage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingInactiveShopMessage()
        {
            var result = await _settingService.GetSettingInactiveShopMessage();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpPut("ShopWelcomeMessage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingShopWelcomeMessage([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingShopWelcomeMessage(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("ShopWelcomeMessage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingShopWelcomeMessage()
        {
            var result = await _settingService.GetSettingShopWelcomeMessage();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpPut("RegistrationFinalMessage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> EditSettingRegistrationFinalMessage([FromBody] SettingDescriptionDto settingDto)
        {
            var result = await _settingService.EditSettingRegistrationFinalMessage(settingDto);
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }



        [HttpPut("Email")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingEmailDto>))]
        public async Task<IActionResult> EditSettingEmail([FromBody] SettingEmailDto settingDto)
        {
            var result = await _settingService.EditSettingEmail(settingDto);
            return new Response<SettingEmailDto>().ResponseSending(result);
        }

        [HttpGet("Email")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingEmailDto>))]
        public async Task<IActionResult> GetSettingEmail()
        {
            var result = await _settingService.GetSettingEmail();
            return new Response<SettingEmailDto>().ResponseSending(result);
        }

        [HttpGet("Plans")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShopPlanDto>>))]
        public async Task<IActionResult> GetShopPlans()
        {
            var result = await _settingService.GetShopPlans();
            return new Response<List<ShopPlanDto>>().ResponseSending(result);
        }

        [HttpGet("Plans/{planId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopPlanDto>))]
        public async Task<IActionResult> GetShopPlansOne([FromRoute]int planId)
        {
            var result = await _settingService.GetShopPlansOne(planId);
            return new Response<ShopPlanDto>().ResponseSending(result);
        }

        [HttpPut("Plans")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShopPlan([FromBody] ShopPlanDto shopPlan)
        {
            var result = await _settingService.EditShopPlan(shopPlan);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("PlansChangeStatus")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<ShopPlanDto>))]
        public async Task<IActionResult> ChangeStatusShopPlan([FromBody] AcceptDto accept)
        {
            var result = await _settingService.ChangeStatusShopPlan(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPost("Plans")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> AddShopPlan([FromBody] ShopPlanDto shopPlan)
        {
            var result = await _settingService.AddShopPlan(shopPlan);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("PaymentMethod")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<PaymentMethodDto>>))]
        public async Task<IActionResult> GetPaymentMethod()
        {
            var result = await _settingService.GetPaymentMethod();
            return new Response<List<PaymentMethodDto>>().ResponseSending(result);
        }

        [HttpPut("PaymentMethod")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditPaymentMethod([FromBody] AcceptDto accept)
        {
            var result = await _settingService.EditPaymentMethod(accept);
            return new Response<bool>().ResponseSending(result);
        }


        [HttpGet("ShipingMethod")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShippingMethodDto>>))]
        public async Task<IActionResult> GetShippingMethod()
        {
            var result = await _settingService.GetShippingMethod();
            return new Response<List<ShippingMethodDto>>().ResponseSending(result);
        }

        [HttpPut("ShipingMethodAccept")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeAcceptShipingMethod([FromBody] AcceptDto accept)
        {
            var result = await _settingService.ChangeAcceptShipingMethod(accept);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("ShipingMethod")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShippingMethod([FromBody] ShippingMethodDto shippingMethod)
        {
            var result = await _settingService.EditShippingMethod(shippingMethod);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("EditShippingMethodDesc")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShippingMethodDesc([FromBody] ShippingMethodDto shippingMethod)
        {
            var result = await _settingService.EditShippingMethodDesc(shippingMethod);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("ShipingMethodCountryCost")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShippingMethodCountryCost([FromBody] List<ShippingOnCountryDto> ShippingOnCountry , [FromQuery] int shippingMethodId)
        {
            var result = await _settingService.EditShippingMethodCountryCost(ShippingOnCountry , shippingMethodId);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpPut("EditShippingMethodCityCost")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> EditShippingMethodCityCost([FromBody] List<ShippingOnCityDto> ShippingOnCity , [FromQuery] int shippingMethodId)
        {
            var result = await _settingService.EditShippingMethodCityCost(ShippingOnCity , shippingMethodId);
            return new Response<bool>().ResponseSending(result);
        }

        [HttpGet("ShipingMethodProvinceCost")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ShipingMethodProvinceCost([FromQuery] int shippingMethodId ,[FromQuery] int countryId)
        {
            var result = await _settingService.GetShippingMethodProvinceCost(shippingMethodId , countryId);
            return new Response<List<ShippingOnCityDto>>().ResponseSending(result);
        }
        [HttpGet("ShipingMethodCityCost")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ShipingMethodCityCost([FromQuery] int shippingMethodId ,[FromQuery] int provinceId)
        {
            var result = await _settingService.GetShippingMethodCityCost(shippingMethodId , provinceId);
            return new Response<List<ShippingOnCityDto>>().ResponseSending(result);
        }

        [HttpPut("ShipingMethodChange")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        public async Task<IActionResult> ChangeShippingMethod([FromBody] ShippingMethodChangeDto shippingMethod)
        {
            var result = await _settingService.ChangeShippingMethod(shippingMethod);
            return new Response<bool>().ResponseSending(result);
        }






    }
}