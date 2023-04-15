using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountCouponCodeType
    {
        public TDiscountCouponCodeType()
        {
            TDiscountPlan = new HashSet<TDiscountPlan>();
        }

        public short CodeTypeId { get; set; }
        public string CodeTypeTitle { get; set; }

        public virtual ICollection<TDiscountPlan> TDiscountPlan { get; set; }
    }
}
