using System;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopAccessDto
    {
        public int ShopStatusId { get; set; }
        public int? PlanId { get; set; }
        public string ShopStatus { get; set; }
        public string ShopStoreName { get; set; }
        public string VendorUrlid { get; set; }
        public string ShopMessage { get; set; }
        public string ShopRegisterDate { get; set; }
        public string ShopStatusDesc { get; set; }
        public bool Active { get; set; }
    }
}