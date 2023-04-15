using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsDocument
    {
        public int ImageId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public string DocumentUrl { get; set; }

        public virtual TGoods FkGoods { get; set; }
        public virtual TGoodsProvider FkVariety { get; set; }
    }
}
