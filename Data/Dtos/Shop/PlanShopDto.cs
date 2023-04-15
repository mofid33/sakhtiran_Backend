using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class PlanShopDto
    {
        public bool CurrentPlan { get; set; }
        public string ExpDate { get; set; }
        public bool Available { get; set; }

        public int PlanId { get; set; }
        public string PlanTitle { get; set; }
        public int? MaxCategory { get; set; }
        public int? MaxProduct { get; set; }
        public double CommissionFraction { get; set; }
        public decimal RentPerMonth { get; set; }
        public bool Microstore { get; set; }
        public string Desription { get; set; }
        public decimal ShopCredit { get; set; }

        public List<ShopCategoryPlanDto> Category { get; set; }
    }
}