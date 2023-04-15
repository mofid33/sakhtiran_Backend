using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopPlanExclusive
    {
        public int ShopPlanExclusiveId { get; set; }
        public int FkShopId { get; set; }
        public int FkPlanId { get; set; }

        public virtual TShopPlan FkPlan { get; set; }
        public virtual TShop FkShop { get; set; }
    }
}
