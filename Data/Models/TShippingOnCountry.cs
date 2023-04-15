using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShippingOnCountry
    {
        public int Id { get; set; }
        public int FkCountryId { get; set; }
        public int FkShippingMethodId { get; set; }
        public int? PostTimeoutDay { get; set; }
        public decimal? ShippingPriceFewerBaseWeight { get; set; }
        public decimal? ShippingPriceMoreBaseWeight { get; set; }

        public virtual TCountry FkCountry { get; set; }
        public virtual TShippingMethod FkShippingMethod { get; set; }
    }
}
