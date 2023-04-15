using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountCategory
    {
        public int AssingnedCategoryId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkCategoryId { get; set; }
        public bool Allowed { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual TDiscountPlan FkDiscountPlan { get; set; }
    }
}
