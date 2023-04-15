using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Models
{
    public partial class TGoodsLike
    {
        public int LikeId { get; set; }
        public int FkCustomerId { get; set; }
        public int FkGoodsId { get; set; }
        public DateTime LikeDate { get; set; }

        public virtual TCustomer FkCustomer { get; set; }
        public virtual TGoods FkGoods { get; set; }
    }
}
