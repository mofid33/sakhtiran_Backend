using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountPlanType
    {
        public TDiscountPlanType()
        {
            TDiscountPlan = new HashSet<TDiscountPlan>();
        }

        public int TypeId { get; set; }
        public string TypeTitle { get; set; }

        public virtual ICollection<TDiscountPlan> TDiscountPlan { get; set; }
    }
}
