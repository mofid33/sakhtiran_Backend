using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountRangeType
    {
        public TDiscountRangeType()
        {
            TDiscountPlan = new HashSet<TDiscountPlan>();
        }

        public int RangeTypeId { get; set; }
        public string RangeTypeTitle { get; set; }

        public virtual ICollection<TDiscountPlan> TDiscountPlan { get; set; }
    }
}
