using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TVariationParameterValues
    {
        public TVariationParameterValues()
        {
            TGoodsVariety = new HashSet<TGoodsVariety>();
        }

        public int ValueId { get; set; }
        public int FkParameterId { get; set; }
        public string Value { get; set; }

        public virtual TVariationParameter FkParameter { get; set; }
        public virtual ICollection<TGoodsVariety> TGoodsVariety { get; set; }
    }
}
