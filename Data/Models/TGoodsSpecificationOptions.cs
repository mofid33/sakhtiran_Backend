using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsSpecificationOptions
    {
        public int SpecOptionId { get; set; }
        public int FkSpecOptionId { get; set; }
        public int FkGsid { get; set; }

        public virtual TGoodsSpecification FkGs { get; set; }
        public virtual TSpecificationOptions FkSpecOption { get; set; }
    }
}
