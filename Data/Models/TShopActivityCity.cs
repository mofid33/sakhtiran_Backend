using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopActivityCity
    {
        public int Id { get; set; }
        public int FkShopId { get; set; }
        public int? FkCityId { get; set; }
        public int FkShippingMethodId { get; set; }
        public int? PostTimeoutDayByShop { get; set; }
        public bool ReturningAllowed { get; set; }
        public decimal? ShippingPriceFewerBaseWeight { get; set; }
        public decimal? ShippingPriceMoreBaseWeight { get; set; }
        public int FkProviceId { get; set; }

        public virtual TCity FkCity { get; set; }
        public virtual TProvince FkProvice { get; set; }
        public virtual TShippingMethod FkShippingMethod { get; set; }
        public virtual TShop FkShop { get; set; }
    }
}
