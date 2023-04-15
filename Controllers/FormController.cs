using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.City;
using MarketPlace.API.Data.Dtos.Country;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Discount;
using MarketPlace.API.Data.Dtos.DocumentType;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Data.Dtos.Help;
using MarketPlace.API.Data.Dtos.MeasurementUnit;
using MarketPlace.API.Data.Dtos.Message;
using MarketPlace.API.Data.Dtos.Order;
using MarketPlace.API.Data.Dtos.OrderCancelingReason;
using MarketPlace.API.Data.Dtos.Pagination;
using MarketPlace.API.Data.Dtos.PaymentMethod;
using MarketPlace.API.Data.Dtos.Province;
using MarketPlace.API.Data.Dtos.ReturningAction;
using MarketPlace.API.Data.Dtos.ReturningReason;
using MarketPlace.API.Data.Dtos.ReturningStatus;
using MarketPlace.API.Data.Dtos.Setting;
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.ShopStatus;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        public IFromService _fromService { get; }
        public ISettingService _settingService { get; }
        public IShopSurveyQuestionsService _shopSurveyQuestionsService { get; }
        public IWebHostEnvironment _hostingEnvironment { get; }
        public FormController(IFromService fromService,
        ISettingService settingService,
        IShopSurveyQuestionsService shopSurveyQuestionsService,
        IWebHostEnvironment hostingEnvironment)
        {
            this._fromService = fromService;
            this._settingService = settingService;
            this._shopSurveyQuestionsService = shopSurveyQuestionsService;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ShopList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<ShopFormDto>>))]
        public async Task<IActionResult> GetShopList([FromQuery] FormPagination pagination)
        {
            var result = await _fromService.GetShopList(pagination);
            return new Response<Pagination<ShopFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("CustomerList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CustomerFormDto>>))]
        public async Task<IActionResult> GetCustomerList([FromQuery] FormPagination pagination)
        {
            var result = await _fromService.GetCustomerList(pagination);
            return new Response<Pagination<CustomerFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("GoodsList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<GoodsFormGetDto>>))]
        public async Task<IActionResult> GetGoodsList([FromQuery] FormPagination pagination)
        {
            var result = await _fromService.GetGoodsList(pagination);
            return new Response<Pagination<GoodsFormGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("VarietyList/{goodsId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<VarietyFormGetDto>>))]
        public async Task<IActionResult> GetVarietyList([FromRoute] int goodsId)
        {
            var result = await _fromService.GetVarietyList(goodsId);
            return new Response<List<VarietyFormGetDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("CategoryList")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CategoryFormListDto>>))]
        public async Task<IActionResult> GetCategoryList([FromQuery] FormPagination pagination)
        {
            var result = await _fromService.GetCategoryList(pagination);
            return new Response<Pagination<CategoryFormListDto>>().ResponseSending(result);
        }


        [HttpGet("ParentAcitveCategory")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CategoryFormListDto>>))]
        public async Task<IActionResult> ParentAcitveCategory()
        {
            var result = await _fromService.ParentAcitveCategory();
            return new Response<List<CategoryFormListDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ShopDocumentsType/{groupId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<DocumentTypeFormDto>>))]
        public async Task<IActionResult> GetShopDocumentsType([FromRoute] int groupId)
        {
            var result = await _fromService.GetShopDocumentsType(groupId,false);
            return new Response<List<DocumentTypeFormDto>>().ResponseSending(result);
        }        
        
        [HttpGet("ActiveShopDocumentsType/{groupId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<DocumentTypeFormDto>>))]
        public async Task<IActionResult> GetActiveShopDocumentsType([FromRoute] int groupId)
        {
            var result = await _fromService.GetShopDocumentsType(groupId,true);
            return new Response<List<DocumentTypeFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ShippingMethod")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShippingMethodFormDto>>))]
        public async Task<IActionResult> GetShippingMethod()
        {
            var result = await _fromService.GetShippingMethod(false);
            return new Response<List<ShippingMethodFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ActiveShippingMethod")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShippingMethodFormDto>>))]
        public async Task<IActionResult> GetActiveShippingMethod()
        {
            var result = await _fromService.GetShippingMethod(true);
            return new Response<List<ShippingMethodFormDto>>().ResponseSending(result);
        }

        [HttpGet("ShopPlan")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShopPlanFormDto>>))]
        public async Task<IActionResult> GetShopPlan()
        {
            var result = await _fromService.GetShopPlan(false);
            return new Response<List<ShopPlanFormDto>>().ResponseSending(result);
        }

        [HttpGet("ActiveShopPlan")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShopPlanFormDto>>))]
        public async Task<IActionResult> GetActiveShopPlan()
        {
            var result = await _fromService.GetShopPlan(true);
            return new Response<List<ShopPlanFormDto>>().ResponseSending(result);
        }

        [HttpGet("ShopStatus")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ShopStatusDto>>))]
        public async Task<IActionResult> GetShopStatus()
        {
            var result = await _fromService.GetShopStatus();
            return new Response<List<ShopStatusDto>>().ResponseSending(result);
        }


        [HttpGet("Country")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<CountryFormDto>>))]
        public async Task<IActionResult> GetCountry([FromQuery] FormPagination pagination)
        {
            var result = await _fromService.GetCountry(pagination,false);
            return new Response<Pagination<CountryFormDto>>().ResponseSending(result);
        }

        [HttpGet("ActiveCountry")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CountryFormDto>>))]
        public async Task<IActionResult> GetActiveCountry()
        {
            var result = await _fromService.GetActiveCountry();
            return new Response<List<CountryFormDto>>().ResponseSending(result);
        }


        [HttpGet("City/{provinceId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CityFormDto>>))]
        public async Task<IActionResult> GetCity([FromRoute]int provinceId)
        {
            var result = await _fromService.GetCity(provinceId,false);
            return new Response<List<CityFormDto>>().ResponseSending(result);
        }

        [HttpGet("ActiveCity/{provinceId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CityFormDto>>))]
        public async Task<IActionResult> GetActiveCity([FromRoute]int provinceId)
        {
            var result = await _fromService.GetCity(provinceId,true);
            return new Response<List<CityFormDto>>().ResponseSending(result);
        }        

        [HttpGet("Province/{countryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ProvinceFormDto>>))]
        public async Task<IActionResult> GetProvince([FromRoute]int countryId)
        {
            var result = await _fromService.GetProvince(countryId,false);
            return new Response<List<ProvinceFormDto>>().ResponseSending(result);
        }

        [HttpGet("ActiveProvince/{countryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ProvinceFormDto>>))]
        public async Task<IActionResult> GetActiveProvince([FromRoute]int countryId)
        {
            var result = await _fromService.GetProvince(countryId,true);
            return new Response<List<ProvinceFormDto>>().ResponseSending(result);
        }        
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ReturningStatus")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ReturningStatusFormDto>>))]
        public async Task<IActionResult> GetReturningStatus()
        {
            var result = await _fromService.GetReturningStatus(false);
            return new Response<List<ReturningStatusFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ActiveReturningStatus")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ReturningStatusFormDto>>))]
        public async Task<IActionResult> GetActiveReturningStatus()
        {
            var result = await _fromService.GetReturningStatus(true);
            return new Response<List<ReturningStatusFormDto>>().ResponseSending(result);
        }        
        
        [HttpGet("DocumentGroup")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<DocumentGroupDto>>))]
        public async Task<IActionResult> GetDocumentGroup()
        {
            var result = await _fromService.GetDocumentGroup();
            return new Response<List<DocumentGroupDto>>().ResponseSending(result);
        }        
                
        [HttpGet("AllShopActiveDocument")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<AllDocumentDto>>))]
        public async Task<IActionResult> GetAllShopActiveDocument()
        {
            var result = await _fromService.GetAllShopActiveDocument();
            return new Response<List<AllDocumentDto>>().ResponseSending(result);
        }        
        
        [HttpGet("Person")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<PersonDto>>))]
        public async Task<IActionResult> GetPerson()
        {
            var result = await _fromService.GetPerson();
            return new Response<List<PersonDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("OrderStatus")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<OrderStatusDto>>))]
        public async Task<IActionResult> GetOrderStatus()
        {
            var result = await _fromService.GetOrderStatus(false);
            return new Response<List<OrderStatusDto>>().ResponseSending(result);
        }        

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ActiveOrderStatus")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<OrderStatusDto>>))]
        public async Task<IActionResult> GetActiveOrderStatus()
        {
            var result = await _fromService.GetOrderStatus(true);
            return new Response<List<OrderStatusDto>>().ResponseSending(result);
        }  
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("TransactionType")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<TransactionTypeFormDto>>))]
        public async Task<IActionResult> GetTransactionType()
        {
            var result = await _fromService.GetTransactionType();
            return new Response<List<TransactionTypeFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Brand")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<BrandFormDto>>))]
        public async Task<IActionResult> GetBrand([FromQuery] PaginationDto pagination)
        {
            pagination.Active = false;
            var result = await _fromService.GetBrand(pagination);
            return new Response<Pagination<BrandFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ActiveBrand")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<BrandFormDto>>))]
        public async Task<IActionResult> GetActiveBrand([FromQuery] PaginationDto pagination)
        {
            pagination.Active = true;
            var result = await _fromService.GetBrand(pagination);
            return new Response<Pagination<BrandFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("Guarantee/{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GuaranteeFormDto>>))]
        public async Task<IActionResult> GetGuarantee([FromRoute] int categoryId)
        {
            var result = await _fromService.GetGuarantee(categoryId, false);
            return new Response<List<GuaranteeFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ActiveGuarantee/{categoryId}")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<GuaranteeFormDto>>))]
        public async Task<IActionResult> GetActiveGuarantee([FromRoute] int categoryId)
        {
            var result = await _fromService.GetGuarantee(categoryId, true);
            return new Response<List<GuaranteeFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("TransactionStatus")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<TransactionStatusDto>>))]
        public async Task<IActionResult> GetTransactionStatus()
        {
            var result = await _fromService.GetTransactionStatus();
            return new Response<List<TransactionStatusDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpPost("CategoryTreeView")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<CategoryTreeView>>))]
        public async Task<IActionResult> GetCategoryTreeView([FromBody] List<int> catIds)
        {
            var result = await _fromService.GetCategoryTreeView(catIds);
            return new Response<List<CategoryTreeView>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("MeasurementUnit")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<MeasurementUnitDto>>))]
        public async Task<IActionResult> GetMeasurementUnit()
        {
            var result = await _fromService.GetMeasurementUnit();
            return new Response<List<MeasurementUnitDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("PaymentMethod")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<PaymentMethodFormDto>>))]
        public async Task<IActionResult> GetPaymentMethod()
        {
            var result = await _fromService.GetPaymentMethod(false);
            return new Response<List<PaymentMethodFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller,Customer")]
        [HttpGet("ActivePaymentMethod")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<PaymentMethodFormDto>>))]
        public async Task<IActionResult> GetActivePaymentMethod()
        {
            var result = await _fromService.GetPaymentMethod(true);
            return new Response<List<PaymentMethodFormDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("DiscountCouponCodeType")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<DiscountCouponCodeTypeDto>>))]
        public async Task<IActionResult> GetDiscountCouponCodeType()
        {
            var result = await _fromService.GetDiscountCouponCodeType();
            return new Response<List<DiscountCouponCodeTypeDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("DiscountPlanType")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<DiscountPlanTypeDto>>))]
        public async Task<IActionResult> GetDiscountPlanType()
        {
            var result = await _fromService.GetDiscountPlanType();
            return new Response<List<DiscountPlanTypeDto>>().ResponseSending(result);
        }
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("DiscountRangeType")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<DiscountRangeTypeDto>>))]
        public async Task<IActionResult> GetDiscountRangeType()
        {
            var result = await _fromService.GetDiscountRangeType();
            return new Response<List<DiscountRangeTypeDto>>().ResponseSending(result);
        }        
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("DiscountType")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<DiscountTypeDto>>))]
        public async Task<IActionResult> GetDiscountType()
        {
            var result = await _fromService.GetDiscountType();
            return new Response<List<DiscountTypeDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<SpecialSellPlanDto>>))]
        [HttpGet("SpecialSellPlan")]
        public async Task<IActionResult> GetSpecialSellPlan([FromQuery] FormPagination pagination)
        {
            var result = await _fromService.GetSpecialSellPlan(pagination,false);
            return new Response<List<SpecialSellPlanDto>>().ResponseSending(result);
        }


        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<SpecialSellPlanDto>>))]
        [HttpGet("ActiveSpecialSellPlan")]
        public async Task<IActionResult> GetActiveSpecialSellPlan([FromQuery] FormPagination pagination)
        {
            var result = await _fromService.GetSpecialSellPlan(pagination,true);
            return new Response<List<SpecialSellPlanDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("DiscountCodePlan")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<SpecialSellPlanDto>>))]
        public async Task<IActionResult> GetDiscountCodePlan([FromQuery] FormPagination pagination)
        {
            var result = await _fromService.GetDiscountCodePlan(pagination,false);
            return new Response<List<SpecialSellPlanDto>>().ResponseSending(result);
        }        
        
        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("ActiveDiscountCodePlan")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<SpecialSellPlanDto>>))]
        public async Task<IActionResult> GetActiveDiscountCodePlan([FromQuery] FormPagination pagination)
        {
            var result = await _fromService.GetDiscountCodePlan(pagination,true);
            return new Response<List<SpecialSellPlanDto>>().ResponseSending(result);
        }


        [Authorize(Roles = "Customer,Seller,Admin")]
        [HttpGet("ReturningReason")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ReturningReasonFormDto>>))]
        public async Task<IActionResult> GetReturningReason()
        {
            var result = await _fromService.GetReturningReason();
            return new Response<List<ReturningReasonFormDto>>().ResponseSending(result);
        }        
        
        [Authorize(Roles = "Customer,Seller,Admin")]
        [HttpGet("ReturningAction")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<ReturningActionDto>>))]
        public async Task<IActionResult> GetReturningAction()
        {
            var result = await _fromService.GetReturningAction();
            return new Response<List<ReturningActionDto>>().ResponseSending(result);
        }


        [Authorize(Roles = "Customer,Seller,Admin")]
        [HttpGet("ActiveCancelingReason")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<OrderCancelingReasonDto>>))]
        public async Task<IActionResult> ActiveCancelingReason()
        {
            var result = await _fromService.ActiveCancelingReason();
            return new Response<List<OrderCancelingReasonDto>>().ResponseSending(result);
        }        
        



        [Authorize(Roles = "Seller,Admin")]
        [HttpGet("Users")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<Pagination<MessageUserFilterDto>>))]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationUserDto pagination)
        {
            var result = await _fromService.GetUsers(pagination);
            return new Response<Pagination<MessageUserFilterDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<HelpTopicFromDto>>))]
        [HttpGet("ActiveHelpTopic")]
        public async Task<IActionResult> GetActiveHelpTopicList([FromQuery]int? FirstLevel)
        {
            var result = await _fromService.GetHelpTopic(true,FirstLevel);
            return new Response<List<HelpTopicFromDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<HelpTopicFromDto>>))]
        [HttpGet("HelpTopic")]
        public async Task<IActionResult> GetHelpTopicList([FromQuery]int? FirstLevel)
        {
            var result = await _fromService.GetHelpTopic(false,FirstLevel);
            return new Response<List<HelpTopicFromDto>>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<UserMenuDto>>))]
        [HttpGet("GetFormMenu")]
        public async Task<IActionResult> GetFormMenu()
        {
            var result = await _fromService.GetFormMenu();
            return new Response<List<UserMenuDto>>().ResponseSending(result);

        }

        [Authorize(Roles = "Admin")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<bool>))]
        [HttpGet("DeleteUserAccess")]
        public async Task<IActionResult> DeleteUserAccess([FromQuery]Guid userId)
        {
            var result = await _fromService.DeleteUserAccess(userId);
            return new Response<bool>().ResponseSending(result);
        }

        [Authorize(Roles = "Admin,Seller")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<OrderCallRequestStatusDto>>))]
        [HttpGet("GetCallRequestStatus")]
        public async Task<IActionResult> GetCallRequestStatus()
        {
            var result = await _fromService.GetCallRequestStatus();
            return new Response<List<OrderCallRequestStatusDto>>().ResponseSending(result);
        }


        [Authorize(Roles = "Admin,Seller")]
        [HttpGet("GeneralMinimumInventory")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<int>))]
        public async Task<IActionResult> GeneralMinimumInventory()
        {
            var result = await _settingService.GeneralMinimumInventory();
            return new Response<int>().ResponseSending(result);
        }


        // پیام آخری که به تامین کننده بعد از ثبت نام داده میشود
        [HttpGet("RegistrationFinalMessage")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetSettingRegistrationFinalMessage()
        {
            var result = await _settingService.GetSettingRegistrationFinalMessage();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

       // جستجوی برند در صفحه ی جستجو

        [HttpPost("BrandForWebsiteWithFillter")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<List<WebsiteBrandDto>>))]
        public async Task<IActionResult> GetBrandForWebsiteWithFillter([FromBody] PaginationBrandDto pagination)
        {
            var result = await _fromService.GetBrandForWebsiteWithFillter(pagination);
            return new Response<List<WebsiteBrandDto>>().ResponseSending(result);
        }



        // متن محاسبه امتیاز های تامین کننده
        [HttpGet("ShopCalculateComment")]
        [ProducesResponseType((int)ResponseStatusEnum.Success, Type = typeof(ApiResponse<SettingDescriptionDto>))]
        public async Task<IActionResult> GetShopCalculateComment()
        {
            var result = await _shopSurveyQuestionsService.GetShopCalculateComment();
            return new Response<SettingDescriptionDto>().ResponseSending(result);
        }

        [HttpGet("CkEditorUploadImage")]
        public async Task<JsonResult> CkEditorUploadImage()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_hostingEnvironment.ContentRootPath))
                {
                    _hostingEnvironment.ContentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "");
                }
                var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Images/vehicle-key");
                var filePath = Path.Combine(uploads, "rich-text");
                var urls = new List<string>();

                //If folder of new key is not exist, create the folder.
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                foreach (var contentFile in Request.Form.Files)
                {
                    if (contentFile != null && contentFile.Length > 0)
                    {
                        await contentFile.CopyToAsync(new FileStream($"{filePath}\\{contentFile.FileName}", FileMode.Create));
                        urls.Add($"{HttpContext.Request.Host}/rich-text/{contentFile.FileName}");
                    }
                }

                return new JsonResult(urls);
            }
            catch (Exception e)
            {
                return new JsonResult(new { error = new { message = e.Message } });
            }
        }


        [HttpPost("uploadEditorImage")]
        public string UploadAsync()
        {
            var ss = HttpContext.Request;
            var file = ss.Form.Files[0];

            if (string.IsNullOrWhiteSpace(_hostingEnvironment.ContentRootPath))
            {
                _hostingEnvironment.ContentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "");
            }

            // string baseUrl = HttpContext.Request;
            var postedFile = file;
            // var path = HttpContext.Current.Server.MapPath("~/ckEditorImages/" + postedFile.FileName);
            var pathDir = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", "CkEditorImages");
            // postedFile.SaveAs(path);
            // path = baseUrl + "/ckEditorImages/" + postedFile.FileName;

            // var filePath = Path.Combine(uploads, uniqueFileName);
            if (!Directory.Exists(pathDir))
            {
                Directory.CreateDirectory(pathDir);
            }

            var path = Path.Combine(pathDir, postedFile.FileName);

            var fileStream = new FileStream(path, FileMode.Create);
            postedFile.CopyTo(fileStream);
            fileStream.Close();

            var filePath = "http://" + HttpContext.Request.Host.Value + "/Uploads/CkEditorImages/" + postedFile.FileName;
            return filePath;
        }

    }
}