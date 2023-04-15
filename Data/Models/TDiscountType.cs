using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountType
    {
        public TDiscountType()
        {
            TDiscountPlan = new HashSet<TDiscountPlan>();
        }

        public int DiscountTypeId { get; set; }
        public string DiscountTypeTitle { get; set; }

        public virtual ICollection<TDiscountPlan> TDiscountPlan { get; set; }
    }
}
