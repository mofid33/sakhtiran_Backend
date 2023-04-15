using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.ShopPlan
{
    public class ShopPlanDto
    {
        public int PlanId { get; set; }
        public string PlanTitle { get; set; }
        public int? MaxCategory { get; set; }
        public int? MaxProduct { get; set; }
        public double PercentOfCommission { get; set; }
        public decimal? RentPerMonth { get; set; }
        public bool Microstore { get; set; }
        public string Desription { get; set; }
        public bool Status { get; set; }
        public bool Exclusive { get; set; }
        public int VendorCount { get; set; }
        public List<ShopPlanExclusiveDto> TShopPlanExclusive { get; set; }

    }
}