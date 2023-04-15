using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountShops
    {
        public int AssignedShopId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkShopId { get; set; }
        public bool Allowed { get; set; }

        public virtual TDiscountPlan FkDiscountPlan { get; set; }
        public virtual TShop FkShop { get; set; }
    }
}
