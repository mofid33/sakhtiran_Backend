using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsVariety
    {
        public int VarietyId { get; set; }
        public int FkProviderId { get; set; }
        public int FkGoodsId { get; set; }
        public int FkVariationParameterId { get; set; }
        public int FkVariationParameterValueId { get; set; }
        public string ImageUrl { get; set; }

        public virtual TGoods FkGoods { get; set; }
        public virtual TGoodsProvider FkProvider { get; set; }
        public virtual TVariationParameter FkVariationParameter { get; set; }
        public virtual TVariationParameterValues FkVariationParameterValue { get; set; }
    }
}
