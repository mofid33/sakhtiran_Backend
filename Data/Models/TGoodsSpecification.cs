using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsSpecification
    {
        public TGoodsSpecification()
        {
            TGoodsSpecificationOptions = new HashSet<TGoodsSpecificationOptions>();
        }

        public int Gsid { get; set; }
        public int FkGoodsId { get; set; }
        public int FkSpecId { get; set; }
        public string SpecValueText { get; set; }

        public virtual TGoods FkGoods { get; set; }
        public virtual TSpecification FkSpec { get; set; }
        public virtual ICollection<TGoodsSpecificationOptions> TGoodsSpecificationOptions { get; set; }
    }
}
