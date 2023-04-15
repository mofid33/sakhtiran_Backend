using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShopActivityCountry
    {
        public int Id { get; set; }
        public int FkCountryId { get; set; }
        public int FkShopId { get; set; }
        public int FkShippingMethodId { get; set; }
        public bool ReturningAllowed { get; set; }

        public virtual TCountry FkCountry { get; set; }
        public virtual TShippingMethod FkShippingMethod { get; set; }
        public virtual TShop FkShop { get; set; }
    }
}
