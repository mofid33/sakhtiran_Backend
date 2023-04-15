using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TSpecificationOptions
    {
        public TSpecificationOptions()
        {
            TGoodsSpecificationOptions = new HashSet<TGoodsSpecificationOptions>();
        }

        public int OptionId { get; set; }
        public string OptionTitle { get; set; }
        public int FkSpecId { get; set; }
        public int? Priority { get; set; }

        public virtual TSpecification FkSpec { get; set; }
        public virtual ICollection<TGoodsSpecificationOptions> TGoodsSpecificationOptions { get; set; }
    }
}
