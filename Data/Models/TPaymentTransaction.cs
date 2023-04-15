using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TPaymentTransaction
    {
        public Guid TransactionId { get; set; }
        public int FkPaymentMethodId { get; set; }
        public string PaymentId { get; set; }
        public string PaymentToken { get; set; }
        public string PayerId { get; set; }
        public bool? Status { get; set; }
        public long? FkOrderId { get; set; }
        public int FkCurrencyId { get; set; }
        public int? FkPlanId { get; set; }
        public int? FkShopId { get; set; }
        public int? PlanMonth { get; set; }
        public string TempDiscountCode { get; set; }

        public virtual TCurrency FkCurrency { get; set; }
        public virtual TOrder FkOrder { get; set; }
        public virtual TPaymentMethod FkPaymentMethod { get; set; }
        public virtual TShopPlan FkPlan { get; set; }
        public virtual TShop FkShop { get; set; }
    }
}
