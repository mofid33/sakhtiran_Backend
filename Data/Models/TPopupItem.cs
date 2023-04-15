using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TPopupItem
    {
        public int PopupId { get; set; }
        public string Title { get; set; }
        public string PopupImageUrl { get; set; }
        public int? FkCategoryId { get; set; }
        public long? FkTDiscountPlanId { get; set; }
        public bool JustNewGoods { get; set; }
        public bool JustShowOnce { get; set; }
        public bool Status { get; set; }

        public virtual TCategory FkCategory { get; set; }
        public virtual TDiscountPlan FkTDiscountPlan { get; set; }
    }
}
