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

namespace MarketPlace.API.Data.Repositories.IRepository
{
    public interface IFormRepository
    {
        Task<List<ShopFormDto>> GetShopList(FormPagination pagination);
        Task<int> GetShopListCount(FormPagination pagination);
        Task<List<CustomerFormDto>> GetCustomerList(FormPagination pagination);
        Task<List<CategoryFormListDto>> ParentAcitveCategory();
        Task<int> GetCustomerCount(FormPagination pagination);

        Task<List<GoodsFormGetDto>> GetGoodsList(FormPagination pagination);
        Task<int> GetGoodsCount(FormPagination pagination);

        Task<List<CategoryFormListDto>> GetCategoryList(FormPagination pagination);
        Task<int> GetCategoryCount(FormPagination pagination);

        Task<List<DocumentTypeFormDto>> GetShopDocumentsType(int groupId, bool active);
        Task<List<ShippingMethodFormDto>> GetShippingMethod(bool active);
        Task<List<ShopPlanFormDto>> GetShopPlan(bool active);
        Task<List<ShopStatusDto>> GetShopStatus();
        Task<List<CountryFormDto>> GetCountry(FormPagination pagination,bool active);
        Task<List<CountryFormDto>> GetActiveCountry();
        Task<int> GetCountryCount(FormPagination pagination,bool active);
        Task<List<CityFormDto>> GetCity(int provinceId, bool active);
        Task<List<ProvinceFormDto>> GetProvince(int countryId, bool active);
        Task<List<ReturningStatusFormDto>> GetReturningStatus(bool active);
        Task<List<DocumentGroupDto>> GetDocumentGroup();
        Task<List<AllDocumentDto>> GetAllShopActiveDocument();
        Task<List<PersonDto>> GetPerson();
        Task<List<OrderStatusDto>> GetOrderStatus(bool active);
        Task<List<TransactionTypeFormDto>> GetTransactionType();
        Task<List<BrandFormDto>> GetBrand(PaginationDto pagination);
        Task<int> GetBrandListCount(PaginationDto pagination);
        Task<List<GuaranteeFormDto>> GetGuarantee(List<int> categoryId, bool active);
        Task<List<TransactionStatusDto>> GetTransactionStatus();
        Task<List<MeasurementUnitDto>> GetMeasurementUnit();
        Task<List<PaymentMethodFormDto>> GetPaymentMethod(bool active);
        Task<List<DiscountCouponCodeTypeDto>> GetDiscountCouponCodeType();
        Task<List<DiscountPlanTypeDto>> GetDiscountPlanType();
        Task<List<DiscountRangeTypeDto>> GetDiscountRangeType();
        Task<List<DiscountTypeDto>> GetDiscountType();
        Task<List<VarietyFormGetDto>> GetVarietyList(int goodsId);
        Task<List<SpecialSellPlanDto>> GetSpecialSellPlan(FormPagination pagination,bool active);
        Task<List<SpecialSellPlanDto>> GetDiscountCodePlan(FormPagination pagination,bool active);

        Task<List<ReturningReasonFormDto>> GetReturningReason();
        Task<List<OrderCancelingReasonDto>> ActiveCancelingReason();
        Task<List<ReturningActionDto>> GetReturningAction();
        Task<List<MessageUserFilterDto>> GetUsers(PaginationUserDto pagination);
        Task<int> GetUsersCount(PaginationUserDto pagination);
        Task<List<HelpTopicFromDto>> GetHelpTopic(bool active, int? FirstLevel);
        Task<List<UserMenuDto>> GetFormMenu();
        Task<RepRes<bool>> DeleteUserAccess(Guid UserId);

        Task<List<OrderCallRequestStatusDto>> GetCallRequestStatus();
    }
}