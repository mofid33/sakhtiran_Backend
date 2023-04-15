using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountCouponCode
    {
        public TDiscountCouponCode()
        {
            TOrder = new HashSet<TOrder>();
            TOrderItem = new HashSet<TOrderItem>();
        }

        public int CodeId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public string DiscountCode { get; set; }
        public int MaxUse { get; set; }
        public int UseCount { get; set; }
        public bool IsValid { get; set; }

        public virtual TDiscountPlan FkDiscountPlan { get; set; }
        public virtual ICollection<TOrder> TOrder { get; set; }
        public virtual ICollection<TOrderItem> TOrderItem { get; set; }
    }
}
