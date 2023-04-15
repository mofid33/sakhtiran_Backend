using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class OrderDetailListDto
    {
        public long ItemId { get; set; }
        public int FkShopId { get; set; }
        public string ShopTitle { get; set; }
        public int FkGoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string SerialNumber { get; set; }
        public string ImageUrl { get; set; }
        public string GoodsCode { get; set; }
        public List<GoodsVarietyGetDto> Varity { get; set; }
        public int FkStatusId { get; set; }
        public string StatusTitle { get; set; }
        public int? FkShippingMethodId { get; set; }
        public string ShippingMethodTitle { get; set; }
        public decimal? UnitPrice { get; set; }
        public double? ItemCount { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Vatamount { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? ComissionPrice { get; set; }
        public string ShippmentDate { get; set; }
        public string DeliveredDate { get; set; }
        public bool HaveGuarantee { get; set; }
        public int? GuaranteeMonthDuration { get; set; }
        public bool? ReturningAllowed { get; set; }

    }
}