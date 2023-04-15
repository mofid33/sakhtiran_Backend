using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TShippingMethodAreaCode
    {
        public long PostAreaCodeId { get; set; }
        public int FkCityId { get; set; }
        public int FkShippingMethodId { get; set; }
        public int Code { get; set; }
        public int? StateCode { get; set; }

        public virtual TCity FkCity { get; set; }
        public virtual TShippingMethod FkShippingMethod { get; set; }
    }
}
