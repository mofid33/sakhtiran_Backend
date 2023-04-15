using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShippingOnCity
    {
        public int Id { get; set; }
        public int FkShippingMethodId { get; set; }
        public int FkProviceId { get; set; }
        public int? FkCityId { get; set; }
        public int? PostTimeoutDay { get; set; }
        public decimal? ShippingPriceFewerBaseWeight { get; set; }
        public decimal? ShippingPriceMoreBaseWeight { get; set; }

        public virtual TCity FkCity { get; set; }
        public virtual TProvince FkProvice { get; set; }
        public virtual TShippingMethod FkShippingMethod { get; set; }
    }
}
