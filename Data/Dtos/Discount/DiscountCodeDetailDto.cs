using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountCodeDetailDto
    {
        public bool Status { get; set; }
        public long PlanId { get; set; }
        public int CodeId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public string DiscountCode { get; set; }
        public int MaxUse { get; set; }
        public int UseCount { get; set; }
        public bool? IsValid { get; set; }
        public List<DiscountCodeOrderDto> Orders { get; set; }
        public string Title { get; set; }
        public string DiscountTypeTitle { get; set; }
        public int FkDiscountTypeId { get; set; }
        public bool FreeShippingCost { get; set; }
        public int FkCouponTypeId { get; set; }
        public string CouponTypeTitle { get; set; }
        public bool? UseLimitationType { get; set; }
        public int? CouponCodeCount { get; set; }
        public int? PermittedUseNumberPerCode { get; set; }
        public int? PermittedUseNumberPerCustomer { get; set; }
        public int? PermittedUseNumberAll { get; set; }
        public short? CouponCodeType { get; set; }
        public string CouponCodeTypeTitle { get; set; }
        public string CouponCodePrefix { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public bool TimingType { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}