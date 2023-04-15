using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.City;
using MarketPlace.API.Data.Dtos.Country;
using MarketPlace.API.Data.Dtos.Customer;
using MarketPlace.API.Data.Dtos.Discount;
using MarketPlace.API.Data.Dtos.DocumentType;
using MarketPlace.API.Data.Dtos.Goods;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Data.Dtos.Header;
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
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.ShopStatus;
using MarketPlace.API.Data.Dtos.Token;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Data.Enums;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Services.Service
{
    public class FromService : IFromService
    {
        public IMapper _mapper { get; }
        public IFormRepository _formRepository { get; }
        public ICategoryService _categoryService { get; }
        public ICategoryRepository _categoryRepository { get; }
        public IBrandRepository _brandRepository { get; }
        public HeaderParseDto header { get; set; }
        public TokenParseDto token { get; set; }
        public IMessageLanguageService _ms { get; set; }

        public FromService(
        IMapper mapper,
        IFormRepository formRepository,
        IHttpContextAccessor httpContextAccessor,
        ICategoryService categoryService,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        IMessageLanguageService ms)
        {
            _categoryService = categoryService;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            header = new HeaderParseDto(httpContextAccessor);
            token = new TokenParseDto(httpContextAccessor);
            this._formRepository = formRepository;
            this._mapper = mapper;
            _ms = ms;
        }

        public async Task<ApiResponse<Pagination<ShopFormDto>>> GetShopList(FormPagination pagination)
        {
            var data = await _formRepository.GetShopList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<ShopFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopGetting));
            }
            var count = await _formRepository.GetShopListCount(pagination);
            return new ApiResponse<Pagination<ShopFormDto>>(ResponseStatusEnum.Success, new Pagination<ShopFormDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<CustomerFormDto>>> GetCustomerList(FormPagination pagination)
        {
            var data = await _formRepository.GetCustomerList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CustomerFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CustomerGetting));
            }
            var count = await _formRepository.GetCustomerCount(pagination);
            return new ApiResponse<Pagination<CustomerFormDto>>(ResponseStatusEnum.Success, new Pagination<CustomerFormDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<GoodsFormGetDto>>> GetGoodsList(FormPagination pagination)
        {
            var data = await _formRepository.GetGoodsList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<GoodsFormGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GoodsGetting));
            }
            var count = await _formRepository.GetGoodsCount(pagination);

            return new ApiResponse<Pagination<GoodsFormGetDto>>(ResponseStatusEnum.Success, new Pagination<GoodsFormGetDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<CategoryFormListDto>>> GetCategoryList(FormPagination pagination)
        {
            var data = await _formRepository.GetCategoryList(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<CategoryFormListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            var count = await _formRepository.GetCategoryCount(pagination);
            return new ApiResponse<Pagination<CategoryFormListDto>>(ResponseStatusEnum.Success, new Pagination<CategoryFormListDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<DocumentTypeFormDto>>> GetShopDocumentsType(int groupId, bool active)
        {
            var data = await _formRepository.GetShopDocumentsType(groupId, active);
            if (data == null)
            {
                return new ApiResponse<List<DocumentTypeFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DocumentTypeGetting));
            }
            return new ApiResponse<List<DocumentTypeFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShippingMethodFormDto>>> GetShippingMethod(bool active)
        {
            var data = await _formRepository.GetShippingMethod(active);
            if (data == null)
            {
                return new ApiResponse<List<ShippingMethodFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShippingMethodGetting));
            }
            return new ApiResponse<List<ShippingMethodFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShopPlanFormDto>>> GetShopPlan(bool active)
        {
            var data = await _formRepository.GetShopPlan(active);
            if (data == null)
            {
                return new ApiResponse<List<ShopPlanFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopPlanGetting));
            }
            return new ApiResponse<List<ShopPlanFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ShopStatusDto>>> GetShopStatus()
        {
            var data = await _formRepository.GetShopStatus();
            if (data == null)
            {
                return new ApiResponse<List<ShopStatusDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopStatusGetting));
            }
            return new ApiResponse<List<ShopStatusDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<CountryFormDto>>> GetCountry(FormPagination pagination, bool active)
        {
            var data = await _formRepository.GetCountry(pagination, active);
            if (data == null)
            {
                return new ApiResponse<Pagination<CountryFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CountryGetting));
            }
            var count = await _formRepository.GetCountryCount(pagination, active);
            return new ApiResponse<Pagination<CountryFormDto>>(ResponseStatusEnum.Success, new Pagination<CountryFormDto>(count, data), _ms.MessageService(Message.Successfull));
        }
        public async Task<ApiResponse<List<CountryFormDto>>> GetActiveCountry()
        {
            var data = await _formRepository.GetActiveCountry();
            if (data == null)
            {
                return new ApiResponse<List<CountryFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CountryGetting));
            }
            return new ApiResponse<List<CountryFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<CityFormDto>>> GetCity(int provinceId, bool active)
        {
            var data = await _formRepository.GetCity(provinceId, active);
            if (data == null)
            {
                return new ApiResponse<List<CityFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CityGetting));
            }
            return new ApiResponse<List<CityFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ProvinceFormDto>>> GetProvince(int countryId, bool active)
        {
            
            var data = await _formRepository.GetProvince(countryId, active);
            if (data == null)
            {
                return new ApiResponse<List<ProvinceFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ProvinceGetting));
            }
            return new ApiResponse<List<ProvinceFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ReturningStatusFormDto>>> GetReturningStatus(bool active)
        {
            var data = await _formRepository.GetReturningStatus(active);
            if (data == null)
            {
                return new ApiResponse<List<ReturningStatusFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ShopStatusGetting));
            }
            return new ApiResponse<List<ReturningStatusFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<DocumentGroupDto>>> GetDocumentGroup()
        {
            var data = await _formRepository.GetDocumentGroup();
            if (data == null)
            {
                return new ApiResponse<List<DocumentGroupDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DocumentGroupGetting));
            }
            return new ApiResponse<List<DocumentGroupDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<AllDocumentDto>>> GetAllShopActiveDocument()
        {
            var data = await _formRepository.GetAllShopActiveDocument();
            if (data == null)
            {
                return new ApiResponse<List<AllDocumentDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DocumentGroupGetting));
            }
            return new ApiResponse<List<AllDocumentDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<PersonDto>>> GetPerson()
        {
            var data = await _formRepository.GetPerson();
            if (data == null)
            {
                return new ApiResponse<List<PersonDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.PersonGetting));
            }
            return new ApiResponse<List<PersonDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<OrderStatusDto>>> GetOrderStatus(bool active)
        {
            
            var data = await _formRepository.GetOrderStatus(active);
            if (data == null)
            {
                return new ApiResponse<List<OrderStatusDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.OrderStatusGetting));
            }
            return new ApiResponse<List<OrderStatusDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<TransactionTypeFormDto>>> GetTransactionType()
        {
            var data = await _formRepository.GetTransactionType();
            if (data == null)
            {
                return new ApiResponse<List<TransactionTypeFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.TransactionTypeGetting));
            }
            return new ApiResponse<List<TransactionTypeFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<BrandFormDto>>> GetBrand(PaginationDto pagination)
        {
            if (pagination.Id != 0)
            {
                pagination.ChildIds = await _categoryRepository.GetParentsAndChildsId(pagination.Id);
            }
            else
            {
                pagination.ChildIds = new List<int>();
            }
            var data = await _formRepository.GetBrand(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<BrandFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.BrandGetting));
            }
            var count = await _formRepository.GetBrandListCount(pagination);
            return new ApiResponse<Pagination<BrandFormDto>>(ResponseStatusEnum.Success, new Pagination<BrandFormDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<GuaranteeFormDto>>> GetGuarantee(int categoryId, bool active)
        {
            var childIds = new List<int>();
            if (categoryId != 0)
            {
                childIds = await _categoryRepository.GetParentsAndChildsId(categoryId);
            }
         
            var data = await _formRepository.GetGuarantee(childIds, active);
            if (data == null)
            {
                return new ApiResponse<List<GuaranteeFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.GuaranteeGetting));
            }
            return new ApiResponse<List<GuaranteeFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<TransactionStatusDto>>> GetTransactionStatus()
        {
            var data = await _formRepository.GetTransactionStatus();
            if (data == null)
            {
                return new ApiResponse<List<TransactionStatusDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.TransactionStatusGetting));
            }
            return new ApiResponse<List<TransactionStatusDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<CategoryTreeView>>> GetCategoryTreeView(List<int> catIds)
        {
            var data = await _categoryService.GetCategoryTreeView(catIds);
            return data;
        }

        public async Task<ApiResponse<List<MeasurementUnitDto>>> GetMeasurementUnit()
        {
            var data = await _formRepository.GetMeasurementUnit();
            if (data == null)
            {
                return new ApiResponse<List<MeasurementUnitDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.MeasurementUnitGetting));
            }
            return new ApiResponse<List<MeasurementUnitDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<PaymentMethodFormDto>>> GetPaymentMethod(bool active)
        {
            var data = await _formRepository.GetPaymentMethod(active);
            if (data == null)
            {
                return new ApiResponse<List<PaymentMethodFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.PaymentMethodGetting));
            }
            return new ApiResponse<List<PaymentMethodFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<DiscountCouponCodeTypeDto>>> GetDiscountCouponCodeType()
        {
            var data = await _formRepository.GetDiscountCouponCodeType();
            if (data == null)
            {
                return new ApiResponse<List<DiscountCouponCodeTypeDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DiscountCouponCodeTypeGetting));
            }
            return new ApiResponse<List<DiscountCouponCodeTypeDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<DiscountPlanTypeDto>>> GetDiscountPlanType()
        {
            var data = await _formRepository.GetDiscountPlanType();
            if (data == null)
            {
                return new ApiResponse<List<DiscountPlanTypeDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DiscountPlanTypeGetting));
            }
            return new ApiResponse<List<DiscountPlanTypeDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<DiscountRangeTypeDto>>> GetDiscountRangeType()
        {
            var data = await _formRepository.GetDiscountRangeType();
            if (data == null)
            {
                return new ApiResponse<List<DiscountRangeTypeDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DiscountRangeTypeGetting));
            }
            return new ApiResponse<List<DiscountRangeTypeDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<DiscountTypeDto>>> GetDiscountType()
        {
            var data = await _formRepository.GetDiscountType();
            if (data == null)
            {
                return new ApiResponse<List<DiscountTypeDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DiscountTypeGetting));
            }
            return new ApiResponse<List<DiscountTypeDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<VarietyFormGetDto>>> GetVarietyList(int goodsId)
        {
            var data = await _formRepository.GetVarietyList(goodsId);
            if (data == null)
            {
                return new ApiResponse<List<VarietyFormGetDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.VarietyListGetting));
            }
            return new ApiResponse<List<VarietyFormGetDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<SpecialSellPlanDto>>> GetSpecialSellPlan(FormPagination pagination, bool active)
        {
            var data = await _formRepository.GetSpecialSellPlan(pagination, active);
            if (data == null)
            {
                return new ApiResponse<List<SpecialSellPlanDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DiscountSpecialSellCouponGetting));
            }
            return new ApiResponse<List<SpecialSellPlanDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<SpecialSellPlanDto>>> GetDiscountCodePlan(FormPagination pagination, bool active)
        {
            var data = await _formRepository.GetDiscountCodePlan(pagination, active);
            if (data == null)
            {
                return new ApiResponse<List<SpecialSellPlanDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.DiscountSpecialSellCouponGetting));
            }
            return new ApiResponse<List<SpecialSellPlanDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<CategoryFormListDto>>> ParentAcitveCategory()
        {
            var data = await _formRepository.ParentAcitveCategory();
            if (data == null)
            {
                return new ApiResponse<List<CategoryFormListDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.CategoryGetting));
            }
            return new ApiResponse<List<CategoryFormListDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ReturningReasonFormDto>>> GetReturningReason()
        {
            var data = await _formRepository.GetReturningReason();
            if (data == null)
            {
                return new ApiResponse<List<ReturningReasonFormDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningReasonGetting));
            }
            return new ApiResponse<List<ReturningReasonFormDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<OrderCancelingReasonDto>>> ActiveCancelingReason()
        {
            var data = await _formRepository.ActiveCancelingReason();
            if (data == null)
            {
                return new ApiResponse<List<OrderCancelingReasonDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningReasonGetting));
            }
            return new ApiResponse<List<OrderCancelingReasonDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<ReturningActionDto>>> GetReturningAction()
        {
            var data = await _formRepository.GetReturningAction();
            if (data == null)
            {
                return new ApiResponse<List<ReturningActionDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.ReturningActionGetting));
            }
            return new ApiResponse<List<ReturningActionDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<Pagination<MessageUserFilterDto>>> GetUsers(PaginationUserDto pagination)
        {
            var data = await _formRepository.GetUsers(pagination);
            if (data == null)
            {
                return new ApiResponse<Pagination<MessageUserFilterDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.UserGetting));
            }
            var count = await _formRepository.GetUsersCount(pagination);
            return new ApiResponse<Pagination<MessageUserFilterDto>>(ResponseStatusEnum.Success, new Pagination<MessageUserFilterDto>(count, data), _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<HelpTopicFromDto>>> GetHelpTopic(bool active, int? FirstLevel)
        {
            var data = await _formRepository.GetHelpTopic(active, FirstLevel);
            if (data == null)
            {
                return new ApiResponse<List<HelpTopicFromDto>>(ResponseStatusEnum.BadRequest, null, _ms.MessageService(Message.HelpTopicGetting));
            }
            return new ApiResponse<List<HelpTopicFromDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<UserMenuDto>>> GetFormMenu()
        {
            var data = await _formRepository.GetFormMenu();
            foreach (var item in data)
            {
                if (item.Child.Count == 0)
                {
                    var menu = new UserMenuDto();
                    menu.MenuId = item.MenuId;
                    menu.Title = item.Title;
                    menu.ParentId = item.ParentId;
                    item.Child.Add(menu);
                }

            }
            return new ApiResponse<List<UserMenuDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<OrderCallRequestStatusDto>>> GetCallRequestStatus()
        {
            var data = await _formRepository.GetCallRequestStatus();

            return new ApiResponse<List<OrderCallRequestStatusDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));
        }

        public async Task<ApiResponse<List<WebsiteBrandDto>>> GetBrandForWebsiteWithFillter(PaginationBrandDto pagination)
        {
            List<int> catIds = new List<int>();
            if (pagination.PageNumber != 0)
            {
                catIds = await _categoryRepository.GetParentsAndChildsId(pagination.Id);
            }
            var data = await _brandRepository.GetBrandForWebsiteWithFillter(pagination, catIds);
            return new ApiResponse<List<WebsiteBrandDto>>(ResponseStatusEnum.Success, data, _ms.MessageService(Message.Successfull));

        }

        public async Task<ApiResponse<bool>> DeleteUserAccess(Guid userId)
        {

            var result = await _formRepository.DeleteUserAccess(userId);
            if (result.Result == false)
            {
                return new ApiResponse<bool>(ResponseStatusEnum.BadRequest, result.Result, _ms.MessageService(result.Message));
            }
            else
            {
                return new ApiResponse<bool>(ResponseStatusEnum.Success, result.Result, _ms.MessageService(result.Message));
            }

        }
    }
}