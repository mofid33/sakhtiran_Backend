using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.ReturningStatus;
using MarketPlace.API.Data.Dtos.ShippingMethod;

namespace MarketPlace.API.Data.Dtos.OrderReturning
{
    public class OrderReturningDetailDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }

        public int ShopId { get; set; }
        public string ShopTitle { get; set; }


        public long FkOrderId { get; set; }
        public long FkOrderItemId { get; set; }
        public int GoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string GoodsCode { get; set; }
        public string GoodsSerialNumber { get; set; }
        public string GoodsImage { get; set; }


        public decimal? UnitPrice { get; set; }
        public double? ItemCount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? FinalPrice { get; set; }
        public string ShippmentDate { get; set; }
        public string DeliveredDate { get; set; }
        public string PlacedDateTime { get; set; }
        public bool? BlockSuccess { get; set; }
        public bool? RefundSuccess { get; set; }
        public int ReturningId { get; set; }
        public int FkStatusId { get; set; }
        public string StatusTitle { get; set; }
        public int FkReturningReasonId { get; set; }
        public string ReturningReasonTitle { get; set; }
        public int FkReturningActionId { get; set; }
        public string ReturningActionTitle { get; set; }
        public string RegisterDateTime { get; set; }
        public string RequestComment { get; set; }
        public string ProviderComment { get; set; }
        public int? FkShippingMethodId { get; set; }
        public string ShippingMethodTitle { get; set; }
        public decimal? ShippingAmount { get; set; }
        public List<ShippingMethodFormDto> ShippingMethodList { get; set; }
        public List<ReturningStatusFormDto> ReturningStatusList { get; set; }

    }
}