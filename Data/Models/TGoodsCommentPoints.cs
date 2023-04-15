using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsCommentPoints
    {
        public int PointId { get; set; }
        public int FkCommentId { get; set; }
        public string PointText { get; set; }
        public bool PointType { get; set; }

        public virtual TGoodsComment FkComment { get; set; }
    }
}
