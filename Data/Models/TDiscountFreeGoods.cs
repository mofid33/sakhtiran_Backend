using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountFreeGoods
    {
        public int FreeGoodsId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public double Quantity { get; set; }

        public virtual TDiscountPlan FkDiscountPlan { get; set; }
        public virtual TGoods FkGoods { get; set; }
        public virtual TGoodsProvider FkVariety { get; set; }
    }
}
