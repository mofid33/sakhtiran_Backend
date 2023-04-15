using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopPlan
    {
        public TShopPlan()
        {
            TPaymentTransaction = new HashSet<TPaymentTransaction>();
            TShop = new HashSet<TShop>();
            TShopPlanExclusive = new HashSet<TShopPlanExclusive>();
        }

        public int PlanId { get; set; }
        public string PlanTitle { get; set; }
        public int? MaxCategory { get; set; }
        public int? MaxProduct { get; set; }
        public double PercentOfCommission { get; set; }
        public decimal? RentPerMonth { get; set; }
        public bool Microstore { get; set; }
        public string Desription { get; set; }
        public bool Status { get; set; }
        public bool? Exclusive { get; set; }

        public virtual ICollection<TPaymentTransaction> TPaymentTransaction { get; set; }
        public virtual ICollection<TShop> TShop { get; set; }
        public virtual ICollection<TShopPlanExclusive> TShopPlanExclusive { get; set; }
    }
}
