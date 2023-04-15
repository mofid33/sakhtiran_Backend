using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TOrderItem
    {
        public TOrderItem()
        {
            TOrderCanceling = new HashSet<TOrderCanceling>();
            TOrderLog = new HashSet<TOrderLog>();
            TOrderReturning = new HashSet<TOrderReturning>();
            TShopSurveyAnswers = new HashSet<TShopSurveyAnswers>();
            TStockOperation = new HashSet<TStockOperation>();
            TUserTransaction = new HashSet<TUserTransaction>();
        }

        public long ItemId { get; set; }
        public long FkOrderId { get; set; }
        public int FkShopId { get; set; }
        public int FkGoodsId { get; set; }
        public int FkVarietyId { get; set; }
        public int FkStatusId { get; set; }
        public int? FkShippingMethodId { get; set; }
        public int? FkDiscountCodeId { get; set; }
        public decimal? UnitPrice { get; set; }
        public double? ItemCount { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? DiscountCouponAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Vatamount { get; set; }
        public double? FinalPrice { get; set; }
        public decimal? ComissionPrice { get; set; }
        public DateTime? ShippmentDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public bool HaveGuarantee { get; set; }
        public int? GuaranteeMonthDuration { get; set; }
        public bool ReturningAllowed { get; set; }
        public int? MaxDeadlineDayToReturning { get; set; }

        public virtual TDiscountCouponCode FkDiscountCode { get; set; }
        public virtual TGoods FkGoods { get; set; }
        public virtual TOrder FkOrder { get; set; }
        public virtual TShippingMethod FkShippingMethod { get; set; }
        public virtual TShop FkShop { get; set; }
        public virtual TOrderStatus FkStatus { get; set; }
        public virtual TGoodsProvider FkVariety { get; set; }
        public virtual ICollection<TOrderCanceling> TOrderCanceling { get; set; }
        public virtual ICollection<TOrderLog> TOrderLog { get; set; }
        public virtual ICollection<TOrderReturning> TOrderReturning { get; set; }
        public virtual ICollection<TShopSurveyAnswers> TShopSurveyAnswers { get; set; }
        public virtual ICollection<TStockOperation> TStockOperation { get; set; }
        public virtual ICollection<TUserTransaction> TUserTransaction { get; set; }
    }
}
