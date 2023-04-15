using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TVariationParameter
    {
        public TVariationParameter()
        {
            TGoodsVariety = new HashSet<TGoodsVariety>();
            TVariationParameterValues = new HashSet<TVariationParameterValues>();
            TVariationPerCategory = new HashSet<TVariationPerCategory>();
        }

        public int ParameterId { get; set; }
        public string ParameterTitle { get; set; }
        public bool ValuesHaveImage { get; set; }

        public virtual ICollection<TGoodsVariety> TGoodsVariety { get; set; }
        public virtual ICollection<TVariationParameterValues> TVariationParameterValues { get; set; }
        public virtual ICollection<TVariationPerCategory> TVariationPerCategory { get; set; }
    }
}
