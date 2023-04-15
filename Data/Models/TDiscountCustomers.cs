using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TDiscountCustomers
    {
        public int AssignedCustomerId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkCustomerId { get; set; }
        public bool Allowed { get; set; }

        public virtual TCustomer FkCustomer { get; set; }
        public virtual TDiscountPlan FkDiscountPlan { get; set; }
    }
}
