using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountPlanEditDto
    {
        public long PlanId { get; set; }
        [Required]
        [MaxLength(250)]
        public string Title { get; set; }
        public bool? UseLimitationType { get; set; }
        public int? FkShopId { get; set; }
        public int? PermittedUseNumberPerCustomer { get; set; }
        public int? PermittedUseNumberPerCode { get; set; }
        public int FkDiscountRangeTypeId { get; set; }
        public double? MinimumOrderAmount { get; set; }
        public double? MaximumDiscountAmount { get; set; }
        public bool TimingType { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int FkDiscountTypeId { get; set; }
        public double DiscountAmount { get; set; }
        public bool DiscountCustomerDomain { get; set; }
        public bool DiscountVendorDomain { get; set; }
        public bool FreeShippingCost { get; set; }
        public bool FreeProduct { get; set; }
        public string Comment { get; set; }
        public bool Status { get; set; }
        public List<DiscountGoodsDto> TDiscountGoods { get; set; }
        public List<DiscountCustomersDto> TDiscountCustomers { get; set; }
        public List<DiscountCategoryDto> TDiscountCategory { get; set; }
        public List<DiscountFreeGoodsDto> TDiscountFreeGoods { get; set; }
        public List<DiscountShopsDto> TDiscountShops { get; set; }

    }
}