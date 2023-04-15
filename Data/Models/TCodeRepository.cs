using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TCodeRepository
    {
        public string DiscountCode { get; set; }
        public int CodeLength { get; set; }
        public bool Used { get; set; }
    }
}
