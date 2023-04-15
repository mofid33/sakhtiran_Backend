using System;
using System.Collections.Generic;
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
using MarketPlace.API.Data.Dtos.ShippingMethod;
using MarketPlace.API.Data.Dtos.Shop;
using MarketPlace.API.Data.Dtos.ShopPlan;
using MarketPlace.API.Data.Dtos.ShopStatus;
using MarketPlace.API.Data.Dtos.Transaction;
using MarketPlace.API.Data.Dtos.User;
using MarketPlace.API.Helper;

namespace MarketPlace.API.Services.IService
{
    public interface IFromService
    {
        Task<ApiResponse<Pagination<ShopFormDto>>> GetShopList(FormPagination pagination);
        Task<ApiResponse<Pagination<CustomerFormDto>>> GetCustomerList(FormPagination pagination);
        Task<ApiResponse<Pagination<GoodsFormGetDto>>> GetGoodsList(FormPagination pagination);
        Task<ApiResponse<Pagination<CategoryFormListDto>>> GetCategoryList(FormPagination pagination);
        Task<ApiResponse<List<CategoryFormListDto>>> ParentAcitveCategory();
        Task<ApiResponse<List<DocumentTypeFormDto>>> GetShopDocumentsType(int groupId,bool active);
        Task<ApiResponse<List<ShippingMethodFormDto>>> GetShippingMethod(bool active);
        Task<ApiResponse<List<ShopPlanFormDto>>> GetShopPlan(bool active);
        Task<ApiResponse<List<ShopStatusDto>>> GetShopStatus();
        Task<ApiResponse<Pagination<CountryFormDto>>> GetCountry(FormPagination pagination,bool active);
        Task<ApiResponse<List<CountryFormDto>>> GetActiveCountry();
        Task<ApiResponse<List<CityFormDto>>> GetCity(int provinceId, bool active);
        Task<ApiResponse<List<ProvinceFormDto>>> GetProvince(int countryId, bool active);
        Task<ApiResponse<List<ReturningStatusFormDto>>> GetReturningStatus(bool active);
        Task<ApiResponse<List<DocumentGroupDto>>> GetDocumentGroup();
        Task<ApiResponse<List<AllDocumentDto>>> GetAllShopActiveDocument();
        Task<ApiResponse<List<PersonDto>>> GetPerson();
        Task<ApiResponse<List<OrderStatusDto>>> GetOrderStatus(bool active);
        Task<ApiResponse<List<TransactionTypeFormDto>>> GetTransactionType();
        Task<ApiResponse<Pagination<BrandFormDto>>> GetBrand(PaginationDto pagination);
        Task<ApiResponse<List<GuaranteeFormDto>>> GetGuarantee(int categoryId, bool active);
        Task<ApiResponse<List<TransactionStatusDto>>> GetTransactionStatus();
        Task<ApiResponse<List<CategoryTreeView>>> GetCategoryTreeView(List<int> catIds);
        Task<ApiResponse<List<MeasurementUnitDto>>> GetMeasurementUnit();
        Task<ApiResponse<List<PaymentMethodFormDto>>> GetPaymentMethod(bool active);
        Task<ApiResponse<List<DiscountCouponCodeTypeDto>>> GetDiscountCouponCodeType();
        Task<ApiResponse<List<DiscountPlanTypeDto>>> GetDiscountPlanType();
        Task<ApiResponse<List<DiscountRangeTypeDto>>> GetDiscountRangeType();
        Task<ApiResponse<List<DiscountTypeDto>>> GetDiscountType();
        Task<ApiResponse<List<VarietyFormGetDto>>> GetVarietyList(int goodsId);
        Task<ApiResponse<List<SpecialSellPlanDto>>> GetSpecialSellPlan(FormPagination pagination,bool active);
        Task<ApiResponse<List<SpecialSellPlanDto>>> GetDiscountCodePlan(FormPagination pagination,bool active);

        Task<ApiResponse<List<ReturningReasonFormDto>>> GetReturningReason();
        Task<ApiResponse<List<OrderCancelingReasonDto>>> ActiveCancelingReason();
        Task<ApiResponse<List<ReturningActionDto>>> GetReturningAction();
        Task<ApiResponse<Pagination<MessageUserFilterDto>>> GetUsers(PaginationUserDto pagination);
        Task<ApiResponse<List<HelpTopicFromDto>>> GetHelpTopic(bool active,int? FirstLevel);
        Task<ApiResponse<List<UserMenuDto>>> GetFormMenu();
        Task<ApiResponse<List<OrderCallRequestStatusDto>>> GetCallRequestStatus();
        Task<ApiResponse<List<WebsiteBrandDto>>> GetBrandForWebsiteWithFillter(PaginationBrandDto pagination);

        Task<ApiResponse<bool>> DeleteUserAccess(Guid userId);


    }
}