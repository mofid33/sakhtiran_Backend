using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountPlanGetDto
    {
        public long PlanId { get; set; }
        public string Title { get; set; }
        public int? FkShopId { get; set; }
        public string ShopTitle { get; set; }
        public int FkPlanTypeId { get; set; }
        public string PlanTypeTitle { get; set; }
        public bool Status { get; set; }
    }
}