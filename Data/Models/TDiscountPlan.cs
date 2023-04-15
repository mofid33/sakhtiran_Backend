using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountPlan
    {
        public TDiscountPlan()
        {
            TDiscountCategory = new HashSet<TDiscountCategory>();
            TDiscountCouponCode = new HashSet<TDiscountCouponCode>();
            TDiscountCustomers = new HashSet<TDiscountCustomers>();
            TDiscountFreeGoods = new HashSet<TDiscountFreeGoods>();
            TDiscountGoods = new HashSet<TDiscountGoods>();
            TDiscountShops = new HashSet<TDiscountShops>();
            TPopupItem = new HashSet<TPopupItem>();
        }

        public long PlanId { get; set; }
        public string Title { get; set; }
        public int? FkShopId { get; set; }
        public int FkPlanTypeId { get; set; }
        public bool? UseLimitationType { get; set; }
        public int? CouponCodeCount { get; set; }
        public int? PermittedUseNumberPerCode { get; set; }
        public int? PermittedUseNumberPerCustomer { get; set; }
        public int? PermittedUseNumberAll { get; set; }
        public bool? ActiveForFirstBuy { get; set; }
        public short? FkCouponCodeTypeId { get; set; }
        public string CouponCodePrefix { get; set; }
        public int FkDiscountRangeTypeId { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public bool? DiscountVendorDomain { get; set; }
        public bool? DiscountCustomerDomain { get; set; }
        public bool TimingType { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool UseWithOtherDiscountPlan { get; set; }
        public int FkDiscountTypeId { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool FreeShippingCost { get; set; }
        public bool FreeProduct { get; set; }
        public string Comment { get; set; }
        public bool Status { get; set; }

        public virtual TDiscountCouponCodeType FkCouponCodeType { get; set; }
        public virtual TDiscountRangeType FkDiscountRangeType { get; set; }
        public virtual TDiscountType FkDiscountType { get; set; }
        public virtual TDiscountPlanType FkPlanType { get; set; }
        public virtual TShop FkShop { get; set; }
        public virtual ICollection<TDiscountCategory> TDiscountCategory { get; set; }
        public virtual ICollection<TDiscountCouponCode> TDiscountCouponCode { get; set; }
        public virtual ICollection<TDiscountCustomers> TDiscountCustomers { get; set; }
        public virtual ICollection<TDiscountFreeGoods> TDiscountFreeGoods { get; set; }
        public virtual ICollection<TDiscountGoods> TDiscountGoods { get; set; }
        public virtual ICollection<TDiscountShops> TDiscountShops { get; set; }
        public virtual ICollection<TPopupItem> TPopupItem { get; set; }
    }
}
