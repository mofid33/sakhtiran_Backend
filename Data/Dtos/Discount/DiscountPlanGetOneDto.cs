using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountPlanGetOneDto
    {
        public long PlanId { get; set; }
        public string Title { get; set; }
        public int? FkShopId { get; set; }
        public string ShopTitle { get; set; }
        public int FkPlanTypeId { get; set; }
        public string PlanTypeTitle { get; set; }
        public bool? UseLimitationType { get; set; }
        public int? CouponCodeCount { get; set; }
        public int? PermittedUseNumberPerCode { get; set; }
        public int? PermittedUseNumberPerCustomer { get; set; }
        public int? PermittedUseNumberAll { get; set; }
        public bool? ActiveForFirstBuy { get; set; }
        public short? FkCouponCodeTypeId { get; set; }
        public string CouponCodeTypeTitle { get; set; }
        public string CouponCodePrefix { get; set; }
        public int FkDiscountRangeTypeId { get; set; }
        public string DiscountRangeTypeTitle { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public bool TimingType { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool UseWithOtherDiscountPlan { get; set; }
        public int FkDiscountTypeId { get; set; }
        public string DiscountTypeTitle { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool DiscountCustomerDomain { get; set; }
        public bool DiscountVendorDomain { get; set; }
        public bool FreeShippingCost { get; set; }
        public bool FreeProduct { get; set; }
        public string Comment { get; set; }
        public bool Status { get; set; }
        public List<DiscountGoodsGetDto> TDiscountGoods { get; set; }
        public List<DiscountCustomersGetDto> TDiscountCustomers { get; set; }
        public List<DiscountCategoryGetDto> TDiscountCategory { get; set; }
        public List<DiscountFreeGoodsGetDto> TDiscountFreeGoods { get; set; }
        public List<DiscountShopsGetDto> TDiscountShops { get; set; }

    }
}