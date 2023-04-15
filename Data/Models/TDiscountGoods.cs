using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountGoods
    {
        public int AssingedGoodsId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public bool Allowed { get; set; }

        public virtual TDiscountPlan FkDiscountPlan { get; set; }
        public virtual TGoods FkGoods { get; set; }
        public virtual TGoodsProvider FkVariety { get; set; }
    }
}
