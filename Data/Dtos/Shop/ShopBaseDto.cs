using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Category;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopBaseDto
    {
        public int ShopId { get; set; }
        public int? PlanId { get; set; }
        public short FkStatusId { get; set; }
        public string VendorUrlid { get; set; }
        public string StoreName { get; set; }
        public string StatusTitle { get; set; }
        public string ShopMessage { get; set; }
        public string StatusDesc { get; set; }
        public string RegisteryDateTime { get; set; }

    }
}