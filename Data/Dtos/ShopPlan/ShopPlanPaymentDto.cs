using System;

namespace MarketPlace.API.Data.Dtos.ShopPlan
{
    public class ShopPlanPaymentDto
    {
        public int PlanId { get; set; }
        public int ShopId { get; set; }
        public int? Month { get; set; }
        public DateTime? FreeDate { get; set; }
        public bool UseCredit { get; set; }
        public int? PaymentId { get; set; }
        public string PaymentPayId { get; set; }

        // card info

        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string CardMonth { get; set; }
        public string CardYear { get; set; }
        public string CardZip { get; set; }
        public string SecurityCode { get; set; }
    }
}